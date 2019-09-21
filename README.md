
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
  Lorem ipsum dolor sit amet consectetur adipisicing elit. Neque beatae magnam iure reiciendis, corrupti ratione ipsa. Explicabo, dolore quia? Nobis ipsum quae saepe numquam possimus excepturi deserunt enim ipsa repellendus.Lorem ipsum dolor sit amet consectetur adipisicing elit. Neque beatae magnam iure reiciendis, corrupti ratione ipsa. Explicabo, dolore quia? Nobis ipsum quae saepe numquam possimus excepturi deserunt enim ipsa repellendus.Lorem ipsum dolor sit amet consectetur adipisicing elit. Neque beatae magnam iure reiciendis, corrupti ratione ipsa. Explicabo, dolore quia? Nobis ipsum quae saepe numquam possimus excepturi deserunt enim ipsa repellendus.

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
`BREADTH` | Lorem ipsum dolor sit amet consectetur adipisicing elit.
`LOA` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`ME_POWER_MCR` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`TRANSVERSE_PROJECTION_AREA_BALLAST` | Lorem ipsum dolor sit amet consectetur adipisicing elit.

##### Draft

| Property | Means |
|---|:---:|
`BALLAST_DRAFT_FORE` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`BALLAST_DRAFT_AFT` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`SCANTLING_DRAFT_FORE` | Lorem ipsum dolor sit amet consectetur adipisicing eli
`SCANTLING_DRAFT_AFT` | Lorem ipsum dolor sit amet consectetur adipisicing eli


##### Speed Power
| Property | Means | 
|---|:---:|
`BALLAST_DRAFT_FORE` |  Lorem ipsum dolor sit amet consectetur adipisicing elit.  
`BALLAST_DRAFT_AFT` |  Lorem ipsum dolor sit amet consectetur adipisicing elit.  
`SCANTLING_DRAFT_FORE` |  Lorem ipsum dolor sit amet consectetur adipisicing elit.
`SCANTLING_DRAFT_AFT` |  Lorem ipsum dolor sit amet consectetur adipisicing elit.


##### Voyage Data
| Property | Means
|---|:---:|
`ID` | Lorem ipsum dolor sit amet consectetur adipisicing elit
`TIME_STAMP` | Lorem ipsum dolor sit amet consectetur adipisicing elit.
`SPEED_VG` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`SPEED_LW` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`REL_WIND_DIR` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`REL_WIND_SPEED` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`SHIP_HEADING` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`WATER_DEPTH` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`RUDDER_ANGLE` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`SW_TEMP` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`DRAFT_FORE` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`DRAFT_AFT` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`ME1_RPM_SHAFT` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 
`ME1_SHAFT_POWER` | Lorem ipsum dolor sit amet consectetur adipisicing elit. 




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
