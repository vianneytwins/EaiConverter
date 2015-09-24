namespace EaiConverter.Processor
{
    using System;
    using System.IO;

    using EaiConverter.Model;
    using EaiConverter.Parser;

    public class AnalyserFileProcessorService : IFileProcessorService
    {
        private string projectDir;

        public AnalyserFileProcessorService()
        {
            this.projectDir = ConfigurationApp.GetProperty(MainClass.ProjectDirectory);
        }

        public void Process(string processFileName)
        {
            string projectDirectory = ConfigurationApp.GetProperty(MainClass.ProjectDirectory);

            var tibcoBwProcess = new TibcoBWProcessLinqParser().Parse(processFileName);
            WriteToFile(processFileName, this.projectDir);
            var activities = tibcoBwProcess.Activities;

            this.ProcessActivities(projectDirectory, activities);
        }

        private static void WriteToFile(string processFileName, string outputDir)
        {
            Console.WriteLine(processFileName);

            using (var file = new StreamWriter(outputDir + "/MyOuput.txt", true))
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

