using System;

namespace EaiConverter.CodeGenerator.Utils
{
	public class Tab
	{
		private string spacing = "";
		private string increment = "  ";

		public Tab ()
		{
		}

		public String Increment()
		{
			spacing = spacing + increment;
 			return spacing;
		}

		public String Decrement()
		{
			if (spacing.Length >= 2) {
				spacing = spacing.Substring (0, spacing.Length - 2);
			}
			return spacing;
		}

		public override string ToString ()
		{
			return spacing;
		}
	}
}

