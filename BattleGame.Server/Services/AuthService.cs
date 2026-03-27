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

        public enum LoginResult
        {
            Success,
            UserNotFound,
            WrongPassword
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
            if (user == null) return (LoginResult.UserNotFound);
            bool ok = BCrypt.Net.BCrypt.Verify(password, user.Value.PasswordHash);
            if (!ok) return (LoginResult.WrongPassword);
            return (LoginResult.Success);
        }

        public RegisterResult Register(string username, string password, string email)
        {
            if (_userRepo.ExistsByUsername(username)) return RegisterResult.UsernameTaken;
            if (_userRepo.ExistsByEmail(email)) return RegisterResult.EmailTaken;
            try
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(password);
                _userRepo.Save(username, hash, email);
                return RegisterResult.Success;
            }
            catch (Exception ex)
            {
                return RegisterResult.Failed;
            }
        }
    }
}