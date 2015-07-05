using System;
using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Model;
using System.CodeDom;
using System.Collections.Generic;
using EaiConverter.Test.Utils;

namespace EaiConverterTest.Builder
{
    [TestFixture]
    public class GlobalVariableBuilderTest
    {

        GlobalVariableBuilder builder;
        GlobalVariablesRepository repo;
        CodeNamespace resultNamspace;

        const string RepoName = "repoName";
        const string Package = "packageName";

        [SetUp]
        public void SetUp()
        {
            builder = new GlobalVariableBuilder();
            repo = new GlobalVariablesRepository();
            repo.Name = RepoName;
            repo.Package = Package;

            repo.GlobalVariables = new List<GlobalVariable>
            {
                new GlobalVariable
                {
                        Name = "propertyName1",
                        Type = GlobalVariableType.String,
                        Value = "my test value"
                },
                new GlobalVariable
                {
                    Name = "propertyName2",
                    Type = GlobalVariableType.Integer,
                    Value = "12"
                }
            };
            resultNamspace = builder.Build(repo);
        }

        [Test]
        public void Should_Generate_namespace_with_package_Name()
        {
            Assert.AreEqual(Package, resultNamspace.Name);
        }

        [Test]
        public void Should_Generate_Class_Name_with_property_Name()
        {
            Assert.AreEqual(RepoName, resultNamspace.Types[0].Name);
        }

        [Test]
        public void Should_Generate_2_properties()
        {
            int propertyCount = 0;
            foreach (var member in resultNamspace.Types[0].Members)
            {
                if (member.GetType() == typeof(CodeMemberProperty))
                {
                    propertyCount++;
                }
            }
            Assert.AreEqual(2, propertyCount);
        }

        [Test]
        public void Should_Generate_first_property_with_Type_String()
        {
            Assert.AreEqual("System.String", ((CodeMemberProperty)resultNamspace.Types[0].Members[0]).Type.BaseType);
        }

        [Test]
        public void Should_Generate_second_property_with_Type_Int()
        {
            Assert.AreEqual("System.Int32", ((CodeMemberProperty)resultNamspace.Types[0].Members[1]).Type.BaseType);
        }

        [Test]
        public void Should_Generate_1_Constructor()
        {
            int constructorCount = 0;
            foreach (var member in resultNamspace.Types[0].Members)
            {
                if (member.GetType() == typeof(CodeConstructor))
                {
                    constructorCount++;
                }
            }
            Assert.AreEqual(1, constructorCount);
        }

        [Test]
        public void Should_Generate_Constructor_CodeStatement()
        {
            string expected = @"this.propertyName1 = ""my test value"";
this.propertyName2 = 12;
";
            string actual = string.Empty;
            foreach (var member in resultNamspace.Types[0].Members)
            {
                if (member.GetType() == typeof(CodeConstructor))
                {
                    var constructor = (CodeConstructor) member;
                    actual = TestCodeGeneratorUtils.GenerateCode(constructor.Statements);
                }
            }
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Should_convert_stringValue_To_Int_When_Property_is_Integer()
        {
            var result = GlobalVariableBuilder.ConvertToPrimitiveType("System.Int32", "12");
            Assert.AreEqual(12, result);
        }

    }
}

