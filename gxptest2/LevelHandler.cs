using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

class LevelHandler : GameObject
{
    public float levelSpeed = 3;

    public int fixedDeltaTime = 20;
    public float accumulatedTime = 0;

    public LevelHandler()
    {
        
    }
    private void Update()
    {
        RunFixedUpdate(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        levelSpeed += 0.001f;
        SpawnShit();
    }
    private void RunFixedUpdate(float deltaTime)
    {
        accumulatedTime += deltaTime;

        while (accumulatedTime >= fixedDeltaTime)
        {
            FixedUpdate();
            accumulatedTime -= fixedDeltaTime;
        }
    }

    private void SpawnShit()
    {
        var rand = new Random();

        if(rand.Next(101) == 69)
        {
            game.AddChild(new Shit(rand.Next(40, game.height - 40), levelSpeed));
        }
    }

    class Shit : Sprite
    {
        float speed;

        public int fixedDeltaTime = 20;
        public float accumulatedTime = 0;

        float mass = 699;
        float baseForce = -1f;
        float friction = 0.1f;

        public Vector2 velocity = new Vector2(0, 0);
        public Vector2 velocityMoment = new Vector2(0, 0);
        public Shit(int randomY, float gameSpeed) : base("checkers.png", false)
        {
            SetOrigin(width / 2, height / 2);
            SetScaleXY(0.5f, 0.5f);
            SetXY(game.width - 100, randomY);
            speed = gameSpeed;
            baseForce = baseForce * speed;
        }

        private void Update()
        {
            RunFixedUpdate(Time.deltaTime);
        }
        private void FixedUpdate()
        {
            ForceToVelocity();

            ApplyVelocity();
        }
        private void RunFixedUpdate(float deltaTime)
        {
            accumulatedTime += deltaTime;

            while (accumulatedTime >= fixedDeltaTime)
            {
                FixedUpdate();
                accumulatedTime -= fixedDeltaTime;
            }
        }
        private void ForceToVelocity()
        {
            Vector2 totalForces = new Vector2(baseForce, 0);

            Vector2 accel = Physics.Accel(mass, totalForces);
            accel = new Vector2(accel.x * fixedDeltaTime, accel.y * fixedDeltaTime);
            velocity = new Vector2(velocity.x + accel.x, velocity.y + accel.y);
            velocity = new Vector2(velocity.x - velocity.x * friction, velocity.y - velocity.y * friction);
            velocityMoment = new Vector2(velocity.x * fixedDeltaTime, velocity.y * fixedDeltaTime);
        }
        private void ApplyVelocity()
        {
            x += velocityMoment.x;
            y += velocityMoment.y;
        }
    }
}
