using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.Calculation
{
    public class Temp
    {
        /// <summary>
        /// 상대풍속,풍향을 절대풍속,풍향으로 변환
        /// </summary>
        /// <param name="relWindSpeed">상대풍속(m/s)</param>
        /// <param name="relWindDir">상대풍향(degree)</param>
        /// <param name="speedVg_knot">선박대지속도(knot)</param>
        /// <param name="shipHeading">선박Heading(degree)</param>
        /// <returns>[0] 절대풍속, [1] 절대풍향</returns>
        public static double[] ConvertRelWindToAbsWind(double relWindSpeed, double relWindDir, double speedVg_knot, double shipHeading)         // 상대 풍향 풍속 --> 절대 풍향 풍속으로 변환하는 메소드
        {
            double[] absWind = new double[2];
            double speedVg = speedVg_knot * 0.5144;         // 선박의 속도를 knot에서 m/s로 변환, 풍속은 앞쪽에서 일괄 변환하였음.

            double REL_WIND_DIR_Rad = relWindDir * Math.PI / 180;
            double shipHeading_Rad = shipHeading * Math.PI / 180;

            absWind[0] = Math.Sqrt(Math.Pow(relWindSpeed, 2) + Math.Pow(speedVg, 2) - 2 * relWindSpeed * speedVg * Math.Cos(REL_WIND_DIR_Rad));       // 상대 풍속을 절대 풍속으로 변환식

            if ((relWindSpeed * Math.Cos(REL_WIND_DIR_Rad + shipHeading_Rad) - speedVg * Math.Cos(shipHeading_Rad)) >= 0)         // 상대 풍향을 절대 풍향으로 변환
            {
                absWind[1] = (Math.Atan((relWindSpeed * Math.Sin(REL_WIND_DIR_Rad + shipHeading_Rad) - speedVg * Math.Sin(shipHeading_Rad)) / (relWindSpeed * Math.Cos(REL_WIND_DIR_Rad + shipHeading_Rad) - speedVg * Math.Cos(shipHeading_Rad))) * 180 / Math.PI);
            }
            else
            {

                absWind[1] = Math.Atan((relWindSpeed * Math.Sin(REL_WIND_DIR_Rad + shipHeading_Rad) - speedVg * Math.Sin(shipHeading_Rad)) / (relWindSpeed * Math.Cos(REL_WIND_DIR_Rad + shipHeading_Rad) - speedVg * Math.Cos(shipHeading_Rad))) * 180 / Math.PI + 180;
            }

            if (double.IsNaN(absWind[0]))
            {
                absWind[0] = 0;
            }

            if (double.IsNaN(absWind[1]))
            {
                absWind[1] = 0;
            }

            if (absWind[1] < 0)
            {
                absWind[1] = absWind[1] + 360;
            }
            return absWind;
        }


        public static double PVcalculator(double[] speedPowerTable, double CORRECTPOW, double SPEED_LW)
        {
            double PV = 0;

            if (CORRECTPOW <= 0 | SPEED_LW <= 0)
            {
                return -9999;
            }

            var expectedSpeed = speedPowerTable[1] * Math.Pow(CORRECTPOW, speedPowerTable[2]);

            PV = 100 * (SPEED_LW - expectedSpeed) / expectedSpeed;

            if (double.IsNaN(PV) || double.IsNegativeInfinity(PV) || double.IsPositiveInfinity(PV))
            {
                PV = -9999;
            }

            return PV;
        }

        public static double PPVcalculator(double[] speedPowerTable, double CORRECTPOW, double SPEED_LW)
        {

            if (CORRECTPOW <= 0 | SPEED_LW < 0)
            {
                return -9999;
            }

            var powerToSpeedCoef1 = Math.Pow(1 / speedPowerTable[1], 1 / speedPowerTable[2]);
            var powerToSpeedCoef2 = 1 / speedPowerTable[2];

            var expectedPower = powerToSpeedCoef1 * Math.Pow(SPEED_LW, powerToSpeedCoef2);

            var PPV = 100 * (CORRECTPOW - expectedPower) / expectedPower;

            if (double.IsNaN(PPV) || double.IsNegativeInfinity(PPV) || double.IsPositiveInfinity(PPV))
            {
                PPV = -9999;
            }

            return PPV;
        }
    }
}
