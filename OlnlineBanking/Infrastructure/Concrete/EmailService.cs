using System.Net;
using System.Net.Mail;
using OlnlineBanking.Infrastructure.Abstract;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class EmailService:IEmailService
    {
        private IConfig _configManager;
        
        public EmailService(IConfig configManager)
        {
            _configManager = configManager;
        }

        public void SendEmail(string to, string subject, string body)
        {
            MailMessage m = new MailMessage(_configManager.MailSetting.SmtpReply, to, subject, body) { IsBodyHtml = true };
            SmtpClient smtp = new SmtpClient(_configManager.MailSetting.SmtpServer, _configManager.MailSetting.SmtpPort)
            {
                EnableSsl = _configManager.MailSetting.EnableSsl, 
                Credentials = new NetworkCredential(_configManager.MailSetting.SmtpUserName, _configManager.MailSetting.SmtpPassword)
            };
            smtp.Send(m);
        }
    }
}