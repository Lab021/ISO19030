using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.DataProcessing
{
    public class CurveFitting
    {
        /// <summary>
        /// Polynominal 커브피팅
        /// </summary>
        /// <param name="x">x축 값 배열</param>
        /// <param name="y">y축 값 배열</param>
        /// <param name="order">커브피팅하고자 하는 다항식 차수</param>
        /// <returns></returns>
        //다항 커브피팅 6차 다항식까지 지원
        public static double[] polynominalRegression(double[] x, double[] y, int order)      // x: x 배열, y: y: 배열, order :차수
        {
            double[] p = Fit.Polynomial(x, y, order);       //curve fitting 하여 반환
            double R2 = 0;

            switch (order)
            {
                case 2:
                    R2 = GoodnessOfFit.RSquared(x.Select(d => p[0] + p[1] * d + p[2] * Math.Pow(d, 2)), y);
                    break;

                case 3:
                    R2 = GoodnessOfFit.RSquared(x.Select(d => p[0] + p[1] * d + p[2] * Math.Pow(d, 2) + p[3] * Math.Pow(d, 3)), y);
                    break;

                case 4:
                    R2 = GoodnessOfFit.RSquared(x.Select(d => p[0] + p[1] * d + p[2] * Math.Pow(d, 2) + p[3] * Math.Pow(d, 3) + p[4] * Math.Pow(d, 4)), y);
                    break;

                case 5:
                    R2 = GoodnessOfFit.RSquared(x.Select(d => p[0] + p[1] * d + p[2] * Math.Pow(d, 2) + p[3] * Math.Pow(d, 3) + p[4] * Math.Pow(d, 4) + p[5] * Math.Pow(d, 5)), y);
                    break;

                case 6:
                    R2 = GoodnessOfFit.RSquared(x.Select(d => p[0] + p[1] * d + p[2] * Math.Pow(d, 2) + p[3] * Math.Pow(d, 3) + p[4] * Math.Pow(d, 4) + p[5] * Math.Pow(d, 5) + p[6] * Math.Pow(d, 6)), y);
                    break;

                case 7:
                    R2 = GoodnessOfFit.RSquared(x.Select(d => p[0] + p[1] * d + p[2] * Math.Pow(d, 2) + p[3] * Math.Pow(d, 3) + p[4] * Math.Pow(d, 4) + p[5] * Math.Pow(d, 5) + p[6] * Math.Pow(d, 6)), y);
                    break;

                case 8:
                    R2 = GoodnessOfFit.RSquared(x.Select(d => p[0] + p[1] * d + p[2] * Math.Pow(d, 2) + p[3] * Math.Pow(d, 3) + p[4] * Math.Pow(d, 4) + p[5] * Math.Pow(d, 5) + p[6] * Math.Pow(d, 6)), y);
                    break;
            }

            double[] result = new double[order + 2];
            result[0] = R2;

            for (var i = 0; i <= order; i++)
            {
                result[i + 1] = p[i];
            }

            return result;
        }

        public static double[] powerRegression(double[] x, double[] y)
        {
            var convert_log = y.Select(d => Math.Log(d)).ToArray();

            double[] p = Fit.LinearCombination(x, convert_log,
                d => 1.0,
                d => Math.Log(d)
                );

            p[0] = Math.Exp(p[0]);

            var R2 = GoodnessOfFit.RSquared(x.Select(d => p[0] * Math.Pow(d, p[1])), y);

            double[] result = new double[3];
            result[0] = R2;
            result[1] = p[0];
            result[2] = p[1];

            return result;
        }

        public double[] LineRegression(float[] x, float[] y)
        {
            var x_data = Array.ConvertAll(x, item => (double)item);
            var y_data = Array.ConvertAll(y, item => (double)item);

            Tuple<double, double> p = Fit.Line(x_data, y_data);

            double a = p.Item1;
            double b = p.Item2;

            //var R2 = GoodnessOfFit.RSquared(x_data.Select(d => a + b * d), y_data);

            double[] result = new double[2];
            //result[0] = R2;  r2 정확성을 보는 일이 거의 없어서 return에서는 제외
            result[0] = b;
            result[1] = a;

            return result;
        }
    }
}
