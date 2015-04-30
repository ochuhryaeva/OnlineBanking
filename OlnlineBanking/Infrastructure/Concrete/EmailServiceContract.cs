using System.Diagnostics.Contracts;
using OlnlineBanking.Infrastructure.Abstract;

namespace OlnlineBanking.Infrastructure.Concrete
{
    [ContractClassFor(typeof(IEmailService))]
    public abstract class EmailServiceContract: IEmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
            Contract.Requires(!string.IsNullOrEmpty(to));
        }
    }
}