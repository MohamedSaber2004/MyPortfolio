
namespace BusinessLogicLayer.Services.Interfaces
{
    public interface IUserService
    {
        Task<int> GetPendingUserCountAsync();
    }
}
