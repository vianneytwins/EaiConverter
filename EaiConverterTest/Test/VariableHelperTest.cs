using System;
using NUnit.Framework;
using EaiConverter.CodeGenerator;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter
{
	[TestFixture]
	public class VariableHelperTest
	{
		[Test]
		public void Should_Return_ClassName_MonactiviteService_When_activity_name_is_mon_activite(){
			var actual = VariableHelper.ToClassName ("mon activite");
			Assert.AreEqual ("Monactivite", actual);
		}

		[Test]
		public void Should_Return_ClassName_MonActiviteService_When_activity_name_is_monMactivite(){
			var actual = VariableHelper.ToClassName ("monActivite");
			Assert.AreEqual ("MonActivite",actual);
		}

		[Test]
		public void Should_Return_myVariable_When_name_To_Convert_is_MyVariable(){
			var actual = VariableHelper.ToVariableName ("MyVariable");
			Assert.AreEqual ("myVariable",actual);
		}

        [Test]
        public void Should_Return_myVariable_When_name_To_Convert_is_My_Variable(){
            var actual = VariableHelper.ToVariableName ("My Variable");
            Assert.AreEqual ("myVariable",actual);
        }

		[Test]
		public void Should_Return_Empty_String_When_name_is_Null(){
			var actual = VariableHelper.ToVariableName (null);
			Assert.AreEqual (string.Empty,actual);
		}

		[Test]
		public void Should_Return_Empty_String_When_name_is_Empty(){
			var actual = VariableHelper.ToVariableName (string.Empty);
			Assert.AreEqual (string.Empty,actual);
		}

		[Test]
		public void Should_Return_Empty_String_When_name_is_a_WhiteSpace(){
			var actual = VariableHelper.ToVariableName (" ");
			Assert.AreEqual (string.Empty,actual);
		}
	}
}

