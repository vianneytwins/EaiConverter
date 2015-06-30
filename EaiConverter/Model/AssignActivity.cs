namespace EaiConverter.Model
{
    public class AssignActivity : Activity
    {
        public AssignActivity (string name, ActivityType type) : base (name, type)
        {
        }

        public AssignActivity () 
        {
        }

        public string VariableName {get; set;}
    }
}

