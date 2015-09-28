namespace EaiConverter.Model
{
    using System.Collections.Generic;

    public class TibcoBWProcess
    {
		public TibcoBWProcess (string fullProcessName)
		{
			this.FullProcessName = fullProcessName;

            this.ShortNameSpace = GetMyShortNameSpace(fullProcessName);

            this.ProcessName = GetMyProcessName(fullProcessName);
		}

        public static string GetMyProcessName(string fullProcessName)
        {
            var indexOfLastSlash = fullProcessName.LastIndexOf("/");

            var indexOfLastDot = fullProcessName.LastIndexOf(".");
            if (indexOfLastDot == -1)
            {
                indexOfLastDot = fullProcessName.Length;
            }

            var nameLenght = indexOfLastDot - indexOfLastSlash - 1;
            var myProcessName =
                fullProcessName.Substring(indexOfLastSlash + 1, nameLenght)
                    .Replace("-", string.Empty)
                    .Replace(".", string.Empty)
                    .Replace("(", "_")
                    .Replace(")", "_")
                    .Replace(" ", string.Empty);
            return myProcessName;
        }

        public static string GetMyShortNameSpace(string fullProcessName)
        {
            if (fullProcessName.StartsWith("/") || fullProcessName.StartsWith("\\"))
            {
                fullProcessName = fullProcessName.Remove(0, 1);
            }
            var indexOfLastSlash = fullProcessName.LastIndexOf("/");
            string myShortNameSpace;
            if (indexOfLastSlash == -1)
            {
                myShortNameSpace = string.Empty;
            }
            else
            {
                myShortNameSpace =
                    (fullProcessName.Substring(0, indexOfLastSlash)).Replace("/", ".").Replace("(", "_").Replace(")", "_").Replace(" ", string.Empty);
            }
            return myShortNameSpace;
        }

        public string ProcessName { get; private set;}
		public string FullProcessName { get; private set;}

        public string Description { get; set;}

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

		public string ShortNameSpace
        {
			get;
			private set;
		}

		public string NameSpace
        {
			get
			{
				return ShortNameSpace + "." + ProcessName;
			}
		}

		public string InputAndOutputNameSpace
        {
			get
            {
				return NameSpace + "InputOutputModel";
			}
		}

        public string VariablesNameSpace
        {
            get
            {
                return NameSpace + "Variables";
            }
        }

        public string StartingPoint
        {
            get
            {
                if (this.StarterActivity != null)
                {
                    return this.StarterActivity.Name;
                }

                return this.StartActivity.Name;
            }
        }
    }
}

