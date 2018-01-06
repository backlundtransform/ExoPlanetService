using FluentScheduler;
using System;


namespace ExoplanetService.Managers
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
