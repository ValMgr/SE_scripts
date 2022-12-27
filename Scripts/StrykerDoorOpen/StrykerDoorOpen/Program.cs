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
        private IMyMotorSuspension _fr;
        private IMyMotorSuspension _fl;
        private IMyMotorSuspension _rr;
        private IMyMotorSuspension _rl;
        private IMyMotorAdvancedStator _hinge;
        private bool _doorState = false;
        private IEnumerator<bool> _stateMachine;

        public Program()
        {
            _fr = GridTerminalSystem.GetBlockWithName("WheelFR") as IMyMotorSuspension;
            _fl = GridTerminalSystem.GetBlockWithName("WheelFL") as IMyMotorSuspension;
            _rr = GridTerminalSystem.GetBlockWithName("WheelRR") as IMyMotorSuspension;
            _rl = GridTerminalSystem.GetBlockWithName("WheelRL") as IMyMotorSuspension;
            _hinge = GridTerminalSystem.GetBlockWithName("Hinge") as IMyMotorAdvancedStator;
            
        }

        IEnumerator<bool> CloseAndRaise()
        {
            _hinge.TargetVelocityRPM = -5;
            float pos = 0.35f;
            while (pos != -0.5f)
            {
                pos -= 0.025f;
                _fr.Height = pos;
                _fl.Height = pos;
                _rr.Height = pos;
                _rl.Height = pos;
                yield return true;
            }
            
        }


        IEnumerator<bool> OpenAndLower()
        {
            _hinge.TargetVelocityRPM = 5;
            float pos = -0.5f;
            while (pos != 0.35f)
            {
                pos += 0.025f;
                _fr.Height = pos;
                _fl.Height = pos;
                _rr.Height = pos;
                _rl.Height = pos;
                yield return true;
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (updateSource == UpdateType.Terminal || updateSource == UpdateType.Trigger)
            {
                if (_doorState)
                {
                    _stateMachine = CloseAndRaise();

                }
                else
                {
                    _stateMachine = OpenAndLower();                    
                }
                _doorState = !_doorState;
            }
           
            RunState();
        }

        public void RunState()
        {
            if (_stateMachine != null)
            {
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
                bool hasMoreStep = _stateMachine.MoveNext();

                if (!hasMoreStep)
                {
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                    _stateMachine.Dispose();
                    _stateMachine = null;
                }
            }
        }
    }
}
