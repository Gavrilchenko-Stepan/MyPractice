using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.DataModel.JournalData
{
    public class JournalData
    {
        public string GroupName { get; set; }
        public string SubjectName { get; set; }
        public List<RowData> Rows { get; set; } = new List<RowData>();
    }
}
