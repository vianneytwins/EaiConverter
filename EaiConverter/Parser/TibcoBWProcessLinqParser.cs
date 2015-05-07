using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CSharp;
using EaiConverter.Model;

namespace EaiConverter.Parser
{
	public class TibcoBWProcessLinqParser
	{
		public static XNamespace tibcoPrefix = "http://xmlns.tibco.com/bw/process/2003";

		public TibcoBWProcess Parse (string filePath)
		{
			XElement allFileElement = XElement.Load(filePath);
			return this.Parse (allFileElement);
		}


		public TibcoBWProcess Parse (XElement allFileElement){

			TibcoBWProcess tibcoBwProcess = new TibcoBWProcess (
				allFileElement.Element (tibcoPrefix + "name").Value
			);

            tibcoBwProcess.StartActivity = this.ParseStartOrEndActivity (allFileElement, tibcoBwProcess.inputAndOutputNameSpace,  ActivityType.startType);

            tibcoBwProcess.EndActivity = this.ParseStartOrEndActivity (allFileElement, tibcoBwProcess.inputAndOutputNameSpace, ActivityType.endType);

			this.ParseTransitions (allFileElement, tibcoBwProcess);

			this.ParseActivities (allFileElement, tibcoBwProcess);

			return tibcoBwProcess;

			// TODO : As t'on interet a mettre toutes les variables dans un grosse map du style :
			// nom de l activité-> ma list de variable (en conciderant start et end comme des activités ?)
		}

        public Activity ParseStartOrEndActivity (XElement allFileElement, string inputAndOutputNameSpace, ActivityType activityType)
		{
			var xsdParser = new XsdParser ();
            var xElement = allFileElement.Element (tibcoPrefix + activityType.ToString());
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
            var activity = new Activity (allFileElement.Element (tibcoPrefix + activityName).Value, activityType);
			var activityParameters = xsdParser.Parse (xElement.Nodes ());
			activity.Parameters = activityParameters;
			activity.ObjectXNodes = xElement.Nodes ();

			return activity;
		}

		public void ParseTransitions (XElement allFileElement, TibcoBWProcess tibcoBwProcess)
		{
			IEnumerable<XElement> transitionElements = from element in allFileElement.Elements (tibcoPrefix + "transition")
			select element;
			tibcoBwProcess.Transitions = new List<Transition> ();
			foreach (XElement element in transitionElements) {
				var transition = new Transition (element.Element (tibcoPrefix + "from").Value, element.Element (tibcoPrefix + "to").Value, (ConditionType)Enum.Parse (typeof(ConditionType), element.Element (tibcoPrefix + "conditionType").Value));
				tibcoBwProcess.Transitions.Add (transition);
			}
			tibcoBwProcess.Transitions.Sort ();
		}

		public void ParseActivities (XElement allFileElement, TibcoBWProcess tibcoBwProcess)
		{
			IEnumerable<XElement> activityElements = from element in allFileElement.Elements (tibcoPrefix + "activity")
			select element;
			tibcoBwProcess.Activities = new List<Activity> ();
            var activityParserFactory = new ActivityParserFactory();

			foreach (XElement element in activityElements) {
				var activityType = element.Element (tibcoPrefix + "type").Value;
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

