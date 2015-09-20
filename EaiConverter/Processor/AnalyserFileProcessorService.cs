using System;
using EaiConverter.Parser;
using EaiConverter.Model;

namespace EaiConverter.Processor
{
    public class AnalyserFileProcessorService : IFileProcessorService
    {
        public AnalyserFileProcessorService()
        {
        }

        public void Process (string fileName)
        {
            string projectDirectory = ConfigurationApp.GetProperty(MainClass.ProjectDirectory);

            var tibcoBwProcess = new TibcoBWProcessLinqParser().Parse (fileName);
            Console.WriteLine(fileName);
            var activities = tibcoBwProcess.Activities;

            ProcessActivities(projectDirectory, activities);
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

