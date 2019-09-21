using CsvHelper;
using ISO19030.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.DataProcessing
{
    public class CsvFileController
    {
        public static SHIP_PARTICULAR ReadParticularFile(string fileName)
        {

            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string sampleFilePath = Path.Combine(projectDirectory, "SAMPLE", fileName);


            var stream = new StreamReader(sampleFilePath);
            var csvData = new CsvReader(stream);

            var shipParticular = csvData.GetRecords<SHIP_PARTICULAR>().ToList().First();

            return shipParticular;
        }

        public static DRAFT ReadDraftFile(string fileName)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string sampleFilePath = Path.Combine(projectDirectory, "SAMPLE", fileName);

            var stream = new StreamReader(sampleFilePath);            
            var csvData = new CsvReader(stream);

            var draft = csvData.GetRecords<DRAFT>().ToList().First();

            return draft;
        }

        public static SHIP_MODEL_TEST ReadShipModelTestFile(string fileName)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string sampleFilePath = Path.Combine(projectDirectory, "SAMPLE", fileName);

            var stream = new StreamReader(sampleFilePath);
            var csvData = new CsvReader(stream);

            var shipModelTest = csvData.GetRecords<SHIP_MODEL_TEST>().ToList().First();

            return shipModelTest;
        }

        public static List<SpeedPower> ReadSpeedPowerDataFileToList(string fileName)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string sampleFilePath = Path.Combine(projectDirectory, "SAMPLE", fileName);

            var stream = new StreamReader(sampleFilePath);
            var csvData = new CsvReader(stream);
            var speedPowers = csvData.GetRecords<SpeedPower>().ToList();

            if (speedPowers.Count() == 0)
            {
                return null;
            }

            return speedPowers;
        }

        

        public static List<ISO19030File> Read19030DataFileToList(string fileName)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string sampleFilePath = Path.Combine(projectDirectory, "SAMPLE", fileName);

            var stream = new StreamReader(sampleFilePath);
            var csvData = new CsvReader(stream);

            var iSO19030Data = csvData.GetRecords<ISO19030File>().ToList();

            if (iSO19030Data.Count() == 0)
            {
                return null;
            }

            return iSO19030Data;
        }
    }
}
