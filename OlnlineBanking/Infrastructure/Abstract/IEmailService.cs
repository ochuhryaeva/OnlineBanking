using System.Diagnostics.Contracts;
using OlnlineBanking.Infrastructure.Concrete;

namespace OlnlineBanking.Infrastructure.Abstract
{
    [ContractClass(typeof(EmailServiceContract))]
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }
}
