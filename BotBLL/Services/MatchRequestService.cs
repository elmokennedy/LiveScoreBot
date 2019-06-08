using BotBLL.Comparers;
using BotBLL.Models;
using BotDAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace BotBLL.Services
{
    public class MatchRequestService
    {
        public UserFavouriteRepository _userFavouriteRepository;
        public WebClient webClient;

        public MatchRequestService(WebClient botWebClient)
        {
            _userFavouriteRepository = new UserFavouriteRepository();

            webClient = botWebClient;
        }

        public List<Match> GetAllMatches()
        {
            string jsonResult;
            using (webClient)
            {
                jsonResult = webClient.DownloadString("https://api.football-data.org/v2/matches");
            }

            var serializer = new JavaScriptSerializer();
            var matchRequest = serializer.Deserialize<MatchRequest>(jsonResult);

            return matchRequest.Matches;
        }

        public List<Match> GetMatchesByCompetition(string query)
        {
            var competitionId = Int32.Parse(query.Split(' ')[1]);

            string jsonResult;
            using (webClient)
            {
                jsonResult = webClient.DownloadString($"https://api.football-data.org/v2/competitions/" + competitionId + "/matches");
            }
            var serializer = new JavaScriptSerializer();
            var competitionRequest = serializer.Deserialize<CompetitionRequest>(jsonResult);

            var matches = query.Contains("Live")
                ? competitionRequest.Matches.Where(m => m.Status == "IN_PLAY" || m.Status == "PAUSED")
                : competitionRequest.Matches.Where(m => m.UtcDate >= DateTime.UtcNow && m.UtcDate <= DateTime.UtcNow.AddDays(10));

            return matches.ToList();
        }

        public List<Competition> GetCompetitions(bool isLive = false)
        {
            string jsonResult;
            using (webClient)
            {
                jsonResult = webClient.DownloadString("https://api.football-data.org/v2/matches");
            }

            var serializer = new JavaScriptSerializer();
            var matchRequest = serializer.Deserialize<MatchRequest>(jsonResult);

            if (isLive)
            {
                var competitionsInPlay = matchRequest.Matches
                            .Where(m => m.Status == "IN_PLAY" || m.Status == "PAUSED")
                            .Select(m => m.Competition)
                            .Distinct(new CompetitionNameComparer());

                return competitionsInPlay.ToList();
            }

            var competitions = matchRequest.Matches
                            .Where(m => m.Status == "SCHEDULED")
                            .Select(m => m.Competition)
                            .Distinct(new CompetitionNameComparer());

            return competitions.ToList();
        }

        public List<Standings> GetStandingsByCompetition(int competitionId)
        {
            string jsonResult;
            using (webClient)
            {
                jsonResult = webClient.DownloadString("https://api.football-data.org/v2/competitions/" + competitionId + "/standings");
            }

            var serializer = new JavaScriptSerializer();
            var competitionRequest = serializer.Deserialize<CompetitionRequest>(jsonResult);

            return competitionRequest.Standings;
        }

        public List<Scorer> GetScorersByCompetition(int competitionId)
        {
            string jsonResult;
            using (webClient)
            {
                jsonResult = webClient.DownloadString("https://api.football-data.org/v2/competitions/" + competitionId + "/scorers");
            }

            var serializer = new JavaScriptSerializer();
            var competitionRequest = serializer.Deserialize<CompetitionRequest>(jsonResult);

            return competitionRequest.Scorers;
        }
    }
}
