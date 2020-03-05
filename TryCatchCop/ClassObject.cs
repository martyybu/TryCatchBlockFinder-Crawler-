using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryCatchCop
{
    public class ClassObject
    {
        public int MethodID { get; set; }
        public string ClassName { get; set; }
        public bool HasTryCatchBlock { get; set; }
        public int LineNumber { get; set; }

        public ClassObject(int methodID, string className, bool hasTryCatch, int lineNumber)
        {
            MethodID = methodID;
            ClassName = className;
            HasTryCatchBlock = hasTryCatch;
            LineNumber = lineNumber;
        }
    }
}
