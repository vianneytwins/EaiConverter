using System;
namespace test
{
    public class groupVariables
    {

        private groupVariablesGroupVariable[] groupVariableField;

        public groupVariablesGroupVariable[] groupVariable
        {
            get
            {
                return this.groupVariableField;
            }
            set
            {
                this.groupVariableField = value;
            }
        }
    }

    public class groupVariablesGroupVariable
    {

        private string nameField;

        private string valueField;

        private string typeField;

        private bool deploymentSettableField;

        private bool serviceSettableField;

        private long modTimeField;

        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        public bool deploymentSettable
        {
            get
            {
                return this.deploymentSettableField;
            }
            set
            {
                this.deploymentSettableField = value;
            }
        }

        public bool serviceSettable
        {
            get
            {
                return this.serviceSettableField;
            }
            set
            {
                this.serviceSettableField = value;
            }
        }

        public long modTime
        {
            get
            {
                return this.modTimeField;
            }
            set
            {
                this.modTimeField = value;
            }
        }
    }
}
