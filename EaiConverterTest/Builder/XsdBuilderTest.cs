using System;
using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Test.Utils;

namespace EaiConverter.Test.Builder
{
    [TestFixture]
    public class XsdBuilderTest
    {
        [Ignore]
        [Test]
        public void Should_Generate_Group_Account_Class()
        {
            var xsdNameSpacetoGenerate = new XsdBuilder().Build("./../../ressources/GlobalVariables.xsd");
            var result = TestCodeGeneratorUtils.GenerateCode(xsdNameSpacetoGenerate);
            Assert.AreEqual("", result);
        }
    }
}

