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

    RailStraight railStraight = new RailStraight(999, -999);
    List<RailStraight> trackPieces = new List<RailStraight>();

    float lastDistance = 0;

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
        UpdateTracks();

        RunFixedUpdate(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        levelSpeed += 0.005f;
        levelDistance += levelSpeed * fixedDeltaTime / 10;
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

    //this currently is unused and wont work with sprite-based tracks
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
    private void UpdateTracks()
    {
        while(trackPieces.Count <= Mathf.Ceiling(game.width*2 / railStraight.width)*(train.trackCount))
        {
            //Console.WriteLine("Added traintrack to track index: {0}", trackPieces.Count % 5);
            RailStraight newTrack = new RailStraight(train.trackHeights[Mathf.Floor((trackPieces.Count - 1) / Mathf.Ceiling(game.width * 2 / railStraight.width))], (trackPieces.Count % Mathf.Ceiling(game.width*2 / railStraight.width)) * (railStraight.width));
            game.GetChildren()[1].LateAddChild(newTrack);
            trackPieces.Add(newTrack);
            Console.WriteLine("Spawned trackpiece ({1}, {2}), now total is {0}", trackPieces.Count, newTrack.x, newTrack.y);
        }

        float deltaDistance = levelDistance - lastDistance;
        float highestDistance = 0;
        foreach(RailStraight piece in trackPieces)
        {
            highestDistance = piece.x >= highestDistance ? piece.x : highestDistance;
        }

        Console.WriteLine("SpawnPoint: {0}", highestDistance);

        for (int i = 0; i < trackPieces.Count; i++)
        {
            RailStraight piece = trackPieces[i];
            int pieceID = (Mathf.Floor((i - 1) / Mathf.Ceiling(game.width * 2 / railStraight.width)));

            if (piece.x < 0 - piece.width)
            {
                piece.x = highestDistance + piece.width;
            }

            else
            {
                piece.x -= deltaDistance;
            }
        }

        lastDistance = levelDistance;
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
        public RailStraight(int yPos, float xPos) : base("Temp_Rail_LessDense.png", false)
        {
            SetOrigin(width / 2, height / 2);
            SetScaleXY(0.5f, 0.5f);
            SetXY(xPos, yPos);
        }

        private void Update()
        {

        }
        private void FixedUpdate()
        {
            
        }
    }
}
