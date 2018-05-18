namespace BizyBoard.Web.Services
{
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;

    public class EmailService : IEmailService
    {
        public void SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("mail.infomaniak.com")
            {
                UseDefaultCredentials = false,
                Port = 587,
                Credentials = new NetworkCredential("", "")
            };

            var mailMessage = new MailMessage {From = new MailAddress("")};
            mailMessage.To.Add(email);
            mailMessage.Body = message;
            mailMessage.Subject = subject;
            client.SendAsync(mailMessage,"test");
        }
    }
}