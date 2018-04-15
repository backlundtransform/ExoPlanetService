using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExoPlanetHunter.Database.Entity
{
   public class Tag
   {
       [Key]
       public int Id { get; set; }

       public string Name { get; set; }
    }
}
