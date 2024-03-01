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

    public float fixedDeltaTime = 0.02f;
    public float accumulatedTime = 0;

    private EasyDraw bg;
    private Train train;

    private Random random = new Random();

    private RailStraight railStraight;
    private List<RailStraight> trackPieces = new List<RailStraight>();

    public float lastDistance = 0;

    private List<int> breakableTracks = new List<int>() {0,1,2,3,4};
    private List<int> gapTracks = new List<int>();
    private List<float> gapStarts = new List<float>();
    private List<float> gapLengths = new List<float>();

    private List<int> tracksTrainCanMoveTo = new List<int> {0,1,2,3,4};

    private MyGame gaym;

    public LevelHandler(EasyDraw bgIn, Train trainIn, MyGame gamm)
    {
        train = trainIn;
        bg = bgIn;
        gaym = gamm;

        railStraight = new RailStraight(999, -999, this);

        int randomIndex = random.Next(0, breakableTracks.Count);
        int trackIndex = breakableTracks[randomIndex];

        breakableTracks.Remove(trackIndex);
        gapTracks.Add(trackIndex);
        gapStarts.Add(game.width * 2);
        gapLengths.Add(random.Next(5, 20) * railStraight.width);
        gaym = gamm;
    }
    private void Update()
    {
        if(gaym.gameState == 0)
        {
            DeathCheck();

            if (train.isAlive)
            {
                ManageGaps();

                //yeah I have up on this
                //detecting an actually impossible track combination is extremely difficult
                //just call it a ploy for more insurance fraud idk man
                //ImpossibleTrackFixer();

                TrackDebug(false, false);

                UpdateTracks();
            }

            RunFixedUpdate(Time.deltaTime);
        }
        else if (gaym.gameState == -1)
        {
            levelSpeed = 3;
            levelDistance = 0;
            lastDistance = 0;
            train.isAlive = true;
            train.trackIndex = 2;
            train.x = game.width/2;

            gapTracks.Clear();
            gapStarts.Clear();
            gapLengths.Clear();
            breakableTracks = new List<int> {0, 1, 2, 3, 4};

            for (int i = 0; i < trackPieces.Count; i++)
            {
                trackPieces[i].SetFrame(random.Next(0, trackPieces[i].frameCount - 2));
                trackPieces[i].y = train.trackHeights[Mathf.Floor((i - 1) / Mathf.Ceiling(game.width * 2 / railStraight.width))];
                trackPieces[i].x = (i % Mathf.Ceiling(game.width * 2 / railStraight.width)) * (railStraight.width);
            }
        }
    }
    private void FixedUpdate()
    {
        if(train.isAlive)
        {
            levelSpeed += 0.005f;
            levelDistance += levelSpeed;
        }
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

    private void DeathCheck()
    {
        if (gapTracks.Contains(train.trackIndex))
        {
            int index = gapTracks.IndexOf(train.trackIndex);
            if (index >= 0 && train.x + levelDistance > gapStarts[index] && train.x + levelDistance < gapStarts[index] + gapLengths[index])
            {
                train.isAlive = false;
                gaym.gameState = 1;
            }
        }
    }

    private void UpdateTracks()
    {
        while(trackPieces.Count < Mathf.Ceiling(game.width*2 / railStraight.width)*(train.trackCount))
        {
            //Console.WriteLine("Added traintrack to track index: {0}", trackPieces.Count % 5);
            RailStraight newTrack = new RailStraight(train.trackHeights[Mathf.Floor((trackPieces.Count - 1) / Mathf.Ceiling(game.width * 2 / railStraight.width))], (trackPieces.Count % Mathf.Ceiling(game.width*2 / railStraight.width)) * (railStraight.width), this);
            game.GetChildren()[4].LateAddChild(newTrack);
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
                
                if(Mathf.Sign(piece.scaleX) < 0)
                {
                    piece.scaleX = piece.scaleFactor;
                }

                piece.SetFrame(random.Next(0, piece.frameCount - 2));
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
                else if(piece.x + levelDistance < start && piece.x + levelDistance > start - piece.width)
                {
                    piece.SetFrame(random.Next(piece.frameCount - 1, piece.frameCount+1));
                }
                else if(piece.x + levelDistance > end && piece.x + levelDistance < end + piece.width)
                {
                    piece.y = train.trackHeights[yIndex];
                    piece.scaleX = -piece.scaleFactor;
                    piece.SetFrame(random.Next(piece.frameCount - 1, piece.frameCount + 1));
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
            int randomIndex = random.Next(0, breakableTracks.Count);
            int trackIndex = breakableTracks[randomIndex];
            
            breakableTracks.Remove(trackIndex);
            gapTracks.Add(trackIndex);
            gapStarts.Add(random.Next(5,20) * railStraight.width + levelDistance + game.width*2);
            gapLengths.Add(random.Next(5, 40) * railStraight.width);
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

    //private void ImpossibleTrackFixer()
    //{
    //    int i = train.trackIndex;
        
    //    bool isIn = gapTracks.Contains(i);
    //    bool isAbove = gapTracks.Contains(i + 1);
    //    bool isBelow = gapTracks.Contains(i - 1);

    //    if (isIn && isAbove && i == train.trackCount-1 || isIn && isBelow && i == 0 || isIn && isAbove && isBelow)
    //    {
    //        int gapIndex = gapTracks.IndexOf(i);
    //        float gapStart = gapStarts[gapIndex];
    //        float gapLength = gapLengths[gapIndex];

    //        float gapStartLower = gapStart;
    //        float gapLengthLower = gapLength;

    //        if (isBelow)
    //        {
    //            int gapIndexLower = gapTracks.IndexOf(i - 1);
    //            gapStartLower = gapStarts[gapIndexLower];
    //            gapLengthLower = gapLengths[gapIndexLower];
    //        }

    //        float gapStartHigher = gapStart;
    //        float gapLengthHigher = gapLength;

    //        if(isAbove)
    //        {
    //            int gapIndexHigher = gapTracks.IndexOf(i + 1);
    //            gapStartHigher = gapStarts[gapIndexHigher];
    //            gapLengthHigher = gapLengths[gapIndexHigher];
    //        }
                

    //        //---------\\

    //        float highestStart = Math.Max(Math.Max(gapStart, gapStartLower), gapStartHigher);
    //        float lowestStart = Math.Min(Math.Min(gapStart, gapStartLower), gapStartHigher);

    //        float highestEnd = Math.Max(Math.Max(gapStart + gapLength, gapStartLower + gapLengthLower), gapStartHigher + gapLengthHigher);
    //        float lowestEnd = Math.Min(Math.Min(gapStart + gapLength, gapStartLower + gapLengthLower), gapStartHigher + gapLengthHigher);

    //        //---------\\

    //        if(highestStart >= lowestEnd)
    //        {
    //            Console.WriteLine("It aint happening chief");
    //        }
            
    //    }
    //}

    class RailStraight : AnimationSprite
    {
        Random random;

        public float scaleFactor = 2f;
       public RailStraight(int yPos, float xPos, LevelHandler lvlHandler) : base("tracks-Sheetv3.png", 4, 3, 12)
        {
            random = new Random(Input.mouseX + Time.now + lvlHandler.trackPieces.Count);

            int randomSkin = random.Next(0, frameCount - 2);
            Console.WriteLine("Skin index: {0}", randomSkin);

            SetFrame(randomSkin);
            SetOrigin(width / 2, height / 2);
            SetScaleXY(scaleFactor, scaleFactor);
            SetXY(xPos, yPos);
        }
    }
}
