using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CSharp;
using EaiConverter.Model;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Parser
{
	public class TibcoBWProcessLinqParser
	{
		
        XsdParser xsdParser;

        public TibcoBWProcessLinqParser(){
            this.xsdParser = new XsdParser ();
        }
        public TibcoBWProcess Parse (string filePath)
		{
			XElement allFileElement = XElement.Load(filePath);
			return this.Parse (allFileElement);
		}


		public TibcoBWProcess Parse (XElement allFileElement){

			TibcoBWProcess tibcoBwProcess = new TibcoBWProcess (
                allFileElement.Element (XmlnsConstant.tibcoPrefix + "name").Value
			);

            tibcoBwProcess.XsdImports = this.ParseXsdImports (allFileElement);

            tibcoBwProcess.StartActivity = this.ParseStartOrEndActivity (allFileElement, tibcoBwProcess.inputAndOutputNameSpace,  ActivityType.startType);

            tibcoBwProcess.EndActivity = this.ParseStartOrEndActivity (allFileElement, tibcoBwProcess.inputAndOutputNameSpace, ActivityType.endType);

            tibcoBwProcess.ProcessVariables = this.ParseProcessVariables (allFileElement);

			this.ParseTransitions (allFileElement, tibcoBwProcess);

			this.ParseActivities (allFileElement, tibcoBwProcess);

			return tibcoBwProcess;

			// TODO : As t'on interet a mettre toutes les variables dans un grosse map du style :
			// nom de l activité-> ma list de variable (en conciderant start et end comme des activités ?)
		}

        public List<XsdImport> ParseXsdImports(XElement allFileElement)
        {
            IEnumerable<XElement> xElement = from element in allFileElement.Elements (XmlnsConstant.xsdNameSpace + "import")
                select element;
            if (xElement == null) {
                return null;
            }
            var xsdImports = new List<XsdImport>();
            foreach (var element in xElement)
            {
                var xsdImport = new XsdImport
                    {
                        Namespace = element.Attribute("namespace").Value,
                        SchemaLocation = element.Attribute("schemaLocation").Value
                    };
                xsdImports.Add(xsdImport);
            }
            return xsdImports;

        }

        public Activity ParseStartOrEndActivity (XElement allFileElement, string inputAndOutputNameSpace, ActivityType activityType)
		{
			
            var xElement = allFileElement.Element (XmlnsConstant.tibcoPrefix + activityType.ToString());
			if (xElement == null) {
				return null;
			}
            string activityName = string.Empty;
            if (activityType == ActivityType.startType)
            {
                activityName = "startName";
            }
            else if (activityType == ActivityType.endType) {
                activityName = "endName";
            }
            var activity = new Activity (allFileElement.Element (XmlnsConstant.tibcoPrefix + activityName).Value, activityType);
			var activityParameters = this.xsdParser.Parse (xElement.Nodes ());
			activity.Parameters = activityParameters;
			activity.ObjectXNodes = xElement.Nodes ();

			return activity;
		}

        public List<ProcessVariable> ParseProcessVariables(XElement allFileElement)
        {
            var xElement = allFileElement.Element (XmlnsConstant.tibcoPrefix + "processVariables");
            if (xElement == null) {
                return null;
            }

            var processVariables = new List<ProcessVariable>();
            foreach (var variable in xElement.Elements())
            {
                var variableParameters = this.xsdParser.Parse (variable.Nodes ());

                var processVariable = new ProcessVariable{
                    Parameter = new ClassParameter{
                        Name = variable.Name.LocalName,
                        Type = variableParameters[0].Type
                    },
                    ObjectXNodes = variable.Nodes()
                };
                processVariables.Add(processVariable);
            }
            return processVariables;
        }

		public void ParseTransitions (XElement allFileElement, TibcoBWProcess tibcoBwProcess)
		{
            IEnumerable<XElement> transitionElements = from element in allFileElement.Elements (XmlnsConstant.tibcoPrefix + "transition")
			select element;
			tibcoBwProcess.Transitions = new List<Transition> ();
			foreach (XElement element in transitionElements) {
                var transition = new Transition {
                    FromActivity = element.Element (XmlnsConstant.tibcoPrefix + "from").Value,
                    ToActivity = element.Element (XmlnsConstant.tibcoPrefix + "to").Value,
                    ConditionType = (ConditionType)Enum.Parse (typeof(ConditionType), element.Element (XmlnsConstant.tibcoPrefix + "conditionType").Value)
                };
				tibcoBwProcess.Transitions.Add (transition);
			}
			tibcoBwProcess.Transitions.Sort ();
		}

		public void ParseActivities (XElement allFileElement, TibcoBWProcess tibcoBwProcess)
		{
            IEnumerable<XElement> activityElements = from element in allFileElement.Elements (XmlnsConstant.tibcoPrefix + "activity")
			select element;
			tibcoBwProcess.Activities = new List<Activity> ();
            var activityParserFactory = new ActivityParserFactory();

			foreach (XElement element in activityElements) {
                var activityType = element.Element (XmlnsConstant.tibcoPrefix + "type").Value;
				Activity activity;
                var activityParser = activityParserFactory.GetParser(activityType);
                if (activityParser != null ) {
                    activity = activityParser.Parse (element);
				} else {
                    activity = new Activity (element.Attribute ("name").Value, ActivityType.NotHandleYet);
				} 
				tibcoBwProcess.Activities.Add (activity);
			}
		}
	}
}

