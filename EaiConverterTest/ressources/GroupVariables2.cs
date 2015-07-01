using System;
namespace test
{
    public class repository
    {
        public groupVariable[] groupVariables {get; set;}
    }

    public class groupVariable
    {

        public string name
        {
            get;
            set;
        }

        public string value
        {
            get;
            set;
        }

        public string type
        {
            get;
            set;
        }

        public bool deploymentSettable
        {
            get;
            set;
        }

        public bool serviceSettable
        {
            get;
            set;
        }

        public long modTime
        {
            get;
            set;
        }
    }
}
