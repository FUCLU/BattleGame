using BattleGame.Server.Database;
using BattleGame.Server.Logging;

namespace BattleGame.Server.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepo;

        public AuthService(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public abstract record LoginResult
        {
            public record Success(int UserId, string Username) : LoginResult;
            public record UserNotFound() : LoginResult;
            public record WrongPassword() : LoginResult;
        }

        public enum RegisterResult
        {
            Success,
            UsernameTaken, 
            EmailTaken,    
            Failed         
        }

        public LoginResult Login(string username, string password)
        {
            var user = _userRepo.FindByUsername(username);
            if(user == null) return new LoginResult.UserNotFound();

            bool ok = BCrypt.Net.BCrypt.Verify(password, user.Value.PasswordHash);
            if (!ok) return new LoginResult.WrongPassword();

            return new LoginResult.Success(user.Value.UserId, user.Value.Username);
        }

        public RegisterResult Register(string username, string email)
        {
            if (_userRepo.ExistsByUsername(username)) return RegisterResult.UsernameTaken;
            if (_userRepo.ExistsByEmail(email)) return RegisterResult.EmailTaken;
            return RegisterResult.Success;
        }
    }
}