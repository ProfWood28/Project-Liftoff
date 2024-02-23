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
            return force / mass;
        }

        public static Vector2Double Accel(float mass, Vector2Double force)
        {
            return force / mass;
        }

        public static Vector2 GravityAccel(float mass, Vector2 force)
        {
            return Accel(mass, new Vector2(0 + force.x, G + force.y));
        }
        public static Vector2Double GravityAccel(float mass, Vector2Double force)
        {
            return Accel(mass, new Vector2Double(0 + force.x, G + force.y));
        }
    }
}