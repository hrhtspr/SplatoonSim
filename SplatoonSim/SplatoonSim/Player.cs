using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplatoonSim
{
    public enum Udemae
    {
        Cminus, C, Cplus,
        Bminus, B, Bplus,
        Aminus, A, Aplus,
        S,Splus
    }
    public class Player
    {
        public static List<int> WinBasePoint = new List<int>(new[] { 20, 15, 12, 10, 10, 10, 10, 10, 10,0,5 });
        public static List<int> LoseBasePoint = new List<int>(Enumerable.Repeat(10,9).Concat(new[] {31,2}));


        public Udemae Udemae = Udemae.Cminus;
        public int UdemaePoint = 0;
        public double Strength;
        public bool isBattle;
        public Queue<bool> WinLose;
        public const int Count = 100;
        public double WinRatio { get { return (double)WinLose.Count(p=>p) / ((double)WinLose.Count); } }

        public Player(double strength)
        {
            Strength = strength;
            WinLose = new Queue<bool>();
        }

        public bool ChengePoint(bool isWin, int distance)
        {
            bool re = false;
            if (isWin)
            {
                UdemaePoint += WinBasePoint[(int)Udemae];// +(distance / 3) * 2;
                if (UdemaePoint >= 100)
                {
                    RankUp();
                    re = true;
                }
            }
            else
            {
                UdemaePoint -= LoseBasePoint[(int)Udemae];// +(distance / 3) * 2;
                if (UdemaePoint < 0)
                {
                    RankDown();
                    re = true;
                }
            }
            WinLose.Enqueue(isWin);
            while (WinLose.Count > Count)
            {
                WinLose.Dequeue();
            }
            return re;
        }

        public void RankUp()
        {
            if (Udemae == SplatoonSim.Udemae.Splus)
            {
                UdemaePoint = 99;
            }
            else
            {
                Udemae = (Udemae)(Udemae + 1);
                UdemaePoint = 30;
            }
        }

        public void RankDown()
        {
            if (Udemae == SplatoonSim.Udemae.Cminus)
            {
                UdemaePoint = 0;
            }
            else
            {
                Udemae = (Udemae)(Udemae - 1);
                UdemaePoint = 70;
            }
        }

        public void JoinBattle(Battle battle)
        {
            isBattle = true;
        }

        public void LeaveBattle(Battle battle)
        {
            isBattle = false;
        }

    }
}
