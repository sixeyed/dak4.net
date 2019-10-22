using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using NATS.Client;
using Prometheus;
using SignUp.Core;
using SignUp.Entities;
using SignUp.MessageHandlers.SaveProspectCore.Model;
using SignUp.Messaging;
using SignUp.Messaging.Messages.Events;

namespace SignUp.MessageHandlers.SaveProspectCore.Workers
{
    public class QueueWorker
    {
        private static ManualResetEvent _ResetEvent = new ManualResetEvent(false);

        private const string QUEUE_GROUP = "save-handler";

        private static Counter _EventCounter = Metrics.CreateCounter("SaveHandler_Events", "Event count", "host", "status");
        private static string _Host = Environment.MachineName;

        private readonly IConfiguration _config;
        private readonly SignUpContext _context;

        public QueueWorker(IConfiguration config, SignUpContext context)
        {
            _config = config;
            _context = context;
        }

        public void Start()
        {
            if (_config.GetValue<bool>("Metrics:Enabled"))
            {
                StartMetricServer();
            }            

            Console.WriteLine($"Connecting to message queue url: {Config.Current["MessageQueue:Url"]}");
            using (var connection = MessageQueue.CreateConnection())
            {
                var subscription = connection.SubscribeAsync(ProspectSignedUpEvent.MessageSubject, QUEUE_GROUP);
                subscription.MessageHandler += SaveProspect;
                subscription.Start();
                Console.WriteLine($"Listening on subject: {ProspectSignedUpEvent.MessageSubject}, queue: {QUEUE_GROUP}");

                _ResetEvent.WaitOne();
                connection.Close();
            }
        }

        private void SaveProspect(object sender, MsgHandlerEventArgs e)
        {
            _EventCounter.Labels(_Host, "received").Inc();

            Console.WriteLine($"Received message, subject: {e.Message.Subject}");
            var eventMessage = MessageHelper.FromData<ProspectSignedUpEvent>(e.Message.Data);
            Console.WriteLine($"Saving new prospect, signed up at: {eventMessage.SignedUpAt}; event ID: {eventMessage.CorrelationId}");

            var prospect = eventMessage.Prospect;

            try
            {
                SaveProspect(prospect);
                Console.WriteLine($"Prospect saved. Prospect ID: {eventMessage.Prospect.ProspectId}; event ID: {eventMessage.CorrelationId}");
                _EventCounter.Labels(_Host, "processed").Inc();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save prospect FAILED, email address: {prospect.EmailAddress}, ex: {ex}");
                _EventCounter.Labels(_Host, "failed").Inc();
            }
        }

        private void SaveProspect(Prospect prospect)
        {
            //reload child objects:
            prospect.Country = _context.Countries.Single(x => x.CountryCode == prospect.Country.CountryCode);
            prospect.Role = _context.Roles.Single(x => x.RoleCode == prospect.Role.RoleCode);

            _context.Prospects.Add(prospect);
            _context.SaveChanges();
        }

        private void StartMetricServer()
        {
            var metricsPort = Config.Current.GetValue<int>("Metrics:Port");
            var server = new MetricServer(metricsPort);
            server.Start();
            Console.WriteLine($"Metrics server listening on port {metricsPort}");
        }
    }
}
