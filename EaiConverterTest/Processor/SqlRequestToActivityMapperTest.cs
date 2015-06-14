using System;
using NUnit.Framework;
using EaiConverter.Processor;

namespace EaiConverter.Test.Processor
{
    [TestFixture]
    public class SqlRequestToActivityMapperTest
    {
        [Test]
        public void Should_Add_sql_request_To_Dico_When_Sql_request_doesnt_exist(){
            SqlRequestToActivityMapper.SaveSqlRequest("select 1", "ServiceClass1");
            Assert.AreEqual(1, SqlRequestToActivityMapper.Count());
        }

        [Test]
        public void Should_Not_Add_sql_request_To_Dico_When_Sql_request_does_exist(){
            SqlRequestToActivityMapper.SaveSqlRequest("select 1", "ServiceClass1");
            SqlRequestToActivityMapper.SaveSqlRequest("select 1", "ServiceClass2");
            Assert.AreEqual(1, SqlRequestToActivityMapper.Count());
        }

        [Test]
        public void Should_return_First_serviceName_When_Sql_request_does_exist(){
            SqlRequestToActivityMapper.SaveSqlRequest("select 1", "ServiceClass1");
            SqlRequestToActivityMapper.SaveSqlRequest("select 1", "ServiceClass2");
            Assert.AreEqual("ServiceClass1", SqlRequestToActivityMapper.GetJdbcServiceName("select 1"));
        }

    }
}

