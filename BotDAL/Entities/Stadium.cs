namespace BotDAL.Entities
{
    public class Stadium
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string PictureUrl { get; set; }

        public string HomeTeam { get; set; }

        public int Opened { get; set; }

        public int Capacity { get; set; } 

        public int FifaRate { get; set; }
    }
}
