using System;
using System.Collections.Generic;

namespace EaiConverter.Model
{
	public class TibcoBWProcess
	{
		public TibcoBWProcess (string fullProcessName)
		{
			this.FullProcessName = fullProcessName;
			var indexOfLastSlash = fullProcessName.LastIndexOf ("/");
            if (indexOfLastSlash == -1)
            {
                this.ShortNameSpace = string.Empty;
            }
            else
            {
                this.ShortNameSpace = (fullProcessName.Substring (0, indexOfLastSlash)).Replace("/",".");
            }


			var indexOfLastDot = fullProcessName.LastIndexOf (".");
            if (indexOfLastDot == -1) {
                indexOfLastDot = fullProcessName.Length;
            }
			var nameLenght = indexOfLastDot - indexOfLastSlash -1;
			this.ProcessName = fullProcessName.Substring (indexOfLastSlash+1, nameLenght).Replace("-", string.Empty).Replace(".",string.Empty);
		}

		public string ProcessName { get; private set;}
		public string FullProcessName { get; private set;}

        public Activity StartActivity { get; set;}

        public Activity StarterActivity
        {
            get;
            set;
        }

		public Activity EndActivity { get; set;}

		public List<Activity> Activities { get; set;}
		public List<Transition> Transitions { get; set;}
        public List<ProcessVariable> ProcessVariables { get; set;}
        public List<XsdImport> XsdImports { get; set;}

		public string ShortNameSpace {
			get;
			private set;
		}

		public string NameSpace {
			get
			{
				return ShortNameSpace + "." + ProcessName;
			}
		}
		public string InputAndOutputNameSpace {
			get
            {
				return NameSpace + ".InputOutputModel";
			}

		}
	}
}

