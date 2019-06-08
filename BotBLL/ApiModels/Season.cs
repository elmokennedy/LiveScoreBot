using System;

namespace BotBLL.Models
{
    public class Season
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndtDate { get; set; }

        public int? CurrentMatchday { get; set; }

        public string Winner { get; set; }
    }
}
