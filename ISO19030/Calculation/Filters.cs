using ISO19030.Calculation;
using ISO19030.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030
{
    public class Filters
    {
        public static int FILTER_TOTAL_COUNT = 0;
        public static int CHAUVENT_COUNT = 0;
        public static int VALID_COUNT = 0;
        public static int REF_CONDITION_COUNT = 0;
        public static int count_rawShipdata = 0;

        

        public static List<ISO19030File> afterChauventData = new List<ISO19030File>();
        public static List<ISO19030File> afterValID_shipdata = new List<ISO19030File>();
        public static List<ISO19030File> afterRef_shipdata = new List<ISO19030File>();
        public static List<checktemp> chCheck = new List<checktemp>();
        public static List<int> errorList = new List<int>();

        public static List<ISO19030File> BasicFilteringContorller(List<ISO19030File> sailingData)
        {
            foreach (var item in sailingData)
            {
                item.ID = count_rawShipdata;
                count_rawShipdata++;
                item.VALID_CHAUVENT = true;
                item.VALID_REFCONDITION = true;
                item.VALID_VALIDATION = true;                

                //Environments 데이터
                item.SW_TEMP = 31.6; //해수
                item.AMBIENT_DENSITY = 1.15; //기온 
                
                var windSpeed = WindFunctions.ConvertRelWindToAbsWind(item.REL_WIND_SPEED, item.REL_WIND_DIR, item.SPEED_VG, item.SHIP_HEADING);
                var absWind = windSpeed[0];
                var absWindDir = windSpeed[1];
            }

            IEnumerable<List<ISO19030File>> tenMinBlock_shipdata =
            from s in sailingData
            group s by new
            {
                s.TIME_STAMP.Year,
                s.TIME_STAMP.Month,
                s.TIME_STAMP.Day,
                s.TIME_STAMP.Hour,
                Minute = s.TIME_STAMP.Minute / 10
            } into groupShipdata
            orderby groupShipdata.Key.Year, groupShipdata.Key.Month, groupShipdata.Key.Day, groupShipdata.Key.Hour, groupShipdata.Key.Minute ascending
            //십분 블록 완성

            select
            groupShipdata.ToList();

            foreach (var item in tenMinBlock_shipdata)
            {
                //샤보네트 필터 적용
                var SPEED_LW = Filters.Chauvent(item.Select(d => d.SPEED_LW));
                var SPEED_VG = Filters.Chauvent(item.Select(d => d.SPEED_VG));
                var SHAFT_POWER = Filters.Chauvent(item.Select(d => d.ME1_SHAFT_POWER));
                var SHAFT_REV = Filters.Chauvent(item.Select(d => d.ME1_RPM_SHAFT));
                var REL_WIND_SPEED = Filters.Chauvent(item.Select(d => d.REL_WIND_SPEED));
                var REL_WIND_DIR = Filters.Chauvent_angle(item.Select(d => d.REL_WIND_DIR));
                var SHIP_HEADING = Filters.Chauvent_angle(item.Select(d => d.SHIP_HEADING));
                var RUDDER_ANGLE = Filters.Chauvent_angle(item.Select(d => d.RUDDER_ANGLE));
                var WATER_DEPTH = Filters.Chauvent(item.Select(d => d.WATER_DEPTH));
                var SW_TEMP = Filters.Chauvent(item.Select(d => d.SW_TEMP));
                var AIR_TEMP = Filters.Chauvent(item.Select(d => d.AMBIENT_TEMP));

                int count_chauvent = 0;
                foreach (var item2 in item)
                {
                    //필터 결과 체크해서 데이터 삽입 여부
                    item2.VALID_CHAUVENT =
                        SPEED_LW[count_chauvent]
                        && SPEED_VG[count_chauvent]
                        && SHAFT_POWER[count_chauvent]
                        && SHAFT_REV[count_chauvent]
                        && REL_WIND_SPEED[count_chauvent]
                        && REL_WIND_DIR[count_chauvent]
                        && SHIP_HEADING[count_chauvent]
                        && RUDDER_ANGLE[count_chauvent]
                        && WATER_DEPTH[count_chauvent]
                        //&& DRAFT_FORE[count_chauvent]
                        //&& DRAFT_AFT[count_chauvent]
                        && SW_TEMP[count_chauvent]
                        && AIR_TEMP[count_chauvent];

                    if (item2.VALID_CHAUVENT == true)
                    {
                        //통과한 데이터만 삽입
                        afterChauventData.Add(item2);
                    }
                    else
                    {
                        //통과하지 않은 데이터도 전부 삽입
                        sailingData.ElementAt((int)item2.ID).VALID_CHAUVENT = false;
                        sailingData.ElementAt((int)item2.ID).VALID_VALIDATION = false;
                        sailingData.ElementAt((int)item2.ID).VALID_REFCONDITION = false;

                        errorList.Add(item2.ID);
                        CHAUVENT_COUNT++;
                        FILTER_TOTAL_COUNT++;
                    }
                    count_chauvent++;
                }
            }
            //샤보네트 필터 종료==================================

            //발리데이션 필터 시작
            IEnumerable<List<ISO19030File>> afterChauvent_tenMinBlock_shipdata =
            from s in afterChauventData
            group s by new
            {
                s.TIME_STAMP.Year,
                s.TIME_STAMP.Month,
                s.TIME_STAMP.Day,
                s.TIME_STAMP.Hour,
                Minute = s.TIME_STAMP.Minute / 10
            } into groupShipdata
            orderby groupShipdata.Key.Year, groupShipdata.Key.Month, groupShipdata.Key.Day, groupShipdata.Key.Hour, groupShipdata.Key.Minute ascending

            select
            groupShipdata.ToList();

            foreach (var item in afterChauvent_tenMinBlock_shipdata)
            {
                //표준편차 검사
                var RUDDER_ANGLE_VALID = Filters.standardError_angle(item.Select(d => d.RUDDER_ANGLE), 1);
                var SHAFT_REV_VALID = Filters.standardError(item.Select(d => d.ME1_RPM_SHAFT), 3);
                var SPEED_LW_VALID = Filters.standardError(item.Select(d => d.SPEED_LW), 0.5f);
                var SPEED_VG_VALID = Filters.standardError(item.Select(d => d.SPEED_VG), 0.5f);

                int temp_i = 0;

                foreach (var item2 in item)
                {
                    //검사 결과 체크
                    item2.VALID_VALIDATION = SHAFT_REV_VALID[temp_i] && SPEED_LW_VALID[temp_i] && RUDDER_ANGLE_VALID[temp_i] && SPEED_VG_VALID[temp_i];

                    var shaft = SHAFT_REV_VALID[temp_i];
                    var spedLw = SPEED_LW_VALID[temp_i];
                    var rudderAngle = RUDDER_ANGLE_VALID[temp_i];
                    var speedVg = SPEED_VG_VALID[temp_i];

                    if (item2.VALID_VALIDATION == true)
                    {
                        //성공한 결과 삽입
                        afterValID_shipdata.Add(item2);
                    }
                    else
                    {
                        //실패한 것도 삽입
                        sailingData.ElementAt((int)item2.ID).VALID_VALIDATION = false;
                        sailingData.ElementAt((int)item2.ID).VALID_REFCONDITION = false;
                        FILTER_TOTAL_COUNT++;
                        VALID_COUNT++;
                        errorList.Add(item2.ID);
                    }
                    temp_i++;
                }
            }
            //발리데이션 필터 종료

            return sailingData;
        }
        public static List<ISO19030File> FilteringForReferenceCondition(List<ISO19030File> sailingData, SHIP_PARTICULAR shipParticular, dynamic ballastValues, dynamic scantlingValues, WindResistance windResistance)
        {
            int index = 0;
            foreach (var item in sailingData)
            {
                //if (item.VALID_CHAUVENT == true && item.VALID_VALIDATION == true && item.VALID_REFCONDITION == true)
                //{
                var minDraft = (item.DRAFT_FORE + item.DRAFT_AFT) / 2;
                var deltaT = (ballastValues.DRAFT_FORE + ballastValues.DRAFT_AFT) / 2 - minDraft;
                var A = shipParticular.TRANSVERSE_PROJECTION_AREA_BALLAST + deltaT * shipParticular.BREADTH;
                var Za = 18.2 + deltaT;
                windResistance.data.zref[index] = (shipParticular.TRANSVERSE_PROJECTION_AREA_BALLAST * (10 + deltaT) + 0.5 * shipParticular.BREADTH * Math.Pow(deltaT, 2)) / A;
                windResistance.data.axv[index] = shipParticular.TRANSVERSE_PROJECTION_AREA_BALLAST + ((ballastValues.DRAFT_FORE + ballastValues.DRAFT_AFT) / 2 - minDraft) * shipParticular.BREADTH;
                windResistance.data.za[index] = /*shipModelTestData.ZA_BALLAST*/18.2 + ((ballastValues.DRAFT_FORE + ballastValues.DRAFT_AFT) / 2 - minDraft);
                windResistance.data.vg[index] = item.SPEED_VG * 0.5144;
                windResistance.data.psi0[index] = Math.PI * item.SHIP_HEADING / 180;
                windResistance.data.rhoair[index] = /*item.AMBIENT_DENSITY;*/1.225;
                windResistance.data.vwr[index] = item.REL_WIND_SPEED;
                windResistance.data.psiwr[index] = Math.PI * item.REL_WIND_DIR / 180;
                index++;
                //}
            }
            //바람 저항도 계산
            windResistance.CalculateWindResistance();

            index = 0;

            //마지막 필터 계산
            foreach (var item in afterValID_shipdata)
            {
                var windSpeedTrue = windResistance.data.vwtref[item.ID];

                var speedLw = item.SPEED_LW * 0.5144f;
                //수심의 깊이 체크
                var waterDepth1 = 3 * Math.Sqrt(shipParticular.BREADTH * ((item.DRAFT_FORE + item.DRAFT_AFT) / 2));
                var waterDepth2 = 2.75 * (speedLw * speedLw) / 9.80665;

                if (waterDepth1 < waterDepth2)
                {
                    waterDepth1 = waterDepth2;
                }
                if (index == 22)
                {
                    var check = 0;
                }


                if (item.WATER_DEPTH < -999)
                {
                    item.WATER_DEPTH = 9999;
                }
                //수온이 너무 낮거나! 바람의 세기가 8이상 타각이 5이상인걸 날림
                if (item.SW_TEMP < 2 || item.WATER_DEPTH <= waterDepth1 || windSpeedTrue > 7.9 || Math.Abs(item.RUDDER_ANGLE) > 5)
                {
                    sailingData.ElementAt((int)item.ID).VALID_REFCONDITION = false;
                    REF_CONDITION_COUNT++;
                    FILTER_TOTAL_COUNT++;
                    errorList.Add(item.ID);
                }
                else
                {
                    //통과한 것만 넣음
                    afterRef_shipdata.Add(item);
                }
                index++;
            }

            return sailingData;
        }
        //샤보네트 필터
        public static bool[] Chauvent(IEnumerable<double> values)       // 10분 블록 데이터 묶음을 인수로 넣음
        {
            int count = values.Count();
            double[] debug = new double[count];     // 디버그용 최종 Chauvent 값을 넣음
            bool[] valID = new bool[count];
            double avg = 0;
            double StdDev = 0;
            if (count > 1)
            {
                avg = values.Average();       // 10 분 블록 평균 계산

                var deviation = values.First() - avg;

                double delta = values.Sum(d => Math.Pow((d - avg), 2));        // 중간 계산 -> (value-avg)^2

                StdDev = Math.Sqrt(delta / count);        // 표준 편차 계산
            }

            int i = 0;

            if (StdDev == 0)                // 표준 편차가 0 일 경우 true를 찍기 위한 코드 (모든 값이 같으면 표준편차가 0)
            {
                foreach (var item in values)
                {
                    valID[i] = true;
                    i++;
                }
            }
            else
            {
                foreach (var item in values)
                {
                    debug[i] = MathNet.Numerics.SpecialFunctions.Erfc(Math.Abs((item - avg) / (StdDev * Math.Sqrt(2)))) * count;                    // 디버깅용 코드
                    valID[i] = MathNet.Numerics.SpecialFunctions.Erfc(Math.Abs((item - avg) / (StdDev * Math.Sqrt(2)))) * count < 0.5 ? false : true;
                    i++;
                }
            }
            return valID;
        }
        public static bool[] Chauvent_angle(IEnumerable<double> values)
        {
            double StdDev = 0;
            int count = values.Count();
            double sintotal = 0;
            double costotal = 0;
            double[] deltai = new double[count];
            double[] debug = new double[count];
            bool[] valID = new bool[count];
            double avg = 0;
            List<double> values_corr = new List<double> { };

            foreach (var item in values)
            {
                if (item < 0)
                {
                    values_corr.Add(item + 360);
                }
                else
                {
                    values_corr.Add(item);
                }
            }

            if (count > 1)
            {
                sintotal = values_corr.Average(d => Math.Sin(d / (180 / Math.PI)));          // sin 값 합계
                costotal = values_corr.Average(d => Math.Cos(d / (180 / Math.PI)));          // cos 값 합계
                avg = Math.Atan2(sintotal, costotal);                             //블록 평균 계산 Atan2 (y, x) parameter 로 넣는다. public static double Atan2(double y, double x)
                avg = avg * (180 / Math.PI);
                if (avg < 0)
                {
                    avg = avg + 360;
                }
                double delta = 0;

                var deviation = values.First() - avg;

                int temp_i = 0;
                foreach (var item in values_corr)
                {
                    var r = (Math.Abs(item - avg) % 360);

                    if ((r) > 180)
                    {
                        deltai[temp_i] = (360 - r);

                        delta = delta + Math.Pow(deltai[temp_i], 2);
                    }
                    else
                    {
                        deltai[temp_i] = r;

                        delta = delta + Math.Pow(deltai[temp_i], 2);
                    }
                    temp_i++;
                }

                StdDev = Math.Sqrt(delta / count);        //표준 편차 계산

                if (StdDev == 0)                          // StdDev 가 0 일 경우 - 한 블록안에 값이 모두 같다면 StdDev 0 이나 이때 true가 되어야 함.
                {
                    for (int i = 0; i < count; i++)
                    {
                        valID[i] = true;
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        var pdi = MathNet.Numerics.SpecialFunctions.Erfc(deltai[i] / (StdDev * Math.Sqrt(2)));
                        debug[i] = MathNet.Numerics.SpecialFunctions.Erfc(deltai[i] / (StdDev * Math.Sqrt(2))) * count;           // 디버깅 코드
                        valID[i] = MathNet.Numerics.SpecialFunctions.Erfc(deltai[i] / (StdDev * Math.Sqrt(2))) * count < 0.5 ? false : true;
                    }
                }
            }
            return valID;
        }
        static public bool[] standardError(IEnumerable<double> values, double threshold)
        {
            int count = values.Count();
            double[] debug = new double[count];
            bool[] valID = new bool[count];
            double avg = 0;
            double StdDev = 0;
            if (count > 1)
            {
                avg = values.Average();       //블록 평균 계산

                double delta = values.Sum(d => (d - avg) * (d - avg));        //중간 계산 -> (value-avg)^2

                StdDev = Math.Sqrt(delta / count);        //표준 편차 계산
            }

            for (int i = 0; i < count; i++)
            {
                valID[i] = StdDev > threshold ? false : true;
            }

            return valID;
        }
        public static bool[] standardError_angle(IEnumerable<double> values, double threshold)
        {
            double StdDev = 0;
            int count = values.Count();
            double sintotal = 0;
            double costotal = 0;
            double[] deltai = new double[count];
            bool[] valID = new bool[count];
            double avg = 0;

            if (count > 1)
            {
                sintotal = values.Sum(d => Math.Sin(d / (180 / Math.PI)));          // 180 / Math.PI 각도를 라디안으로 변환
                costotal = values.Sum(d => Math.Cos(d / (180 / Math.PI)));
                avg = Math.Atan2(sintotal, costotal);      //블록 평균 계산 Atan2 (y, x) parameter 로 간다. public static double Atan2(double y, double x)

                avg = avg * (180 / Math.PI);

                double delta = 0;

                int temp_i = 0;
                foreach (var item in values)
                {
                    var r = (Math.Abs(item - avg) % 360);

                    if ((r) > 180)
                    {
                        deltai[temp_i] = (360 - r);
                        delta = delta + Math.Pow(deltai[temp_i], 2);
                    }
                    else
                    {
                        deltai[temp_i] = r;
                        delta = delta + Math.Pow(deltai[temp_i], 2);
                    }
                    temp_i++;
                }

                StdDev = Math.Sqrt(delta / count);        //표준 편차 계산

                for (int i = 0; i < count; i++)
                {
                    valID[i] = StdDev > threshold ? false : true;
                }
            }
            return valID;
        }
    }
}
