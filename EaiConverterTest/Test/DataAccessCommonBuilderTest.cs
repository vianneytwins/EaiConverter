using System;
using NUnit.Framework;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using EaiConverter.Builder;

namespace EaiConverter
{
	[TestFixture]
	public class DataAccessCommonBuilderTest
	{
		[Test]
		public void Should_return_2_interfaces_when_building_dataAccessCommon(){
			var dataAccessCommonBuilder = new DataAccessInterfacesCommonBuilder ();
			var codeNamespace = dataAccessCommonBuilder.Build ();
			Assert.AreEqual (2, codeNamespace.Types.Count);
		}
	}
}

