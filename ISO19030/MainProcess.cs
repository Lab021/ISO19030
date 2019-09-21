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
        public static List<ISO19030RESULT> result = new List<ISO19030RESULT>();
        public static List<int> errorList = new List<int>();


        public static List<ISO19030File> sailingData = new List<ISO19030File>();
        public static List<double> ballastPowerList = new List<double>();
        public static List<double> ballastSpeedList = new List<double>();
        public static List<double> scantlingPowerList = new List<double>();
        public static List<double> scantlingSpeedList = new List<double>();

        public static SHIP_PARTICULAR shipParticular = new SHIP_PARTICULAR();
        public static List<SpeedPower> speedPowerData = new List<SpeedPower>();
        public static DRAFT draft = new DRAFT();

        public static dynamic ballastValues = new ExpandoObject();
        public static dynamic scantlingValues = new ExpandoObject();

        public static WindResistance windResistance;

        static void Main(string[] args)
        {
            var result = Analysis();
            var average = CalcAverageData(result);
            
            return;
        }

        //데이터 수집
        public static bool DataRetrieval()
        {
            var functionResult = true;
            
            //Particular File load
            shipParticular = CsvFileController.ReadParticularFile("particular.csv");
            //speed Power File Load
            speedPowerData = CsvFileController.ReadSpeedPowerDataFileToList("speedPower.csv");
            //draft Data 
            draft = CsvFileController.ReadDraftFile("draft.csv");

            //선박 운항 데이터 불러오기
            sailingData = CsvFileController.Read19030DataFileToList("voyageData.csv");

            foreach (var _d in speedPowerData)
            {
                ballastPowerList.Add(_d.BALLAST_POWER);
                ballastSpeedList.Add(_d.BALLAST_SPEED);
                scantlingPowerList.Add(_d.SCANTLING_POWER);
                scantlingSpeedList.Add(_d.SCANTLING_SPEED);
            }

            if (shipParticular == null || sailingData == null)
            {
                functionResult = false;                
            }
            return functionResult;
        }

        public static bool DataCompilation()
        {
            var functionResult = true;
            
            var ballastSpeedToPower = CurveFitting.powerRegression(ballastSpeedList.ToArray(), ballastPowerList.ToArray());
            var ballastPowerToSpeed = CurveFitting.powerRegression(ballastPowerList.ToArray(), ballastSpeedList.ToArray());

            var scantlingSpeedToPower = CurveFitting.powerRegression(scantlingSpeedList.ToArray(), scantlingPowerList.ToArray());
            var scantlingPowerToSpeed = CurveFitting.powerRegression(scantlingPowerList.ToArray(), scantlingSpeedList.ToArray());

            ballastValues.DRAFT_FORE = draft.BALLAST_DRAFT_FORE;
            ballastValues.DRAFT_AFT = draft.BALLAST_DRAFT_AFT;

            ballastValues.A_POWER_TO_SPEED = ballastPowerToSpeed[2];
            ballastValues.B_POWER_TO_SPEED = ballastPowerToSpeed[1];
            ballastValues.A_SPEED_TO_POWER = ballastSpeedToPower[2];
            ballastValues.B_SPEED_TO_POWER = ballastSpeedToPower[1];


            scantlingValues.DRAFT_FORE = draft.SCANTLING_DRAFT_FORE;
            scantlingValues.DRAFT_AFT = draft.SCANTLING_DRAFT_AFT;

            scantlingValues.A_POWER_TO_SPEED = scantlingPowerToSpeed[2];
            scantlingValues.B_POWER_TO_SPEED = scantlingPowerToSpeed[1];
            scantlingValues.A_SPEED_TO_POWER = scantlingSpeedToPower[2];
            scantlingValues.B_SPEED_TO_POWER = scantlingSpeedToPower[1];

            if(ballastValues == null || scantlingValues == null)
            {
                functionResult = false;
            }

            return functionResult;
        }

        public static ISO19030RESULT CalcAverageData(List<ISO19030RESULT> iso19030resultList)
        {
            var result = new ISO19030RESULT();

            result.PV = iso19030resultList.Average(d => d.PV);                       
            result.SPEED_VG = iso19030resultList.Average(d => d.SPEED_VG);
            result.SPEED_LW = iso19030resultList.Average(d => d.SPEED_LW);
            result.SHAFT_POWER = iso19030resultList.Average(d => d.SHAFT_POWER);
            result.SHAFT_REV = iso19030resultList.Average(d => d.SHAFT_REV);

            result.DRAFT_FORE = iso19030resultList.Average(d => d.DRAFT_FORE);
            result.DRAFT_AFT = iso19030resultList.Average(d => d.DRAFT_AFT);

            return result;
        }


        public static List<ISO19030RESULT> Analysis()
        {
            // Collect Data and Compilation
            if (!(DataRetrieval() && DataCompilation()))
            {
                return null;
            }

            //Chauvent Filtering & Validation 
            sailingData = Filters.BasicFilteringContorller(sailingData);

            //Create wind Resistance
            windResistance = WindResistance.CreateWindResistance(shipParticular, sailingData.Count());
            
            //Environment Filtering
            sailingData = Filters.FilteringForReferenceCondition(sailingData, shipParticular, ballastValues, scantlingValues, windResistance);

            //Create Reference 
            List<double[]> powerToSpeedTable = DataFunctions.PowerToSpeedTable(shipParticular, ballastValues, scantlingValues);  

            //var nowDraft = sailingData.Where(d => d.DRAFT_FORE != -9999).Average(d => d.DRAFT_FORE);
            //var modelDraftMeanBallast = (ballastValues.DRAFT_FORE + ballastValues.DRAFT_AFT) / 2;
            //var modelDraftMeanScantling = (scantlingValues.DRAFT_FORE + scantlingValues.DRAFT_AFT) / 2;

            //Create Result Data
            var iso19030resultList = CreateResultData(sailingData, powerToSpeedTable);
            
            return iso19030resultList;
        }
        
        public static List<ISO19030RESULT> CreateResultData(List<ISO19030File> sailingData, List<double[]> powerToSpeedTable)
        {
            int index = 0;
            var iso19030resultList = new List<ISO19030RESULT>();

            foreach (var item in sailingData)
            {
                if (item.VALID_CHAUVENT == true && item.VALID_VALIDATION == true && item.VALID_REFCONDITION == true)
                {
                    try
                    {
                        var iso19030result = new ISO19030RESULT();


                        var speedLwMeter = item.SPEED_VG * 0.5144;
                        var Rw = windResistance.data.raa[index] / 1000 * speedLwMeter / 0.98;
                        var correctPower = item.ME1_SHAFT_POWER - (windResistance.data.raa[index] / 1000 * speedLwMeter) / 0.98;
                        var powerToSpeedEquation = powerToSpeedTable[(int)((Math.Truncate(((item.DRAFT_FORE + item.DRAFT_AFT) * 0.5 + 5) * 10)) - Math.Truncate((ballastValues.DRAFT_FORE + ballastValues.DRAFT_AFT) * 0.5 * 10))]; //  Draft (Ballast) Fore, Draft (Ballast) Aft

                        iso19030result.SPEED_VG = item.SPEED_VG;
                        iso19030result.SPEED_LW = item.SPEED_LW;
                        iso19030result.SHAFT_POWER = item.ME1_SHAFT_POWER;
                        iso19030result.SHAFT_REV = item.ME1_RPM_SHAFT;
                        iso19030result.DRAFT_FORE = item.DRAFT_FORE;
                        iso19030result.DRAFT_AFT = item.DRAFT_AFT;
                        iso19030result.CORRECT_POWER = correctPower;

                        //Calculate Performance 
                        iso19030result.PV = DataFunctions.PVcalculator(powerToSpeedEquation, correctPower, item.SPEED_LW);
                        iso19030result.PPV = DataFunctions.PPVcalculator(powerToSpeedEquation, correctPower, item.SPEED_LW);

                        iso19030resultList.Add(iso19030result);
                    }
                    catch
                    {
                        Filters.FILTER_TOTAL_COUNT++;
                    }
                }
                index++;
            }

            return iso19030resultList;
        }
        
    }    
}
