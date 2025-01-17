using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsEFCoreApp
{
    public class Class
    {
        public int ClassId { get; set; }
        public int FileId { get; set; }
        public string ClassName { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public int MethodsCount { get; set; }
        public File File { get; set; }
        public List<Method> Methods { get; set; } = new List<Method>();
    }

    public class File
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int Lines { get; set; }
        public DateTime AnalysisDate { get; set; }
        public List<Class> Classes { get; set; } = new List<Class>();
    }

    public class Method
    {
        public int MethodId { get; set; }
        public int ClassId { get; set; }
        public string MethodName { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public Class Class { get; set; }
    }
}
