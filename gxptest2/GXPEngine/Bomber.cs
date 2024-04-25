using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

class Bomber : AnimationSprite
{
    Random random = new Random();
    public bool isRunning = false;

    public Sprite bmbst;
    public Sprite trgtst;
    public Sprite kbmst;

    int nAttacks = 0;
    public int nAttacked = 0;
    public int nLastAttack = 0;
    bool isAttacking = false;

    float secondsNextPass = 5;
    public float lastRun = 0;

    public Train trn;

    List<Vector2> bombingSpots = new List<Vector2>();

    MyGame gaym;
    public Bomber(string fileName, int cols, int rows, int frames, Train train, Sprite bmbs, Sprite trgts, Sprite kbms, MyGame gamer) : base(fileName, cols, rows, frames)
    {
        x = -200;

        SetOrigin(width / 2, height / 2);
        SetXY(-200, 250);

        nAttacks = Mathf.Round(random.Next(2, 7));

        trn = train;

        bmbst = bmbs;
        trgtst = trgts;
        kbmst = kbms;

        gaym = gamer;
    }

    void AnimHandler(bool doAttack)
    {
        //okay so frame 1 is always the start point
        //then we can either go with idle or attack frames
        //idle is frames 2-8, attack is 9-15 (both are 7 frames long)

        if (currentFrame == 7 || currentFrame == 14)
        {
            SetFrame(0);

            int idleStart = 1;
            int idleLength = 7;

            int attackStart = 8;
            int attackLength = 7;

            int animStart = doAttack ? attackStart : idleStart;
            int animLength = doAttack ? attackLength : idleLength;

            //Console.WriteLine("AnimStart: {0}, AnimLength: {1}", animStart, animLength);

            SetCycle(animStart, animLength, 15, true);
            isAttacking = false;
        }

        AnimateFixed();
    }

    void Attacks()
    {
        int sideBuffers = 300;
        float attackInterval = (game.width - sideBuffers * 2) / nAttacks - 1;

        //Console.WriteLine("AttackInterval: {0}", attackInterval);

        float range = 5f;
        if(x > sideBuffers && x < game.width - sideBuffers)
        {
            //Console.WriteLine("Possible to Attack");
            if (x > attackInterval * nAttacked + sideBuffers - range && x < attackInterval * nAttacked + sideBuffers + range)
            {
                isAttacking = true;
                nAttacked++;
            }
        }
        
        //if the anim frame has ID 13, it throws the bomb (for visual consistancy)
        if(currentFrame == 13 && nLastAttack != nAttacked)
        {
            nLastAttack = nAttacked;
            int id = Mathf.Round(random.Next(0, trn.trackCount - 1));
            Vector2 landing = new Vector2(random.Next(100, game.width - 100), trn.trackHeights[id]);
            Console.WriteLine("Spawned bomb at: {0}", landing.ToString());
            Bomb bomb = new Bomb("dynamite.png", this, landing, id, gaym);
            bmbst.LateAddChild(bomb);

            bombingSpots.Add(landing);
        }

        List<GameObject> bombStuff = bmbst.GetChildren();
        List<GameObject> targetStuff = trgtst.GetChildren();

        if (x > game.width + 199 && trn.isAlive)
        {
            for (int i = 0; i < bombingSpots.Count; i++)
            {
                Bomb bomb = (Bomb)bombStuff[i];
                Target trgt = (Target)targetStuff[i];

                bomb.x = bombingSpots[i].x;
                bomb.y = -100 - 100*i;
                bomb.falling = true;

                trgt.reverse = true;
            }

            bombingSpots.Clear();
        }

    }

    void BomberUpdate()
    {
        if(Time.time > lastRun + secondsNextPass*1000)
        {
            lastRun = Time.time;

            isRunning = true;
        }

        if (isRunning)
        {
            Attacks();
            AnimHandler(isAttacking);

            x += 1.0f;
            if (x > game.width + 200)
            {
                x = -200;
                nAttacks = Mathf.Round(random.Next(2, 7));
                secondsNextPass = random.Next(10, 30);

                nAttacked = 0;

                isRunning = false;
            }
        }
    }

    void Update()
    {
        if(trn.isAlive)
        {
            BomberUpdate();
        }
    }
}

class Bomb : Sprite
{
    Bomber bbr;

    Vector2 landingSpot;
    bool thrown = false;

    public bool falling = false;

    int id;

    MyGame gammert;
    public Bomb(string fileName, Bomber bomber, Vector2 landing, int idx, MyGame gamme) : base(fileName, false)
    {
        SetScaleXY(1);
        SetOrigin(width/2, height/2);
        landingSpot = landing;
        Target trgt = new Target("Target.png", landingSpot, bomber);

        id = idx;

        bbr = bomber;

        bbr.trgtst.LateAddChild(trgt);

        gammert = gamme;
    }

    void Update()
    {
        Throw();
    }

    void Throw()
    {
        if (!thrown)
        {
            x = bbr.x;
            y = bbr.y;
            thrown = true;
        }
        else if (falling && y < landingSpot.y)
        {
            y += 10f;
        }
        else if(falling && y > landingSpot.y)
        {
            Kaboom boom = new Kaboom("Kaboom_Sheet.png", 2, 4, 8, bbr.trn, id, gammert);
            boom.SetXY(x, y);
            bbr.kbmst.LateAddChild(boom);
            this.LateDestroy();
        }
        else if(!falling && thrown && y > -100)
        {
            x++;
            y-=1.5f;

            rotation += 4;
        }
        
    }
}

class Kaboom : AnimationSprite
{
    Train train;

    int idx;

    MyGame gaym;
    public Kaboom(string fileName, int cols, int rows, int frames, Train trn, int index, MyGame gaymert) : base(fileName, cols, rows, frames)
    {
        SetOrigin(width / 2, height / 2);
        SetCycle(0, 8, 10, true);
        SetScaleXY(0.25f);
        train = trn;
        idx = index;

        gaym = gaymert;
    }

    void Update()
    {
        if(currentFrame == 7)
        {
            if(idx == train.trackIndex && HitTest(train))
            {
                train.isAlive = false;
                gaym.gameState = 1;
            }

            this.LateDestroy();
        }
        AnimateFixed();
    }
}

class Target : Sprite
{
    Bomber bombman;
    public bool reverse = false;
    public Target(string fileName, Vector2 pos, Bomber bomber) : base(fileName, false) 
    {
        SetOrigin(width / 2, height / 2);
        SetScaleXY(0.5f);
        x = pos.x;
        y = pos.y;
        alpha = 0;

        bombman = bomber;
    }

    void Update()
    {

        if (alpha < 1 && !reverse)
        {
            alpha += 0.05f;
        }
        if(alpha > 0 && reverse)
        {
            alpha -= 0.05f;
        }
        if(alpha <= 0 && reverse)
        {
            this.LateDestroy();
        }

        rotation = Mathf.Sin(Time.time/100) * 5f;
    }
}
