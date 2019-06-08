using System.Collections.Generic;

namespace BotBLL.Models
{
    public class MatchRequest
    {
        public int Count { get; set; }

        //public Filters Filters { get; set; }

        public List<Match> Matches { get; set; }
    }
}
