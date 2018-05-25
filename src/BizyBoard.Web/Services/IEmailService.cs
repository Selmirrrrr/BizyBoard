namespace BizyBoard.Web.Services
{
    using System.Threading.Tasks;
    using SendGrid;

    public interface IEmailService
    {
        Task<Response> SendEmailAsync(string email, string subject, string link);
    }
}