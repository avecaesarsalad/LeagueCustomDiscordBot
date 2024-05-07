using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueCustomBot.resx;

namespace LeagueCustomBot.teamcreator
{
    public class TeamCreator
    {
        private static TeamCreator? _instance;
        private static readonly Random Random = new(Guid.NewGuid().GetHashCode());


        private bool _lobbyRunning;
        private string _lobbyStartedByName = ""; 
        private bool _rolled;
        private const int LobbyCount = 10;

        private List<Player> _allPlayers = [];
        private List<Player> _redPlayers = [];
        private List<Player> _bluePlayers = [];

        public static TeamCreator Instance
        {
            get { return _instance ??= new TeamCreator(); }
        }

        public bool AddPlayerToList(Player player)
        {
            
            if (!_lobbyRunning) return false;

            if (_allPlayers.Count >= LobbyCount) return false;

            //if (_allPlayers.FirstOrDefault(x => x.Name == player.Name) != null) return false;

            _allPlayers.Add(player);
            return true;
        }

        public bool RemovePlayerFromList(string name)
        {
            if (!_lobbyRunning) return false;
            
            var playerToRemove = _allPlayers.FirstOrDefault(x => x.Name == name);
            if (playerToRemove == null) return false;
            _allPlayers.Remove(playerToRemove);
            return true;
        }

        public bool ChangePlayerInformation(Player newPlayerInfo)
        {
            if (!_lobbyRunning) return false;

            var playerToChange = _allPlayers.FirstOrDefault(x => x.Name == newPlayerInfo.Name);
            if(playerToChange == null) return false;

            playerToChange.FirstRole = newPlayerInfo.FirstRole;
            playerToChange.SecondRole = newPlayerInfo.SecondRole;
            playerToChange.Rank = newPlayerInfo.Rank;
            return true;
        }

        public bool PlayerExists(string name)
        {
            return _allPlayers.FirstOrDefault(x => x.Name == name) != null;
        }

        public bool Roll()
        {
            if (!_lobbyRunning) return false;

            if (_allPlayers.Count < LobbyCount) return false;
            
            RollTeams();
            RollRoles(_redPlayers);
            RollRoles(_bluePlayers);
            return true;
        }

        private void RollTeams()
        {
            for (var i = 0; i <= (LobbyCount/2); i++)
            {
                for (var x = 0; x < LobbyCount; x++)
                {
                    RollPlayers();
                    var td = GetTeamScoreDifference();
                    if (td <= i) return;
                }
            }
        }
        
        /*
         * Robin: Support, Mid => Mid
         * AveCaesarSalad: Mid, Top => Support
         */

        private void RollRoles(List<Player> team)
        {
            var randomTeam = Shuffle(team);
            ResetRoles(randomTeam);
            AssignFirstRoles(randomTeam);
            AssignSecondRoles(randomTeam);
            AssignRandomRoles(randomTeam);
            team = randomTeam;
        }


        private void ResetRoles(List<Player> team)
        {
            team.ForEach(player => player.SelectedRole = Role.Fill);
        }

        private void AssignFirstRoles(List<Player> team)
        {
            foreach (var player in team.Where(player => player.SelectedRole == Role.Fill 
                                                        && player.FirstRole != Role.Fill 
                                                        && RoleFree(team, player.FirstRole)))
            {
                player.SelectedRole = player.FirstRole;
            }
        }

        private void AssignSecondRoles(List<Player> team)
        {
            foreach (var player in team.Where(player => player.SelectedRole == Role.Fill 
                                                        && player.SecondRole != Role.Fill 
                                                        && RoleFree(team, player.SecondRole)))
            {
                player.SelectedRole = player.SecondRole;
            }
        }

        private void AssignRandomRoles(List<Player> team)
        {
            foreach (var player in team.Where(player => player.SelectedRole == Role.Fill))
            {
                for (var i = 1; i < 6; i++)
                {
                    var roleFree = RoleFree(team, (Role)i);
                    if (roleFree) player.SelectedRole = (Role)i;
                }
            }
        }

        private bool RoleFree(List<Player> team, Role role)
        {
            return team.All(x => x.SelectedRole != role);
        }

        private List<Player> Shuffle(List<Player> list)
        {
            var shuffledList = new List<Player>(list);

            var n = shuffledList.Count;
            while (n > 1)
            {
                n--;
                var k = Random.Next(n + 1);
                (shuffledList[k], shuffledList[n]) = (shuffledList[n], shuffledList[k]);
            }

            return shuffledList;
        }

        private bool RollPlayers()
        {
            if (_allPlayers.Count < LobbyCount) return false;

            _redPlayers.Clear();
            _bluePlayers.Clear();

            var shuffledList = Shuffle(_allPlayers);

            for (var i = 0; i < 4; i++)
            {
                if (i % 2 == 0)
                {
                    _redPlayers.Add(shuffledList[0]);
                }
                else
                {
                    _bluePlayers.Add(shuffledList[0]);
                }

                shuffledList.RemoveAt(0);
            }

            var sortedList = SortByScore(shuffledList);

            while (sortedList.Count > 0)
            {
                var blueScore = _bluePlayers.Sum(x => (int)x.Rank);
                var redScore = _redPlayers.Sum(x => (int)x.Rank);

                if (_bluePlayers.Count < (LobbyCount) && blueScore < redScore)
                {
                    _bluePlayers.Add(sortedList[0]);
                    sortedList.RemoveAt(0);
                }
                else if (_redPlayers.Count < (LobbyCount/2) && redScore < blueScore)
                {
                    _redPlayers.Add(sortedList[0]);
                    sortedList.RemoveAt(0);
                }
                else if (_bluePlayers.Count < (LobbyCount/2))
                {
                    _bluePlayers.Add(sortedList[0]);
                    sortedList.RemoveAt(0);
                }
                else
                {
                    _redPlayers.Add(sortedList[0]);
                    sortedList.RemoveAt(0);
                }
            }

            return true;
        }

        private List<Player> SortByScore(List<Player> shuffledList)
        {
            var sortedList = shuffledList.OrderByDescending(player => player.Rank).ToList();
            return sortedList;
        }

        private List<Player> SortByRole(List<Player> shuffledList)
        {
            var sortedList = shuffledList.OrderBy(player => player.SelectedRole).ToList();
            return sortedList;
        }

        public string PrintLobbyMembers()
        {
            var output = new StringBuilder();
            output.AppendLine("**Current Lobby (" + _allPlayers.Count + "/" + LobbyCount + "):**");

            if (_allPlayers.Count > 0)
            {
                var maxNameLength = _allPlayers.Max(player => player.Name.Length);
                var maxFirstRoleLength = _allPlayers.Max(player => player.FirstRole.ToString().Length);
                var maxSecondRoleLength = _allPlayers.Max(player => player.SecondRole.ToString().Length);
                var maxRatingLength = _allPlayers.Max(player => player.Rank.ToString().Length);
                
                var format = "**{0,-" + (maxNameLength + 2) + "}** **{1,-" + (maxFirstRoleLength + 6) + "}** **{2,-" + (maxSecondRoleLength + 6) + "}** **{3,-" + (maxRatingLength + 2) + "}**";

                foreach (var player in _allPlayers)
                {
                    output.AppendLine(string.Format(format, player.Name, player.FirstRole, player.SecondRole, player.Rank));
                }

                if (_allPlayers.Count == LobbyCount)
                {
                    output.AppendLine(string.Format(BotResources.LobbyReady, _lobbyStartedByName));
                }
            }
    
            return output.ToString();
        }


        public string PrintTeams()
        {
            var output = new StringBuilder();

            var sortByRoleRed = SortByRole(_redPlayers);
            var sortByRoleBlue = SortByRole(_bluePlayers);


            output.AppendLine("**Blue Team (Team 1):**");
            if (ChannelManager.GetInstance().BlueTeamChannelId != null)
            {
                output.AppendLine("<#" + ChannelManager.GetInstance().BlueTeamChannelId + ">");
            }
            foreach (var bPlayer in sortByRoleBlue)
            {
                output.AppendLine(bPlayer.Name + "[" + (bPlayer.Rank) + "] : " + bPlayer.SelectedRole);
            }

            output.AppendLine("**Total score: " + _bluePlayers.Sum(x => (int)x.Rank) + "**");
            output.AppendLine();

            output.AppendLine("**Red Team (Team 2):**");
            if (ChannelManager.GetInstance().RedTeamChannelId != null)
            {
                output.AppendLine("<#" + ChannelManager.GetInstance().RedTeamChannelId + ">");
            }
            foreach (var rPlayer in sortByRoleRed)
            {
                output.AppendLine(rPlayer.Name + "(" + (rPlayer.Rank) + ") : " + rPlayer.SelectedRole);
            }
            output.AppendLine("**Total score: " + _redPlayers.Sum(x => (int)x.Rank) + "**");
            output.AppendLine();
            
            output.AppendLine("Teamdiff: " + GetTeamScoreDifference() + " Rank");

            return output.ToString();
        }

        private int GetTeamScoreDifference()
        {
            var blueScore = _bluePlayers.Sum(x => (int)x.Rank);
            var redScore = _redPlayers.Sum(x => (int)x.Rank);

            if (blueScore > redScore)
            {
                return blueScore - redScore;
            }
            else
            {
                return redScore - blueScore;
            }
        }

        public bool StartLobby(string starterName)
        {
            if (_lobbyRunning) return false;
            
            RestartLobby(starterName);
            return true;
        }

        public void RestartLobby(string starterName)
        {
            _lobbyRunning = true;
            _allPlayers = [];
            _redPlayers = [];
            _bluePlayers = [];
            _lobbyStartedByName = starterName;
        }

        public bool LobbyFull()
        {
            return _allPlayers.Count == LobbyCount;
        }

        public bool LobbyRunning()
        {
            return _lobbyRunning;
        }

        public string GetLobbyMasterName()
        {
            return _lobbyStartedByName;
        }

        public List<Player> GetRedTeam()
        {
            return _redPlayers;
        }

        public List<Player> GetBlueTeam()
        {
            return _bluePlayers;
        }

        public void SetRolled(bool rolled)
        {
            _rolled = rolled;
        }

        public bool GetRolled()
        {
            return _rolled;
        }

        public List<Player> GetAllPlayers()
        {
            return _allPlayers;
        }
    }
}
