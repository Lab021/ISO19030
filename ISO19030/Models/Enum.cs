using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.Models
{
    public static class Enum
    {
        public static string GetDescriptionFromEnumValue(System.Enum value)
        {
            DescriptionAttribute descriptionAttribute = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault<object>() as DescriptionAttribute;
            if (descriptionAttribute != null)
            {
                return descriptionAttribute.Description;
            }
            return value.ToString();
        }

        public static T GetEnumValueFromDescription<T>(string description)
        {
            Type type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException();
            }
            FieldInfo[] fields = type.GetFields();
            var variable = (
                from f in fields
                from a in f.GetCustomAttributes(typeof(DescriptionAttribute), false)
                select new { Field = f, Att = a } into a
                where ((DescriptionAttribute)a.Att).Description == description
                select a).SingleOrDefault();
            if (variable == null)
            {
                return default(T);
            }
            return (T)variable.Field.GetRawConstantValue();
        }

        public enum CurrentMethods
        {
            [Description("Mean of means")]
            MeanofMeans,
            [Description("Iterative")]
            Iterative
        }

        public enum DirectPowerMethods
        {
            [Description("Direct power method (12.2.3, Annex J)")]
            LoadVariation,
            [Description("DPM with full scale wake (Annex K)")]
            FullScaleWake
        }

        public enum ITTCChartTypes
        {
            [Description("Container carrier, laden with container")]
            ContainerLadenContainer,
            [Description("Container carrier, laden without container, with lashing bridge")]
            ContainerLadenLashing,
            [Description("Container carrier, ballast without container, with lashing bridge")]
            ContainerBallastLashing,
            [Description("Container carrier, ballast without container, without lashing bridge")]
            ContainerBallast,
            [Description("Tanker Conventional bow, laden")]
            TankerConventionalLaden,
            [Description("Tanker Conventional bow, ballast")]
            TankerConventionalBallast,
            [Description("Tanker Cylinderical bow, ballast")]
            TankerCylindericalBallast,
            [Description("Car carrier, average")]
            CarCarrierAverage,
            [Description("LNG carrier, spherical")]
            LNGSpherical,
            [Description("LNG carrier, prismatic extended deck")]
            LNGPrismaticExtended,
            [Description("LNG carrier, prismatic integrated")]
            LNGPrismaticIntegrated,
            [Description("General cargo, average")]
            GeneralCargoAverage,
            [Description("Cruise and ferry, average")]
            CruiseFerryAverage
        }

        public enum PowerTypes
        {
            ShaftPower,
            BrakePower,
            DeliveredPower
        }

        public enum TemperatureInputTypes
        {
            [Description("Temperature")]
            Temperature,
            [Description("Density")]
            Density
        }

        public enum WaveMethods
        {
            [Description("Theoretical method with simplified tank tests in short waves")]
            TheoreticalwithModelTest,
            [Description("Theoretical method with fitting formulae for the reflection wave")]
            TheoreticalwithFittingFormulae,
            [Description("STAWAVE-2")]
            STA2,
            [Description("STAWAVE-1")]
            STA1,
            [Description("Seakeeping model tests")]
            SeakeepingModelTests
        }

        public enum WindChartTypes
        {
            [Description("Fujiwara regression formulae")]
            Fujiwara,
            [Description("ITTC charts")]
            ITTC,
            [Description("Wind tunnel test results")]
            WindTunnel
        }
    }
}
