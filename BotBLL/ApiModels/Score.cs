namespace BotBLL.Models
{
    public class Score
    {
        public string Winner { get; set; }

        public string Duration { get; set; }

        public MatchTimeScore FullTime { get; set; }

        public MatchTimeScore HalfTime { get; set; }

        public MatchTimeScore ExtraTime { get; set; }

        public MatchTimeScore Penalties { get; set; }
    }
}
