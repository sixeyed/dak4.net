using Microsoft.Extensions.Logging;
using SignUp.Entities;
using SignUp.Messaging;
using SignUp.Messaging.Messages.Events;
using System;

namespace SignUp.Web.Blazor.Services
{
    public class ProspectSaveService
    {
        private readonly ILogger<ProspectSaveService> _logger;

        public ProspectSaveService(ILogger<ProspectSaveService> logger)
        {
            _logger = logger;
        }

        public void SaveProspect(Prospect prospect)
        {
            var eventMessage = new ProspectSignedUpEvent
            {
                Prospect = prospect,
                SignedUpAt = DateTime.UtcNow
            };

            MessageQueue.Publish(eventMessage);

            _logger.LogInformation("Published ProspectSignedUpEvent - CorrelationId: {CorrelationId}", eventMessage.CorrelationId);
        }
    }
}