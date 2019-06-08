using BotBLL.Models;
using System.Collections.Generic;

namespace BotBLL.Comparers
{
    public class CompetitionNameComparer : IEqualityComparer<Competition>
    {
        public bool Equals(Competition x, Competition y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(Competition obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
