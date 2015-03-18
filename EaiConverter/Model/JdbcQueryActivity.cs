using System;
using System.Collections.Generic;

namespace EaiConverter.Model
{

	public class JdbcQueryActivity : Activity
	{
		public const string jdbcCallActivityType = "com.tibco.plugin.jdbc.JDBCCallActivity";
		public const string jdbcUpdateActivityType = "com.tibco.plugin.jdbc.JDBCUpdateActivity";
		public const string jdbcQueryActivityType = "com.tibco.plugin.jdbc.JDBCQueryActivity";

		public JdbcQueryActivity (string name, string type) : base (name, type)
		{
		}

		public JdbcQueryActivity () 
		{
		}


		public int? TimeOut {get; set;}

		public bool? Commit {get; set;}
		public int? MaxRows {get; set;}
		public bool? EmptyStringAsNull {get; set;}
		public string JdbcSharedConfig {get; set;}
		public string QueryStatement {get; set;}

		/// <summary>
		/// Gets or sets the query statement parameters.
		/// Key is the parameter Name
		/// Value is the parameter Type (VARCHAR, INT, ...)
		/// </summary>
		/// <value>The query statement parameters.</value>
		public Dictionary <string,string> QueryStatementParameters  {get; set;}
	
		public string QueryOutputCachedSchemaColumns {
			get;
			set;
		}

		public int? QueryOutputCachedSchemaDataTypes {
			get;
			set;
		}

		public string QueryOutputCachedSchemaStatus {
			get;
			set;
		}
	}

}

