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
        private static readonly Random Random = new();

        private bool _lobbyRunning;

        private List<Player> _allPlayers = new();
        private List<Player> _redPlayers = new();
        private List<Player> _bluePlayers = new();

        public static TeamCreator Instance
        {
            get { return _instance ??= new TeamCreator(); }
        }

        public bool AddPlayerToList(Player player)
        {
            
            if (!_lobbyRunning) return false;

            if (_allPlayers.Count >= 10) return false;

            if (_allPlayers.FirstOrDefault(x => x.Name == player.Name) != null) return false;

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

        public bool PlayerExists(string name)
        {
            return _allPlayers.FirstOrDefault(x => x.Name == name) != null;
        }

        public bool Roll()
        {
            if (!_lobbyRunning) return false;

            if (_allPlayers.Count < 10) return false;
            
            RollTeams();
            RollRoles(_redPlayers);
            RollRoles(_bluePlayers);
            return true;
        }

        private void RollTeams()
        {
            for (var i = 0; i <= 5; i++)
            {
                for (var x = 0; x < 10; x++)
                {
                    RollPlayers();
                    var td = GetTeamScoreDifference();
                    if (td <= i) return;
                }
            }
        }

        private void RollRoles(List<Player> team)
        {
            var randomTeam = Shuffle(team);

            foreach (var player in randomTeam)
            {
                if (player.FirstRole == Role.Fill)
                {
                    break;
                }
                else
                {
                    if (RoleFree(randomTeam, player.FirstRole))
                    {
                        player.SelectedRole = player.FirstRole;
                    }
                    else
                    {
                        if (player.SecondRole == Role.Fill)
                        {
                            break;
                        }
                        else
                        {
                            if (RoleFree(randomTeam, player.SecondRole))
                            {
                                player.SelectedRole = player.SecondRole;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            foreach (var player in randomTeam.Where(x => x.SelectedRole == Role.Fill))
            {
                for (var i = 1; i < 6; i++)
                {
                    var roleFree = RoleFree(randomTeam, (Role)i);
                    if (roleFree) player.SelectedRole = (Role)i;
                }
            }

            team = randomTeam;
        }

        private bool RoleFree(List<Player> team, Role role)
        {
            return team.All(x => x.SelectedRole != role);
        }

        private bool RollPlayers()
        {
            if (_allPlayers.Count < 10) return false;

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

                if (_bluePlayers.Count < 5 && blueScore < redScore)
                {
                    _bluePlayers.Add(sortedList[0]);
                    sortedList.RemoveAt(0);
                }
                else if (_redPlayers.Count < 5 && redScore < blueScore)
                {
                    _redPlayers.Add(sortedList[0]);
                    sortedList.RemoveAt(0);
                }
                else if (_bluePlayers.Count < 5)
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
            output.AppendLine("**Current Lobby (" + _allPlayers.Count + "/10):**");

            if (_allPlayers.Count > 0)
            {
                var maxNameLength = _allPlayers.Max(player => player.Name.Length);
                var maxFirstRoleLength = _allPlayers.Max(player => player.FirstRole.ToString().Length);
                var maxSecondRoleLength = _allPlayers.Max(player => player.SecondRole.ToString().Length);
                var maxRatingLength = _allPlayers.Max(player => player.Rank.ToString().Length);
                
                var format = "Name: **{0,-" + (maxNameLength + 2) + "}** 1: **{1,-" + (maxFirstRoleLength + 6) + "}** 2: **{2,-" + (maxSecondRoleLength + 6) + "}** Rating: **{3,-" + (maxRatingLength + 2) + "}**";

                foreach (var player in _allPlayers)
                {
                    output.AppendLine(string.Format(format, player.Name, player.FirstRole, player.SecondRole, player.Rank));
                }

                if (_allPlayers.Count == 10)
                {
                    output.AppendLine(BotResources.LobbyReady);
                }
            }
    
            return output.ToString();
        }


        public string PrintTeams()
        {
            var output = new StringBuilder();

            var sortByRoleRed = SortByRole(_redPlayers);
            var sortByRoleBlue = SortByRole(_bluePlayers);

            output.AppendLine("Red Team:");
            foreach (var rPlayer in sortByRoleRed)
            {
                output.AppendLine(rPlayer.Name + "(" + (rPlayer.Rank) + ") : " + rPlayer.SelectedRole);
            }
            output.AppendLine("Total score: " + _redPlayers.Sum(x => (int)x.Rank));
            output.AppendLine();

            output.AppendLine("Blue Team:");
            foreach (var bPlayer in sortByRoleBlue)
            {
                output.AppendLine(bPlayer.Name + "(" + (bPlayer.Rank) + ") : " + bPlayer.SelectedRole);
            }
            output.AppendLine("Total score: " + _bluePlayers.Sum(x => (int)x.Rank));
            output.AppendLine();

            output.AppendLine("Teamdiff: " + GetTeamScoreDifference() + " Rank");

            return output.ToString();
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

        public bool StartLobby()
        {
            if (_lobbyRunning) return false;
            
            _lobbyRunning = true;
            _allPlayers = [];
            _redPlayers = [];
            _bluePlayers = [];
            return true;
        }

        public void RestartLobby()
        {
            _lobbyRunning = true;
            _allPlayers = [];
            _redPlayers = [];
            _bluePlayers = [];
        }

        public bool LobbyFull()
        {
            return _allPlayers.Count == 10;
        }

        public bool LobbyRunning()
        {
            return _lobbyRunning;
        }
    }
}
