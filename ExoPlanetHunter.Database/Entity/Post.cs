using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExoPlanetHunter.Database.Entity
{
    public class Post
    {

        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastModified { get; set; }

        public List<Tag> Tags { get; set; }
    }
}
