using NUnit.Framework;
using EaiConverter.Builder;

namespace EaiConverter.Test.Builder
{
    [TestFixture]
    public class XpathBuilderTest
    {

        IXpathBuilder xpathBuilder;

        [SetUp]
        public void SetUp(){
            this.xpathBuilder = new XpathBuilder();
        }

        [Test]
        public void Should_Replace_Simple_quote_with_double_Quote(){
            Assert.AreEqual("\"test\"", this.xpathBuilder.Build("'test'"));
        }

        [Test]
        public void Should_Replace_ASCI_quote_with_double_Quote(){
            Assert.AreEqual("\"test\"", this.xpathBuilder.Build("&quot;test&quot;"));
        }

		[Test]
		public void Should_Replace_Dash_InVariable_name(){
			Assert.AreEqual("my_test.", this.xpathBuilder.Build("$My-test/"));
		}
    }
}

