using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

class MovingBackground : Sprite
{
    float moveMulti;
    float moveSpeed;
    Sprite unit1;
    Sprite unit2;

    public float fixedDeltaTime = 0.02f;
    public float accumulatedTime = 0;

    private LevelHandler levelHandler;
    private Train train;

    public MovingBackground(string fileName, float scale, float yPos, float spdMulti, LevelHandler lvlHandler, Train trn) : base ("Empty.png", false, false)
    {
        SetOrigin(0.5f, 0.5f);
        x = 0;
        y = 0;

        levelHandler = lvlHandler;

        train = trn;

        unit1 = new Sprite(fileName, false, false);
        unit1.SetOrigin(0.5f, 0.5f);
        unit1.SetScaleXY(scale);
        unit1.x = 0;
        unit1.y = yPos;
        AddChild(unit1);

        unit2 = new Sprite(fileName, false, false);
        unit2.SetOrigin(0.5f, 0.5f);
        unit2.SetScaleXY(scale);
        unit2.x = unit1.width;
        unit2.y = yPos;
        AddChild(unit2);

        moveMulti = spdMulti;
    }

    private void CycleUnits()
    {
        Console.WriteLine("Current unit1.x: {0} \nCurrent unit2.x: {1}", unit1.x, unit2.x);

        if(unit1.x < 0 - unit1.width)
        {
            unit1.x = unit2.x + unit2.width;
        }
        if (unit2.x < 0 - unit2.width)
        {
            unit2.x = unit1.x + unit1.width;
        }
    }

    private void MoveUnits()
    {
        moveSpeed = levelHandler.levelDistance - levelHandler.lastDistance;

        unit1.x += moveSpeed * moveMulti;
        unit2.x += moveSpeed * moveMulti;
    }

    private void Update()
    {
        if(train.isAlive)
        {
            CycleUnits();

            MoveUnits();
        }
    }

    private void FixedUpdate()
    {
        
    }

    private void RunFixedUpdate(float deltaTime)
    {
        accumulatedTime += deltaTime;
        while (accumulatedTime >= fixedDeltaTime * 1000)
        {
            FixedUpdate();
            accumulatedTime -= fixedDeltaTime * 1000;
        }
    }
}
