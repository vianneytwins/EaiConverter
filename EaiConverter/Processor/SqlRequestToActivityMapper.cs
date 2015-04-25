using System;
using System.Collections.Generic;

namespace EaiConverter.Processor
{
    public static class SqlRequestToActivityMapper
    {
        /// <summary>
        /// Dictionnary that contains the list of SQL request and its corresponding Service
        /// </summary>
        static Dictionary<string,string> sqlToJbdcService = new Dictionary<string, string> ();

        public static void SaveSqlRequest (string sqlRequest, string serviceClassName)
        {
            if (!sqlToJbdcService.ContainsKey (sqlRequest)){
                sqlToJbdcService.Add (sqlRequest, serviceClassName);
            } 
        }

        public static bool ContainsKey (string sqlRequest){
            return sqlToJbdcService.ContainsKey(sqlRequest);
        }

        public static int Count(){
            return sqlToJbdcService.Count;
        }

        public static string GetJdbcServiceName (string sqlRequest)
        {
            string jdbcServiceName = String.Empty;
            sqlToJbdcService.TryGetValue (sqlRequest, out jdbcServiceName);
            return jdbcServiceName;
        }
    }

}

