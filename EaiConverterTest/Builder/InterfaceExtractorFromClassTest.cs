namespace EaiConverterTest.Builder
{
    using System.CodeDom;

    using EaiConverter.Builder;

    using NUnit.Framework;

    [TestFixture]
    public class InterfaceExtractorFromClassTest
    {
        private CodeTypeDeclaration classToConvertInInterface;

        private string namespaceName = "myNewNameSpace";

        [SetUp]
        public void SetUp()
        {
            this.classToConvertInInterface = new CodeTypeDeclaration
            {
                Name = "MyClass",
                IsClass = true
            };
            classToConvertInInterface.Members.Add(new CodeMemberMethod
                                                      {
                                                          Attributes = MemberAttributes.Private | MemberAttributes.Final,
                                                          Name = "MyPrivateMethod",
                                                          ReturnType = new CodeTypeReference("void")
                                                      });
            classToConvertInInterface.Members.Add(new CodeMemberMethod
                                                      {
                                                          Attributes = MemberAttributes.Public | MemberAttributes.Final,
                                                          Name = "MyPublicMethod",
                                                          ReturnType = new CodeTypeReference("void")
                                                      });
             

        }

        [Test]
        public void Should_remove_private_method()
        {
            var codeNamespace = InterfaceExtractorFromClass.Extract(this.classToConvertInInterface, this.namespaceName);
            Assert.AreEqual(1, codeNamespace.Types[0].Members.Count);
        }

        [Test]
        public void Should_set_new_namespaceName()
        {
            var codeNamespace = InterfaceExtractorFromClass.Extract(this.classToConvertInInterface, this.namespaceName);
            Assert.AreEqual(this.namespaceName, codeNamespace.Name);
        }
    }
}
