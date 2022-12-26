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
                DrawSprites(frame, _display.TextureSize * 0.5f, 0.1f);
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

        public void DrawSprites(MySpriteDrawFrame frame, Vector2 centerPos, float scale = 1f)
        {
            frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(2f, -39f) * scale + centerPos, new Vector2(90f, 90f) * scale, new Color(192, 192, 192, 255), null, TextAlignment.CENTER, 0f)); // Piston_baseCopy
            frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(2f, 204f) * scale + centerPos, new Vector2(48f, 30f) * scale, new Color(192, 192, 192, 255), null, TextAlignment.CENTER, 0f)); // Piston_base
            frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(2f, 69f) * scale + centerPos, new Vector2(40f, 300f) * scale, new Color(192, 192, 192, 255), null, TextAlignment.CENTER, 0f)); // Piston
            frame.Add(new MySprite(SpriteType.TEXTURE, "CircleHollow", new Vector2(2f, -39f) * scale + centerPos, new Vector2(100f, 100f) * scale, new Color(34, 187, 46, 255), null, TextAlignment.CENTER, 0f)); // Connector_status
            frame.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(2f, -39f) * scale + centerPos, new Vector2(95f, 95f) * scale, new Color(221, 211, 23, 255), null, TextAlignment.CENTER, 0f)); // Connector_outline
            frame.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(2f, -39f) * scale + centerPos, new Vector2(80f, 80f) * scale, new Color(192, 192, 192, 255), null, TextAlignment.CENTER, 0f)); // Connector_center
            frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(2f, -41f) * scale + centerPos, new Vector2(5f, 90f) * scale, new Color(221, 211, 23, 255), null, TextAlignment.CENTER, 0f)); // Connector_bar
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

