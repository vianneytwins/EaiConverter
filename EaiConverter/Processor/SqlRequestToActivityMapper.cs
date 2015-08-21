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

        static HashSet<string> jbdcActivityNameAlreadyUsed = new HashSet<string>();

        public static int Counter = 0;

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

		public static void Clear(){
			sqlToJbdcService.Clear();
		}

        public static string GetJdbcServiceName (string sqlRequest)
        {
            string jdbcServiceName = String.Empty;
            sqlToJbdcService.TryGetValue (sqlRequest, out jdbcServiceName);
            return jdbcServiceName;
        }


        public static void SetThisJdbcActivityNameHasUsed(string activityName)
        {
            if (!jbdcActivityNameAlreadyUsed.Contains(activityName))
            {
                jbdcActivityNameAlreadyUsed.Add(activityName);
            }
        }

        public static bool IsThisJdbcActivityNameUsed(string activityName)
        {
            return jbdcActivityNameAlreadyUsed.Contains(activityName);
        }

        public static void ClearActivityHasSet()
        {
            jbdcActivityNameAlreadyUsed.Clear();
        }
    }

}

