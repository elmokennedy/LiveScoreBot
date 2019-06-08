using BotBLL.Models;
using BotDAL.Repositories;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotLogic.Helpers
{
    public class MarkupCreator
    {
        UserFavouriteRepository userFavouriteRepository;

        public MarkupCreator()
        {
            userFavouriteRepository = new UserFavouriteRepository();
        }

        public static InlineKeyboardMarkup CreateChampioshipsListButtons(
            List<Competition> competitions, 
            bool isLive = false)
        {
            InlineKeyboardMarkup champioshipsListButtons;
            var inlineButtonsList = new List<List<InlineKeyboardButton>>();
            foreach (var competition in competitions)
            {
                var inlineButtons = new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        Text = competition.Name,
                        CallbackData = isLive 
                        ? "competitionLive " : "competition " 
                        + competition.Id
                    }
                };
                inlineButtonsList.Add(inlineButtons);
            }
            champioshipsListButtons = inlineButtonsList.ToArray();
            return champioshipsListButtons;
        }

        public static InlineKeyboardMarkup CreateMatchListButtons(List<Match> matches, bool isLive)
        {
            var inlineButtonsList = new List<List<InlineKeyboardButton>>();

            foreach (var match in matches)
            {
                var inlineButtons = new List<InlineKeyboardButton>();
                var matchButton = new InlineKeyboardButton
                {
                    Text = match.Status == "IN_PLAY" || match.Status == "PAUSED"
                    ? $"{match.HomeTeam.Name} : {match.AwayTeam.Name} [{match.Score.FullTime.HomeTeam}:{match.Score.FullTime.AwayTeam}]"
                    : $"{match.HomeTeam.Name} : {match.AwayTeam.Name} [{match.UtcDate}]",
                    CallbackData = isLive
                    ? "matchLive " + match.Id
                    : "match " + match.Id
                };

                inlineButtons.Add(matchButton);
                inlineButtonsList.Add(inlineButtons);
            }

            InlineKeyboardMarkup matchListButtons = inlineButtonsList.ToArray();

            return matchListButtons;
        }

        public InlineKeyboardMarkup CreateMatchDetailsButtons(int matchId, bool isLiveMatch, long chatId)
        {            
            var inlineButtonsList = new List<List<InlineKeyboardButton>>();

            var userFavourites = userFavouriteRepository.GetUserFavourites();

            if (!userFavourites.Any(fav => fav.UserId == chatId && fav.MatchId == matchId))
            {
                var addToFavouriteButton = new List<InlineKeyboardButton>
                    {
                        new InlineKeyboardButton
                        {
                            Text = "Add to Favourite",
                            CallbackData = "add " + matchId
                        }
                    };
                inlineButtonsList.Add(addToFavouriteButton);
            }
            else
            {
                var deleteFromFavouriteButton = new List<InlineKeyboardButton>
                    {
                        new InlineKeyboardButton
                        {
                            Text = "Delete from Favourite",
                            CallbackData = "delete " + matchId
                        }
                    };
                inlineButtonsList.Add(deleteFromFavouriteButton);
            }

            if (isLiveMatch)
            {
                var lineupButton = new List<InlineKeyboardButton>
                    {
                        new InlineKeyboardButton
                        {
                            Text = "See the LineUp",
                            CallbackData = "lineup " + matchId
                        }
                    };
                inlineButtonsList.Add(lineupButton);

                var statisticsButton = new List<InlineKeyboardButton>
                    {
                        new InlineKeyboardButton
                        {
                            Text = "See the statistics",
                            CallbackData = "statistics " + matchId
                        }
                    };
                inlineButtonsList.Add(statisticsButton);
            }

            InlineKeyboardMarkup myInlineKeyboard = inlineButtonsList.ToArray();

            return myInlineKeyboard;
        }

        public static InlineKeyboardMarkup CreateButtonsForPopularChampionships()
        {
            InlineKeyboardMarkup buttonsForPopularChampionships;

            var inlineButtonsList = new List<List<InlineKeyboardButton>>();

            var plButton = new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton { Text = "Premier League", CallbackData = "championshipInfoId " + 2021 }
                };

            var bundesligaButton = new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton { Text = "Bundesliga", CallbackData = "championshipInfoId " + 2012 }
                };

            var primeraButton = new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton { Text = "Primera Division", CallbackData = "championshipInfoId " + 2014 }
                };

            var seriaAButton = new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton { Text = "Serie A", CallbackData = "championshipInfoId " + 2019 }
                };

            var ligueOneButtons = new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton { Text = "Ligue 1", CallbackData = "championshipInfoId " + 2015 }
                };

            inlineButtonsList.Add(plButton);
            inlineButtonsList.Add(bundesligaButton);
            inlineButtonsList.Add(primeraButton);
            inlineButtonsList.Add(seriaAButton);
            inlineButtonsList.Add(ligueOneButtons);

            buttonsForPopularChampionships = inlineButtonsList.ToArray();

            return buttonsForPopularChampionships;
        }

        public static InlineKeyboardMarkup CreateChapmionshipDetailsButtons(int competitionId)
        {
            InlineKeyboardMarkup chapmionshipDetailsButtons;

            var inlineButtonsList = new List<List<InlineKeyboardButton>>();

            var inlineButtons = new List<InlineKeyboardButton>()
                {
                    new InlineKeyboardButton { Text = "Standings", CallbackData = "championshipStandings " + competitionId },
                    new InlineKeyboardButton { Text = "Scorers", CallbackData = "championshipScorers " + competitionId }
                };

            inlineButtonsList.Add(inlineButtons);

            chapmionshipDetailsButtons = inlineButtonsList.ToArray();

            return chapmionshipDetailsButtons;
        }

        public static InlineKeyboardMarkup CreateInfoButtons()
        {
            InlineKeyboardMarkup infoButtons;

            var inlineButtonsList = new List<List<InlineKeyboardButton>>();

            var inlineButtons = new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton { Text = "Competitions", CallbackData = "championshipInfo" },
                new InlineKeyboardButton { Text = "Stadiums", CallbackData = "stadiumInfo" }
            };

            inlineButtonsList.Add(inlineButtons);

            infoButtons = inlineButtonsList.ToArray();

            return infoButtons;
        }
    }
}
