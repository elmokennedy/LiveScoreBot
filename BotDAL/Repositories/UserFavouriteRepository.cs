using BotDAL.EF;
using BotDAL.Entities;
using System.Collections.Generic;

namespace BotDAL.Repositories
{
    public class UserFavouriteRepository
    {
        private BotContext context;

        public UserFavouriteRepository()
        {
            context = new BotContext();
        }

        public IEnumerable<UserFavourite> GetUserFavourites()
        {
            return context.UserFavourites;
        }

        public void Add(UserFavourite entity)
        {
            context.UserFavourites.Add(entity);
        }

        public void Remove(UserFavourite entity)
        {
            context.UserFavourites.Remove(entity);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
