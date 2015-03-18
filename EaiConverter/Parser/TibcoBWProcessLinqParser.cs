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

			tibcoBwProcess.StartActivity = this.ParseStartOrEndActivity (allFileElement, tibcoBwProcess.inputAndOutputNameSpace,  "start");

			tibcoBwProcess.EndActivity = this.ParseStartOrEndActivity (allFileElement, tibcoBwProcess.inputAndOutputNameSpace, "end");

			this.ParseTransitions (allFileElement, tibcoBwProcess);

			this.ParseActivities (allFileElement, tibcoBwProcess);

			return tibcoBwProcess;

			// TODO : As t'on interet a mettre toutes les variables dans un grosse map du style :
			// nom de l activité-> ma list de variable (en conciderant start et end comme des activités ?)
		}

		public Activity ParseStartOrEndActivity (XElement allFileElement, string inputAndOutputNameSpace, string activityType)
		{
			var xsdParser = new XsdParser ();
			var xElement = allFileElement.Element (tibcoPrefix + activityType + "Type");
			if (xElement == null) {
				return null;
			}
			var activity = new Activity (allFileElement.Element (tibcoPrefix + activityType + "Name").Value, activityType);
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
			foreach (XElement element in activityElements) {
				var activityType = element.Element (tibcoPrefix + "type").Value;
				Activity activity;
				// TODO : faire une factory
				if (activityType == JdbcQueryActivity.jdbcQueryActivityType || activityType == JdbcQueryActivity.jdbcUpdateActivityType || activityType == JdbcQueryActivity.jdbcCallActivityType ) {
					activity = new JdbcQueryActivityParser ().Parse (element);
				} else {
					activity = new Activity (element.Attribute ("name").Value, activityType);
				} 
				tibcoBwProcess.Activities.Add (activity);
			}
		}
	}
}

