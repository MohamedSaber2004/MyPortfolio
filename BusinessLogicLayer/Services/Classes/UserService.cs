
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Models.UserModels;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogicLayer.Services.Classes
{
    public class UserService(UserManager<User> _userManager) : IUserService
    {
        public async Task<int> GetPendingUserCountAsync()
        {
            var pendingUsers = await _userManager.GetUsersInRoleAsync("Pending");
            return pendingUsers.Count;
        }
    }
}
