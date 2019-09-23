
![hi](./image/iso_logo.jpg)

## Table Of Content

* [About](#📋About)
* [ISO19030 Process](#🛳️ISO19030-Process)
    * [Methods](️#⚙️Methods)
* [Attachments](#📁Attachments)
    * [SHIP PARTICULAR](#Particular)
    * [DRAFT](#Draft)
    * [SPEED POWER](#Speed-power)
    * [VOYAGE DATA](#Voyage-data)
    
* [Installation](#✏️Installation)
* [Customize](#🔧Customize-Sample)
* [Contact](#☎️Contact)
* [License](#📜License )


## 📋About 
  선체와 프로펠러 성능은 선박의 선체와 프로펠러의 상태와 선박을 일정 속도로 이동시키는 데 필요한 힘 사이의 관계를 나타냅니다. 시간에 따른 선박 별 선체 및 프로펠러 성능의 변화를 측정하면 선체 및 프로펠러 유지 보수, 수리 및 개조 활동이 해당 선박의 전체 에너지 효율에 미치는 영향을 나타낼 수 있습니다.
  
## 🛳️ISO19030 Process
![MethodProcess](./image/iso_inforgraph.png)

## ⚙️Methods
| Step | Match Method | File
|:---|:---|:---:|
|`Data retrieval`| DataRetrieval | MainProcess.cs
|`Data Compilation`| DataCompilation | MainProcess.cs
|`Data filtering and validation`| BasicFilteringContorller | Filters.cs
|`Correction for environmental factors`| Skip (Set Default Value) | None
|`Caculation of performance values`| PVcalculator, PPVcalculator | DataFunctions.cs
|`Filtering for reference condition`| FilteringForReferenceCondition | Filters.cs
|`Calculation of average percent speed loss in reference and evaluation periods`| Skip | None
|`Calculation of performance indicators`| Skip | None









## 📁Attachments

##### Particular 
| Property | Means | 
|---|:---:|
`BREADTH` | 선박의 폭
`LOA` | 선박의 전체 길이(Length of all) 
`ME_POWER_MCR` | MRC(Maximum Continuous Rating)에서 ME 출력
`TRANSVERSE_PROJECTION_AREA_BALLAST` | BALLAST 상태의 선박의 수선위 단면

##### Draft

| Property | Means |
|---|:---:|
`BALLAST_DRAFT_FORE` | BALLAST 상태의 선수 흘수
`BALLAST_DRAFT_AFT` | BALLAST 상태의 선미 흘수 
`SCANTLING_DRAFT_FORE` | SCANTLING 상태의 선수 흘수
`SCANTLING_DRAFT_AFT` | SCANTLING 상태의 선미 흘수


##### Speed Power
| Property | Means | 
|---|:---:|
`BALLAST_DRAFT_FORE` |  BALLAST 상태의 선수 흘수 
`BALLAST_DRAFT_AFT` |  BALLAST 상태의 선미 흘수 
`SCANTLING_DRAFT_FORE` |  SCANTLING 상태의 선수 흘수
`SCANTLING_DRAFT_AFT` |  SCANTLING 상태의 선미 흘수

##### Voyage Data
| Property | Means
|---|:---:|
`ID` | 선박의 고유식별번호(ex callsign)
`TIME_STAMP` | 해당 데이터 수집 시점(UTC)
`SPEED_VG` | 대지속도 
`SPEED_LW` | 대수속도 
`REL_WIND_DIR` | 상대풍향 
`REL_WIND_SPEED` | 상대풍속 
`SHIP_HEADING` | 선수각도 
`WATER_DEPTH` | 수심 
`RUDDER_ANGLE` | 타각 
`SW_TEMP` | 해수온도 
`DRAFT_FORE` | 선수흘수 
`DRAFT_AFT` | 선미흘수 
`ME1_RPM_SHAFT` | ME RPM 
`ME1_SHAFT_POWER` | ME POWER 




## ✏️Installation

1. Clone Project Repository 
````
    git clone https://github.com/Lab021/ISO19030.git
````
2. Open the solution file with Visual Studio (Recommed Version 2019)
3. Open Package Manager Console.
4. Input below message on Console.
````
    nuget install packages.config
````
5. Build Project
6. Get Data

## 🔧Customize Sample 
1. Open Csv File in Sample Folder
2. Add Custom Value [See Data Format](#📁Attachments)

## ☎️Contact
>Mail : lab021@lab021.co.kr
<br>Tel : +82 051-462-1029

## 📜License 
This Project is licensed Apache 2.0
