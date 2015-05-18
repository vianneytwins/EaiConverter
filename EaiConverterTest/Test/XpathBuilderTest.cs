using System;
using NUnit.Framework;
using EaiConverter.Mapper;

namespace EaiConverterTest
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
    }
}

