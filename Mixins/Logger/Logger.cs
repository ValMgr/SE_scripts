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
    partial class Program
    {
        public class Logger
        {
            public static void Log(string textToLog, IMyTextSurface screen, bool isError = false)
            {
                Color _textColor = isError ? Color.Red : Color.Green;
                Color _bkgColor = Color.Black;

                screen.ContentType = ContentType.TEXT_AND_IMAGE;
                screen.TextPadding = 50f;
                screen.Alignment = TextAlignment.CENTER;
                screen.FontColor = _textColor;
                screen.BackgroundColor = _bkgColor;
                screen.WriteText(textToLog);
            }
        }
    }
}

