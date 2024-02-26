using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;
using System.IO.Ports;

class Train : AnimationSprite
{
    public Vector2Double movement = new Vector2Double(0, 0);

    public Vector2Double velocity = new Vector2Double(0, 0);
    public Vector2Double velocityMoment = new Vector2Double(0, 0);

    public Vector2Double otherForces = new Vector2Double(0, 0);
    public Vector2Double totalForces = new Vector2Double(0, 0);

    public float mass = 2000f;
    public bool grounded = false;
    public float friction = 0.15f;
    public float moveForce = 2500f;
    public float noControlBreak = 500f;

    public float fixedDeltaTime = 0.02f;
    public float accumulatedTime = 0;

    private bool generatedTracks = false;
    public List<int> trackHeights = new List<int>();
    public int trackIndex = 2;
    public int trackCount = 5;

    SerialPort sPort;
    int lastButton = 0;

    private InputBuffer inputBuffer = new InputBuffer();
    public Train(string fileName, int cols, int rows, int frames, SerialPort SP) : base(fileName, cols, rows, frames)
    {
        SetOrigin(width/2,height*0.92f);
        SetCycle(0, frames, 40, true);
        sPort = SP;
    }

    private void Update()
    {
        AnimateFixed();

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
        //controller
        string serialInput = sPort.ReadExisting();

        if (serialInput != "")
        {
            string[] subs = serialInput.Split(':');

            string input = subs.Length > 1 ? subs[1] : "";

            int inputKey = 0;

            if (!string.IsNullOrEmpty(input))
            {
                int.TryParse(input, out inputKey);
                inputKey -= 32;
            }

            if (inputKey == Key.W || inputKey == Key.S)
            {
                if (inputKey != lastButton)
                {
                    int output = inputKey == Key.W ? -1 : 1;
                    inputBuffer.AddAxisInput(output, Key.W);

                }
            }
            else if (inputKey == Key.A || inputKey == Key.D)
            {
                int output = inputKey == Key.A ? -1 : 1;
                inputBuffer.AddAxisInput(output, Key.A);
            }
        }

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
        double frictionForce = friction * (velocity.x * velocity.x * (velocity.Normalize().x * -1));

        double controlBreak = movement.sqrMagnitude() == 0 ? noControlBreak * velocity.Normalize().x * -1 : 0;

        totalForces = movement + otherForces + new Vector2Double(frictionForce + controlBreak, 0);
       
        Vector2Double accel = Physics.Accel(mass, totalForces);
        velocity += accel;

        velocity = velocity * (velocity.sqrMagnitude() < 0.05f ? 0 : 1);
    }

    private void ApplyVelocity()
    {
        x += (float)velocity.x * fixedDeltaTime;
        y += (float)velocity.y * fixedDeltaTime;

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
