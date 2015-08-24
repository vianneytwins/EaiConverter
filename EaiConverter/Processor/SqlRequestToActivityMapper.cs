using System;
using System.Collections.Generic;

namespace EaiConverter.Processor
{
    public static class SqlRequestToActivityMapper
    {
        public static int Counter = 0;

        /// <summary>
        /// Dictionnary that contains the list of SQL request and its corresponding Service
        /// </summary>
        private static readonly Dictionary<string,string> sqlToJbdcService = new Dictionary<string, string>();

        private static readonly HashSet<string> jbdcActivityNameAlreadyUsed = new HashSet<string>();

        public static void SaveSqlRequest(string sqlRequest, string serviceClassName)
        {
            if (!sqlToJbdcService.ContainsKey(sqlRequest.ToUpper()))
            {
                sqlToJbdcService.Add(sqlRequest.ToUpper(), serviceClassName);
            } 
        }

        public static bool ContainsKey(string sqlRequest)
        {
            return sqlToJbdcService.ContainsKey(sqlRequest.ToUpper());
        }

        public static int Count()
        {
            return sqlToJbdcService.Count;
        }

		public static void Clear()
        {
			sqlToJbdcService.Clear();
		}

        public static string GetJdbcServiceName(string sqlRequest)
        {
            string jdbcServiceName;
            sqlToJbdcService.TryGetValue(sqlRequest.ToUpper(), out jdbcServiceName);
            return jdbcServiceName;
        }


        public static void SetThisJdbcActivityNameHasUsed(string activityName)
        {
            if (!jbdcActivityNameAlreadyUsed.Contains(activityName.ToUpper()))
            {
                jbdcActivityNameAlreadyUsed.Add(activityName.ToUpper());
            }
        }

        public static bool IsThisJdbcActivityNameUsed(string activityName)
        {
            return jbdcActivityNameAlreadyUsed.Contains(activityName.ToUpper());
        }

        public static void ClearActivityHasSet()
        {
            jbdcActivityNameAlreadyUsed.Clear();
        }
    }

}

