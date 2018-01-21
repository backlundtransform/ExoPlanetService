﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Pocos
{
    public class Constellation
    {

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public virtual ICollection<Star> Stars { get; set; }
    }
}
