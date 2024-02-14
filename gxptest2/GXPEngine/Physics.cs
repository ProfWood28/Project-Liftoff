using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

namespace GXPEngine
{
    public class Physics
    {
        public static float G = 0f;
        
        public static Vector2 Accel(float mass, Vector2 force)
        {
            return new Core.Vector2(force.x / mass, force.y / mass);
        }

        public static Vector2 GravityAccel(float mass, Vector2 force)
        {
            return Accel(mass, new Vector2(0 + force.x, G + force.y));
        }
    }
}