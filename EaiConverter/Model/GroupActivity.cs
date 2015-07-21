using System.Collections.Generic;

namespace EaiConverter.Model
{
    public class GroupActivity : Activity
    {
        public GroupActivity (string name, ActivityType type) : base (name, type)
        {
        }

        public GroupActivity()
        {
        }

        public GroupType GroupType { get; set;}

        public string RepeatCondition
        {
            get;
            set;
        }

        public string Over { get; set;}

        public string IterationElementSlot { get; set;}

        public string IndexSlot { get; set;}

        public List<Activity> Activities { get; set;}

        public List<Transition> Transitions { get; set;}
    }

    public enum GroupType {
        INPUTLOOP,
        SIMPLEGROUP,
        REPEAT,
        CRITICALSECTION,
        WHILE
    }
}

