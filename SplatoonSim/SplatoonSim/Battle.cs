using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplatoonSim
{
    public class Battle
    {
        public Player[] Players;
        public int[][] Teams;
        public int Count;
        bool isFull;
        public bool IsFull { get { return isFull; } }
        public Udemae Udemae;

        public const double StrengthDeviation = 3;

        public Battle(Udemae udemae)
        {
            Players = new Player[8];
            Teams = new int[2][];
            Teams[0] = new int[4] { -1, -1, -1, -1 };
            Teams[1] = new int[4] { -1, -1, -1, -1 };
            Udemae = udemae;
        }
        public Battle(Player player)
            : this(player.Udemae)
        {
            Players[0] = player;
            player.JoinBattle(this);
        }
        public bool CanJoin(Udemae udemae)
        {
            if (IsFull) return false;
            var i = (int)udemae - (int)Udemae;
            return i >= -1 && i <= 2;

        }

        public bool Join(Player player)
        {
            bool re = false;
            for (int i = 0; i < 8; i++)
            {
                if (Players[i] == null)
                {
                    Players[i] = player;
                    re = true;
                    break;
                }
            }
            if (re)
            {
                player.JoinBattle(this);
                isFull = Players.Count(p => p != null) == 8;
            }

            return re;
        }
        public void MakeTeam()
        {
            var t = Enumerable.Range(0, 8).OrderBy(p => GB.Random.NextDouble()).ToArray();
            for (int i = 0; i < 4; i++)
            {
                Teams[0][i] = t[i];
                Teams[1][i] = t[i + 4];
            }
        }
        public void Play()
        {
            MakeTeam();
            var team0Strength = Teams[0].Sum(p => Players[p].Strength + GB.Random.NextNormal() * StrengthDeviation);
            var team1Strength = Teams[1].Sum(p => Players[p].Strength + GB.Random.NextNormal() * StrengthDeviation);
            int win, lose;
            if (team0Strength >= team1Strength) { win = 0; lose = 1; }
            else { win = 1; lose = 0; }
            var winUdemaes = Teams[win].Select(p=>Players[p].Udemae).ToArray();
            var loseUdemaes = Teams[lose].Select(p => Players[p].Udemae).ToArray();
            for (int i = 0; i < 4; i++)
            {

                if (Players[Teams[win][i]].ChengePoint(true, winUdemaes,loseUdemaes))
                {
                    Players[Teams[win][i]].LeaveBattle(this);
                    Players[Teams[win][i]] = null;
                };
                if (Players[Teams[lose][i]].ChengePoint(false, winUdemaes,loseUdemaes))
                {
                    Players[Teams[lose][i]].LeaveBattle(this);
                    Players[Teams[lose][i]] = null;
                }
            }
            Count++;
            Leave();
        }

        public const double LeaveProbabilyty = 0.2;
        public void Leave()
        {
            for (int i = 0; i < 8; i++)
            {
                if (Players[i] != null && GB.Random.NextDouble() < LeaveProbabilyty)
                {
                    Players[i].LeaveBattle(this);
                    Players[i] = null;
                }
            }
            isFull = Players.Count(p => p != null) == 8;

        }
    }
}
