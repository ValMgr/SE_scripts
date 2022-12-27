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
        private IMyShipConnector _connector;
        private IMyPistonBase _piston;
        private IMyTextPanel _display;

        public Program()
        {
            _connector = GridTerminalSystem.GetBlockWithName("igorConnector") as IMyShipConnector;
            _piston = GridTerminalSystem.GetBlockWithName("igorPiston") as IMyPistonBase;
            _display = GridTerminalSystem.GetBlockWithName("igorScreen1") as IMyTextPanel;
        }


        public void Main()
        {
            Vector3D _connectorWorldPosition = _connector.GetPosition();
            Vector3D _connectorPosition = GetRelativePosition(_piston, new Vector3I((int)_connectorWorldPosition.X, (int)_connectorWorldPosition.Y, (int)_connectorWorldPosition.Z));
            //_display.WriteText(string.Format("X: {0} \n Y: {1} \n Z: {2}", _connectorPosition.X, _connectorPosition.Y, _connectorPosition.Z));

            double connectorPosY = Math.Floor(_connectorPosition.Y);
            double posToReach = 29736;

            _display.WriteText(Convert.ToString(posToReach) + " " + connectorPosY);

            _piston.Enabled = true;
            _display.WriteText("\n", true);
            _display.WriteText("Turning on piston", true);

            
            // Change with coroutine instead of weird UpdateFrequency
            if (connectorPosY != posToReach)
            {
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
                if (connectorPosY < posToReach)
                {
                    _piston.Velocity = 5;
                    _display.WriteText("\n", true);
                    _display.WriteText("Extending.", true);

                }
                if (connectorPosY > posToReach)
                {
                    _piston.Velocity = -5;
                    _display.WriteText("\n", true);
                    _display.WriteText("Descending.", true);
                }
            }
            else
            {
                Runtime.UpdateFrequency = UpdateFrequency.None;
                _display.WriteText("\n", true);
                _display.WriteText("Turning off piston", true);
                _piston.Enabled = false;
            }
            
        }

        Vector3D GetRelativePosition(IMyCubeBlock origin, Vector3I offset)
        {
            Vector3D basePosition = origin.GetPosition();

            Vector3D upVector = origin.WorldMatrix.Up;
            Vector3D leftVector = origin.WorldMatrix.Left;
            Vector3D backwardVector = origin.WorldMatrix.Backward;

            int leftDistance = offset.X;
            int upDistance = offset.Y;
            int backDistance = offset.Z;

            return basePosition + upVector * upDistance + backwardVector * backDistance + leftVector * leftDistance;
        }

    }
}
