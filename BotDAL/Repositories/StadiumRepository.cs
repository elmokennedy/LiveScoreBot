using BotDAL.EF;
using BotDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotDAL.Repositories
{
    public class StadiumRepository
    {
        private BotContext context;

        public StadiumRepository()
        {
            context = new BotContext();
        }

        public Stadium GetStadiumByName(string name)
        {
            return context.Stadiums.FirstOrDefault(s => s.Name == name);
        }
    }
}
