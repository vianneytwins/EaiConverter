namespace EaiConverter.Model
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    public class Activity
	{
        private string name;

    	public Activity(string name, ActivityType type)
        {
			this.Name = name;
			this.Type = type;
		}

		public Activity()
		{
		} 

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = FormatActivityName(value);
            }
        }

        public static string FormatActivityName(string value)
        {
            return value.Replace(' ', '_').Replace('.', '_').Replace('-', '_').Replace("=", "Equals").Replace("+", "Add").Replace("&", "");
        }

        public ActivityType Type { get; set; }

		public List<ClassParameter> Parameters { get; set; }

		public IEnumerable<XNode> ObjectXNodes
        {
			get;
			set;
		}

        public IEnumerable<XNode> InputBindings { get; set; }

		public override string ToString()
		{
			return string.Format("[Activity: Name={0}, Type={1}]", this.Name, this.Type);
		}
	}
}

