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
        ActivityParserFactory activityParserFactory;

        public TibcoBWProcessLinqParser(){
            this.xsdParser = new XsdParser ();
            this.activityParserFactory = new ActivityParserFactory();
        }
        public TibcoBWProcess Parse (string filePath)
		{
			XElement allFileElement = XElement.Load(filePath);
			return this.Parse (allFileElement);
		}


		public TibcoBWProcess Parse (XElement allFileElement){

			TibcoBWProcess tibcoBwProcess = new TibcoBWProcess (
                allFileElement.Element (XmlnsConstant.tibcoProcessNameSpace + "name").Value
			);

            if (allFileElement.Element(XmlnsConstant.tibcoProcessNameSpace + "label") != null)
            {
                tibcoBwProcess.Description = XElementParserUtils.GetStringValue(allFileElement.Element(XmlnsConstant.tibcoProcessNameSpace + "label").Element(XmlnsConstant.tibcoProcessNameSpace + "description"));
            }
            tibcoBwProcess.XsdImports = this.ParseXsdImports (allFileElement);

            tibcoBwProcess.StartActivity = this.ParseStartOrEndActivity (allFileElement, tibcoBwProcess.InputAndOutputNameSpace,  ActivityType.startType);

            tibcoBwProcess.StarterActivity = this.ParseStarterActivity (allFileElement);

            tibcoBwProcess.EndActivity = this.ParseStartOrEndActivity (allFileElement, tibcoBwProcess.InputAndOutputNameSpace, ActivityType.endType);

            tibcoBwProcess.ProcessVariables = this.ParseProcessVariables (allFileElement);

            tibcoBwProcess.Transitions = this.ParseTransitions (allFileElement);

            tibcoBwProcess.Activities = this.ParseActivities (allFileElement);

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
			
            var xElement = allFileElement.Element (XmlnsConstant.tibcoProcessNameSpace + activityType.ToString());
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
            var activity = new Activity (allFileElement.Element (XmlnsConstant.tibcoProcessNameSpace + activityName).Value, activityType);
            if (xElement.Attribute("ref") == null)
            {
                var activityParameters = this.xsdParser.Parse (xElement.Nodes (), inputAndOutputNameSpace);
		    	activity.Parameters = activityParameters;
			    activity.ObjectXNodes = xElement.Nodes ();
            }
            else
            {
                var inputReferences = xElement.Attribute("ref").Value.Split(':');

                activity.Parameters = new List<ClassParameter>
                {
                        new ClassParameter{
                            Name = inputReferences[1],
                            // TODO : find out to convert prefix in type
                            Type = inputReferences[0]
                        }
                };
            }
			return activity;
		}

        public Activity ParseStarterActivity(XElement allFileElement)
        {
            var element = allFileElement.Element (XmlnsConstant.tibcoProcessNameSpace + "starter");

            if (element ==null)
            {
                return null;
            }
            var activity = this.ParseActivity(element);
            return activity;
        }

        public List<ProcessVariable> ParseProcessVariables(XElement allFileElement)
        {
            var xElement = allFileElement.Element (XmlnsConstant.tibcoProcessNameSpace + "processVariables");
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

        public List<Transition> ParseTransitions (XElement allFileElement)
		{
            IEnumerable<XElement> transitionElements = from element in allFileElement.Elements (XmlnsConstant.tibcoProcessNameSpace + "transition")
			select element;
			var transitions = new List<Transition> ();
			foreach (XElement element in transitionElements) {
                var transition = new Transition {
                    FromActivity = element.Element (XmlnsConstant.tibcoProcessNameSpace + "from").Value,
                    ToActivity = element.Element (XmlnsConstant.tibcoProcessNameSpace + "to").Value,
                    ConditionType = (ConditionType)Enum.Parse (typeof(ConditionType), element.Element (XmlnsConstant.tibcoProcessNameSpace + "conditionType").Value)
                };
				transitions.Add (transition);
			}

			transitions.Sort ();
            return transitions;
		}

        public List<Activity> ParseActivities (XElement allFileElement)
		{
			var activities = new List<Activity> ();
            activities.AddRange(this.ParseStandardActivities(allFileElement));
            activities.AddRange(this.ParseGroupActivities(allFileElement));

            return activities;
		}

        public List<Activity> ParseStandardActivities (XElement allFileElement)
        {
            IEnumerable<XElement> activityElements = from element in allFileElement.Elements (XmlnsConstant.tibcoProcessNameSpace + "activity")
                select element;
            var activities = new List<Activity> ();

            foreach (XElement element in activityElements) {
                var activity = this.ParseActivity(element);
                activities.Add (activity);
            }
            return activities;
        }

        public List<Activity> ParseGroupActivities (XElement allFileElement)
        {
            IEnumerable<XElement> groupElements = from element in allFileElement.Elements (XmlnsConstant.tibcoProcessNameSpace + "group")
                select element;
            var activities = new List<Activity> ();

            foreach (XElement element in groupElements)
            {
                GroupActivity activity = new GroupActivity();
                activity.Name = element.Attribute ("name").Value;
                activity.Type = (ActivityType) element.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;
                var configElement = element.Element ("config");

                var groupTypeString = XElementParserUtils.GetStringValue(configElement.Element(XmlnsConstant.tibcoProcessNameSpace +"groupType"));
                activity.GroupType = (GroupType) Enum.Parse(typeof(GroupType), groupTypeString);

                if (activity.GroupType == GroupType.inputLoop)
                {
                    activity.Over = XElementParserUtils.GetStringValue(configElement.Element(XmlnsConstant.tibcoProcessNameSpace +"over"));
                    activity.IterationElementSlot = XElementParserUtils.GetStringValue(configElement.Element(XmlnsConstant.tibcoProcessNameSpace +"iterationElementSlot"));
                    activity.IndexSlot = XElementParserUtils.GetStringValue(configElement.Element(XmlnsConstant.tibcoProcessNameSpace +"indexSlot"));
                    activity.AccumulateOutput= XElementParserUtils.GetBoolValue(configElement.Element(XmlnsConstant.tibcoProcessNameSpace +"accumulateOutput"));
                }

                if (element.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings") != null)
                {
                    activity.InputBindings = element.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Nodes();
                }

                activity.Transitions = this.ParseTransitions(element);
                activity.Activities = this.ParseStandardActivities(element);
                activities.Add (activity);
            }

            return activities;
         }

        Activity ParseActivity(XElement element)
        {
            var activityType = element.Element(XmlnsConstant.tibcoProcessNameSpace + "type").Value;
            Activity activity;
            var activityParser = this.activityParserFactory.GetParser(activityType);
            if (activityParser != null)
            {
                activity = activityParser.Parse(element);
            }
            else
            {
                activity = new Activity(element.Attribute("name").Value, ActivityType.NotHandleYet);
            }
            return activity;
        }
	}
}

