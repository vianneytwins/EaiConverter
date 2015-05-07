using System;
using System.Collections.Generic;
using EaiConverter.Model;
using System.Xml.Linq;

namespace EaiConverter.Model
{

	public class JdbcQueryActivity : Activity
	{
        public JdbcQueryActivity (string name, ActivityType type) : base (name, type)
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

        public IEnumerable<XNode> InputBindings {get; set;}
	}

}

