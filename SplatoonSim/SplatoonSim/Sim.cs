using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplatoonSim
{
    class Sim
    {
        static void Main(string[] args)
        {
            var sim = new Sim();
            bool flag = true;
            int phase = 0;
            while (flag)
            {
                phase++;
                Console.WriteLine("Phase:{0}", phase);
                sim.SimlationOnePhase();
                foreach (var item in Enum.GetValues(typeof(Udemae)))
                {
                    var min = double.MaxValue;
                    var max = double.MinValue;
                    var ave = 0.0;
                    var i = 0;
                    foreach (var p in sim.Players.Where(p => p.Udemae == (Udemae)item))
                    {
                        i++;
                        min = Math.Min(min, p.Strength);
                        max = Math.Max(max, p.Strength);
                        ave += p.Strength;
                    }
                    if (i != 0)
                    {
                        ave /= i;
                    }
                    Console.WriteLine("{0}:{1}({2},{3},{4})", item.ToString(), i, min, ave, max);
                }
                string s = "";
                while (true)
                {
                    s = Console.ReadLine();
                    if (s == "s")
                    {
                        using (var st = new FileStream("data"+phase+".csv", FileMode.Create))
                        {
                            using (var sw = new StreamWriter(st))
                            {
                                for (int i = 0; i < PlayerCount; i++)
                                {
                                    sw.WriteLine("{0},{1},{2}",sim.Players[i].Udemae,sim.Players[i].UdemaePoint,sim.Players[i].Strength);
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                flag = s == "";
            }
        }
        public List<Player> Players;
        public List<Battle> Battles;
        public const int PlayerCount = 20000;
        public const int PlayerStrengthDeviation = 50;

        public Sim()
        {
            Players = new List<Player>(PlayerCount);
            for (int i = 0; i < PlayerCount; i++)
            {
                Players.Add(new Player(GB.Random.NextNormal() * PlayerStrengthDeviation));
            }


            Battles = new List<Battle>();
        }

        public const int PhaseTime = 50;
        public const double PlayerResetProbability = 1;
        public const int RoomMax = PlayerCount;
        public void SimlationOnePhase()
        {
            Battles.Clear();
            Parallel.For(0, PlayerCount, (i) => { Players[i].isBattle = false; });

            if (GB.Random.NextDouble() < PlayerResetProbability)
            {
                SomeOneReset();
            }

            for (int time = 0; time < PhaseTime; time++)
            {
                for (int i = 0; i < PlayerCount; i++)
                {
                    if (!Players[i].isBattle)
                    {
                        Battle battle = null;
                        foreach (var item in Battles)
                        {
                            if (item.CanJoin(Players[i].Udemae))
                            {
                                battle = item;
                                break;
                            }
                        }
                        if (battle == null && Battles.Count < RoomMax)
                        {
                            battle = new Battle(Players[i].Udemae);
                            Battles.Add(battle);
                        }
                        if (battle != null)
                        {
                            battle.Join(Players[i]);
                        }
                    }
                }

                foreach (var item in Battles.Where(p => p.IsFull).ToList())
                {
                    item.Play();
                    if (item.Players.All(p => p == null))
                    {
                        Battles.Remove(item);
                    }
                }
                Battles.Sort((Battle x, Battle y) =>
                {
                    if (x.IsFull != y.IsFull)
                    {
                        return x.IsFull.CompareTo(y.IsFull);
                    }
                    else return y.Udemae.CompareTo(x.Udemae);
                }
                );
                Players.Sort((x, y) => x.isBattle.CompareTo(y.isBattle));

            }
        }

        public const int ResetMin = PlayerCount / 400;
        public const int ResetMax = PlayerCount / 50;
        public void SomeOneReset()
        {
            var reset = GB.Random.Next(ResetMin, ResetMax);
            var indexes = Enumerable.Range(0, PlayerCount).OrderBy(p => GB.Random.NextDouble()).Take(reset);
            foreach (var item in indexes)
            {
                Players[item] = new Player(GB.Random.NextNormal() * PlayerStrengthDeviation);
            }
        }
    }

    public static class GB
    {
        public static Random Random = new Random();
    }
}
