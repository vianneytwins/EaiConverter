using System;
using NUnit.Framework;
using System.Xml;
using System.Xml.Xsl;

namespace EaiConverter
{
	[TestFixture]
	public class XslBuilderTest
	{
		[Ignore]
		[Test]
		public void Should_Merge_2_Files_in_1_With_XSLT(){
			XslTransform myXslTransform; 
			myXslTransform = new XslTransform();
			myXslTransform.Load("books.xsl"); 
			myXslTransform.Transform("adminFormat.xml", "NTM.xml"); 
		}
	}
}

