using BotDAL.Entities;
using BotDAL.Repositories;

namespace BotBLL.Services
{
    public class StadiumService
    {
        private StadiumRepository stadiumRepository;

        public StadiumService()
        {
            stadiumRepository = new StadiumRepository();
        }

        public Stadium GetStadiumByName(string name)
        {
            return stadiumRepository.GetStadiumByName(name);
        }
    }
}
