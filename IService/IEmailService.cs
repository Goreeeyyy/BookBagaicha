using System.Threading.Tasks;

namespace BookBagaicha.IService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
    }
}