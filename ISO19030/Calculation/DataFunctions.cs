using ISO19030.DataProcessing;
using ISO19030.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.Calculation
{
    public class DataFunctions
    {        
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

        /// <summary>
        /// Based on the commissioning graph, the power and speed coefficients of each draft between the ballast and the scanlt are obtained.
        /// </summary>
        /// <param name="shipParticular">Ship Basic Data</param>
        /// <param name="seaTrialPowerToSpeedAtBallast">Power Speed Conversion factor(ballast)</param>
        /// <param name="seaTrialPowerToSpeedAtScant">Power Speed Conversion factor(scant)</param>
        /// <returns>각 10cm 별 power speed 계수 배열</returns>
        public static List<double[]> PowerToSpeedTable(SHIP_PARTICULAR shipParticular, dynamic ballastValues, dynamic scantlingValues)
        {
            double seatrialdraftScantling = (scantlingValues.DRAFT_FORE + scantlingValues.DRAFT_AFT) * 0.5;  //Draft (Scantling) Fore , Draft (Scantling) Aft
            double seaTrialdraftBallast = (ballastValues.DRAFT_FORE + ballastValues.DRAFT_AFT) * 0.5;     //Draft (Ballast) Fore, Draft (Ballast) Aft

            List<double[]> draftTable = new List<double[]> { };
            double startDraft = Math.Truncate((seaTrialdraftBallast - 7) * 10);        // ref draft -2 부터 시작 (-2는 여유분)

            int numerOfDraft = (int)((Math.Truncate((seatrialdraftScantling + 7) * 10)) - (Math.Truncate((seaTrialdraftBallast - 7) * 10)));         // 스캔틀링까지 소수점 한자리 까지 검색, 기준 draft보다 -2 +2 만큼 큼
            int numberOfPwer = (int)(shipParticular.ME_POWER_MCR / 200);          //ME_POWER_MCr
            double[] powerTable = new double[numberOfPwer];         // 확인 완  200 씩 파워 간격을 띄움 --> 커브피팅 알고리즘으로 x 축 배열
            double[] speedTable = new double[numberOfPwer];         // 확인 완  파워 간격 만큼 스피드를 계산 --> 커브피팅 알고리즘으로 y 축 배열s            

            for (int i = 0; i < numerOfDraft; i++)          // 드라프트 간격 0.1 만큼 해상도의 커프피팅 테이블을 만듦
            {
                for (int j = 0; j < numberOfPwer; j++)
                {
                    powerTable[j] = (j + 1) * 200;
                    speedTable[j] =
                    ((scantlingValues.B_POWER_TO_SPEED * Math.Pow(powerTable[j], scantlingValues.A_POWER_TO_SPEED) - ballastValues.B_POWER_TO_SPEED * Math.Pow(powerTable[j], ballastValues.A_POWER_TO_SPEED)) /  // Seatrial PowerToSpeed(Scant')
                    (seatrialdraftScantling - seaTrialdraftBallast) *
                    ((i + startDraft) * 0.1 - seaTrialdraftBallast) + (ballastValues.B_POWER_TO_SPEED * Math.Pow(powerTable[j], ballastValues.A_POWER_TO_SPEED)));  // Seatrial PowerToSpeed(Ballast')
                }

                var curve = CurveFitting.powerRegression(powerTable, speedTable);
                draftTable.Add(curve);
            }

            return draftTable;
        }
    }
}
