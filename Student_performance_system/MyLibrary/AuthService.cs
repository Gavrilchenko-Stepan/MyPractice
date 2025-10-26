using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private User _currentUser;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;

        public bool Login(string login, string password)
        {
            if (_userRepository.ValidateUser(login, password))
            {
                _currentUser = _userRepository.GetUserByLogin(login);
                return true;
            }
            return false;
        }

        public void Logout()
        {
            _currentUser = null;
        }
    }
}
