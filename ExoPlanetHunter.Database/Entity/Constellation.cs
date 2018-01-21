
using ExoPlanetHunter.Database.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ExoPlanetHunter.Database.entity
{
    public class Constellation : IEntity
    {

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public virtual ICollection<Star> Stars { get; set; }
    }
}
