namespace BizyBoard.Web.Services
{
    using System.Threading.Tasks;

    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}