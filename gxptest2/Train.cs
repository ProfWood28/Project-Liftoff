using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

class Train : Sprite
{
    public Vector2 movement = new Vector2(0, 0);

    public Vector2 velocity = new Vector2(0, 0);
    public Vector2 velocityMoment = new Vector2(0, 0);

    public Vector2 otherForces = new Vector2(0, 0);
    public Vector2 totalForces = new Vector2(0, 0);

    public float mass = 3000f;
    public bool grounded = false;
    public float friction = 0.03f;
    public float moveForce = 1f;

    public int fixedDeltaTime = 20;
    public float accumulatedTime = 0;

    private bool generatedTracks = false;
    public List<int> trackHeights = new List<int>();
    public int trackIndex = 2;
    public int trackCount = 5;

    public float slowMaxDistance = 100f;
    public float slowStartDistance = 300f;

    private InputBuffer inputBuffer = new InputBuffer();
    public Train(string fileName) : base(fileName)
    {
        SetOrigin(width/2,height*0.85f);
    }

    private void Update()
    {
        genTrack(trackCount);

        HandleInput();

        RunFixedUpdate(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        HandleBufferedInputs();

        ForceToVelocity();

        ApplyVelocity();
    }
    private void HandleInput()
    {
        //main
        inputBuffer.AddAxisInput(Input.GetAxis(Key.A, Key.D), Key.A);
        inputBuffer.AddAxisInput(Input.GetAxisDown(Key.W, Key.S), Key.W);

        //alt
        inputBuffer.AddAxisInput(Input.GetAxis(Key.LEFT, Key.RIGHT), Key.A);
        inputBuffer.AddAxisInput(Input.GetAxisDown(Key.UP, Key.DOWN), Key.W);

    }
    private void HandleBufferedInputs()
    {
        List<InputEvent> axises = inputBuffer.GetAxisInputs();

        float hDir = 0;
        float vDir = 0;

        foreach (InputEvent axis in axises)
        {
            switch (axis.keyID)
            {
                case (int)Key.A:
                    hDir += axis.axisValue;
                    break;

                case (int)Key.W:
                    vDir += axis.axisValue;
                    break;
            }
        }

        hDir = Mathf.Clamp(hDir, -1f, 1f);
        vDir = Mathf.Clamp(vDir, -1, 1);

        movement = new Vector2(hDir * moveForce, 0);

        trackIndex += Mathf.Round(vDir);
        trackIndex = Mathf.Round(Mathf.Clamp(trackIndex, 0, trackHeights.Count-1));
        y = trackHeights[trackIndex];

        inputBuffer.ProcessInputs();
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
        totalForces = new Vector2(movement.x + otherForces.x, movement.y + otherForces.y);

        float slowFactor = ApplyScreenCompensation();

        Vector2 accel = Physics.Accel(mass, totalForces);
        accel = new Vector2(accel.x * fixedDeltaTime, accel.y * fixedDeltaTime);
        velocity = new Vector2((velocity.x + accel.x), velocity.y + accel.y);
        velocity = new Vector2(velocity.x - velocity.x * (friction + slowFactor), velocity.y - velocity.y * friction);

        velocityMoment = new Vector2(velocity.x * fixedDeltaTime, velocity.y * fixedDeltaTime);
    }

    //currently only applies slow when moving towards the edge, not away from it
    //should not be awefully difficult to add (famous last words probs lmfao)
    private float ApplyScreenCompensation()
    {
        float slowingFactor = 1f;
        
        int directionalEdgePos = totalForces.x < 0 ? 0 : game.width;
        float edgeSlowStart = directionalEdgePos - slowStartDistance * totalForces.Normalize().x;
        float edgeSlowMax = directionalEdgePos - slowMaxDistance * totalForces.Normalize().x;

        if(directionalEdgePos < game.width/2)
        {
            float f = (edgeSlowStart - x) / (edgeSlowStart - edgeSlowMax);
            slowingFactor = Mathf.Clamp(f, 0f, 1f);
        }
        else
        {
            float f = (x - edgeSlowStart) / (edgeSlowMax - edgeSlowStart);
            slowingFactor = Mathf.Clamp(f, 0f, 1f);
        }

        Console.WriteLine("slowingFactor = {0}", slowingFactor * 0.9);
        return 0.9f * slowingFactor;
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
        otherForces = new Vector2(otherForces.x + force.x, otherForces.y + force.y);
    }
    private void GetInputs(out List<InputEvent> keys, out List<InputEvent> axises, out List<InputEvent> mouse)
    {
        List<InputEvent> keyInputs = inputBuffer.GetKeyInputs();
        List<InputEvent> axisInputs = inputBuffer.GetAxisInputs();
        List<InputEvent> mouseInputs = inputBuffer.GetMouseInputs();

        keys = keyInputs;
        axises = axisInputs;
        mouse = mouseInputs;
    }
    private void genTrack(int nTracks)
    {
        if (!generatedTracks)
        {
            int spacing = (game.height - 150) / (nTracks);

            for (int i = 0; i < nTracks; i++)
            {
                trackHeights.Add(spacing * i + spacing / 2 + 150);
            }
            generatedTracks = true;
        }
    }
}
