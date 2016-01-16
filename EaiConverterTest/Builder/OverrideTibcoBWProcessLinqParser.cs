using EaiConverter.Parser;

namespace EaiConverter.Test.Builder
{
	using System.Collections.Generic;
	using System.Xml.Linq;

	using EaiConverter.Builder;
	using EaiConverter.Model;
	using EaiConverter.Test.Utils;


    public class OverrideTibcoBWProcessLinqParser : TibcoBWProcessLinqParser
	{
        public override TibcoBWProcess Parse(string filePath)
        {
            return new TibcoBWProcess("MyProcess"){ EndActivity = new Activity("EndActivity",ActivityType.endType)
                    {
                        Parameters = new List<ClassParameter>{ new ClassParameter { Type = "MyType", Name = "myReturn" } }
                }};
        }
	}

}

