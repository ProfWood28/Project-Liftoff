using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using GXPEngine;
using GXPEngine.Core;

class PlayerMK2 : PlayerController
{
    public PlayerMK2(string fileName, Vector2 pos, Level level) : base(fileName)
    {
        moveForce = 6f;
        jumpForce = 150;

        x = pos.x;
        y = pos.y;
        lvl = level;
    }
}