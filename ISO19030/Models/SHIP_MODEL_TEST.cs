using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO19030.Models
{
    public class SHIP_MODEL_TEST
    {
        public long ID { get; set; }
        public string SHIP_KEY { get; set; }
        //옵션
        //바람 관련
        public double ZA_BALLAST { get; set; }   // anemometer height at ballast height / 단위 m (ITTC)

        public double TRANSVERSE_PROJECTION_AREA_BALLAST { get; set; }  //Transverse Projected Area above Waterline / 단위 m (ITTC)
        public double TRANSVERSE_PROJECTION_AREA_SCANTLING { get; set; }  //Transverse Projected Area above Waterline / 단위 m (ITTC)

        //파도 관련
        public double KYY { get; set; }  //Non-Dimensional Radius / 단위 -

        public double DRAFT_FORE { get; set; }
        public double DRAFT_AFT { get; set; }
        public double CB_BALLAST { get; set; }   // Block Coefficient at ballast / 단위 -
        public double CB_SCANTLING { get; set; }   // Block Coefficient at ballast / 단위 -

        //수온 및 염도 관련
        public double SUBMERGED_SURFACE_BALLAST { get; set; }   //수선 밑 면적 Ballast - interpolation 필요

        public double SUBMERGED_SURFACE_SCANTLING { get; set; } // 수선 및 면적 Scantling - interpolation 필요

        //Speed Power Estimation
        public double MID_SHIP_SECTION_AREA_BALLAST { get; set; }       //수면 하부 중앙 단면 면적 at Ballast

        public double MID_SHIP_SECTION_AREA_SCANTLING { get; set; }       //수면 하부 중앙 단면 면적 at Sacntling

        public double DISPLACEMENT_BALLAST { get; set; }
        public double DISPLACEMENT_SCANTLING { get; set; }

        public string SPEED_etaD_BALLAST { get; set; }
        public string etaD_BALLAST { get; set; }
        public string SPEED_etaD_SCANTLING { get; set; }
        public string etaD_SCANTLING { get; set; }
    }
}
