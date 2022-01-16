using ExoPlanetHunter.Database.entity;
using System.Collections.Generic;


namespace ExoPlanetHunter.PHL.Integration
{
    public interface IPlanetClassifier
    {
        void AddClassifiedStars(List<Star> stars);
        List<Star> DownloadAndClassifieStars();

    }
}
