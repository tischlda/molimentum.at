using System;
using System.Collections.Generic;

namespace Molimentum.Models
{
    public class Picture
    {
        public string Id { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public Position Position { get; set; }
        
        public IEnumerable<PictureLink> Links { get; set; }
    }
}
