using BotDAL.Entities;
using System.Data.Entity;

namespace BotDAL.EF
{
    public class BotContext : DbContext
    {
        public DbSet<UserFavourite> UserFavourites { get; set; }

        public DbSet<Stadium> Stadiums { get; set; }

        public BotContext()
            : base("LiveScoreBotConnection")
        {
        }
    }
}
