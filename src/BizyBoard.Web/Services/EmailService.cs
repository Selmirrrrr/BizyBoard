namespace BizyBoard.Web.Services
{
    using System.Threading.Tasks;

    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string email, string subject, string message) => Task.FromResult(0);
    }
}