using System;
using NUnit.Framework;
using EaiConverter.Parser;
using System.Xml.Linq;
using EaiConverter.Model;

namespace EaiConverterTest.Parser
{
	[TestFixture]
	public class TimerEventActivityParserTest
	{
		IActivityParser timerActivityParser;

		XElement doc;

		TimerEventActivity activity;

		[Test]
		public void SetUp()
		{
			this.timerActivityParser = new TimerEventActivityParser();
			var xml =
				@"<pd:activity name=""GetUndlCurrency"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<pd:type>com.tibco.plugin.timer.TimerEventSource</pd:type>
<config>
	<FrequencyIndex>Minute</FrequencyIndex>
	<Frequency>false</Frequency>
	<TimeInterval>10</TimeInterval>
	<StartTime>86400000</StartTime>
</config>
</pd:activity>";
			doc = XElement.Parse(xml);

			this.activity = (TimerEventActivity) this.timerActivityParser.Parse(doc);
		}

		[Test]
		public void Should_return_IntervalUnit()
		{
			Assert.AreEqual(TimerUnit.Minute, this.activity.IntervalUnit);
		}

		[Test]
		public void Should_return_RunOnce()
		{
			Assert.AreEqual(false, this.activity.RunOnce);
		}

		[Test]
		public void Should_return_TimeInterval()
		{
			Assert.AreEqual(10, this.activity.TimeInterval);
		}

		[Test]
		public void Should_return_StartTime()
		{
			Assert.AreEqual(new DateTime(1970, 1, 2), this.activity.StartTime);
		}
	}
}

