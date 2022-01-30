using ExoPlanetHunter.PHL.Integration;
using FluentScheduler;
using System;
using System.Threading.Tasks;

namespace ExoPlanetHunter.PHL.Schedules
{
    public class MyJob : IJob
    {
        private readonly IPhlService _phlService = new PhlService();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MyJob));
        private readonly IPlanetClassifier _planetClassifier = new PlanetClassifier();
        public void Execute()
        {
            ExecuteAsync().Wait();
        }

        private async Task ExecuteAsync()
        {

            try
            {
                log.Info("Start Dowload");
                var constellations = await Task.FromResult(_phlService.DownloadExoData());
                log.Info("Dowload Finished");
                await Task.Run(() => _phlService.UpdateConstellations(constellations));
                log.Info("Database Updated");
            }
            catch (Exception e)
            {
                log.Info(e.Message);
            }
         
        }

        public void Execute2()
        {
            Execute2Async().Wait();
        }

        private async Task Execute2Async()
        {

            try
            {
                log.Info("Start Dowload");
                var stars = await Task.FromResult(_planetClassifier.DownloadAndClassifieStars());
                log.Info("Dowload Finished");
                await Task.Run(() => _planetClassifier.AddClassifiedStars(stars));
                log.Info("Database Updated");
            }
            catch (Exception e)
            {
                log.Info(e.Message);
            }

        }
    }
}