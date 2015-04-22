using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OlnlineBanking.Infrastructure.Concrete;

namespace OlnlineBanking.Infrastructure.Abstract
{
    [ContractClass(typeof(EmailServiceContract))]
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }
}
