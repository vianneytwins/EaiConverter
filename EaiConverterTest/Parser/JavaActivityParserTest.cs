using NUnit.Framework;
using EaiConverter.Model;
using EaiConverter.Parser;
using System.Xml.Linq;

namespace EaiConverter.Test.Parser
{
    [TestFixture]
    public class JavaActivityParserTest
    {
        JavaActivityParser activityParser;
        XElement doc;

        [SetUp]
        public void SetUp ()
        {
            activityParser = new JavaActivityParser ();
            var xml =
                @"<pd:activity name=""java call activity"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<pd:type>com.tibco.plugin.java.JavaActivity</pd:type>
<config>
<fileName>MyJavaFileName</fileName>
<packageName>My.Package.Name</packageName>
<fullsource> package My.Package.Name
import java.util.*;
import java.io.*;
public class MyJavaFileName{

}

</fullsource>
<inputData>
    <row>
        <fieldName>plt</fieldName>
        <fieldType>string</fieldType>
        <fieldRequired>required</fieldRequired>
    </row>
</inputData>
<outputData>
    <row>
        <fieldName>result</fieldName>
        <fieldType>date</fieldType>
        <fieldRequired>required</fieldRequired>
    </row>
</outputData>
</config>
<pd:inputBindings>
    <javaCodeActivityInput>
        <plt>
            <xsl:value-of select=""testvalue""/>
        </plt>
    </javaCodeActivityInput>
</pd:inputBindings>
</pd:activity>";
            doc = XElement.Parse(xml);
        }

        [Test]
        public void Should_Return_Activity_Type_Is_JavaActivity (){
            var activity = (JavaActivity) activityParser.Parse (doc);

            Assert.AreEqual ("com.tibco.plugin.java.JavaActivity", activity.Type.ToString());
        }

        [Test]
        public void Should_Return_Filename (){
            var activity = (JavaActivity) activityParser.Parse (doc);

            Assert.AreEqual ("MyJavaFileName", activity.FileName);
        }

        [Test]
        public void Should_Return_packageName (){
            var activity = (JavaActivity) activityParser.Parse (doc);

            Assert.AreEqual ("My.Package.Name", activity.PackageName);
        }

        [Test]
        public void Should_Return_sourceCode (){
            var activity = (JavaActivity) activityParser.Parse (doc);

            Assert.AreEqual (@" package My.Package.Name
import java.util.*;
import java.io.*;
public class MyJavaFileName{

}

", activity.FullSource);
        }

        [Test]
        public void Should_Return_Parameter(){
            var activity = (JavaActivity) activityParser.Parse (doc);

            Assert.AreEqual ("plt", activity.Parameters[0].Name);
        }

        [Test]
        public void Should_Return_inputdata(){
            var activity = (JavaActivity) activityParser.Parse (doc);

            Assert.AreEqual ("plt", activity.InputData[0].Name);
        }


        [Test]
        public void Should_Return_Outputdata(){
            var activity = (JavaActivity) activityParser.Parse (doc);

            Assert.AreEqual ("result", activity.OutputData[0].Name);
        }
    }
}

