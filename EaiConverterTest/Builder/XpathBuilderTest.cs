using NUnit.Framework;
using EaiConverter.Builder;

namespace EaiConverter.Test.Builder
{
    [TestFixture]
    public class XpathBuilderTest
    {

        IXpathBuilder xpathBuilder;

        [SetUp]
        public void SetUp()
        {
            this.xpathBuilder = new XpathBuilder();
        }

        [Test]
        public void Should_Replace_Simple_quote_with_double_Quote(){
            Assert.AreEqual("\"test\"", this.xpathBuilder.Build("'test'"));
        }
        
        [Test]
        public void Should_Replace_Simple_quote_with_double_Quote_and_value_is_empty(){
            Assert.AreEqual("\"\"", this.xpathBuilder.Build("''"));
        }
        
        [Test]
        public void Should_Replace_Simple_quote_with_double_Quote2(){
            Assert.AreEqual("\"te\\\"st\"", this.xpathBuilder.Build("'te\"st'"));
        }

        [Test]
        public void Should_escape_double_quote_inside_expression(){
			Assert.AreEqual(@"""test""", this.xpathBuilder.Build("\"test\""));
        }

        [Test]
        public void Should_escape_double_quote_inside_expression5()
        {
            Assert.AreEqual(@"""te'st""", this.xpathBuilder.Build("\"te'st\""));
        }

        [Test]
        public void Should_escape_double_quote_inside_expression2()
        {
            Assert.AreEqual(@"Concat(""test"")", this.xpathBuilder.Build(@"Concat(""test"")"));
        }

        [Test]
        public void Should_escape_double_quote_inside_expression3()
        {
            Assert.AreEqual(@"Concat(""te\""st"")", this.xpathBuilder.Build(@"Concat('te""st')"));
        }

        [Test]
        public void Should_escape_double_quote_inside_expression4()
        {
            Assert.AreEqual(@"Concat(""te\""s\""t"")", this.xpathBuilder.Build(@"Concat('te""s""t')"));
        }
        
        [Test]
		public void Should_Replace_Dash_InVariable_name(){
			Assert.AreEqual("my_test.", this.xpathBuilder.Build("$My-test/"));
		}

        [Test]
        public void Should_Replace_rename_startactivity_variable()
        {
            Assert.AreEqual("start_root", this.xpathBuilder.Build("$Start/root"));
            Assert.AreEqual("start_root", this.xpathBuilder.Build("$start/root"));
        }

        [Test]
        public void Should_Remove_global_variable_prefix()
        {
            Assert.AreEqual("GlobalVariables", this.xpathBuilder.Build("$_globalVariables/ns:GlobalVariables"));
        }
     
        [Test]
        public void Should_Remove_global_variable_prefix_preceded_by_a_comma()
        {
            Assert.AreEqual(",GlobalVariables.DAI.PNO.Project.Platform", this.xpathBuilder.Build(",$_globalVariables/ns:GlobalVariables/DAI/PNO/Project/Platform"));
        }
    }
}

