using System.Collections.Generic;

namespace EaiConverter.Model
{
    public class JavaActivity : Activity
    {
        public JavaActivity (string name, ActivityType type) : base (name, type)
        {
        }

        public JavaActivity () 
        {
        }

        public string FileName {get; set;}
        public string PackageName {get; set;}
        public string FullSource {get; set;}
        public List<ClassParameter> InputData {get; set;}
        public List<ClassParameter> OutputData {get; set;}
    }
}

