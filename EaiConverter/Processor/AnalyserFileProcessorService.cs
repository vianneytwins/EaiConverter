using System;
using EaiConverter.Parser;
using EaiConverter.Model;

namespace EaiConverter.Processor
{
    using System.IO;

    public class AnalyserFileProcessorService : IFileProcessorService
    {
        public AnalyserFileProcessorService()
        {
        }

        public void Process(string processFileName)
        {
            string projectDirectory = ConfigurationApp.GetProperty(MainClass.ProjectDirectory);

            var tibcoBwProcess = new TibcoBWProcessLinqParser().Parse(processFileName);
            WriteToFile(processFileName);
            var activities = tibcoBwProcess.Activities;

            this.ProcessActivities(projectDirectory, activities);
        }

        private static void WriteToFile(string processFileName)
        {
            Console.WriteLine(processFileName);

            using (var file = new StreamWriter("C:/Homeware/MyOuput.txt", true))
            {
                file.WriteLine(processFileName);
            }
        }

        public void ProcessActivities(string projectDirectory, System.Collections.Generic.List<Activity> activities)
        {
            foreach (var activity in activities)
            {
                if (activity.Type == ActivityType.callProcessActivityType)
                {
                    var callActivity = (CallProcessActivity)activity;
                    this.Process(projectDirectory + callActivity.ProcessName);
                }
                else if (activity.Type == ActivityType.criticalSectionGroupActivityType || activity.Type == ActivityType.loopGroupActivityType )
                {
                    var groupActivity = (GroupActivity)activity;
                    this.ProcessActivities(projectDirectory, groupActivity.Activities);
                }
            }
        }
    }
}

