using SignUp.MessageHandlers.SaveProspectCore.Model;
using SignUp.Messaging;
using System;
using System.Linq;

namespace SignUp.MessageHandlers.SaveProspectCore.Workers
{
    public class CheckWorker
    {
        private readonly SignUpContext _context;

        public CheckWorker(SignUpContext context)
        {
            _context = context;
        }

        public int Run()
        {
            return CheckDatabase() + CheckMessageQueue();
        }

        private int CheckDatabase()
        {
            try
            {
                var roleCount = _context.Roles.Count();
                Console.WriteLine("Database check OK");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database check FAILED: {ex}");
                return 1;
            }
        }


        private int CheckMessageQueue()
        {
            try
            {
                using (var connection = MessageQueue.CreateConnection()) { }
                Console.WriteLine("Message queue check OK");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message queue check FAILED: {ex}");
                return 1;
            }
        }
    }
}
