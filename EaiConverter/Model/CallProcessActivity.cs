namespace EaiConverter.Model
{
    public class CallProcessActivity : Activity
    {
        private string processName;

        public CallProcessActivity(string name, ActivityType type) : base(name, type)
        {
        }

        public CallProcessActivity() 
        {
        }

        public string ProcessName
        {
            get
            {
                return this.processName;
            }

            set
            {
                this.processName = value;
                this.TibcoProcessToCall = new TibcoBWProcess(value);
            }
        }

        public TibcoBWProcess TibcoProcessToCall {get; private set;}
    }
}

