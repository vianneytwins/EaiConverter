namespace EaiConverter.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Parser.Utils;
    using EaiConverter.Utils;

    public class JdbcQueryActivityParser : IActivityParser
    {
        public Activity Parse(XElement inputElement)
        {
            var jdbcQueryActivity = new JdbcQueryActivity();

            jdbcQueryActivity.Name = inputElement.Attribute("name").Value;
            jdbcQueryActivity.Type = (ActivityType)inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;
            var configElement = inputElement.Element("config");

            jdbcQueryActivity.TimeOut = XElementParserUtils.GetIntValue(configElement.Element("timeout"));
            jdbcQueryActivity.Commit = XElementParserUtils.GetBoolValue(configElement.Element("commit"));
            jdbcQueryActivity.MaxRows = XElementParserUtils.GetIntValue(configElement.Element("maxRows"));
            jdbcQueryActivity.EmptyStringAsNull = XElementParserUtils.GetBoolValue(configElement.Element("emptyStrAsNil"));
            jdbcQueryActivity.JdbcSharedConfig = XElementParserUtils.GetStringValue(configElement.Element("jdbcSharedConfig"));

            if (jdbcQueryActivity.Type == ActivityType.jdbcCallActivityType)
            {
                jdbcQueryActivity.QueryStatement = XElementParserUtils.GetStringValue(configElement.Element("ProcedureName"));
                if (jdbcQueryActivity.QueryStatement.Contains(";"))
                {
                    jdbcQueryActivity.QueryStatement =
                        jdbcQueryActivity.QueryStatement.Remove(jdbcQueryActivity.QueryStatement.LastIndexOf(';'), 2);
                }

                var preparedParamDataTypeElement = configElement.Element("parameterTypes");
                jdbcQueryActivity.QueryStatementParameters = new Dictionary<string, string>();

                var parameterElements = preparedParamDataTypeElement.Elements("parameter");

                jdbcQueryActivity.QueryOutputStatementParameters = new List<ClassParameter>
                                                                       {
                                                                           new ClassParameter
                                                                               {
                                                                                   Name = "UnresolvedResultsets",
                                                                                   Type = "VARCHAR"
                                                                               }
                                                                       };
                foreach (var parameterElement in parameterElements)
                {
                    string parameterName = (XElementParserUtils.GetStringValue(parameterElement.Element("colName")).Replace(".",string.Empty));
                    parameterName = VariableHelper.ToSafeType(parameterName.Substring(1, parameterName.Length - 1));
                    string parameterType = XElementParserUtils.GetStringValue(parameterElement.Element("typeName"));
                    string colonneType = XElementParserUtils.GetStringValue(parameterElement.Element("colType"));

                    //ColonneType= 1 : input parameter
                    if (colonneType == "1")
                    {
                        jdbcQueryActivity.QueryStatementParameters.Add(parameterName, parameterType);
                    }
                    //ColonneType= 4 : output parameter
                    else if (colonneType == "4" || colonneType == "2" || colonneType == "5")
                    {
                        jdbcQueryActivity.QueryOutputStatementParameters.Add(new ClassParameter { Name = parameterName, Type = parameterType });
                    }
                }

                var xElement = configElement.Element("ResultSets");
                if (xElement != null)
                {
                    jdbcQueryActivity.QueryOutputStatementParameters.AddRange(this.GetOutputParameters(xElement.Element("ResultSet")));
                }

                if (inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings") != null && inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Element("inputs") != null && inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Element("inputs").Element("inputSet") != null)
                {
                    jdbcQueryActivity.InputBindings = inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Element("inputs").Element("inputSet").Nodes();
                    jdbcQueryActivity.Parameters = new XslParser().Parse(jdbcQueryActivity.InputBindings);
                }
            }
            else
            {
                jdbcQueryActivity.QueryStatement = XElementParserUtils.GetStringValue(configElement.Element("statement"));

                var preparedParamDataTypeElement = configElement.Element("Prepared_Param_DataType");
                jdbcQueryActivity.QueryStatementParameters = new Dictionary<string, string>();

                if (preparedParamDataTypeElement != null)
                {
                    var parameterElements = preparedParamDataTypeElement.Elements("parameter");
                    foreach (var parameterElement in parameterElements)
                    {
                        jdbcQueryActivity.QueryStatementParameters.Add(
                            VariableHelper.ToSafeType(XElementParserUtils.GetStringValue(parameterElement.Element("parameterName")).Replace(".", string.Empty)),
                            XElementParserUtils.GetStringValue(parameterElement.Element("dataType")));
                    }
                }

                jdbcQueryActivity.QueryOutputStatementParameters = this.GetOutputParameters(configElement);

                if (inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings") != null && inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Element("jdbcQueryActivityInput") != null)
                {
                    jdbcQueryActivity.InputBindings = inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Element("jdbcQueryActivityInput").Nodes();
                    jdbcQueryActivity.Parameters = new XslParser().Parse(jdbcQueryActivity.InputBindings);
                }
            }
            

            return jdbcQueryActivity;
        }

        private List<ClassParameter> GetOutputParameters(XElement rootElement)
        {
            IEnumerable<XElement> queryOutPutElements = rootElement.Elements("QueryOutputCachedSchemaColumns");
            IEnumerable<XElement> typeElements = rootElement.Elements("QueryOutputCachedSchemaDataTypes");

            var parameters = new List<ClassParameter>();
            int i = 0;
            var typesList = typeElements.ToList();
            foreach (XElement element in queryOutPutElements)
            {
                parameters.Add(new ClassParameter
                                   {
                                       Name = element.Value,
                                       Type = ((XElement)typesList[i]).Value
                                   });
                i++;
            }

            return parameters;
        }
    }
}

