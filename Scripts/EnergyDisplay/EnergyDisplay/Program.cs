using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
  

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        readonly string lcdName = "power_display";

        public void Main(string argument)
        {
            Logger.Log("Script running...", Me.GetSurface(0));
            PrintToLCD("", true);
            PrintToLCD("                    Base Current Power");
            PrintToLCD("-------------------------------------------------------");
            GetStoredPower();
        }

        void GetStoredPower()
        {
            List<IMyTerminalBlock> _batteries = new List<IMyTerminalBlock>();
            IMyBlockGroup _batteriesGroup = GridTerminalSystem.GetBlockGroupWithName("Base_Batteries");
            _batteriesGroup.GetBlocks(_batteries);

            for (int i = 0; i < _batteries.Count(); i++)
            {
                IMyBatteryBlock _b = _batteries[i] as IMyBatteryBlock;
                float percentage = _b.CurrentStoredPower / _b.MaxStoredPower;
                string state = _b.IsCharging ? "Charging..." : "Decharging...";
                PrintToLCD("Batterie             " + String.Format("{0:P2}", percentage) + "       " + state);
            }
        }

        void PrintToLCD(string textToPrint, bool clear = false)
        {
            IMyTextSurface _lcd = GridTerminalSystem.GetBlockWithName(lcdName) as IMyTextSurface;
            if (_lcd != null)
            {
                if (clear)
                {
                    _lcd.WriteText("", false);
                }
                else
                {
                    _lcd.WriteText(textToPrint, true);
                    _lcd.WriteText("\n", true);
                }
            }
            else
            {
                Logger.Log("Error: LCD unavailable.", Me.GetSurface(0), true);
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
        }
    }
}
