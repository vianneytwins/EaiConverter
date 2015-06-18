using System;
using System.Collections.Generic;
using EaiConverter.Model;

namespace EaiConverter.Builder.Utils
{
	public class TransitionUtils
	{
        public static List<Transition>  GetValidTransitionsFrom(string activityName, List<Transition> transitions){
			List<Transition>  returnedTransistion = new List<Transition>();
			foreach (var transition in transitions) {
                if (transition.FromActivity == activityName && transition.ConditionType != ConditionType.error) {
					returnedTransistion.Add (transition);
				}
			}
			return returnedTransistion;
		}

        public static string ToActivityOfErrorTransitionFrom(string activityName, List<Transition> transitions)
        {
            foreach (var transition in transitions) {
                if (transition.FromActivity == activityName && transition.ConditionType ==ConditionType.error) {
                    return transition.ToActivity;
                }
            }
            return null;
        }

		// first draft based on max 2 paths
		// TODO : Increase number of possible path
		public static string GetNextCommonActivity (List<string> activityNames, List<Transition> transitions){
			// boucle sur chaque truc et reboucle sur les 2 (n) resultats pour voir si on truc en commun si oui return
			if (activityNames.Count != 2) {
				throw new NotImplementedException();
			}
            List<string> allNextActivitiesPath1 = GetAllNextActivities (activityNames[0], transitions);
            List<string> allNextActivitiesPath2 = GetAllNextActivities (activityNames[1], transitions);

			foreach (var activity in allNextActivitiesPath1) {
				if (allNextActivitiesPath2.Contains (activity)) {
					return activity;
				}
			}

            return null;
			//throw new NotImplementedException();

		}

        public static List<string> GetAllNextActivities (string activityName, List<Transition> transitions)
		{
            var transitionFrom = GetValidTransitionsFrom ( activityName, transitions);
			var nextActivities = new List<string> ();
			foreach (var transition in transitionFrom) {
				nextActivities.Add (transition.ToActivity);
                nextActivities.AddRange (GetAllNextActivities (transition.ToActivity, transitions));
			}
			return nextActivities;
		}
	}
}

