using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Model;
using EaiConverter.Test.Utils;
using System.Xml.Linq;
using System.Collections.Generic;

namespace EaiConverter.Test.Builder
{
    [TestFixture]
    public class JavaActivityBuilderTest
    {
        JavaActivityBuilder activityBuilder;
        JavaActivity activity;

        [SetUp]
        public void SetUp()
        {
            this.activityBuilder = new JavaActivityBuilder(new XslBuilder(new XpathBuilder()));
            this.activity = new JavaActivity( "My Activity Name",ActivityType.javaActivityType);
            this.activity.FileName = "MyJavaFileName";
            this.activity.PackageName = "My.Package.Name";
            this.activity.FullSource = @" package My.Package.Name
import java.util.*;
import java.io.*;
public class MyJavaFileName{
    protected String platform = "";
    public String getplatform(){return platform;}
    public String setplatform(String val){platform = val;}

    protected Date lastDate = null;
    public String getlastDate(){return lastDate;}
    public String setlastDate(Date val){lastDate = val;}

    public MyJavaFileName(){}

public void invoke () throws Exception{
    setlastDate(System.getDate());
}
    
}

";
            var xml =
                @"
    <javaCodeActivityInput xmlns:xsl=""http://w3.org/1999/XSL/Transform"" >
        <platform>
            <xsl:value-of select=""'testvalue'""/>
        </platform>
    </javaCodeActivityInput>
";
            XElement doc = XElement.Parse(xml);
            this.activity.InputBindings = doc.Nodes();
            this.activity.Parameters = new List<ClassParameter>{
                new ClassParameter{
                    Name = "platform",
                    Type= "string"}
            };
            this.activity.InputData = new List<ClassParameter>{
                new ClassParameter{
                    Name = "platform",
                    Type= "string"}
            };
            this.activity.OutputData = new List<ClassParameter>{
                new ClassParameter{
                    Name = "lastDate",
                    Type= "date"}
            };
        }

       
        [Test]
        public void Should_Generate_invocation_method()
        {
			var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.plugin.java.JavaActivity"");
System.String platform;
platform = ""testvalue"";

My.Package.Name.MyJavaFileName myJavaFileName = new My.Package.Name.MyJavaFileName();
myJavaFileName.setplatform(platform);
myJavaFileName.Invoke();
My.Package.Name.My_Activity_Name my_Activity_Name = new My.Package.Name.My_Activity_Name();
my_Activity_Name.lastDate = myJavaFileName.getlastDate();
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(activityBuilder.GenerateMethod(this.activity, new Dictionary<string, string>()).Statements);
            Assert.AreEqual(expected,generatedCode);
        }
    }
}

