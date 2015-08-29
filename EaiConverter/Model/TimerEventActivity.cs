using System;

namespace EaiConverter.Model
{
    public class TimerEventActivity : Activity
    {
        /// <summary>
        /// Indicates this process should be run only once at the day and time indicated by the Start Time field.
        /// If unchecked, the TimeInterval and IntervalUnit fields allow you to specify the frequency of the process.
        /// </summary>
        /// <value>The run once.</value>
        public bool? RunOnce { get; set; }

        public DateTime StartTime { get; set; }

        /// <summary>
        /// Integer indicating the number of units specified in the Interval Unit field
        /// </summary>
        /// <value>The time interval.</value>
        public int? TimeInterval { get; set; }

        /// <summary>
        /// Unit of time to use with the Time Interval field to determine how often to start a new process.
        /// The units can be: Millisecond, Second, Minute, Hour, Day, Week, Month, Year.
        /// </summary>
        /// <value>The interval unit.</value>
        public TimerUnit IntervalUnit { get; set; }

    }

    public enum TimerUnit {
        Millisecond,
        Second,
        Minute,
        Hour,
        Day,
        Week,
        Month,
        Year
    }
}
