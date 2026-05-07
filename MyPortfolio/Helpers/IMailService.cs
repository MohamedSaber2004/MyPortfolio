using MyPortfolio.Helpers.CustomerServiceModels;

namespace MyPortfolio.Helpers
{
    public interface IMailService
    {
        Task SendEmailAsync(EmailMessageFormat _emailMessage);
    }
}
