using System;
using System.Collections.Generic;

namespace EaiConverter.Model
{
    public class GlobalVariablesRepository
    {
        public string Package
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public List<GlobalVariable> GlobalVariables
        {
            get;
            set;
        }
    }
}

