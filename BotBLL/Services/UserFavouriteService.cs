using BotDAL.Entities;
using BotDAL.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace BotBLL.Services
{
    public class UserFavouriteService
    {
        private UserFavouriteRepository userFavouriteRepository;

        public UserFavouriteService()
        {
            userFavouriteRepository = new UserFavouriteRepository();
        }

        public void AddUserFavourite(long userId, int matchId)
        {
            userFavouriteRepository.Add(new UserFavourite
            {
                UserId = (int)userId,
                MatchId = matchId
            });
            userFavouriteRepository.SaveChanges();
        }

        public void RemoveUserFavourite(long userId, int matchId)
        {
            var addedMatch = userFavouriteRepository.GetUserFavourites().Where(fav => fav.UserId == userId && fav.MatchId == matchId).FirstOrDefault();
            if (addedMatch != null)
            {
                userFavouriteRepository.Remove(addedMatch);
            }
            userFavouriteRepository.SaveChanges();
        }

        public List<UserFavourite> GetUserFavourites(long userId)
        {
            var allUserFavourites = userFavouriteRepository.GetUserFavourites();

            return allUserFavourites.Where(uf => uf.UserId == userId).ToList();
        }
    }
}
