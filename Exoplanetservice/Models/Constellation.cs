using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExoplanetService.Models
{
    public class Constellation
    {

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
