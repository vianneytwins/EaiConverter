namespace EaiConverter.Model
{
    public class SetSharedVariableActivity : Activity
	{

        public SetSharedVariableActivity (string name, ActivityType type) : base (name, type)
        {
        }

        public SetSharedVariableActivity () 
        {
        }

        public string VariableConfig {get; set;}

        public bool? ShowResult {get; set;}
	}

}

