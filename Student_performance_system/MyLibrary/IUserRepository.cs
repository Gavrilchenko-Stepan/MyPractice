using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public interface  IUserRepository
    {
        User GetUserByLogin(string login);
        bool ValidateUser(string login, string password);
    }
}
