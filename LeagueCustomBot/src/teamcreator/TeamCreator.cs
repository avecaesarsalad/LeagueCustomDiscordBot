using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueCustomBot.teamcreator
{
    public class TeamCreator
    {
        private static TeamCreator? _instance;
        private static readonly Random _random = new();

        private bool _lobbyExisting = false;

        private List<Player> _allPlayers = new();
        private List<Player> _redPlayers = new();
        private List<Player> _bluePlayers = new();

        public static TeamCreator Instance
        {
            get { return _instance ??= new TeamCreator(); }
        }

        public string AddPlayerToList(Player player)
        {
            if (!_lobbyExisting) return "No lobby running, please use /start-lobby!";

            if (_allPlayers.Count >= 10) return "Lobby already full!";

            if (_allPlayers.FirstOrDefault(x => x.Name == player.Name) != null) return "You are already in the lobby!";

            _allPlayers.Add(player);
            return "You joined! Lobby count: (" + _allPlayers.Count +  "/10)";
        }

        public bool RemovePlayerFromList(string name)
        {
            if (!_lobbyExisting) return false;
            
            var playerToRemove = _allPlayers.FirstOrDefault(x => x.Name == name);
            if (playerToRemove == null) return false;
            _allPlayers.Remove(playerToRemove);
            return true;
        }

        public string Roll()
        {
            if (!_lobbyExisting) return "Please start lobby first with /start-lobby";

            if (_allPlayers.Count < 10) return "Please add more players";
            
            RollTeams();
            RollRoles(_redPlayers);
            RollRoles(_bluePlayers);

            return PrintTeams();
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

        private string PrintLobbyMembers()
        {
            var output = new StringBuilder();

            foreach (var player in _allPlayers)
            {
                output.AppendLine("Name: " + player.Name + "  1:" + player.FirstRole + "  2:" + player.SecondRole +
                                  "  Rating:" + player.Rank);
            }

            return output.ToString();
        }

        private string PrintTeams()
        {
            var output = new StringBuilder();

            var sortByRoleRed = SortByRole(_redPlayers);
            var sortByRoleBlue = SortByRole(_bluePlayers);

            output.AppendLine("Red Team:");
            foreach (var rPlayer in sortByRoleRed)
            {
                output.AppendLine(rPlayer.Name + "(" + ((int)rPlayer.Rank) + ") : " + rPlayer.SelectedRole);
            }
            output.AppendLine("Total score: " + _redPlayers.Sum(x => (int)x.Rank));
            output.AppendLine();

            output.AppendLine("Blue Team:");
            foreach (var bPlayer in sortByRoleBlue)
            {
                output.AppendLine(bPlayer.Name + "(" + ((int)bPlayer.Rank) + ") : " + bPlayer.SelectedRole);
            }
            output.AppendLine("Total score: " + _bluePlayers.Sum(x => (int)x.Rank));
            output.AppendLine();

            output.AppendLine("Teamdiff: " + GetTeamScoreDifference());

            return output.ToString();
        }

        private List<Player> Shuffle(List<Player> list)
        {
            var shuffledList = new List<Player>(list);

            var n = shuffledList.Count;
            while (n > 1)
            {
                n--;
                var k = _random.Next(n + 1);
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

        public string StartLobby()
        {
            if (_lobbyExisting) return "Lobby already running, please use /restart-lobby";
            
            _lobbyExisting = true;
            _allPlayers = [];
            _redPlayers = [];
            _bluePlayers = [];
            return "Lobby started!";
        }

        public string RestartLobby()
        {
            if (!_lobbyExisting) return "No Lobby running, please use /start-lobby";
            
            _allPlayers = [];
            _redPlayers = [];
            _bluePlayers = [];
            return "Lobby restarted!";
        }

        public string ShowLobby()
        {
            if (!_lobbyExisting) return "No Lobby running, please use /start-lobby";

            var text = PrintLobbyMembers();
            return text == "" ? "Nobody in Lobby yet!" : text;
        }
    }
}
