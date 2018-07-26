using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Postal;

namespace SimpleSecurity.AspNetIdentity
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            dynamic email = new Email("ChngPasswordEmail");
            email.To = message.Destination;
            email.Send();
            return Task.FromResult(0);

        }
    }
}
