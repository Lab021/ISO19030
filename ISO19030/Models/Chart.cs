using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.Models
{
    [Serializable]
    public class Chart
    {
        public string name;

        public int noY;

        public int size;

        public double[] x;

        public double[,] y;

        public Chart(string name, int noY, int size)
        {
            this.name = name;
            this.noY = noY;
            this.size = size;
            x = new double[size];
            y = new double[noY, size];
            for (int i = 0; i < size; i++)
            {
                x[i] = -9999.99;
                for (int j = 0; j < noY; j++)
                {
                    y[j, i] = -9999.99;
                }
            }
        }


        public double GetValue(double x, int noY)
        {
            if (noY > this.noY)
            {
                return double.NaN;
            }
            if (x <= this.x.First<double>())
            {
                return y[noY - 1, 0];
            }
            if (x >= this.x[size - 1])
            {
                return y[noY - 1, size - 1];
            }
            for (int i = 0; i < size; i++)
            {
                if (x <= this.x[i])
                {
                    return y[noY - 1, i - 1] + (y[noY - 1, i] - y[noY - 1, i - 1]) * (x - this.x[i - 1]) / (this.x[i] - this.x[i - 1]);
                }
            }
            return double.NaN;
        }
    }
}
