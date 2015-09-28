using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EaiConverterTest.Parser
{
    using EaiConverter.Parser;

    using NUnit.Framework;

    [TestFixture]
    public class AdapterSchemaParserTest
    {
        private AdapterSchemaParser adapterSchemaParser;

        [SetUp]
        public void SetUp()
        {
            this.adapterSchemaParser = new AdapterSchemaParser();
        }

        [Ignore]
        [Test]
        public void Should_Return_nameSpace_Name()
        {
            var model = this.adapterSchemaParser.Parse("c:/tibcoae-pno/AESchemas/my/namespace/is/here/schema.aeschema");
            Assert.AreEqual("my.namespace.is.here", model.NameSpace);
        }
    }
}
