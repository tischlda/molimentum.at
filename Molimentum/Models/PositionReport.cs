using System;

namespace Molimentum.Models
{
    public class PositionReport
    {
        public string Id { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public Position Position { get; set; }

        public string Comment { get; set; }
   }
}