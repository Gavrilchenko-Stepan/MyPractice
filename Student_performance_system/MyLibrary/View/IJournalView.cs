using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.View
{
    public interface IJournalView
    {
        string GroupName { get; }
        string SubjectName { get; }
    }
}
