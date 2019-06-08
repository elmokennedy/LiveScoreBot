using System.Collections.Generic;

namespace BotBLL.Models
{
    public class Standings
    {
        public string Type { get; set; }

        public List<Table> Table { get; set; }
    }
}
