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
    public float levelDistance = 0;

    public int fixedDeltaTime = 20;
    public float accumulatedTime = 0;

    EasyDraw bg;
    Train train;

    Random random = new Random();

    List<int> breakLengths = new List<int>();
    List<int> breakStarts = new List<int>();
    List<int> breakTracks = new List<int>();

    List<int> breakAbleTracks = new List<int>() { 0, 1, 2, 3, 4 };

    int distanceToNextBreak = 0;
    int lastBreak = 0;

    public LevelHandler(EasyDraw bgIn, Train trainIn)
    {
        train = trainIn;
        bg = bgIn;

        distanceToNextBreak = game.width * 2;

        breakLengths.Add(random.Next(865, 5051));
        breakTracks.Add(random.Next(0, 5));
        breakStarts.Add(distanceToNextBreak + lastBreak);
    }
    private void Update()
    {
        genTrackBreaks();
        drawTempTrack();

        RunFixedUpdate(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        levelSpeed += 0.005f;
        levelDistance += levelSpeed * fixedDeltaTime / 10;
        //SpawnShit();
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

    private void genTrackBreaks()
    {
        if(breakStarts != null && levelDistance > breakStarts[0] + breakLengths[0])
        {
            breakStarts.RemoveAt(0);
            breakLengths.RemoveAt(0);

            breakAbleTracks.Add(breakTracks[0]);
            breakTracks.RemoveAt(0);
        }

        if(levelDistance > distanceToNextBreak + lastBreak && breakAbleTracks.Count > 4)
        {
            int trackID = breakAbleTracks[random.Next(0, breakAbleTracks.Count)];
            breakAbleTracks.Remove(trackID);

            lastBreak = distanceToNextBreak + lastBreak;
            distanceToNextBreak = random.Next(352, 1024);

            int randomNewStart = distanceToNextBreak + Mathf.Round(levelDistance) + game.width;

            breakTracks.Add(trackID);

            breakStarts.Add(randomNewStart);
            
            breakLengths.Add(random.Next(865, 5051));
        }
    }
    private void drawTempTrack()
    {
        int trackoffset = 20;
        for (int i = 0; i < train.trackHeights.Count; i++)
        {
            if(breakTracks.Contains(i))
            {
                int index = breakTracks.IndexOf(i);
                //first set
                bg.Line(-100, train.trackHeights[i] - trackoffset, breakStarts[index] - levelDistance, train.trackHeights[i] - trackoffset);
                bg.Line(-100, train.trackHeights[i] + trackoffset, breakStarts[index] - levelDistance, train.trackHeights[i] + trackoffset);
                //second set
                bg.Line(breakStarts[index] + breakLengths[index] - levelDistance, train.trackHeights[i] - trackoffset, game.width, train.trackHeights[i] - trackoffset);
                bg.Line(breakStarts[index] + breakLengths[index] - levelDistance, train.trackHeights[i] + trackoffset, game.width, train.trackHeights[i] + trackoffset);
            }

            else
            {
                bg.Line(-100, train.trackHeights[i] - trackoffset, game.width, train.trackHeights[i] - trackoffset);
                bg.Line(-100, train.trackHeights[i] + trackoffset, game.width, train.trackHeights[i] + trackoffset);
            }
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
