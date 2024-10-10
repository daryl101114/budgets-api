using budget_api.Models.DTOs;
using budget_api.Models.Entities;

namespace budget_api.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserWalletsAsync(Guid userId);
        Task AddUser(User user);
        Task<bool> SaveChangesAsync();
    }
}
