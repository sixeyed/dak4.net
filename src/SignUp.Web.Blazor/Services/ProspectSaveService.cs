using SignUp.Entities;
using SignUp.Messaging;
using SignUp.Messaging.Messages.Events;
using System;

namespace SignUp.Web.Blazor.Services
{
    public class ProspectSaveService 
    {
        public void SaveProspect(Prospect prospect)
        {
            var eventMessage = new ProspectSignedUpEvent
            {
                Prospect = prospect,
                SignedUpAt = DateTime.UtcNow
            };

            MessageQueue.Publish(eventMessage);
        }
    }
}