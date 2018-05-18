namespace BizyBoard.Web.Services
{
    using System.Threading.Tasks;

    public interface IEmailService
    {
        void SendEmailAsync(string email, string subject, string message);
    }
}