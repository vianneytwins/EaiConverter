using NUnit.Framework;

using EaiConverter.Builder;

namespace EaiConverter.Test.Builder
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

