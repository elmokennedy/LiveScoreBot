using System.Collections.Generic;

namespace BotBLL.Models
{
    public class CompetitionRequest
    {
        public int Count { get; set; }

        public Competition Competition { get; set; }

        public List<Match> Matches { get; set; }

        public List<Standings> Standings { get; set; }

        public List<Scorer> Scorers { get; set; }
    }
}
