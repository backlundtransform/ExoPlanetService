using FluentScheduler;
using System;


namespace ExoPlanetHunter.PHL
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
