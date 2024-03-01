using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

class Button : AnimationSprite
{
    public int send;


    public Button(string fileName, Vector2 pos, int buttonSend, float scale) : base(fileName, 2, 1, 2, false, true)
    {
        x = pos.x;
        y = pos.y;
   
        SetScaleXY(scale);

        send = buttonSend;
    }
}