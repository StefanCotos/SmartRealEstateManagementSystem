using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserRepository 
    {
        Task<Result<Guid>> Register(User user, CancellationToken cancellationToken);
        Task<Result<string>> Login(User user);
        Task<Result<string>> SendResetPassword(string email);
        Task<Result<string>> ResetPassword(string token, string newPassword);
        Task<Result<Guid>> RemoveAsync(Guid id);
        Task<Result<string>> EditAsync(User user);
        Task<Result<bool>> CheckPassword(User user);
        Task<Result<bool>> ChangePassword(User user);
        Task<Result<string>> SendContactForm(string name, string email, string subject, string message);

        Task<Result<string>> ConfirmEmail(string token);
    }
}

