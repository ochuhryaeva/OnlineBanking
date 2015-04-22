using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mail;
using System.Web;
using OlnlineBanking.Infrastructure.Abstract;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class EmailService:IEmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
            //TODO: 
            string from = "";
            string password = "";
            MailMessage m = new MailMessage(from, to, subject, body) { IsBodyHtml = true };
            SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587) { EnableSsl = true, Credentials = new System.Net.NetworkCredential(from, password) };
            smtp.Send(m);
        }
    }
}