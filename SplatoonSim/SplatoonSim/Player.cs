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
        Aminus, A, Aplus
    }
    public class Player
    {
        public static List<int> WinBasePoint = new List<int>(new[] { 20, 15, 12, 10, 10, 10, 10, 10, 10 });
        public static List<int> LoseBasePoint = new List<int>(Enumerable.Repeat(10, 9));


        public Udemae Udemae = Udemae.Cminus;
        public int UdemaePoint = 30;
        public double Strength;
        public bool isBattle;

        public Player(double strength)
        {
            Strength = strength;
        }

        public void ChengePoint(bool isWin)
        {
            if (isWin)
            {
                UdemaePoint += WinBasePoint[(int)Udemae];
                if (UdemaePoint >= 100)
                {
                    RankUp();
                }
            }
            else
            {
                UdemaePoint -= LoseBasePoint[(int)Udemae];
                if (UdemaePoint < 0)
                {
                    RankDown();
                }
            }
        }

        public void RankUp()
        {
            if (Udemae == SplatoonSim.Udemae.Aplus)
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
