using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlanetHunter.Service.Dto
{
   public class StatisticsDto
    {
        public int ConfirmedPlanets { get; set; }

        public int ConfirmedHabitablePlanets { get; set; }

        public int PossibleHabitableMoons { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}
