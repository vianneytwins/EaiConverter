using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace EaiConverter.Model
{
	public class GlobalVariable
	{
        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public GlobalVariableType Type
        {
            get;
            set;
        }
            
	}

    public enum GlobalVariableType
    {
        String,
        Integer
    }

}

