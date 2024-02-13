using System;
using GXPEngine.Core;

namespace GXPEngine
{
   public struct InputEvent
    {
        public enum InputType { KeyPress, MouseClick, Axis }

        public InputType type;
        public int keyID;
        public int axisValue;
        public Vector2 mousePosition;
        public int mouseButton;
        public float timestamp;
    }
}
