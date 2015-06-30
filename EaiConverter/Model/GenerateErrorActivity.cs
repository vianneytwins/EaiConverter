namespace EaiConverter.Model
{
    public class GenerateErrorActivity : Activity
    {
        public GenerateErrorActivity (string name, ActivityType type) : base (name, type)
        {
        }

        public GenerateErrorActivity () 
        {
        }

        public string FaultName {get; set;}
    }
}

