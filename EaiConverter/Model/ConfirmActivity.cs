using System;
using System.Xml.Linq;

namespace EaiConverter.Model
{

    public class ConfirmActivity : Activity
	{
        public ConfirmActivity (string name, ActivityType type) : base (name, type)
        {
        }

        public ConfirmActivity () 
        {
        }

        public string ActivityNameToConfirm
        {
            get;
            set;
        }
	}


}

