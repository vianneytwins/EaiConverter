using System;
using NUnit.Framework;
using EaiConverter.Mapper;
using EaiConverter.Test.Utils;

namespace EaiConverterTest
{
    [TestFixture]
    public class XmlParserHelperBuilderTest
    {
        [Test]
        public void Should_Return_FromXml_Method_with_Generic_Type(){
            var expected =
                @"public class XmlParserHelperService : IXmlParserHelperService
{
    
    // Call it using this code: YourStrongTypedEntity entity = FromXml<YourStrongTypedEntity>(YourMsgString);
    public T FromXml<T>(String xml)
    
    {
        T returnedXmlClass = default(T);

        using (TextReader reader = new StringReader(xml))
        {
            returnedXmlClass = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
        }
        return returnedXmlClass ; 
    }
}
";
            var methodCodeDom = new XmlParserHelperBuilder().GenerateClass();
            Assert.AreEqual(expected,TestCodeGeneratorUtils.GenerateCode(methodCodeDom));
        }
    }
}

