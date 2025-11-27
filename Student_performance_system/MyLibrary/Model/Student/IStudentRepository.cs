using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.Repositories
{
    public interface IStudentRepository
    {
        List<Student> GetStudentsByGroup(string groupName);
        List<string> GetGroups();
    }
}
