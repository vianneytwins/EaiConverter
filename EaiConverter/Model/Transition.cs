using System;

namespace EaiConverter.Model
{
	public class Transition : IComparable
	{
        private string fromActivity;

        private string toActivity;

        public string FromActivity
        {
            get
            {
                return fromActivity;
            }
            set
            {
                fromActivity = value.Replace(' ','_');
            }
        }

		public string ToActivity
        {
            get
            {
                return toActivity;
            }
            set
            {
                toActivity = value.Replace(' ','_');
            }
        }

		public ConditionType ConditionType { get; set;}

		public string ConditionPredicateName { get; set; }

		public string ConditionPredicate { get; set; }

		public override string ToString ()
		{
			return string.Format ("[Transition: FromActivity={0}, ToActivity={1}, ConditionType={2}, ConditionPredicateName={3}, ConditionPredicate={4}]", FromActivity, ToActivity, ConditionType, ConditionPredicateName, ConditionPredicate);
		}

		#region IComparable implementation

		int IComparable.CompareTo (object obj)
		{
			if (obj == null) return 1;

			Transition otherTransition = obj as Transition;
			if (otherTransition != null){
				return this.ConditionType.CompareTo (otherTransition.ConditionType);
			} else {
				throw new ArgumentException ("Object is not a Transition");
			}
		}

		#endregion
	}
}

