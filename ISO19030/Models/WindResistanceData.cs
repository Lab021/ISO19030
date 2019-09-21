using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.Models
{
    public class WindResistanceData
    {
        public Chart Chart;

        public int nTrial;

        public double[] zref;

        //public double za;

        public double[] za;

        public double aod;

        public double ayv;

        public double cmc;

        public double hbr;

        public double hc;

        public double mu;

        public bool isAveraging;

        public double loa;

        public double breadth;

        //public double axv;

        public double[] axv;

        public double lbp;

        public Enum.WindChartTypes windChartType;

        public Enum.ITTCChartTypes iTTCChartType;

        public double[] vg;

        public double[] psi0;

        public double[] rhoair;

        public double[] vwr;

        public double[] psiwr;

        public Chart windTunnel;

        public Chart iTTCCarCarrierAverage;

        public Chart iTTCContainerBallast;

        public Chart iTTCContainerBallastLashing;

        public Chart iTTCContainerLadenContainer;

        public Chart iTTCContainerLadenLashing;

        public Chart iTTCCruiseFerryAverage;

        public Chart iTTCGeneralCargoAverage;

        public Chart iTTCLNGPrismaticExtended;

        public Chart iTTCLNGPrismaticIntegrated;

        public Chart iTTCLNGSpherical;

        public Chart iTTCTankerConventionalBallast;

        public Chart iTTCTankerConventionalLaden;

        public Chart iTTCTankerCylindericalBallast;

        public double caa0;

        public double[] vwt;

        public double[] psiwt;

        public double[] vwtaverage;

        public double[] psiwtaverage;

        public double[] vwraverage;

        public double[] psiwraverage;

        public double[] vwtref;

        public double[] vwref;

        public double[] psiwref;

        public double[] caa;

        public double[] raa;
        public double[] raa0;

        public string warning = "";

        public WindResistanceData()
        {

            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            var programPath = Path.Combine(projectDirectory, "resource");

            iTTCCarCarrierAverage = InitialiseChartfromResourceStream(programPath + "\\WindResistance.Resources.ITTCCarCarrierAverage.txt", "ITTC Car Carrier Average");
            iTTCContainerBallast = InitialiseChartfromResourceStream(programPath + "\\WindResistance.Resources.ITTCContainerBallast.txt", "ITTC Container Ballast");
            iTTCContainerBallastLashing = InitialiseChartfromResourceStream(programPath + "\\WindResistance.Resources.ITTCContainerBallastLashing.txt", "ITTC Container Ballast Lashing");
            iTTCContainerLadenContainer = InitialiseChartfromResourceStream(programPath + "\\WindResistance.Resources.ITTCContainerLadenContainer.txt", "ITTC Container Laden Container");
            iTTCContainerLadenLashing = InitialiseChartfromResourceStream(programPath + "\\WindResistance.Resources.ITTCContainerLadenLashing.txt", "ITTC Container Laden Lashing");
            iTTCCruiseFerryAverage = InitialiseChartfromResourceStream(programPath + "\\WindResistance.Resources.ITTCCruiseFerryAverage.txt", "ITTC Cruise Ferry Average");
            iTTCGeneralCargoAverage = InitialiseChartfromResourceStream(programPath + "\\WindResistance.Resources.ITTCGeneralCargoAverage.txt", "ITTC General Cargo Average");
            iTTCLNGPrismaticExtended = InitialiseChartfromResourceStream(programPath + "\\WindResistance.Resources.ITTCLNGPrismaticExtended.txt", "ITTC LNG Prismatic Extended");
            iTTCLNGPrismaticIntegrated = InitialiseChartfromResourceStream(programPath + "\\WindResistance.Resources.ITTCLNGPrismaticIntegrated.txt", "ITTC LNG Prismatic Integrated");
            iTTCLNGSpherical = InitialiseChartfromResourceStream(programPath + "\\WindResistance.Resources.ITTCLNGSpherical.txt", "ITTC LNG Spherical");
            iTTCTankerConventionalBallast = InitialiseChartfromResourceStream(programPath + "\\WindResistance.Resources.ITTCTankerConventionalBallast.txt", "ITTC Tanker Conventional Ballast");
            iTTCTankerConventionalLaden = InitialiseChartfromResourceStream(programPath + "\\WindResistance.Resources.ITTCTankerConventionalLaden.txt", "ITTC Tanker Conventiona Laden");
            iTTCTankerCylindericalBallast = InitialiseChartfromResourceStream(programPath + "\\WindResistance.Resources.ITTCTankerCylindericalBallast.txt", "ITTC Tanker Cylinderical Ballast");
        }

        private Chart InitialiseChartfromResourceStream(string stream, string chartname)
        {
            StreamReader streamReader = new StreamReader(stream);
            string[] strArrays = streamReader.ReadLine().Split(new char[] { '\t' });
            int num = Convert.ToInt32(strArrays[0]);
            int num1 = Convert.ToInt32(strArrays[1]);
            Chart chart = new Chart(chartname, num1, num);
            for (int i = 0; i < num; i++)
            {
                strArrays = new string[num1 + 1];
                strArrays = streamReader.ReadLine().Split(new char[] { '\t' });
                chart.x[i] = Convert.ToDouble(strArrays[0]);
                for (int j = 0; j < num1; j++)
                {
                    chart.y[j, i] = Convert.ToDouble(strArrays[j + 1]);
                }
            }
            streamReader.Close();
            return chart;
        }

        public void SetSize(int nTrial)
        {
            this.nTrial = nTrial;
            zref = new double[nTrial];

            vg = new double[nTrial];
            psi0 = new double[nTrial];
            rhoair = new double[nTrial];
            vwr = new double[nTrial];
            psiwr = new double[nTrial];
            vwt = new double[nTrial];
            psiwt = new double[nTrial];
            vwtaverage = new double[nTrial];
            psiwtaverage = new double[nTrial];
            vwraverage = new double[nTrial];
            psiwraverage = new double[nTrial];
            vwtref = new double[nTrial];
            vwref = new double[nTrial];
            psiwref = new double[nTrial];
            caa = new double[nTrial];
            raa = new double[nTrial];
            raa0 = new double[nTrial];

            axv = new double[nTrial];
            za = new double[nTrial];
        }
    }
}
