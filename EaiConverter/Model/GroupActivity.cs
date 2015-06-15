using System;
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

        public string GroupType { get; set;}

        public List<Activity> Activities { get; set;}
        public List<Transition> Transitions { get; set;}

    }
}

