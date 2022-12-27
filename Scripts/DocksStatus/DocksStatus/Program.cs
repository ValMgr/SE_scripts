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
        private List<Dock> _docks = new List<Dock>();
        private List<IMyPistonBase> _dockPistons = new List<IMyPistonBase>();
        private List<IMyShipConnector> _dockConnectors = new List<IMyShipConnector>();

        private IMyTextSurface _display;


        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Once;
            Setup();
        }


        public void Main()
        {
            if (!_isInitialized)
            {
                Setup();
            }

            using(var frame = _display.DrawFrame())
            {

                int index = 1;
                foreach (var dock in _docks)
                {
                    Vector2 position = new Vector2(_display.TextureSize * (1f / (_docks.Count() + 1) * index), _display * 0.5f);
                    DrawSprites(frame, position, dock.GetStatus());
                    index++;
                }
                Echo(string.Format(@"Count: {0}, Div: {1}", _docks.Count(), 1f / (_docks.Count() + 1)));
            }
        }

       
        public void Setup()
        {
            _display = GridTerminalSystem.GetBlockWithName("SR_Debug") as IMyTextSurface;

            if(_display == null)
            {
                Logger.Log("Error: Display \"SR_Debug\" not found !", Me.GetSurface(0), true);
                return;
            }

            SetupDrawSurface(_display);

            IMyBlockGroup _groups = GridTerminalSystem.GetBlockGroupWithName("Docks");

            if(_groups == null)
            {
                Logger.Log("Error: Group \"Docks\" not found !", Me.GetSurface(0), true);
                return;
            }

            _groups.GetBlocksOfType(_dockPistons);
            _groups.GetBlocksOfType(_dockConnectors);

            if (_dockPistons.Count() != _dockConnectors.Count())
            {
                Logger.Log("Error: docks not complety built !", Me.GetSurface(0), true);
                return;
            }

            for (int i = 0; i < _dockPistons.Count(); i++)
            {
                _docks.Add(new Dock(_dockPistons[i], _dockConnectors[i]));
            }

            _isInitialized = true;
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            Echo("Succesfully initialized " + _docks.Count() +  " docks !");
            Logger.Log("Running ... ", Me.GetSurface(0));
        }

        public void SetupDrawSurface(IMyTextSurface surface)
        {
            surface.ScriptBackgroundColor = new Color(0, 0, 0, 255);
            surface.ContentType = ContentType.SCRIPT;
            surface.Script = "";
        }

        public void DrawSprites(MySpriteDrawFrame frame, Vector2 centerPos, MyShipConnectorStatus status, float scale = 1f)
        {
            Color colorState = status == MyShipConnectorStatus.Connected ? new Color(34, 187, 46, 255) : (status == MyShipConnectorStatus.Connectable ? new Color(0, 0, 255, 255) : new Color(255, 0, 0));

            frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(0f, 0f) * scale + centerPos, new Vector2(90f, 90f) * scale, new Color(128, 128, 128, 255), null, TextAlignment.CENTER, 0f)); // Connector_block
            frame.Add(new MySprite(SpriteType.TEXTURE, "CircleHollow", new Vector2(0f, 0f) * scale + centerPos, new Vector2(100f, 100f) * scale, colorState, null, TextAlignment.CENTER, 0f)); // Connector_status
            frame.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(0f, 0f) * scale + centerPos, new Vector2(95f, 95f) * scale, new Color(221, 211, 23, 255), null, TextAlignment.CENTER, 0f)); // Connector_outline
            frame.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(0f, 0f) * scale + centerPos, new Vector2(80f, 80f) * scale, new Color(128, 128, 128, 255), null, TextAlignment.CENTER, 0f)); // Connector_center
            frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(3f, 0f) * scale + centerPos, new Vector2(2f, 90f) * scale, new Color(221, 211, 23, 255), null, TextAlignment.CENTER, 0f)); // Connector_barCopy
            frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-3f, 0f) * scale + centerPos, new Vector2(2f, 90f) * scale, new Color(221, 211, 23, 255), null, TextAlignment.CENTER, 0f)); // Connector_bar
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

        public MyShipConnectorStatus GetStatus()
        {
            return _connector.Status;
        }

        public bool HasSomethingDock()
        {
            return _connector.Status == MyShipConnectorStatus.Connected;
        }

        public bool CanConnectToSomething()
        {
            return _connector.Status == MyShipConnectorStatus.Connectable;
        }
    }

}

