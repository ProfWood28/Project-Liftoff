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

    RailStraight railStraight;
    int lastTrackSpawn = 0;

    int distanceToNextBreak = 0;
    int lastBreak = 0;

    public LevelHandler(EasyDraw bgIn, Train trainIn)
    {
        train = trainIn;
        bg = bgIn;
        railStraight = new RailStraight(999,this);

        distanceToNextBreak = game.width * 2;

        breakLengths.Add(random.Next(865, 5051));
        breakTracks.Add(random.Next(0, 5));
        breakStarts.Add(distanceToNextBreak + lastBreak);
    }
    private void Update()
    {
        genTrackBreaks();

        SpawnTempTrack();
        //drawTempTrack();

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
    private void SpawnTempTrack()
    {
        if(lastTrackSpawn + railStraight.width - levelDistance < 6)
            for (int i = 0; i < train.trackHeights.Count; i++)
            {
                //Console.WriteLine("Currently on loop {0}/{1}", i, train.trackHeights.Count);
                if(breakAbleTracks.Contains(i))
                {
                    lastTrackSpawn = Mathf.Round(levelDistance);
                    RailStraight newTrack = new RailStraight(train.trackHeights[i], this);
                    //make this smarter in case we add more things to the game lol 
                    game.GetChildren()[2].LateAddChild(newTrack);
                    Console.WriteLine("Spawned trackpiece");
                }
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

    class RailStraight : Sprite
    {
        public int fixedDeltaTime = 20;
        public float accumulatedTime = 0;

        LevelHandler levelHandler;
        float lastDistance = 0;

        public RailStraight(int yPos, LevelHandler lvlHandler) : base("Temp_Rail_LessDense.png", false)
        {
            SetOrigin(width / 2, height / 2);
            SetScaleXY(0.5f, 0.5f);
            SetXY(game.width + width, yPos);
            levelHandler = lvlHandler;
            lastDistance = levelHandler.levelDistance;
        }

        private void Update()
        {
            float deltaDistance = lastDistance - levelHandler.levelDistance;
            lastDistance = levelHandler.levelDistance;

            x += deltaDistance;

            if (x < -200)
            {
                Destroy();
            }

            RunFixedUpdate(Time.deltaTime);
        }
        private void FixedUpdate()
        {
            
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
    }
}
