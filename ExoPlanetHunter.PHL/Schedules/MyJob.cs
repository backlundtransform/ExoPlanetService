using ExoPlanetHunter.PHL.Integration;
using FluentScheduler;
using System.Threading.Tasks;

namespace ExoPlanetHunter.PHL.Schedules
{
    public class MyJob : IJob
    {
        private readonly IPhlService _phlService = new PhlService();

        public void Execute()
        {
            ExecuteAsync().Wait();
        }

        private async Task ExecuteAsync()
        {
           var constellations = await Task.FromResult(_phlService.DownloadExoData());
            await Task.Run(()=>_phlService.UpdateConstellations(constellations));
        }
    }
}