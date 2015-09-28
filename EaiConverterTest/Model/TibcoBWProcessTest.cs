using NUnit.Framework;
using EaiConverter.Model;

namespace EaiConverter.Test.Model
{
	[TestFixture]
	public class TibcoBWProcessTest
	{
		[Test]
		public void Should_Return_ShortNameSpace_TOTO_When_fullProcessName_is_TOTO_slash_MyProcess_dot_process()
		{
			Assert.AreEqual ("TOTO", new TibcoBWProcess ("TOTO/myProcess.process").ShortNameSpace);
		}

		[Test]
		public void Should_Return_ShortNameSpace_TOTOdotTITI_When_fullProcessName_is_TOTO_slash_TITI_slash_MyProcess_dot_process()
		{
			Assert.AreEqual ("TOTO.TITI", new TibcoBWProcess ("TOTO/TITI/myProcess.process").ShortNameSpace);
		}

        [Test]
        public void Should_Return_remove_first_slash()
        {
            Assert.AreEqual("TOTO.TITI", new TibcoBWProcess("/TOTO/TITI/myProcess.process").ShortNameSpace);
        }

		[Test]
		public void Should_Return_ProcessName_myProcess_When_fullProcessName_is_TOTO_slash_MyProcess_dot_process()
		{
			Assert.AreEqual("myProcess", new TibcoBWProcess("TOTO/myProcess.processjkljh").ProcessName);
		}

        [Test]
        public void Should_Return_ProcessName_When_processName_contains_a_space()
        {
            Assert.AreEqual("myProcess", new TibcoBWProcess("TOTO/my Process.process").ProcessName);
        }

		[Test]
		public void Should_Return_ProcessName_PrMapRM3DtoPNOEquitytoEquity_When_fullProcessName_is_fullOfSlashDashAndDot()
		{
			Assert.AreEqual ("PrMapRM3DtoPNOEquitytoEquity", new TibcoBWProcess ("Process/DAI/PNO/Mapping/Common/PrMap.RM3D-to-PNO.Equity-to-Equity.process").ProcessName);
		}

        [Test]
        public void Should_Return_ProcessName_MyProcessName_When_fullProcessName_has_no_Slash_And_No_Dot()
        {
            Assert.AreEqual ("MyProcessName", new TibcoBWProcess ("MyProcessName").ProcessName);
        }
            

	}
}

