using MyPortfolio.Helpers.CustomerServiceModels;

namespace MyPortfolio.Helpers
{
    public interface IMailService
    {
        void SendEmail(EmailMessageFormat _emailMessage);
    }
}
