using System;
using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Model;
using System.Collections.Generic;
using EaiConverter.Test.Utils;
using EaiConverter.Builder.Utils;

namespace EaiConverter.Test.Builder
{
    [TestFixture]
    public class ResultSetBuilderTest
    {
		private ResultSetBuilder resultSetBuilder;
		private JdbcQueryActivity jdbcQueryActivity;

		[SetUp]
		public void SetUp()
		{
			this.resultSetBuilder = new ResultSetBuilder();
			this.jdbcQueryActivity = new JdbcQueryActivity ("Currency" , ActivityType.jdbcQueryActivityType);
			jdbcQueryActivity.QueryStatement = "my_proc_stock";
			jdbcQueryActivity.ClassName = jdbcQueryActivity.Name;
			jdbcQueryActivity.QueryOutputStatementParameters = new List<ClassParameter>
			{
				new ClassParameter
				{
					Name = "FirstOutput",
					Type = "VARCHAR"
				},
				new ClassParameter
				{
					Name = "SecondOutput",
					Type = "VARCHAR"
				}
			};
		}

        [Test]
        public void Should_return_1_class()
        {
			var resultSetNameSpace = this.resultSetBuilder.Build (this.jdbcQueryActivity);
			Assert.AreEqual (1,resultSetNameSpace.Types.Count);
        }


		[Test]
		public void Should_Build_2_properties()
		{
			var resultSetNameSpace = this.resultSetBuilder.Build (this.jdbcQueryActivity);
			Assert.AreEqual (2,resultSetNameSpace.Types[0].Members.Count);
		}

		[Ignore]
		[Test]
		public void Should_generate_class()
		{
			var resultSetNameSpace = this.resultSetBuilder.Build (this.jdbcQueryActivity);
			var generatedCode = TestCodeGeneratorUtils.GenerateCode (resultSetNameSpace);
			Assert.AreEqual (@"namespace "+TargetAppNameSpaceService.domainContractNamespaceName+@"
{
    using System;
    
    
    public class CurrencyResultSet
    {
         
         public string FirstOutput
    }
", generatedCode);
		}

    }
}

