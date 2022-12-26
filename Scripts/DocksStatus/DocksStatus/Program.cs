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
        private bool _isInitialized = false;
        private List<Dock> _docks;
        private List<IMyPistonBase> _dockPistons = new List<IMyPistonBase>();
        private List<IMyShipConnector> _dockConnectors = new List<IMyShipConnector>();

        private IMyTextSurface _display;


        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Once;
            Setup();
        }

           

        public void Main(string argument, UpdateType updateSource)
        {
            if (!_isInitialized)
            {
                Setup();
            }


        }

       
        public void Setup()
        {
           
            IMyBlockGroup _groups = GridTerminalSystem.GetBlockGroupWithName("Docks");
            _groups.GetBlocksOfType(_dockPistons);
            _groups.GetBlocksOfType(_dockConnectors);

            if (_dockPistons.Count() != _dockConnectors.Count())
            {
                Logger.Log("Error: docks not complety built !", Me.GetSurface(0), true);
                Echo("Error: docks not complety built !");
                return;
            }

            for (int i = 0; i < _dockPistons.Count(); i++)
            {
                _docks.Add(new Dock(_dockPistons[i], _dockConnectors[i]));
            }

            _isInitialized = true;
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            Echo("Succesfully initialized");
            Logger.Log("Running ... ", Me.GetSurface(0));
        }
        
    }

    public class Dock
    {
        private IMyPistonBase _piston;
        private IMyShipConnector _connector;

        public Dock(IMyPistonBase piston, IMyShipConnector connector)
        {
            _piston = piston;
            _connector = connector;
        }

        public void GetPistonPosition()
        {
            _piston.
        }

        public double GetConnectorPosition()
        {
            return _connector.GetPosition().Y;
        }

        public bool HasSomethingDock()
        {
            return _connector.Status == MyShipConnectorStatus.Connected ? true : false;
        }

        public bool CanConnectToSomething()
        {
            return _connector.Status == MyShipConnectorStatus.Connectable;
        }
    }

}

