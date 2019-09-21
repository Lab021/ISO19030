using ISO19030.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.Calculation
{
    public class WindFunctions
    {
        /// <summary>
        /// Convert Relative Wind Data to Abstract Wind Data
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
    }
    public class WindResistance
    {
        public WindResistanceData data;

        public WindResistance()
        {
        }



        private void Averaging()
        {
            if (!data.isAveraging)
            {
                for (int i = 0; i < data.nTrial; i++)
                {
                    data.vwtaverage[i] = data.vwt[i];
                    data.psiwtaverage[i] = data.psiwt[i];
                    data.vwraverage[i] = data.vwr[i];
                    data.psiwraverage[i] = data.psiwr[i];
                }
            }
            else
            {
                for (int j = 0; j < data.nTrial / 2; j++)
                {
                    double num = data.vwt[2 * j];
                    double num1 = data.psiwt[2 * j];
                    double num2 = data.vwt[2 * j + 1];
                    double num3 = data.psiwt[2 * j + 1];
                    double num4 = (num * Math.Cos(num1) + num2 * Math.Cos(num3)) / 2;
                    double num5 = (num * Math.Sin(num1) + num2 * Math.Sin(num3)) / 2;
                    double num6 = Math.Sqrt(Math.Pow(num4, 2) + Math.Pow(num5, 2));
                    double num7 = Math.Atan2(num5, num4);
                    num7 = (num7 + 6.28318530717959) % 6.28318530717959;
                    data.vwtaverage[2 * j] = num6;
                    data.psiwtaverage[2 * j] = num7;
                    data.vwtaverage[2 * j + 1] = num6;
                    data.psiwtaverage[2 * j + 1] = num7;
                }
                for (int k = 0; k < data.nTrial; k++)
                {
                    double num8 = data.vwtaverage[k] * Math.Cos(data.psiwtaverage[k] - data.psi0[k]);
                    double num9 = data.vwtaverage[k] * Math.Sin(data.psiwtaverage[k] - data.psi0[k]);
                    data.vwraverage[k] = Math.Sqrt(Math.Pow(num8 + data.vg[k], 2) + Math.Pow(num9, 2));
                    data.psiwraverage[k] = Math.Atan2(num9, num8 + data.vg[k]);
                }
            }
        }

        public void CalculateWindResistance()
        {
            try
            {
                TrueWind();
                Averaging();
                CorrectHeight();
                WindCoeff();
                CalWindResistance();
            }
            catch (Error error)
            {
                throw error;
            }
        }

        private void CalWindResistance()
        {
            for (int i = 0; i < data.nTrial; i++)
            {
                var raa = 0.5 * data.rhoair[i] * data.caa[i] * data.axv[i] * Math.Pow(data.vwref[i], 2);
                data.raa[i] = 0.5 * data.rhoair[i] * data.caa[i] * data.axv[i] * Math.Pow(data.vwref[i], 2) - 0.5 * data.rhoair[i] * data.caa0 * data.axv[i] * Math.Pow(data.vg[i], 2);
                data.raa0[i] = 0.5 * data.rhoair[i] * data.caa0 * data.axv[i] * Math.Pow(data.vg[i], 2);
            }
        }

        private void CorrectHeight()
        {
            for (int i = 0; i < data.nTrial; i++)
            {
                double num = 1;

                double num1 = 7;
                data.vwtref[i] = Math.Pow(data.zref[i] / data.za[i], num / num1) * data.vwtaverage[i];
                double num2 = data.vwtref[i] * Math.Cos(data.psiwtaverage[i] - data.psi0[i]);
                double num3 = data.vwtref[i] * Math.Sin(data.psiwtaverage[i] - data.psi0[i]);
                var ddd = Math.Sin(data.psiwtaverage[i] - data.psi0[i]);
                var ddddd = Math.Cos(data.psiwtaverage[i] - data.psi0[i]);


                var ddssss = Math.Sqrt(Math.Pow(data.vwtref[i], 2) + Math.Pow(data.vg[i], 2) + 2 * data.vwtref[i] * data.vg[i] * Math.Cos(data.psiwtaverage[i] - data.psi0[i]));

                data.vwref[i] = Math.Sqrt(Math.Pow(num2 + data.vg[i], 2) + Math.Pow(num3, 2));

                var dddd55d = num2 + data.vg[i];
                data.psiwref[i] = Math.Atan2(num3, num2 + data.vg[i]);

                var degree = data.psiwref[i] * 180 / Math.PI;
            }
        }

        private double FujiwaraCoeff(double psiwrref, double axv)
        {
            double num;
            double num1;
            double num2;
            double num3;
            double num4 = 0.922;
            double num5 = -0.507;
            double num6 = -1.162;
            double num7 = -0.018;
            double num8 = 5.091;
            double num9 = -10.367;
            double num10 = 3.011;
            double num11 = 0.341;
            double num12 = -0.458;
            double num13 = -3.245;
            double num14 = 2.313;
            double num15 = 1.901;
            double num16 = -12.727;
            double num17 = -24.407;
            double num18 = 40.31;
            double num19 = 5.481;
            double num20 = 0.585;
            double num21 = 0.906;
            double num22 = -3.239;
            double num23 = 0.314;
            double num24 = 1.117;
            if (psiwrref >= 0 && psiwrref < 1.5707963267949)
            {
                num = num4 + num5 * data.ayv / (data.loa * data.breadth) + num6 * data.cmc / data.loa;
                num1 = num12 + num13 * data.ayv / (data.loa * data.hbr) + num14 * axv / (data.breadth * data.hbr);
                num2 = num20 + num21 * data.aod / data.ayv + num22 * data.breadth / data.loa;
                num3 = num * Math.Cos(psiwrref) + num1 * (Math.Sin(psiwrref) - 0.5 * Math.Sin(psiwrref) * Math.Pow(Math.Cos(psiwrref), 2)) * Math.Sin(psiwrref) * Math.Cos(psiwrref) + num2 * Math.Sin(psiwrref) * Math.Pow(Math.Cos(psiwrref), 3);
                return num3;
            }
            if (psiwrref > 3.14159265358979 || psiwrref <= 1.5707963267949)
            {
                if (psiwrref != 1.5707963267949)
                {
                    return 0;
                }
                num3 = 0.5 * (FujiwaraCoeff(psiwrref - Conversion.RadToDegree(data.mu), axv) + FujiwaraCoeff(psiwrref + Conversion.RadToDegree(data.mu), axv));
                return num3;
            }
            num = num7 + num8 * data.breadth / data.loa + num9 * data.hc / data.loa + num10 * data.aod / Math.Pow(data.loa, 2) + num11 * axv / Math.Pow(data.breadth, 2);
            num1 = num15 + num16 * data.ayv / (data.loa * data.hbr) + num17 * axv / data.ayv + num18 * data.breadth / data.loa + num19 * axv / (data.breadth * data.hbr);
            num2 = num23 + num24 * data.aod / data.ayv;
            num3 = num * Math.Cos(psiwrref) + num1 * (Math.Sin(psiwrref) - 0.5 * Math.Sin(psiwrref) * Math.Pow(Math.Cos(psiwrref), 2)) * Math.Sin(psiwrref) * Math.Pow(Math.Cos(psiwrref), 2) + num2 * Math.Sin(psiwrref) * Math.Pow(Math.Cos(psiwrref), 3);
            return num3;
        }

        private void TrueWind()
        {
            try
            {
                for (int i = 0; i < data.nTrial; i++)
                {
                    double num = data.vwr[i] * Math.Cos(data.psi0[i] + data.psiwr[i]);
                    double num1 = data.vwr[i] * Math.Sin(data.psi0[i] + data.psiwr[i]);
                    double num2 = data.vg[i] * Math.Cos(data.psi0[i]);
                    double num3 = data.vg[i] * Math.Sin(data.psi0[i]);
                    double num4 = num - num2;
                    double num5 = num1 - num3;
                    data.vwt[i] = Math.Sqrt(Math.Pow(num4, 2) + Math.Pow(num5, 2));
                    //if (this.data.lbp > 100 && this.data.vwt[i] > 13.8 || this.data.lbp <= 100 && this.data.vwt[i] > 10.7)
                    //{
                    //    this.data.warning = "Wind velocity is larger than the allowed limit of ISO 15016:2015";
                    //}
                    data.psiwt[i] = Math.Atan2(num5, num4);
                    data.psiwt[i] = (data.psiwt[i] + 6.28318530717959) % 6.28318530717959;
                    var degree = (data.psiwt[i] + 6.28318530717959) % 6.28318530717959 * 180 / Math.PI;

                }
            }
            catch (Error error)
            {
                throw error;
            }
            catch (Exception exception)
            {
                throw new Error("TrueWind", "WindResistance", exception.Message);
            }
        }

        private void WindCoeff()
        {
            switch (data.windChartType)
            {
                case Models.Enum.WindChartTypes.Fujiwara:
                    {
                        WindCoeffFujiwara();
                        break;
                    }
                case Models.Enum.WindChartTypes.ITTC:
                    {
                        WindCoeffITTC();
                        break;
                    }
                case Models.Enum.WindChartTypes.WindTunnel:
                    {
                        WindCoeffWindTunnel();
                        break;
                    }
            }
        }

        private void WindCoeffFujiwara()
        {
            for (int i = 0; i < data.nTrial; i++)
            {
                double num = data.psiwref[i];
                if (num > 3.14159265358979)
                {
                    num = 6.28318530717959 - num;
                }
                else if (num < 0)
                {
                    num = Math.Abs(num);
                }
                data.caa[i] = FujiwaraCoeff(num, data.axv[i]);
                data.caa0 = FujiwaraCoeff(0, data.axv[i]);
            }
        }

        private void WindCoeffITTC()
        {
            for (int i = 0; i < data.nTrial; i++)
            {
                double degree = data.psiwref[i];
                degree = Conversion.RadToDegree(degree);
                if (degree > 180)
                {
                    degree = 360 - degree;
                }
                else if (degree < 0)
                {
                    degree = Math.Abs(degree);
                }
                switch (data.iTTCChartType)
                {
                    case Models.Enum.ITTCChartTypes.ContainerLadenContainer:
                        {
                            data.caa[i] = -data.iTTCContainerLadenContainer.GetValue(degree, 1);
                            data.caa0 = -data.iTTCContainerLadenContainer.GetValue(0, 1);
                            break;
                        }
                    case Models.Enum.ITTCChartTypes.ContainerLadenLashing:
                        {
                            data.caa[i] = -data.iTTCContainerLadenLashing.GetValue(degree, 1);
                            data.caa0 = -data.iTTCContainerLadenLashing.GetValue(0, 1);
                            break;
                        }
                    case Models.Enum.ITTCChartTypes.ContainerBallastLashing:
                        {
                            data.caa[i] = -data.iTTCContainerBallastLashing.GetValue(degree, 1);
                            data.caa0 = -data.iTTCContainerBallastLashing.GetValue(0, 1);
                            break;
                        }
                    case Models.Enum.ITTCChartTypes.ContainerBallast:
                        {
                            data.caa[i] = -data.iTTCContainerBallast.GetValue(degree, 1);
                            data.caa0 = -data.iTTCContainerBallast.GetValue(0, 1);
                            break;
                        }
                    case Models.Enum.ITTCChartTypes.TankerConventionalLaden:
                        {
                            data.caa[i] = -data.iTTCTankerConventionalLaden.GetValue(degree, 1);
                            data.caa0 = -data.iTTCTankerConventionalLaden.GetValue(0, 1);


                            var v1 = 0.00000000000117123499947647;
                            var v2 = -0.00000000052511115870235;
                            var v3 = 0.0000000782952986643856;
                            var v4 = -0.00000356045199257532;
                            var v5 = -0.000100927975153741;
                            var v6 = -0.00271257158080664;
                            var v7 = 0.969153537514452;

                            data.caa[i] = v1 * Math.Pow(degree, 6) + v2 * Math.Pow(degree, 5) + v3 * Math.Pow(degree, 4) +
                                v4 * Math.Pow(degree, 3) + v5 * Math.Pow(degree, 2) + v6 * degree + v7;

                            break;
                        }
                    case Models.Enum.ITTCChartTypes.TankerConventionalBallast:
                        {
                            data.caa[i] = -data.iTTCTankerConventionalBallast.GetValue(degree, 1);
                            data.caa0 = -data.iTTCTankerConventionalBallast.GetValue(0, 1);
                            break;
                        }
                    case Models.Enum.ITTCChartTypes.TankerCylindericalBallast:
                        {
                            data.caa[i] = -data.iTTCTankerCylindericalBallast.GetValue(degree, 1);
                            data.caa0 = -data.iTTCTankerCylindericalBallast.GetValue(0, 1);
                            break;
                        }
                    case Models.Enum.ITTCChartTypes.CarCarrierAverage:
                        {
                            data.caa[i] = -data.iTTCCarCarrierAverage.GetValue(degree, 1);
                            data.caa0 = -data.iTTCCarCarrierAverage.GetValue(0, 1);
                            break;
                        }
                    case Models.Enum.ITTCChartTypes.LNGSpherical:
                        {
                            data.caa[i] = -data.iTTCLNGSpherical.GetValue(degree, 1);
                            data.caa0 = -data.iTTCLNGSpherical.GetValue(0, 1);
                            break;
                        }
                    case Models.Enum.ITTCChartTypes.LNGPrismaticExtended:
                        {
                            data.caa[i] = -data.iTTCLNGPrismaticExtended.GetValue(degree, 1);
                            data.caa0 = -data.iTTCLNGPrismaticExtended.GetValue(0, 1);
                            break;
                        }
                    case Models.Enum.ITTCChartTypes.LNGPrismaticIntegrated:
                        {
                            data.caa[i] = -data.iTTCLNGPrismaticIntegrated.GetValue(degree, 1);
                            data.caa0 = -data.iTTCLNGPrismaticIntegrated.GetValue(0, 1);
                            break;
                        }
                    case Models.Enum.ITTCChartTypes.GeneralCargoAverage:
                        {
                            data.caa[i] = -data.iTTCGeneralCargoAverage.GetValue(degree, 1);
                            data.caa0 = -data.iTTCGeneralCargoAverage.GetValue(0, 1);
                            break;
                        }
                    case Models.Enum.ITTCChartTypes.CruiseFerryAverage:
                        {
                            data.caa[i] = -data.iTTCCruiseFerryAverage.GetValue(degree, 1);
                            data.caa0 = -data.iTTCCruiseFerryAverage.GetValue(0, 1);
                            break;
                        }
                }
            }
        }

        private void WindCoeffWindTunnel()
        {
            for (int i = 0; i < data.nTrial; i++)
            {
                double degree = data.psiwref[i];
                degree = Conversion.RadToDegree(degree);
                if (degree < 0)
                {
                    degree = 360 + degree;
                }
                if (data.windTunnel.x[data.windTunnel.size - 1] < degree)
                {
                    degree = 360 - degree;
                }
                data.caa[i] = data.windTunnel.GetValue(degree, 1);
            }
            data.caa0 = data.windTunnel.GetValue(0, 1);
        }


        /// <summary>
        /// Create Wind Resistance Data
        /// </summary>      
        /// <param name="shipParticular">Ship Basic Data</param>
        /// <param name="dataCount">Count of Sailing Data</param>
        /// <returns></returns>
        public static WindResistance CreateWindResistance(SHIP_PARTICULAR shipParticular, int dataCount)
        {
            var windResistance = new WindResistance(); //해당 클래스 부르고

            windResistance.data = new WindResistanceData(); // 데이터 입력
            windResistance.data.SetSize(dataCount);
            windResistance.data.isAveraging = false;
            windResistance.data.loa = shipParticular.LOA;
            windResistance.data.lbp = shipParticular.LOA;
            windResistance.data.breadth = shipParticular.BREADTH;
            windResistance.data.windChartType = Models.Enum.WindChartTypes.ITTC; //0 = fujiwara, 1 = ITTC, 2 = WindTunnel
            windResistance.data.iTTCChartType = Models.Enum.ITTCChartTypes.TankerConventionalLaden; // 0 = ContainerLadenContainer, 1 = ContainerLadenLashing, 2 = ContainerBallastLashing, 3 = ContainerBallast, 4 = TankerConventionalLaden, 5 = TankerConventionalBallast, 6 = TankerCylindericalBallast, 7 = CarCarrierAverage, 8 = LNGSpherical, 9 = LNGPrismaticExtended, 10 = LNGPrismaticIntegrated, 11 = GeneralCargoAverage, 12 = CruiseFerryAverage

            return windResistance;
        }


        


    }

    public class Error : Exception
    {
        public string functionName;

        public string className;

        public string contents;

        public Error(string functionName, string className, string contents)
        {
            this.functionName = functionName;
            this.className = className;
            this.contents = contents;
        }
    }

    public static class Conversion
    {
        public static double[] airTemp;

        public static double[] airDensity;

        public static double[] waterTemp;

        public static double[] waterDensity;

        static Conversion()
        {
            Conversion.airTemp = new double[] { -25, -20, -15, -10, -5, 0, 5, 10, 15, 20, 25, 30, 35 };
            Conversion.airDensity = new double[] { 1.4224, 1.3943, 1.3673, 1.3413, 1.3163, 1.2922, 1.269, 1.2466, 1.225, 1.2041, 1.1839, 1.1644, 1.1455 };
            Conversion.waterTemp = new double[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2, 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 2.7, 2.8, 2.9, 3, 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8, 3.9, 4, 4.1, 4.2, 4.3, 4.4, 4.5, 4.6, 4.7, 4.8, 4.9, 5, 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7, 5.8, 5.9, 6, 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 6.7, 6.8, 6.9, 7, 7.1, 7.2, 7.3, 7.4, 7.5, 7.6, 7.7, 7.8, 7.9, 8, 8.1, 8.2, 8.3, 8.4, 8.5, 8.6, 8.7, 8.8, 8.9, 9, 9.1, 9.2, 9.3, 9.4, 9.5, 9.6, 9.7, 9.8, 9.9, 10, 10.1, 10.2, 10.3, 10.4, 10.5, 10.6, 10.7, 10.8, 10.9, 11, 11.1, 11.2, 11.3, 11.4, 11.5, 11.6, 11.7, 11.8, 11.9, 12, 12.1, 12.2, 12.3, 12.4, 12.5, 12.6, 12.7, 12.8, 12.9, 13, 13.1, 13.2, 13.3, 13.4, 13.5, 13.6, 13.7, 13.8, 13.9, 14, 14.1, 14.2, 14.3, 14.4, 14.5, 14.6, 14.7, 14.8, 14.9, 15, 15.1, 15.2, 15.3, 15.4, 15.5, 15.6, 15.7, 15.8, 15.9, 16, 16.1, 16.2, 16.3, 16.4, 16.5, 16.6, 16.7, 16.8, 16.9, 17, 17.1, 17.2, 17.3, 17.4, 17.5, 17.6, 17.7, 17.8, 17.9, 18, 18.1, 18.2, 18.3, 18.4, 18.5, 18.6, 18.7, 18.8, 18.9, 19, 19.1, 19.2, 19.3, 19.4, 19.5, 19.6, 19.7, 19.8, 19.9, 20, 20.1, 20.2, 20.3, 20.4, 20.5, 20.6, 20.7, 20.8, 20.9, 21, 21.1, 21.2, 21.3, 21.4, 21.5, 21.6, 21.7, 21.8, 21.9, 22, 22.1, 22.2, 22.3, 22.4, 22.5, 22.6, 22.7, 22.8, 22.9, 23, 23.1, 23.2, 23.3, 23.4, 23.5, 23.6, 23.7, 23.8, 23.9, 24, 24.1, 24.2, 24.3, 24.4, 24.5, 24.6, 24.7, 24.8, 24.9, 25, 25.1, 25.2, 25.3, 25.4, 25.5, 25.6, 25.7, 25.8, 25.9, 26, 26.1, 26.2, 26.3, 26.4, 26.5, 26.6, 26.7, 26.8, 26.9, 27, 27.1, 27.2, 27.3, 27.4, 27.5, 27.6, 27.7, 27.8, 27.9, 28, 28.1, 28.2, 28.3, 28.4, 28.5, 28.6, 28.7, 28.8, 28.9, 29, 29.1, 29.2, 29.3, 29.4, 29.5, 29.6, 29.7, 29.8, 29.9, 30, 30.1, 30.2, 30.3, 30.4, 30.5, 30.6, 30.7, 30.8, 30.9, 31, 31.1, 31.2, 31.3, 31.4, 31.5, 31.6, 31.7, 31.8, 31.9, 32, 32.1, 32.2, 32.3, 32.4, 32.5, 32.6, 32.7, 32.8, 32.9, 33, 33.1, 33.2, 33.3, 33.4, 33.5, 33.6, 33.7, 33.8, 33.9, 34, 34.1, 34.2, 34.3, 34.4, 34.5, 34.6, 34.7, 34.8, 34.9, 35, 35.1, 35.2, 35.3, 35.4, 35.5, 35.6, 35.7, 35.8, 35.9, 36, 36.1, 36.2, 36.3, 36.4, 36.5, 36.6, 36.7, 36.8, 36.9, 37, 37.1, 37.2, 37.3, 37.4, 37.5, 37.6, 37.7, 37.8, 37.9, 38, 38.1, 38.2, 38.3, 38.4, 38.5, 38.6, 38.7, 38.8, 38.9, 39, 39.1, 39.2, 39.3, 39.4, 39.5, 39.6, 39.7, 39.8, 39.9, 40, 40.1, 40.2, 40.3, 40.4, 40.5, 40.6, 40.7, 40.8, 40.9, 41, 41.1, 41.2, 41.3, 41.4, 41.5, 41.6, 41.7, 41.8, 41.9, 42, 42.1, 42.2, 42.3, 42.4, 42.5, 42.6, 42.7, 42.8, 42.9, 43, 43.1, 43.2, 43.3, 43.4, 43.5, 43.6, 43.7, 43.8, 43.9, 44, 44.1, 44.2, 44.3, 44.4, 44.5, 44.6, 44.7, 44.8, 44.9, 45, 45.1, 45.2, 45.3, 45.4, 45.5, 45.6, 45.7, 45.8, 45.9, 46, 46.1, 46.2, 46.3, 46.4, 46.5, 46.6, 46.7, 46.8, 46.9, 47, 47.1, 47.2, 47.3, 47.4, 47.5, 47.6, 47.7, 47.8, 47.9, 48, 48.1, 48.2, 48.3, 48.4, 48.5, 48.6, 48.7, 48.8, 48.9, 49, 49.1, 49.2, 49.3, 49.4, 49.5, 49.6, 49.7, 49.8, 49.9, 50 };
            Conversion.waterDensity = new double[] { 1028.1499, 1028.1442, 1028.1384, 1028.1325, 1028.1264, 1028.1202, 1028.1139, 1028.1074, 1028.1008, 1028.0941, 1028.0872, 1028.0802, 1028.0731, 1028.0658, 1028.0585, 1028.051, 1028.0433, 1028.0356, 1028.0277, 1028.0197, 1028.0115, 1028.0033, 1027.9949, 1027.9864, 1027.9778, 1027.969, 1027.9601, 1027.9511, 1027.942, 1027.9327, 1027.9234, 1027.9139, 1027.9042, 1027.8945, 1027.8846, 1027.8747, 1027.8646, 1027.8544, 1027.844, 1027.8336, 1027.823, 1027.8123, 1027.8015, 1027.7906, 1027.7795, 1027.7684, 1027.7571, 1027.7457, 1027.7342, 1027.7225, 1027.7108, 1027.6989, 1027.687, 1027.6749, 1027.6627, 1027.6504, 1027.6379, 1027.6254, 1027.6127, 1027.6, 1027.5871, 1027.5741, 1027.561, 1027.5478, 1027.5344, 1027.521, 1027.5075, 1027.4938, 1027.48, 1027.4662, 1027.4522, 1027.4381, 1027.4239, 1027.4095, 1027.3951, 1027.3806, 1027.3659, 1027.3512, 1027.3363, 1027.3214, 1027.3063, 1027.2911, 1027.2758, 1027.2605, 1027.245, 1027.2294, 1027.2136, 1027.1978, 1027.1819, 1027.1659, 1027.1498, 1027.1335, 1027.1172, 1027.1008, 1027.0842, 1027.0676, 1027.0508, 1027.034, 1027.017, 1027, 1026.9828, 1026.9655, 1026.9482, 1026.9307, 1026.9131, 1026.8955, 1026.8777, 1026.8598, 1026.8419, 1026.8238, 1026.8056, 1026.7873, 1026.769, 1026.7505, 1026.7319, 1026.7133, 1026.6945, 1026.6756, 1026.6567, 1026.6376, 1026.6184, 1026.5992, 1026.5798, 1026.5604, 1026.5408, 1026.5212, 1026.5014, 1026.4816, 1026.4617, 1026.4416, 1026.4215, 1026.4013, 1026.3809, 1026.3605, 1026.34, 1026.3194, 1026.2987, 1026.2779, 1026.257, 1026.236, 1026.2149, 1026.1938, 1026.1725, 1026.1511, 1026.1297, 1026.1081, 1026.0865, 1026.0647, 1026.0429, 1026.021, 1025.999, 1025.9769, 1025.9547, 1025.9324, 1025.91, 1025.8875, 1025.865, 1025.8423, 1025.8196, 1025.7967, 1025.7738, 1025.7508, 1025.7276, 1025.7044, 1025.6811, 1025.6578, 1025.6343, 1025.6107, 1025.5871, 1025.5633, 1025.5395, 1025.5156, 1025.4916, 1025.4675, 1025.4433, 1025.419, 1025.3947, 1025.3702, 1025.3457, 1025.321, 1025.2963, 1025.2715, 1025.2466, 1025.2216, 1025.1966, 1025.1714, 1025.1462, 1025.1209, 1025.0955, 1025.07, 1025.0444, 1025.0187, 1024.9929, 1024.9671, 1024.9412, 1024.9152, 1024.8891, 1024.8629, 1024.8366, 1024.8103, 1024.7838, 1024.7573, 1024.7307, 1024.704, 1024.6772, 1024.6504, 1024.6234, 1024.5964, 1024.5693, 1024.5421, 1024.5148, 1024.4874, 1024.46, 1024.4325, 1024.4049, 1024.3772, 1024.3494, 1024.3215, 1024.2936, 1024.2656, 1024.2375, 1024.2093, 1024.181, 1024.1526, 1024.1242, 1024.0957, 1024.0671, 1024.0384, 1024.0097, 1023.9808, 1023.9519, 1023.9229, 1023.8938, 1023.8647, 1023.8354, 1023.8061, 1023.7767, 1023.7472, 1023.7177, 1023.6881, 1023.6583, 1023.6285, 1023.5987, 1023.5687, 1023.5387, 1023.5086, 1023.4784, 1023.4481, 1023.4178, 1023.3873, 1023.3568, 1023.3263, 1023.2956, 1023.2649, 1023.234, 1023.2032, 1023.1722, 1023.1411, 1023.11, 1023.0788, 1023.0475, 1023.0162, 1022.9848, 1022.9533, 1022.9217, 1022.89, 1022.8583, 1022.8265, 1022.7946, 1022.7626, 1022.7306, 1022.6985, 1022.6663, 1022.634, 1022.6017, 1022.5693, 1022.5368, 1022.5042, 1022.4716, 1022.4389, 1022.4061, 1022.3733, 1022.3403, 1022.3073, 1022.2743, 1022.2411, 1022.2079, 1022.1746, 1022.1412, 1022.1078, 1022.0743, 1022.0407, 1022.007, 1021.9733, 1021.9395, 1021.9056, 1021.8716, 1021.8376, 1021.8035, 1021.7694, 1021.7351, 1021.7008, 1021.6664, 1021.632, 1021.5975, 1021.5629, 1021.5282, 1021.4935, 1021.4587, 1021.4238, 1021.3889, 1021.3538, 1021.3188, 1021.2836, 1021.2484, 1021.2131, 1021.1777, 1021.1423, 1021.1068, 1021.0712, 1021.0356, 1020.9999, 1020.9641, 1020.9283, 1020.8924, 1020.8564, 1020.8203, 1020.7842, 1020.748, 1020.7118, 1020.6755, 1020.6391, 1020.6026, 1020.5661, 1020.5295, 1020.4929, 1020.4561, 1020.4193, 1020.3825, 1020.3456, 1020.3086, 1020.2715, 1020.2344, 1020.1972, 1020.16, 1020.1227, 1020.0853, 1020.0478, 1020.0103, 1019.9727, 1019.9351, 1019.8974, 1019.8596, 1019.8218, 1019.7839, 1019.7459, 1019.7079, 1019.6698, 1019.6317, 1019.5934, 1019.5552, 1019.5168, 1019.4784, 1019.4399, 1019.4014, 1019.3628, 1019.3241, 1019.2854, 1019.2466, 1019.2078, 1019.1689, 1019.1299, 1019.0909, 1019.0518, 1019.0126, 1018.9734, 1018.9341, 1018.8948, 1018.8554, 1018.8159, 1018.7764, 1018.7368, 1018.6972, 1018.6575, 1018.6177, 1018.5779, 1018.538, 1018.4981, 1018.4581, 1018.418, 1018.3779, 1018.3377, 1018.2975, 1018.2572, 1018.2168, 1018.1764, 1018.136, 1018.0954, 1018.0548, 1018.0142, 1017.9735, 1017.9327, 1017.8919, 1017.851, 1017.8101, 1017.7691, 1017.7281, 1017.687, 1017.6458, 1017.6046, 1017.5633, 1017.522, 1017.4806, 1017.4392, 1017.3977, 1017.3561, 1017.3145, 1017.2729, 1017.2311, 1017.1894, 1017.1475, 1017.1057, 1017.0637, 1017.0217, 1016.9797, 1016.9376, 1016.8954, 1016.8532, 1016.811, 1016.7687, 1016.7263, 1016.6839, 1016.6414, 1016.5989, 1016.5563, 1016.5137, 1016.471, 1016.4283, 1016.3855, 1016.3427, 1016.2998, 1016.2568, 1016.2138, 1016.1708, 1016.1277, 1016.0846, 1016.0414, 1015.9981, 1015.9548, 1015.9115, 1015.8681, 1015.8246, 1015.7811, 1015.7376, 1015.694, 1015.6504, 1015.6067, 1015.5629, 1015.5191, 1015.4753, 1015.4314, 1015.3875, 1015.3435, 1015.2995, 1015.2554, 1015.2112, 1015.1671, 1015.1229, 1015.0786, 1015.0343, 1014.9899, 1014.9455, 1014.901, 1014.8565, 1014.812, 1014.7674, 1014.7228, 1014.6781, 1014.6334, 1014.5886, 1014.5438, 1014.4989, 1014.454, 1014.409, 1014.364, 1014.319, 1014.2739, 1014.2288, 1014.1836, 1014.1384, 1014.0931, 1014.0478, 1014.0025, 1013.9571, 1013.9117, 1013.8662, 1013.8207, 1013.7751, 1013.7295, 1013.6839 };
        }

        public static double AirTemptoDensity(double temp)
        {
            if (temp <= Conversion.airTemp.First<double>())
            {
                return Conversion.airDensity.First<double>();
            }
            if (temp >= Conversion.airTemp.Last<double>())
            {
                return Conversion.airDensity.Last<double>();
            }
            int num = Conversion.airTemp.Count<double>();
            for (int i = 0; i < num; i++)
            {
                if (temp <= Conversion.airTemp[i])
                {
                    return Conversion.airDensity[i - 1] + (Conversion.airDensity[i] - Conversion.airDensity[i - 1]) * (temp - Conversion.airTemp[i - 1]) / (Conversion.airTemp[i] - Conversion.airTemp[i - 1]);
                }
            }
            return double.NaN;
        }

        public static double DateTimeToDays(DateTime time, DateTime refTime)
        {
            return (time - refTime).TotalDays;
        }

        public static DateTime DaysToDateTime(double days, DateTime refTime)
        {
            return refTime.AddDays(days);
        }

        public static double DegreeToRad(double deg)
        {
            return 0.0174532925199433 * deg;
        }

        public static double KnotToMS(double knot)
        {
            return knot * 1852 / 3600;
        }

        public static double MSToKnot(double ms)
        {
            return ms * 3600 / 1852;
        }

        public static double RadToDegree(double rad)
        {
            return 57.2957795130823 * rad;
        }

        public static double WaterTemptoDensity(double temp)
        {
            if (temp <= Conversion.waterTemp.First<double>())
            {
                return Conversion.waterDensity.First<double>();
            }
            if (temp >= Conversion.waterTemp.Last<double>())
            {
                return Conversion.waterDensity.Last<double>();
            }
            int num = Conversion.waterTemp.Count<double>();
            for (int i = 0; i < num; i++)
            {
                if (temp <= Conversion.waterTemp[i])
                {
                    return Conversion.waterDensity[i - 1] + (Conversion.waterDensity[i] - Conversion.waterDensity[i - 1]) * (temp - Conversion.waterTemp[i - 1]) / (Conversion.waterTemp[i] - Conversion.waterTemp[i - 1]);
                }
            }
            return double.NaN;
        }
    }
}
