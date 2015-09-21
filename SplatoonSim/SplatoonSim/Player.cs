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
        S, Splus
    }
    public class Player
    {
        public static List<int[]> WinBasePoint = new List<int[]>(new[] { new[] { 20, 20, 20 }, new[] { 15, 15, 15 }, new[] { 12, 12,12 },
                                                                        new[]{10,10,10}, new[] {10,10,10},  new[] {10,10,10}, 
                                                                        new[]{12,10,8}, new[] {12,10,8},  new[] {12,10,8}, 
                                                                        new[] {5,4,3}, new[] {3,2,2} });
        public static List<int[]> LoseBasePoint = new List<int[]>(new[] { new[] {10,10,10}, new[] {10,10,10},  new[] {10,10,10}, 
                                                                        new[] {10,10,10}, new[] {10,10,10},  new[] {10,10,10}, 
                                                                        new[] {8,10,12}, new[] {8,10,12},  new[] {8,10,12}, 
                                                                        new[] {4,5,6}, new[] {6,6,7} });
        public static int[,] GradePoint = new[,] {{0, 2, 4, 6, 8, 8, 8, 8, 8, 8, 8},
                                                 {-2, 0, 2, 4, 6, 8, 8, 8, 8, 8, 8},
                                                 {-4,-2, 0, 2, 4, 6, 8, 8, 8, 8, 8},
                                                 {-6,-4,-2, 0, 2, 4, 6, 8, 8, 8, 8},
                                                 {-8,-6,-4,-2, 0, 2, 4, 6, 8, 8, 8},
                                                 {-8,-8,-6,-4,-2, 0, 2, 4, 6, 8, 8},
                                                 {-8,-8,-8,-6,-4,-2, 0, 2, 4, 6, 8},
                                                 {-8,-8,-8,-8,-6,-4,-2, 0, 2, 4, 6},
                                                 {-8,-8,-8,-8,-8,-6,-4,-2, 0, 2, 4},
                                                 {-8,-8,-7,-6,-5,-4,-3,-2,-1, 0, 0},
                                                 {-8,-8,-8,-7,-6,-5,-4,-3,-2,-1, 0}};


        public Udemae Udemae = Udemae.Cminus;
        public int UdemaePoint = 0;
        public double Strength;
        public bool isBattle;
        public Queue<bool> WinLose;
        public const int Count = 100;
        public double WinRatio { get { return (double)WinLose.Count(p => p) / ((double)WinLose.Count); } }
        public bool IsCountStop = false;

        public Player(double strength)
        {
            Strength = strength;
            WinLose = new Queue<bool>();
            IsCountStop = false;
        }

        public bool ChengePoint(bool isWin, IEnumerable<Udemae> wins, IEnumerable<Udemae> loses)
        {
            bool re = false;
            int k = UdemaePoint < 40 ? 0 : UdemaePoint <= 80 ? 1 : 2;
            var f = 0;// (wins.Sum(p => GradePoint[(int)Udemae, (int)p]) - loses.Sum(p => GradePoint[(int)Udemae, (int)p])) / 2;
            if (isWin)
            {
                var t = WinBasePoint[(int)Udemae][k] + f;
                if (t <= 0) t = 1;
                UdemaePoint += t;
                if (UdemaePoint >= 100)
                {
                    RankUp();
                    re = true;
                }
            }
            else
            {
                var t = LoseBasePoint[(int)Udemae][k] + f;
                if (t <= 0) t = 1;
                UdemaePoint -= t;
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
                IsCountStop = true;
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
