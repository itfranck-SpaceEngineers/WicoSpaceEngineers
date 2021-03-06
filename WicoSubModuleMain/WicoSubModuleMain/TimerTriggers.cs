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
        string sFastTimer="[WCCT]";
        string sSubModuleTimer = "[WCCS]";
        string sMainTimer = "[WCCM]";

        // 03182018 Added check for enabled. Added CustomData
        // added doTriggerMain() for sub-module.
        // 11/06 return true if timer was found and triggered
        //03/27: Added caching for performance

        string sTimersSection = "WICOTIMERS";
        void TimersInitCustomData(INIHolder iNIHolder)
        {
            iNIHolder.GetValue(sTimersSection, "FastTimer", ref sFastTimer, true);
            iNIHolder.GetValue(sTimersSection, "SubModuleTimer", ref sSubModuleTimer, true);
            iNIHolder.GetValue(sTimersSection, "MainTimer", ref sMainTimer, true);
        }

        // cache the timer blocks
        Dictionary<string, List<IMyTerminalBlock>> dTimers = new Dictionary<string, List<IMyTerminalBlock>>();

        void initTimers()
        {
            dTimers.Clear();
        }

        void doTriggerMain()
        {
            if (!doSubModuleTimerTriggers(sFastTimer))
                doSubModuleTimerTriggers(sMainTimer);
        }

        bool doSubModuleTimerTriggers(string sKeyword = "[WCCS]")
        {
            bool bTriggered = false;
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

            IMyTimerBlock theTriggerTimer = null;

            if (dTimers.ContainsKey(sKeyword))
            {
                blocks = dTimers[sKeyword];
            }
            else
            {
                blocks = GetBlocksContains<IMyTerminalBlock>(sKeyword);
                dTimers.Add(sKeyword, blocks);
            }

            for (int i = 0; i < blocks.Count; i++)
            {
                theTriggerTimer = blocks[i] as IMyTimerBlock;
                if (theTriggerTimer != null)
                {
                    //            Echo("dSMT:" + blocks[i].CustomName);
                    if (theTriggerTimer.Enabled)
                    {
                        theTriggerTimer.Trigger();
                        //                    theTriggerTimer.ApplyAction("TriggerNow");
                        bTriggered = true;
                    }
                    else
                    {
                        Echo("Timer:" + theTriggerTimer.CustomName + " is OFF");
                    }
                }
            }
            return bTriggered;
        }
    }
}