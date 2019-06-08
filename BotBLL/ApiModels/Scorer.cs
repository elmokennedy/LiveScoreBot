using System.Collections.Generic;

namespace BotBLL.Models
{
    public class Scorer
    {
        public Player Player { get; set; }

        public Team Team { get; set; }

        public int NumberOfGoals { get; set; }
    }
}
