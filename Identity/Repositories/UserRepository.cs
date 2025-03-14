using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Identity.Persistence;
using Identity.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Repositories
{
    public class UserRepository : IUserRepository
    {
        private const string JwtKeyConfig = "Jwt:Key";
        private readonly UsersDbContext usersDbContext;
        private readonly IConfiguration configuration;
        private readonly IGenericEntityRepository<User> repository;
        private readonly EmailService emailService;

        public UserRepository(UsersDbContext usersDbContext, IConfiguration configuration, IGenericEntityRepository<User> repository, EmailService emailService)
        {
            this.usersDbContext = usersDbContext;
            this.configuration = configuration;
            this.repository = repository;
            this.emailService = emailService;
        }

        static private bool VerifyPassword(string inputPassword, string storedPasswordHash)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedPasswordHash);
        }

        public async Task<Result<bool>> CheckPassword(User user)
        {
            var existingUser = await usersDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser == null || !VerifyPassword(user.Password, existingUser.Password))
            {
                return Result<bool>.Failure("Incorrect password");
            }
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ChangePassword(User user)
        {
            var existingUser = await usersDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser == null)
            {
                return Result<bool>.Failure("The user does not exist");
            }

            existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            usersDbContext.Entry(existingUser).State = EntityState.Modified;
            await usersDbContext.SaveChangesAsync();
            await repository.UpdateAsync(existingUser);

            return Result<bool>.Success(true);
        }

        public async Task<Result<string>> Login(User user)
        {
            var existingUser = await usersDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser == null || !VerifyPassword(user.Password, existingUser.Password))
            {
                return Result<string>.Failure("Invalid credentials");
            }

            if (!existingUser.IsEmailConfirmed)
            {
                return Result<string>.Failure("Email not confirmed");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()), // UserId
                new Claim(ClaimTypes.Name, existingUser.UserName ?? string.Empty), // Numele utilizatorului
                new Claim("email", existingUser.Email), // Email-ul utilizatorului (opțional)
                new Claim(ClaimTypes.Role, existingUser.IsAdmin ? "Admin" : "User") // Rolul utilizatorului
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration[JwtKeyConfig]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Result<string>.Success($"{{\"token\": \"{tokenHandler.WriteToken(token)}\"}}");
        }

        public async Task<Result<Guid>> Register(User user, CancellationToken cancellationToken)
        {
            var existingEmail = await usersDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email, cancellationToken);
            if (existingEmail != null)
            {
                return Result<Guid>.Failure("Email already exist");
            }
            var existingUsername = await usersDbContext.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName, cancellationToken);
            if (existingUsername != null)
            {
                return Result<Guid>.Failure("Username already exist");
            }

            var emailConfirmationToken = Guid.NewGuid().ToString();
            user.EmailConfirmationToken = emailConfirmationToken; // Adăugăm token-ul în entitatea utilizatorului
            user.EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(12); // Setăm expirarea token-ului


            await usersDbContext.Users.AddAsync(user, cancellationToken);
            await usersDbContext.SaveChangesAsync(cancellationToken);
            await repository.AddAsync(user);

            var confirmationLink = $"https://smart-real-estate-frontend-405e53ac73f5.herokuapp.com/email-confirmation-valid?token={emailConfirmationToken}";

            var emailBody = $@"<p>Hi {user.UserName},</p>
                    <p>Click <a href='{confirmationLink}'>here</a> to confirm your email.</p>
                    <p>This link will expire in 12 hours.</p>";

            try
            {
                await emailService.SendEmailAsync(user.Email, "Confirm  your email", emailBody);
                return Result<Guid>.Success(user.Id);

            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Failed to send email: {ex.Message}");

            }
        }

        public async Task<Result<string>> SendResetPassword(string email)
        {

            var existingUser = await usersDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser == null)
            {
                return Result<string>.Failure("User not found");
            }


            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
                        new Claim(ClaimTypes.Name, existingUser.UserName ?? string.Empty),
                    };
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration[JwtKeyConfig]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);


            var resetLink = $"https://smart-real-estate-frontend-405e53ac73f5.herokuapp.com/reset-password?token={jwtToken}";


            var emailBody = $@"
                    <p>Hi {existingUser.UserName},</p>
                    <p>Click <a href='{resetLink}'>here</a> to reset your password.</p>
                    <p>This link will expire in 1 hour.</p>";
            try
            {
                await emailService.SendEmailAsync(email, "Password Reset Request", emailBody);
                return Result<string>.Success("Reset link sent to your email.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Failed to send email: {ex.Message}");
            }


        }

        public async Task<Result<string>> ResetPassword(string token, string newPassword)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration[JwtKeyConfig]!);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return Result<string>.Failure("Invalid token");
            }
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Result<string>.Failure("Invalid token");
            }
            else
            {
                User? user = await usersDbContext.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
                if (user == null)
                {
                    return Result<string>.Failure("User not found");
                }
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await usersDbContext.SaveChangesAsync();
                await repository.UpdateAsync(user);
                return Result<string>.Success("Password reset successfully");
            }
        }

        public async Task<Result<Guid>> RemoveAsync(Guid id)
        {
            var user = await usersDbContext.Users.FindAsync(id);
            if (user == null)
            {
                return Result<Guid>.Failure("User not found");
            }
            usersDbContext.Users.Remove(user);
            await usersDbContext.SaveChangesAsync();
            await repository.DeleteAsync(id);
            return Result<Guid>.Success(id);
        }

        public async Task<Result<string>> EditAsync(User user)
        {
            try
            {
                var existingEmail = await usersDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Id != user.Id);
                if (existingEmail != null)
                {
                    return Result<string>.Failure("Email already exist");
                }
                var existingUsername = await usersDbContext.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName && u.Id != user.Id);
                if (existingUsername != null)
                {
                    return Result<string>.Failure("Username already exist");
                }
                usersDbContext.Entry(user).State = EntityState.Modified;
                await usersDbContext.SaveChangesAsync();
                await repository.UpdateAsync(user);

                // Generate new token
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                        new Claim("email", user.Email),
                        new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
                    };
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(configuration[JwtKeyConfig]!);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(12),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Result<string>.Success($"{{\"token\": \"{tokenHandler.WriteToken(token)}\"}}");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(ex.InnerException!.ToString());
            }
        }

        public async Task<Result<string>> SendContactForm(string name, string email, string subject, string message)
        {

            try
            {
                var emailMessage = new StringBuilder()
                    .AppendLine($"Name: {name}")
                    .AppendLine($"Email: {email}")
                    .AppendLine()
                    .AppendLine("Message:")
                    .AppendLine(message)
                    .ToString();

                await emailService.SendEmailAsync("smartestates11@gmail.com", subject, emailMessage);

                return Result<string>.Success("Message sent successfully!");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Failed to send email: {ex.Message}");
            }



        }

        public async Task<Result<string>> ConfirmEmail(string token)
        {
            var user = await usersDbContext.Users.FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);
            if (user == null)
            {
                return Result<string>.Failure("Invalid token");
            }
            if (user.EmailConfirmationTokenExpires < DateTime.UtcNow)
            {
                return Result<string>.Failure("Token expired");
            }
            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpires = null;
            await usersDbContext.SaveChangesAsync();
            await repository.UpdateAsync(user);
            return Result<string>.Success("Email confirmed successfully");
        }
    }
}
