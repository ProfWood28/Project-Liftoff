﻿using System;
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

    public float fixedDeltaTime = 0.02f;
    public float accumulatedTime = 0;

    private EasyDraw bg;
    private Train train;

    private Random random = new Random();

    private RailStraight railStraight = new RailStraight(999, -999);
    private List<RailStraight> trackPieces = new List<RailStraight>();

    private float lastDistance = 0;

    private List<int> breakableTracks = new List<int>() {0,1,2,3,4};
    private List<int> gapTracks = new List<int>();
    private List<float> gapStarts = new List<float>();
    private List<float> gapLengths = new List<float>();

    private List<int> tracksTrainCanMoveTo = new List<int> {0,1,2,3,4};
    
    public LevelHandler(EasyDraw bgIn, Train trainIn)
    {
        train = trainIn;
        bg = bgIn;

        int randomIndex = random.Next(0, breakableTracks.Count);
        int trackIndex = breakableTracks[randomIndex];

        breakableTracks.Remove(trackIndex);
        gapTracks.Add(trackIndex);
        gapStarts.Add(game.width*3);
        gapLengths.Add(random.Next(5, 20)*railStraight.width);
    }
    private void Update()
    {
        ManageGaps();

        Console.Write("Breakables: ");

        for (int i = 0; i < breakableTracks.Count; i++)
        {
            Console.Write("{0}; ", breakableTracks[i]);
        }
        Console.WriteLine("");
        
        TrackDebug(true, true);
        UpdateTracks();

        RunFixedUpdate(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        levelSpeed += 0.005f;
        levelDistance += levelSpeed;
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

    private void UpdateTracks()
    {
        while(trackPieces.Count < Mathf.Ceiling(game.width*2 / railStraight.width)*(train.trackCount))
        {
            //Console.WriteLine("Added traintrack to track index: {0}", trackPieces.Count % 5);
            RailStraight newTrack = new RailStraight(train.trackHeights[Mathf.Floor((trackPieces.Count - 1) / Mathf.Ceiling(game.width * 2 / railStraight.width))], (trackPieces.Count % Mathf.Ceiling(game.width*2 / railStraight.width)) * (railStraight.width));
            game.GetChildren()[1].LateAddChild(newTrack);
            trackPieces.Add(newTrack);
            //Console.WriteLine("Spawned trackpiece ({1}, {2}), now total is {0}", trackPieces.Count, newTrack.x, newTrack.y);
        }

        float deltaDistance = levelDistance - lastDistance;
        float highestDistance = 0;
        foreach(RailStraight piece in trackPieces)
        {
            highestDistance = piece.x >= highestDistance ? piece.x : highestDistance;
        }

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

            int yIndex = Mathf.Floor(i / (trackPieces.Count / train.trackCount));

            if (gapTracks.Contains(yIndex))
            {
                float start = gapStarts[gapTracks.IndexOf(yIndex)];
                float end = gapStarts[gapTracks.IndexOf(yIndex)] + gapLengths[gapTracks.IndexOf(yIndex)];

                if (piece.x + levelDistance > start && piece.x + levelDistance < end)
                {
                    piece.y = -9999;
                }
                else
                {
                    piece.y = train.trackHeights[yIndex];
                }
            }
            else
            {
                piece.y = train.trackHeights[yIndex];
            }
        }

        lastDistance = levelDistance;
    }

    private void TrackDebug(bool doTrackDebug, bool doPlayerDebug)
    {
        if(doPlayerDebug)
        {
            string debugVelocity = String.Format("Velocity: ({0}, {1})", train.velocity.x, train.velocity.y);
            bg.Text(debugVelocity, 40, 120);
        }

        if (doTrackDebug)
        {
            bg.StrokeWeight(5);

            for (int i = 0; i < train.trackCount; i++)
            {
                if(i == train.trackIndex)
                {
                    bg.Stroke(255, 0, 0);
                }
                else
                {
                    bg.Stroke(255, 255, 255);
                }

                if (gapTracks.Contains(i))
                {
                    int index = gapTracks.IndexOf(i);

                    bg.Line(-10, train.trackHeights[i], gapStarts[index] - levelDistance, train.trackHeights[i]);
                    bg.Line(gapStarts[index] + gapLengths[index] - levelDistance, train.trackHeights[i], game.width + 10, train.trackHeights[i]);
                }
                else
                {
                    bg.Line(0, train.trackHeights[i], game.width, train.trackHeights[i]);
                }
            }
        }
    }

    private void ManageGaps()
    {
        for (int i = gapTracks.Count-1; i >= 0; i--)
        {
            if (gapTracks != null && gapStarts[i] + gapLengths[i] - levelDistance < 0 - railStraight.width*2)
            {
                //Console.WriteLine("Deleted a gap in lists on track {0}", gapTracks[i]+1);

                breakableTracks.Add(gapTracks[i]);
                gapTracks.RemoveAt(i);
                gapStarts.RemoveAt(i);
                gapLengths.RemoveAt(i);
            }
        }
        
        if(gapTracks.Count < 3)
        {
            bool noInstaKill = false;

            if(!breakableTracks.Contains(train.trackIndex - 1) && !breakableTracks.Contains(train.trackIndex + 1))
            {
                breakableTracks.Remove(train.trackIndex);
                noInstaKill = true;
            }

            int randomIndex = random.Next(0, breakableTracks.Count);
            int trackIndex = breakableTracks[randomIndex];

            //why the fuck no work
            //I am going to kill someone
            //it might be me but alas
            if(noInstaKill)
            {
                breakableTracks.Add(train.trackIndex);
            }
            
            breakableTracks.Remove(trackIndex);
            gapTracks.Add(trackIndex);
            gapStarts.Add(random.Next(5,20) * railStraight.width + levelDistance + game.width*2);
            gapLengths.Add(random.Next(5, 40) * railStraight.width);
        }

        if(breakableTracks.Contains(train.trackIndex))
        {
            breakableTracks.Remove((train.trackIndex));
        }

        if(breakableTracks.Count < train.trackCount)
        {
            for (int i = 0; i < train.trackCount; i++)
            {
                if(!breakableTracks.Contains(i) && !gapTracks.Contains(i))
                {
                    breakableTracks.Add(i);
                }
            }
        }

        tracksTrainCanMoveTo.Clear();
        for (int i = 0; i < gapTracks.Count; i++)
        {
            if(!(train.x + levelDistance > gapStarts[i] && train.x + levelDistance < gapStarts[i] + gapLengths[i]))
            {
                tracksTrainCanMoveTo.Add(gapTracks[i]);
            }
        }

        for (int i = 0; i < train.trackCount; i++)
        {
            if (!gapTracks.Contains(i))
            {
                tracksTrainCanMoveTo.Add(i);
            }
        }

        train.moveableToTracks = tracksTrainCanMoveTo;
    }

    class RailStraight : Sprite
    {
        public RailStraight(int yPos, float xPos) : base("Temp_Rail_LessDense.png", false)
        {
            SetOrigin(width / 2, height / 2);
            SetScaleXY(0.5f, 0.5f);
            SetXY(xPos, yPos);
        }
    }
}
