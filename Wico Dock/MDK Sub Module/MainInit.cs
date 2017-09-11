﻿using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
#region maininit

string sInitResults = "";
string sArgResults = "";

int currentInit = 0;

string doInit()
{

	if(currentInit==0) initLogging();

	// set autogyro defaults.
	LIMIT_GYROS = 1;
	minAngleRad = 0.09f;
	CTRL_COEFF = 0.75;

	Log("Init:"+currentInit.ToString());
	double progress=currentRun*100/3;
	string sProgress=progressBar(progress);
	StatusLog(sProgress,getTextBlock(sTextPanelReport));

	Echo("Init:"+currentInit.ToString());
	if(currentInit==0) 
	{
//StatusLog("clear",textLongStatus,true);
StatusLog(DateTime.Now.ToString()+OurName+":"+moduleName+":INIT",textLongStatus,true);

	if(!modeCommands.ContainsKey("launch")) modeCommands.Add("launch", MODE_LAUNCH);
	if(!modeCommands.ContainsKey("godock")) modeCommands.Add("godock", MODE_DOCKING);

		sInitResults+=	gridsInit();

		initTimers();

		sInitResults+=initSerializeCommon();

		Deserialize();
		sInitResults+=	gridsInit();
		sInitResults+=BlockInit();

		sInitResults+=thrustersInit(gpsCenter);
		sInitResults += sensorInit();
       sInitResults += camerasensorsInit(gpsCenter);

		sInitResults += connectorsInit();
		sInitResults+=gyrosetup(); 

		sInitResults += lightsInit();
		initShipDim();
		sInitResults+=modeOnInit(); 
		init=true;

	}

	currentInit++;
	if(init) currentInit=0;

	Log(sInitResults);
    Echo(sInitResults);

	return sInitResults;
}

IMyTextPanel gpsPanel=null;

string BlockInit()
{
	string sInitResults="";

	List < IMyTerminalBlock > centerSearch = new List < IMyTerminalBlock > ();
	GridTerminalSystem.SearchBlocksOfName(sGPSCenter, centerSearch);
	if (centerSearch.Count == 0) 
	{
		centerSearch=GetBlocksContains<IMyRemoteControl>("[NAV]");
		if(centerSearch.Count==0)
		{
			GridTerminalSystem.GetBlocksOfType < IMyRemoteControl > (centerSearch,localGridFilter);
			if(centerSearch.Count == 0) 
			{
				GridTerminalSystem.GetBlocksOfType < IMyShipController > (centerSearch,localGridFilter);
				if(centerSearch.Count == 0) 
				{
					sInitResults+="!!NO Controller";
					return sInitResults;
				}
				else
				{
					sInitResults+="S";
				}
			}
			else 
			{
				sInitResults+="R";
			}
		}
	}
	else
	{
		sInitResults+="N";
	}
	gpsCenter = centerSearch[0];

	List<IMyTerminalBlock> blocks =new List<IMyTerminalBlock>();
	blocks=GetBlocksContains<IMyTextPanel>("[GPS]");
	if(blocks.Count>0)
		gpsPanel=blocks[0] as IMyTextPanel;
    if (gpsCenter == null) Echo("ERROR: No control block found!");
	return sInitResults;
}

#endregion

string modeOnInit()
{
	if (iMode == MODE_DOCKING) 
	{
		ResetMotion();
		current_state = 0;
	}
	return ">";
}

    }
}