using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplatoonSim
{
    static class RandomEx
    {
        static double coeff;
        static double theta;
        static bool flag = false;
        public static double NextNormal(this Random rand)
        {
            if (!flag)
            {
                coeff = Math.Sqrt(-2*Math.Log(rand.NextDouble()));
                theta = rand.NextDouble()*Math.PI*2;
                flag = true;
                return coeff * Math.Sin(theta);
            }
            else
            {
                flag = false;
                return coeff * Math.Cos(theta);
            }
        }

        public static double NextNormal(this Random rand,double mean, double sigma2)
        {
            return rand.NextNormal() * Math.Sqrt(sigma2) + mean;
        }
    }
}
