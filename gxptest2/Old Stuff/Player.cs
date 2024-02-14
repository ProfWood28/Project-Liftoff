using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using GXPEngine;
using GXPEngine.Core;

class Player : Sprite
{
    Level level;
    List<Sprite> lvlObjs;

    float moveForce = 4f;
    float jumpForce = 250f;
    float friction = 0.1f;
    float airStrafeMod = 0.7f;

    Vector2 movement = new Vector2(0, 0);
    public Vector2 velocity = new Vector2(0, 0);
    Vector2 velocityMoment = new Vector2(0, 0);

    Sprite playerProjection;

    bool grounded = false;

    Vector2 otherForces = new Vector2(0, 0);
    Vector2 totalForces = new Vector2(0, 0);

    float mass;

    int fixedDeltaTime = 20;
    float accumulatedTime = 0;

    private InputBuffer inputBuffer = new InputBuffer();

    public Player(float xPos, float yPos, float massGet, Level levelGet) : base("circle.png", true, true)
    {
        x = xPos;
        y = yPos;
        mass = massGet;
        level = levelGet;  
    }

    void Update()
    {
        HandleInput();

        RunFixedUpdate(Time.deltaTime);

        if (playerProjection == null)
        {
            GenerateProjection();
        }

        if(lvlObjs == null)
        {
            lvlObjs = level.GetAll();
        }

    }
    private void FixedUpdate()
    {
        HandleBufferedInputs();

        ForceToVelocity();

        List<Vector2> tempV = GenerateNormals(lvlObjs, true);
        List<Vector2> tempH = GenerateNormals(lvlObjs, false);

        CollisionHandler(tempV, tempH);

        ApplyVelocity();

        inputBuffer.ProcessInputs();
    }

    private void PlayerMovement(bool jumpedTrue, int moveAxis)
    {
        int jumped = jumpedTrue && grounded ? 1 : 0;
        movement = new Vector2(moveAxis * moveForce * (grounded ? 1f : airStrafeMod), jumped * -jumpForce);
    }

    private void ForceToVelocity()
    {
        totalForces = new Vector2(movement.x + otherForces.x, movement.y + otherForces.y);

        Vector2 accel = Physics.GravityAccel(mass, totalForces);
        accel = new Vector2(accel.x * fixedDeltaTime, accel.y * fixedDeltaTime);
        velocity = new Vector2(velocity.x + accel.x, velocity.y + accel.y);
        velocity = new Vector2(velocity.x - velocity.x * friction, velocity.y - velocity.y * friction);
        velocityMoment = new Vector2(velocity.x * fixedDeltaTime, velocity.y * fixedDeltaTime);
    }

    private void CollisionHandler(List<Vector2> vertical, List<Vector2> horizontal) 
    {
        int changeX = 1;
        int changeY = 1;

        grounded = false;

        foreach (Vector2 v in vertical)
        {
            if(v.Magnitude() > 0)
            {
                changeY = 0;
            }

            if(v.y > 0)
            {
                grounded = true;
            }
        }

        foreach (Vector2 h in horizontal)
        {
            if (h.Magnitude() > 0)
            {
                changeX = 0;
            }
        }

        //Console.WriteLine("Modifiers are V: {0}, H: {1}", changeY, changeX);

        velocityMoment = new Vector2(velocityMoment.x * changeX, velocityMoment.y * changeY);
    } 

    //Generate a list of collision normals for either horizontal or vertical position changes, using 
    private List<Vector2> GenerateNormals(List<Sprite> objects, bool isVertical)
    {
        //list of normals to be returned
        List<Vector2> normals = new List<Vector2>();

        //temp holding vector for the normal
        Vector2 normal;

        //apply velocity changes to the player projection with relavancy to direction
        playerProjection.x = x + velocityMoment.x * (isVertical ? 0 : 1);
        playerProjection.y = y + velocityMoment.y * (isVertical ? 1 : 0);

        //check for every object in the list provided...
        foreach (Sprite sprite in objects)
        {
            //if they overlap with the player projection
            if(playerProjection.HitTest(sprite))
            {
                //if they do, calculate the offset vector
                Vector2 offset = new Vector2(sprite.x - x, sprite.y - y);

                //if the check is for vertical collision...
                if (isVertical)
                {
                    //add the normal change to the x - aka left - if negative
                    //or to the y - aka right - if positive
                    normal = new Vector2(offset.y < 0 ? 1 : 0, offset.y > 0 ? 1 : 0);
                }
                //else if the check is for horizontal collision...
                else
                {
                    //add the normal change to the x - aka up - if negative
                    //or to the y - aka down - if positive
                    normal = new Vector2(offset.x < 0 ? 1 : 0, offset.x > 0 ? 1 : 0);
                }
                //finally, add the created normal to the list
                normals.Add(normal);
            }
        }
        //reverts the projection to the player position
        playerProjection.SetXY(x, y);

        //and then return the list of either vertical collision normals or horizontal collision normals
        return normals;
    }

    private void GenerateProjection()
    {
        playerProjection = new Sprite(texture.filename, true, true);
        playerProjection.alpha = 0f;
        playerProjection.SetOrigin(playerProjection.width/2, playerProjection.height/2);
        playerProjection.SetScaleXY(scaleX, scaleY);
        playerProjection.SetXY(x, y);
        Console.WriteLine("Generated Player Projection with filename '{0}'", playerProjection.texture.filename);
    }

    private void ApplyVelocity()
    {
        x += velocityMoment.x;
        y += velocityMoment.y;

        movement = new Vector2(0, 0);
        otherForces = new Vector2(0, 0);
        totalForces = new Vector2(0, 0);
    }

    public void AddForce(Vector2 force)
    {
        Console.WriteLine("Got an external force of {0}", force);
        otherForces = new Vector2(otherForces.x + force.x, otherForces.y + force.y);
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
    private void HandleInput()
    {
        inputBuffer.AddAxisInput(Input.GetAxis(Key.A, Key.D), Key.A);

        if (Input.GetKeyDown(Key.SPACE))
        {
            inputBuffer.AddKeyInput(Key.SPACE);
        }
    }
    private void HandleBufferedInputs()
    {
        List<InputEvent> keyInputs = inputBuffer.GetKeyInputs();
        bool jumped = false;

        foreach (InputEvent input in keyInputs)
        {
            switch (input.keyID)
            {
                case (int)Key.SPACE:
                    jumped = true;
                    break;
            }
        }

        List<InputEvent> axisInputs = inputBuffer.GetAxisInputs();
        int moveDir = 0;

        foreach (InputEvent axis in axisInputs)
        {
            switch (axis.keyID)
            {
                case (Key.A):
                    moveDir += axis.axisValue;
                    break;
            }
        }

        moveDir = Mathf.Round(Mathf.Clamp(moveDir, -1, 1));
        PlayerMovement(jumped, moveDir);
    }
}