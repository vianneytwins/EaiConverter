namespace EaiConverter.Test.Builder
{
    using EaiConverter.Builder;

    using NUnit.Framework;

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
        public void Should_Remove_prefix()
        {
            Assert.AreEqual("start_root", this.xpathBuilder.Build("$Start/pfx1:root"));
        }

        [Test]
        public void Should_Remove_dollars_variable_And_put_next_char_to_lower()
        {
            Assert.AreEqual("iterator.value > 3 || TibcoXslHelper.StringLength(isPresentCheck.result.Id)>0", this.xpathBuilder.Build("$Iterator/value > 3 or string-length($IsPresentCheck/result/Id)>0"));
        }
        
        [Test]
        public void Should_Remove_prefix_complex_usecase()
        {
            Assert.AreEqual(@"TibcoXslHelper.Concat(getInstanceInfo.out.GetProcessInstanceInfo[1].ProcessInstanceName,"" "", getInstanceInfo.out.GetProcessInstanceInfo[1].MainProcessName,"""", start_logInfo.message,"""", TibcoXslHelper.RenderXml((getExceptions.out, true))",
                this.xpathBuilder.Build("concat($GetInstanceInfo/pfx4:out/pfx4:GetProcessInstanceInfo[1]/ProcessInstanceName,' ', $GetInstanceInfo/pfx4:out/pfx4:GetProcessInstanceInfo[1]/MainProcessName,'', $Start/pfx3:logInfo/message,'', tib:render-xml($GetExceptions/pfx4:out, true()))"));
        }
        
        [Test]
        public void Should_Reformat_process_info_element()
        {
            Assert.AreEqual(@"TibcoXslHelper.Concat(getInstanceInfo[1].ProcessInstanceName,"" "")",
                this.xpathBuilder.Build("concat($GetInstanceInfo/pfx4:output/pfx4:GetProcessInstanceInfo[1]/ProcessInstanceName,' ')"));
        }
        
        [Test]
        public void Should_Reformat_process_exception_element()
        {
            Assert.AreEqual(@"getExceptions[1].ProcessDefinitionName", this.xpathBuilder.Build("$GetExceptions/pfx4:output/pfx4:GetProcessInstanceExceptions[1]/ProcessDefinitionName"));
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

