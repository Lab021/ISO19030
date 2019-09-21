using ISO19030.Calculation;
using ISO19030.DataProcessing;
using ISO19030.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030
{
    public class MainProcess
    {
        public static WindResistance windResistance;
        static void Main(string[] args)
        {
            var result = Analysis();
            Console.WriteLine(result);

            return;
        }

        public static void DataRetrieval()
        {

        }

        public static void DataCompilation()
        {

        }

        

        public static List<ISO19030RESULT> Analysis()
        {
            var referencePvList = new Dictionary<string, double>();
            var evaluationPvList = new Dictionary<string, double>();

            var errorList = new List<int>();
            var result = new List<ISO19030RESULT>();
            var sailingData = new List<ISO19030File>();


            //Particular File load
            var shipParticular = CsvFileController.ReadParticularFile("particular.csv");
            
                      

            //speed Power File Load
            var speedPowerData = CsvFileController.ReadSpeedPowerDataFileToList("speedPower.csv");
            

            var ballastPowerList = new List<double>();
            var ballastSpeedList = new List<double>();
            var scantlingPowerList = new List<double>();
            var scantlingSpeedList = new List<double>();

            foreach (var _d in speedPowerData)
            {
                ballastPowerList.Add(_d.BALLAST_POWER);
                ballastSpeedList.Add(_d.BALLAST_SPEED);
                scantlingPowerList.Add(_d.SCANTLING_POWER);
                scantlingSpeedList.Add(_d.SCANTLING_SPEED);
            }

            var draft = CsvFileController.ReadDraftFile("draft.csv");
            
            var ballastSpeedToPower = CurveFitting.powerRegression(ballastSpeedList.ToArray(), ballastPowerList.ToArray());
            var ballastPowerToSpeed = CurveFitting.powerRegression(ballastPowerList.ToArray(), ballastSpeedList.ToArray());

            var scantlingSpeedToPower = CurveFitting.powerRegression(scantlingSpeedList.ToArray(), scantlingPowerList.ToArray());
            var scantlingPowerToSpeed = CurveFitting.powerRegression(scantlingPowerList.ToArray(), scantlingSpeedList.ToArray());
           

            dynamic ballastValues = new ExpandoObject();
            ballastValues.DRAFT_FORE = draft.BALLAST_DRAFT_FORE;
            ballastValues.DRAFT_AFT = draft.BALLAST_DRAFT_AFT;

            ballastValues.A_POWER_TO_SPEED = ballastPowerToSpeed[2];
            ballastValues.B_POWER_TO_SPEED = ballastPowerToSpeed[1];
            ballastValues.A_SPEED_TO_POWER = ballastSpeedToPower[2];
            ballastValues.B_SPEED_TO_POWER = ballastSpeedToPower[1];

            dynamic scantlingValues = new ExpandoObject();
            scantlingValues.DRAFT_FORE = draft.SCANTLING_DRAFT_FORE;
            scantlingValues.DRAFT_AFT = draft.SCANTLING_DRAFT_AFT;

            scantlingValues.A_POWER_TO_SPEED = scantlingPowerToSpeed[2];
            scantlingValues.B_POWER_TO_SPEED = scantlingPowerToSpeed[1];
            scantlingValues.A_SPEED_TO_POWER = scantlingSpeedToPower[2];
            scantlingValues.B_SPEED_TO_POWER = scantlingSpeedToPower[1];

                      
            //선박 운항 데이터 불러오기
            sailingData = CsvFileController.Read19030DataFileToList("voyageData.csv");

            if (shipParticular == null || ballastValues == null || scantlingValues == null)
            {
                return result;
            }


            if (sailingData == null)
            {
                return result;
            }
            //데이터 읽기!!끝================================================================================


            Filters.BasicFilteringContorller(sailingData);

            var afterChauventData = new List<ISO19030File>();
            var afterValID_shipdata = new List<ISO19030File> { };
            var afterRef_shipdata = new List<ISO19030File> { };
            List<checktemp> chCheck = new List<checktemp>();
            


            int FILTER_TOTAL_COUNT = 0;
            int CHAUVENT_COUNT = 0;
            int VALID_COUNT = 0;
            int REF_CONDITION_COUNT = 0;
            int count_rawShipdata = 0;

            foreach (var item in sailingData)
            {
                item.ID = count_rawShipdata;
                count_rawShipdata++;
                item.VALID_CHAUVENT = true;
                item.VALID_REFCONDITION = true;
                item.VALID_VALIDATION = true;
                //위 3개 필터
                // 각 필터에 대해서 플래그로 T/F로 필터통과 여부 확인

                //날씨 값은 기본 값으로 대체한다.
                item.SW_TEMP = 31.6; //해수
                item.AMBIENT_DENSITY = 1.15; //기온 



                //상대 풍향을 절대 풍향을 바꿔준다.
                //헬퍼 펑션안에 있는 함수를 여기에 넣어야 함
                var windSpeed = Temp.ConvertRelWindToAbsWind(item.REL_WIND_SPEED, item.REL_WIND_DIR, item.SPEED_VG, item.SHIP_HEADING);
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



            //바람 저항도 계산
            // Calculate wind resistance
            windResistance = new WindResistance(); //해당 클래스 부르고
            windResistance.data = new WindResistanceData(); // 데이터 입력
            windResistance.data.SetSize(sailingData.Count);
            windResistance.data.isAveraging = false;
            windResistance.data.loa = shipParticular.LOA;
            windResistance.data.lbp = shipParticular.LOA;
            windResistance.data.breadth = shipParticular.BREADTH;
            windResistance.data.windChartType = Models.Enum.WindChartTypes.ITTC; //0 = fujiwara, 1 = ITTC, 2 = WindTunnel
            windResistance.data.iTTCChartType = Models.Enum.ITTCChartTypes.TankerConventionalLaden; // 0 = ContainerLadenContainer, 1 = ContainerLadenLashing, 2 = ContainerBallastLashing, 3 = ContainerBallast, 4 = TankerConventionalLaden, 5 = TankerConventionalBallast, 6 = TankerCylindericalBallast, 7 = CarCarrierAverage, 8 = LNGSpherical, 9 = LNGPrismaticExtended, 10 = LNGPrismaticIntegrated, 11 = GeneralCargoAverage, 12 = CruiseFerryAverage

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
            //마지막 필터 종료

            var nowDraft = sailingData.Where(d => d.DRAFT_FORE != -9999).Average(d => d.DRAFT_FORE);
            var modelDraftMeanBallast = (ballastValues.DRAFT_FORE + ballastValues.DRAFT_AFT) / 2;
            var modelDraftMeanScantling = (scantlingValues.DRAFT_FORE + scantlingValues.DRAFT_AFT) / 2;

            //드라프트를 맞추는 식이다.
            //드라프트가 다른 값 끼리 성능 비교가 불가능하다.
            //x:스피드 y:파워
            //csv 에서 스피드 파워를 만들어야 한다.
            //파일로 받아와야 한다.
            //
            //powerRegression 함수로 구해야한다.
            //R2 정확도
            //1 : A, 2 : B 값
            //GoodnessOffit Nuget 패키지서 받아서 같은버전으로 받아야함            
            List<double[]> powerToSpeedTable = DataController.PowerToSpeedTable(shipParticular, ballastValues, scantlingValues);  // Ref 테이블 작성

            index = 0;

            var iso19030resultList = new List<ISO19030RESULT>();

            foreach (var item in sailingData)
            {


                if (item.VALID_CHAUVENT == true && item.VALID_VALIDATION == true && item.VALID_REFCONDITION == true)
                {                    
                    try
                    {
                        var iso19030result = new ISO19030RESULT();
                        iso19030result.SPEED_VG = item.SPEED_VG;
                        iso19030result.SPEED_LW = item.SPEED_LW;
                        iso19030result.SHAFT_POWER = item.ME1_SHAFT_POWER;
                        iso19030result.SHAFT_REV = item.ME1_RPM_SHAFT;
                        iso19030result.DRAFT_FORE = item.DRAFT_FORE;
                        iso19030result.DRAFT_AFT = item.DRAFT_AFT;

                        var speedLwMeter = item.SPEED_VG * 0.5144;
                        var Rw = windResistance.data.raa[index] / 1000 * speedLwMeter / 0.98;


                        var correctPower = item.ME1_SHAFT_POWER - (windResistance.data.raa[index] / 1000 * speedLwMeter) / 0.98;
                        //바람 보정
                        iso19030result.CORRECT_POWER = correctPower;
                        //똑같은 드라프트 맞추는 작업
                        var powerToSpeedEquation = powerToSpeedTable[(int)((Math.Truncate(((item.DRAFT_FORE + item.DRAFT_AFT) * 0.5 + 5) * 10)) - Math.Truncate((ballastValues.DRAFT_FORE + ballastValues.DRAFT_AFT) * 0.5 * 10))]; //  Draft (Ballast) Fore, Draft (Ballast) Aft

                        iso19030result.PV = Temp.PVcalculator(powerToSpeedEquation, correctPower, item.SPEED_LW);
                        iso19030result.PPV = Temp.PPVcalculator(powerToSpeedEquation, correctPower, item.SPEED_LW);
                        iso19030resultList.Add(iso19030result);
                    }
                    catch
                    {
                        FILTER_TOTAL_COUNT++;
                    }
                }
                index++;
            }
            

            return iso19030resultList;
        }
    }    
}
