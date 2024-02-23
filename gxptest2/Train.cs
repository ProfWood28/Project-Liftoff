using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

class Train : Sprite
{
    public Vector2Double movement = new Vector2Double(0, 0);

    public Vector2Double velocity = new Vector2Double(0, 0);
    public Vector2Double velocityMoment = new Vector2Double(0, 0);

    public Vector2Double otherForces = new Vector2Double(0, 0);
    public Vector2Double totalForces = new Vector2Double(0, 0);

    public float mass = 60f;
    public bool grounded = false;
    public float friction = 0.03f;
    public float moveForce = 200f;

    public float fixedDeltaTime = 0.02f;
    public float accumulatedTime = 0;

    private bool generatedTracks = false;
    public List<int> trackHeights = new List<int>();
    public int trackIndex = 2;
    public int trackCount = 5;

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

        movement = new Vector2Double(hDir * moveForce, 0);

        trackIndex += Mathf.Round(vDir);
        trackIndex = Mathf.Round(Mathf.Clamp(trackIndex, 0, trackHeights.Count-1));
        y = trackHeights[trackIndex];

        inputBuffer.ProcessInputs();
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
    private void ForceToVelocity()
    {
        totalForces = movement + otherForces;

        Vector2Double accel = Physics.Accel(mass, totalForces);
        accel = accel * fixedDeltaTime;
        velocity = velocity + accel;
        velocity -= velocity * friction;
    }

    private void ApplyVelocity()
    {
        x += (float)velocity.x;
        y += (float)velocity.y;

        movement = new Vector2Double(0, 0);
        otherForces = new Vector2Double(0, 0);
        totalForces = new Vector2Double(0, 0);
    }
    public void AddForce(Vector2 force)
    {
        double forceX = (double)new decimal(force.x);
        double forceY = (double)new decimal(force.y);

        otherForces += new Vector2Double(forceX, forceY);
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
