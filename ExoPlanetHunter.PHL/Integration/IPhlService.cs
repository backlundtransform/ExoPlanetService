using ExoPlanetHunter.Database.entity;
using System.Collections.Generic;

namespace ExoPlanetHunter.PHL.Integration
{
    public interface IPhlService
    {
        List<Constellation> DownloadExoData();

        void UpdateConstellations(List<Constellation> constellations);
    }
}