using BotBLL.Models;
using BotBLL.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using BotBLL.Infrastructure;
using BotLogic.Helpers;
using NLog;

namespace BotLogic
{
    class Program
    {
        public static TelegramBotClient botClient;
        public static MatchRequestService matchRequestService;
        public static WebClient webClient;
        public static MarkupCreator markupCreator;
        public static UserFavouriteService userFavouriteService;
        public static StadiumService stadiumService;

        public static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Debug("Bot started");

            matchRequestService = new MatchRequestService(ClientConfigurator.CreateWebClient(ConfigurationManager.AppSettings["authToken"]));
            userFavouriteService = new UserFavouriteService();
            stadiumService = new StadiumService();
            webClient = ClientConfigurator.CreateWebClient(ConfigurationManager.AppSettings["authToken"]);

            markupCreator = new MarkupCreator();

            botClient = new TelegramBotClient(ConfigurationManager.AppSettings["botToken"]);

            //event handlers
            botClient.OnMessage += Bot_OnMessage;
            botClient.OnCallbackQuery += BotClient_OnCallbackQuery;
            
            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        static async void BotClient_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            //Click on competition
            if (e.CallbackQuery.Data.Contains("competition"))
            {
                var matches = matchRequestService.GetMatchesByCompetition(e.CallbackQuery.Data);
                logger.Debug($"There was request for api.football-data.org");
                var isLive = e.CallbackQuery.Data.Contains("Live") ? true : false;
                InlineKeyboardMarkup myInlineKeyboard = MarkupCreator.CreateMatchListButtons(matches, isLive);
                await botClient.SendTextMessageAsync(
                        chatId: e.CallbackQuery.Message.Chat,
                        text: "Please select the match from choosen competition",
                        replyMarkup: myInlineKeyboard
                    );
            }

            //Click on match
            if (e.CallbackQuery.Data.Contains("match"))
            {
                var matchId = Int32.Parse(e.CallbackQuery.Data.Split(' ')[1]);
                var isLiveMatch = e.CallbackQuery.Data.Contains("matchLive") ? true : false;
                var chatId = e.CallbackQuery.Message.Chat.Id;
                var myInlineKeyboard = markupCreator.CreateMatchDetailsButtons(matchId, isLiveMatch, chatId);

                await botClient.SendTextMessageAsync(
                        chatId: e.CallbackQuery.Message.Chat,
                        text: "Select match details",
                        replyMarkup: myInlineKeyboard
                    );
            }

            //Click on add to fav
            if (e.CallbackQuery.Data.Contains("add"))
            {
                var matchId = Int32.Parse(e.CallbackQuery.Data.Split(' ')[1]);
                var chatId = e.CallbackQuery.Message.Chat.Id;

                userFavouriteService.AddUserFavourite(chatId, matchId);
                logger.Debug($"New favourite match has been added for user with id {e.CallbackQuery.Message.Chat.Id}");

                await botClient.SendTextMessageAsync(
                        chatId: e.CallbackQuery.Message.Chat,
                        text: "Match was added to Favourite!"
                    );
            }

            //Click on remove from fav
            if (e.CallbackQuery.Data.Contains("delete"))
            {
                var matchId = Int32.Parse(e.CallbackQuery.Data.Split(' ')[1]);

                userFavouriteService.RemoveUserFavourite(e.CallbackQuery.Message.Chat.Id, matchId);
                logger.Debug($"Existing favourite match has been removed by user with id {e.CallbackQuery.Message.Chat.Id}");

                await botClient.SendTextMessageAsync(
                        chatId: e.CallbackQuery.Message.Chat,
                        text: "Match was removed from Favourite!"
                    );
            }

            //Click on stadium info
            if (e.CallbackQuery.Data == "stadiumInfo")
            {
                await botClient.SendTextMessageAsync(
                        chatId: e.CallbackQuery.Message.Chat,
                        text: "Enter the name of stadium starting with 'stadium' (for example, stadium Camp Nou)"
                    );
            }

            //Click on championship info
            if (e.CallbackQuery.Data == "championshipInfo")
            {
                var buttonsForPopularChampionships = MarkupCreator.CreateButtonsForPopularChampionships();

                await botClient.SendTextMessageAsync(
                    chatId: e.CallbackQuery.Message.Chat,
                    text: "Please select the competition",
                    replyMarkup: buttonsForPopularChampionships
                );
            }

            //Click on championship from championship info
            if (e.CallbackQuery.Data.Contains("championshipInfoId"))
            {
                var competitionId = Int32.Parse(e.CallbackQuery.Data.Split(' ')[1]);

                var chapmionshipDetailsButtons = MarkupCreator.CreateChapmionshipDetailsButtons(competitionId);

                await botClient.SendTextMessageAsync(
                    chatId: e.CallbackQuery.Message.Chat,
                    text: "Please select the competition",
                    replyMarkup: chapmionshipDetailsButtons
                );
            }

            //Click on standings from championship info
            if (e.CallbackQuery.Data.Contains("championshipStandings"))
            {
                var competitionId = Int32.Parse(e.CallbackQuery.Data.Split(' ')[1]);
                string standingsResult = "";
                var standings = matchRequestService.GetStandingsByCompetition(competitionId);
                logger.Debug($"There was request for api.football-data.org");
                var totalStandings = standings.FirstOrDefault(s => s.Type == "TOTAL");
                
                foreach (var table in totalStandings.Table)
                {
                    standingsResult += $"{table.Position}. {table.Team.Name}      {table.Points} \n";
                }

                await botClient.SendTextMessageAsync(
                    chatId: e.CallbackQuery.Message.Chat,
                    text: standingsResult
                );
            }

            //Click on scorers from championship info
            if (e.CallbackQuery.Data.Contains("championshipScorers"))
            {
                var competitionId = Int32.Parse(e.CallbackQuery.Data.Split(' ')[1]);
                string scorersResult = "";
                var scorers = matchRequestService.GetScorersByCompetition(competitionId);
                logger.Debug($"There was request for api.football-data.org");
                int i = 1;

                foreach (var scorer in scorers)
                {
                    scorersResult += $"{i}. {scorer.Player.Name} ({scorer.Team.Name})      {scorer.NumberOfGoals} goals \n";
                    i++;
                }

                await botClient.SendTextMessageAsync(
                    chatId: e.CallbackQuery.Message.Chat,
                    text: scorersResult
                );
            }
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                //Bot start
                if (e.Message.Text == "/start")
                {
                    logger.Info($"User with id {e.Message.Chat.Id} started using bot");
                    await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "Welcome to LiveScoreBot! Select command entering / and follow bot's instructions."
                    );
                }

                //Live matches
                if (e.Message.Text == "/live")
                {
                    logger.Info($"User with id {e.Message.Chat.Id} requested live matches");
                    try
                    {
                        var competitionsInPlay = matchRequestService.GetCompetitions(isLive: true);
                        logger.Debug($"There was request for api.football-data.org");
                        var champioshipsListButtons = MarkupCreator.CreateChampioshipsListButtons(competitionsInPlay, isLive: true);
                        await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: "Please select the competition",
                            replyMarkup: champioshipsListButtons
                        );
                    }
                    catch (Exception)
                    {
                        logger.Error($"There has been an error getting live matches.");
                        await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "There has been an error getting live matches.");
                    }
                }

                //Scheduled matches
                if (e.Message.Text == "/schedule")
                {
                    try
                    {
                        logger.Info($"User with id {e.Message.Chat.Id} requested scheduled matches");
                        var competitions = matchRequestService.GetCompetitions();
                        logger.Debug($"There was request for api.football-data.org");
                        var champioshipsListButtons = MarkupCreator.CreateChampioshipsListButtons(competitions);
                        await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: "Please select the competition",
                            replyMarkup: champioshipsListButtons
                        );
                    }
                    catch (Exception)
                    {
                        logger.Error($"There has been an error getting scheduled matches.");
                        await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "There has been an error getting scheduled matches.");
                    }
                }

                //Favourite matches
                if (e.Message.Text == "/fav")
                {
                    try
                    {
                        logger.Info($"User with id {e.Message.Chat.Id} requested favourite matches");
                        var favouriteMatches = userFavouriteService.GetUserFavourites(e.Message.Chat.Id);
                        logger.Debug($"Favourite matches for user id {e.Message.Chat.Id} were got from Database");
                        var allMatches = matchRequestService.GetAllMatches();
                        logger.Debug($"There was request for api.football-data.org");
                        var matches = new List<Match>();

                        foreach (var match in allMatches)
                        {
                            foreach (var favouriteMatch in favouriteMatches)
                            {
                                if (favouriteMatch.MatchId == match.Id)
                                {
                                    matches.Add(match);
                                }
                            }
                        }

                        var matchListButtons = MarkupCreator.CreateMatchListButtons(matches, true);

                        await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: "Select match from favourite list",
                            replyMarkup: matchListButtons
                        );
                    }
                    catch (Exception)
                    {
                        logger.Error($"There has been an error getting favourite matches.");
                        await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "There has been an error getting favourite matches.");
                    }
                }

                //Information
                if (e.Message.Text == "/info")
                {
                    logger.Info($"User with id {e.Message.Chat.Id} requested information");
                    var infoButtons = MarkupCreator.CreateInfoButtons();

                    await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "Select what do you want to see information about",
                        replyMarkup: infoButtons
                    );
                }

                //Stadium input
                if (e.Message.Text.Contains("stadium"))
                {
                    var stadiumName = e.Message.Text.Substring(8);
                    var stadium = stadiumService.GetStadiumByName(stadiumName);
                    logger.Debug($"Stadium with name {stadiumName} were got from Database");

                    if (stadium != null)
                    {
                        await botClient.SendPhotoAsync(
                            e.Message.Chat, 
                            stadium.PictureUrl, 
                            $@"Name: {stadium.Name}
HomeTeam: {stadium.HomeTeam}
Opened: {stadium.Opened}
Capacity: {stadium.Capacity}
FifaRate: {stadium.FifaRate} stars");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "Sorry, stadium with this name hasn't been found. Try one more time!"
                    );
                    }
                }
            }
        }
    }
}

