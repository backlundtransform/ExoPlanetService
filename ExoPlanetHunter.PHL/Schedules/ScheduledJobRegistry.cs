using FluentScheduler;
using System;


namespace ExoPlanetHunter.PHL.Schedules
{
    public class ScheduledJobRegistry : Registry
    {
        public ScheduledJobRegistry(int week)
        {
            Schedule<MyJob>()
                    .NonReentrant()
                    .ToRunOnceAt(DateTime.Now)
                    .AndEvery(week).Weeks();
        }
    }
}
