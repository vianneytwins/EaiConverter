using System;
using System.Collections.Generic;
using EaiConverter.Model;

namespace EaiConverter.Mapper.Utils
{
	public class TransitionUtils
	{
		public static List<Transition>  GetTransitionsFrom(List<Transition> transitions, string activityName){
			List<Transition>  returnedTransistion = new List<Transition>();
			foreach (var transition in transitions) {
				if (transition.FromActivity == activityName) {
					returnedTransistion.Add (transition);
				}
			}
			return returnedTransistion;
		}


		// first draft based on max 2 paths
		// TODO : Increase number of possible path
		public static string GetNextCommonActivity (List<string> activityNames, List<Transition> transitions){
			// boucle sur chaque truc et reboucle sur les 2 (n) resultats pour voir si on truc en commun si oui return
			if (activityNames.Count != 2) {
				throw new NotImplementedException();
			}
			List<string> allNextActivitiesPath1 = GetAllNextActivities (transitions, activityNames[0] );
			List<string> allNextActivitiesPath2 = GetAllNextActivities (transitions, activityNames[1] );

			foreach (var activity in allNextActivitiesPath1) {
				if (allNextActivitiesPath2.Contains (activity)) {
					return activity;
				}
			}
			throw new NotImplementedException();

		}

		public static List<string> GetAllNextActivities ( List<Transition> transitions, string activityName)
		{
			var transitionFrom = GetTransitionsFrom (transitions, activityName);
			var nextActivities = new List<string> ();
			foreach (var transition in transitionFrom) {
				nextActivities.Add (transition.ToActivity);
				nextActivities.AddRange (GetAllNextActivities (transitions, transition.ToActivity));
			}
			return nextActivities;
		}
	}
}

