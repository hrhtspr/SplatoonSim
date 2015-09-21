using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
                for (int k = 0; k < 100; k++)
                {
                    phase++;
                    Console.WriteLine("Phase:{0}", phase);
                    sim.SimlationOnePhase();
                    foreach (var item in Enum.GetValues(typeof(Udemae)))
                    {
                        var min = double.MaxValue;
                        var max = double.MinValue;
                        var ave = 0.0;
                        var ratioMin = 1.0;
                        var ratioMax = 0.0;
                        var ratioave = 0.0;
                        var i = 0;
                        foreach (var p in sim.Players.Where(p => p.Udemae == (Udemae)item))
                        {
                            i++;
                            min = Math.Min(min, p.Strength);
                            max = Math.Max(max, p.Strength);
                            ave += p.Strength;
                            ratioMin = Math.Min(ratioMin, p.WinRatio);
                            ratioMax = Math.Max(ratioMax, p.WinRatio);
                            ratioave += p.WinRatio;
                        }
                        if (i != 0)
                        {
                            ave /= i;
                            ratioave /= i;
                        }
                        else
                        {
                            min = max = ratioMin = ratioMax = 0.0;
                        }
                        Console.WriteLine("{0}:{1}({2:0.0},{3:0.0},{4:0.0})({5:0.000},{6:0.000},{7:0.000})", item.ToString(), i, min, ave, max, ratioMin, ratioave, ratioMax);
                    }
                    Thread.Sleep(500);
                }
                string s = "";
                while (true)
                {
                    s = Console.ReadLine();
                    if (s == "s")
                    {
                        using (var st = new FileStream("data" + phase + ".csv", FileMode.Create))
                        {
                            using (var sw = new StreamWriter(st))
                            {
                                sw.WriteLine("Udemae,Point,Strength,WinRatio,IsCountStop");
                                for (int i = 0; i < PlayerCount; i++)
                                {
                                    sw.WriteLine("{0},{1},{2},{3},{4}", sim.Players[i].Udemae, sim.Players[i].UdemaePoint, sim.Players[i].Strength, sim.Players[i].WinRatio, sim.Players[i].IsCountStop);
                                }
                            }
                        }
                    }
                    else if (s == "h")
                    {
                        int m = (int)Math.Ceiling(sim.Players.Max(p => Math.Abs(p.Strength)) / 10);
                        var hists = Enumerable.Range(-m, 2 * m).ToDictionary(p => p, q => Enum.GetValues(typeof(Udemae)).Cast<Udemae>().ToDictionary(p => p, p => 0));
                        for (int i = 0; i < PlayerCount; i++)
                        {
                            hists[(int)Math.Floor(sim.Players[i].Strength / 10)][sim.Players[i].Udemae]++;
                        }
                        using (var st = new FileStream("hist" + phase + ".csv", FileMode.Create))
                        {
                            using (var sw = new StreamWriter(st))
                            {
                                sw.WriteLine("Udemae,{0}", string.Join(",", Enum.GetValues(typeof(Udemae)).Cast<Udemae>()));
                                foreach (var item in hists)
                                {
                                    var log = (item.Key * 10).ToString() + "," + string.Join(",", item.Value.Values);
                                    sw.WriteLine(log);
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
        public const int PlayerCount = 50000;
        public const int PlayerStrengthDeviation = 50;
        public Func<double> PlayerStrengthFunc;
        public Sim()
        {
            PlayerStrengthFunc = () => (GB.Random.NextDouble()-0.5)*PlayerStrengthDeviation*4;
            Players = new List<Player>(PlayerCount);
            for (int i = 0; i < PlayerCount; i++)
            {
                Players.Add(new Player(PlayerStrengthFunc()));
            }


            Battles = new List<Battle>();
        }

        public const int PhaseTime = 50;
        public const double PlayerResetProbability = 0;
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
                SimlationOneTime();
            }
        }
        public void SimlationOneTime()
        {
            SearchBattle();
            Battles.Sort((Battle x, Battle y) =>
            {
                //if (x.IsFull != y.IsFull)
                {
                    return x.IsFull.CompareTo(y.IsFull);
                }
                //else return y.Udemae.CompareTo(x.Udemae);
            }
            );
            foreach (var item in Battles.Where(p => p.IsFull).ToList())
            {
                item.Play();
                if (item.Players.All(p => p == null))
                {
                    Battles.Remove(item);
                }
            }
            //Players.Sort((x, y) => x.isBattle.CompareTo(y.isBattle));
        }

        public void SearchBattle()
        {
            var opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = 4;
            Parallel.ForEach(Players.Where(p => !p.isBattle).OrderBy(p=>GB.Random.NextDouble()).ToList(), opt, () => new List<Battle>(), (player, pls, battles) =>
            {
                if (Battles.Find(p => p != null && p.CanJoin(player.Udemae) && p.Join(player)) == null)
                {
                    if (battles.Find(p => p != null && p.CanJoin(player.Udemae) && p.Join(player)) == null)
                    {
                        var battle = new Battle(player.Udemae);
                        lock (this)
                        {
                            battles.Add(battle);
                            battle.Join(player);
                        }
                    }
                }
                return battles;
            }
            ,
            (battles) =>
            {
                lock (this)
                {
                    Battles.AddRange(battles);
                }
            }
            );
        }

        public const int ResetMin = PlayerCount / 400;
        public const int ResetMax = PlayerCount / 50;
        public void SomeOneReset()
        {
            var reset = GB.Random.Next(ResetMin, ResetMax);
            var indexes = Enumerable.Range(0, PlayerCount).OrderBy(p => GB.Random.NextDouble()).Take(reset);
            foreach (var item in indexes)
            {
                Players[item] = new Player(PlayerStrengthFunc());
            }
        }
    }

    public static class GB
    {
        public static Random Random = new Random();
    }
}
