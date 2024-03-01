using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;
using System.IO.Ports;
using System.Windows.Forms;

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
    public float moveForce = 3500f;
    public float noControlBreak = 500f;

    public float fixedDeltaTime = 0.02f;
    public float accumulatedTime = 0;

    private bool generatedTracks = false;
    public List<int> trackHeights = new List<int>();
    public int trackIndex = 2;
    public int trackCount = 5;

    public bool isAlive = true;

    public List<int> moveableToTracks = new List<int>();

    SerialPort sPort;
    Vector2 lastJoystick = new Vector2(512, 512);

    private InputBuffer inputBuffer = new InputBuffer();
    public Train(string fileName, int cols, int rows, int frames, SerialPort SP) : base(fileName, cols, rows, frames)
    {
        SetOrigin(width/2,height*0.92f);
        SetCycle(0, frames, 15, true);
        sPort = SP;
    }

    private void Update()
    {
        HandleInput();

        if (isAlive)
        {
            AnimateFixed();

            RunFixedUpdate(Time.deltaTime);
        }

        genTrack(trackCount);
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

        //this doesnt fuckin work I hate serial ports I am so god fuckin done with this why do buttons suck major ass
        //I am banning the usage of buttons
        //yeah fuck you no more buttons go kill yourself or fix this shit yourself

        //ControllerInput();
        //Console.WriteLine("Current joystick inputs: ({0}, {1})", lastJoystick.x, lastJoystick.y);

        //main
        inputBuffer.AddAxisInput(Input.GetAxis(Key.A, Key.D), Key.A);
        inputBuffer.AddAxisInput(Input.GetAxisDown(Key.W, Key.S), Key.W);

        //alt
        inputBuffer.AddAxisInput(Input.GetAxis(Key.LEFT, Key.RIGHT), Key.A);
        inputBuffer.AddAxisInput(Input.GetAxisDown(Key.UP, Key.DOWN), Key.W);

    }

    private void ControllerInput()
    {
        float analogueComp = -512;
        float threshold = 100;
        float moveThreshold = 300;
        //process all serial inputs
        SerialInput();

        //compensation for 0 - 1023 --> -512 - 512
        Vector2 adjustedStick = lastJoystick + new Vector2(analogueComp, analogueComp);

        if(Mathf.Abs(adjustedStick.x) < threshold)
        {
            adjustedStick.x = 0;
        }

        //horizontal inputs
        //Console.WriteLine("Adjusted & signed joystick inputs: ({0}, {1})", (Mathf.Sign(lastJoystick.x)), (Mathf.Sign(lastJoystick.y)));

        if (Mathf.Abs(adjustedStick.x) > moveThreshold)
        {
            inputBuffer.AddAxisInput(Mathf.Sign(adjustedStick.Normalize().x), Key.A);
        }
    }

    private void SerialInput()
    {
        //toggle serial data stream sending from the controller
        if (Input.GetKeyDown(Key.X))
        {
            //by sending any data to the controller
            sPort.Write("1");
        }

        //read the serial port
        string serialInput = sPort.ReadLine();

        //if not empty
        if (serialInput != "")
        {
            //split data by input, which are seperated by '/'
            string[] inputs = serialInput.Split('/');
            Console.WriteLine("serial: {0}", serialInput);

            //for each input
            for (int i = 0; i < inputs.Length; i++)
            {
                //seperate input type from input value
                string[] inputData = inputs[i].Split(':');

                //check if the input is a joystick input, denoted by the input type being 'J'
                if (inputData[0].Contains("J"))
                {
                    //extract (x,y) from string, seperated by ','
                    string[] joystickData = inputData[1].Split(',');

                    

                    //check if the serial port didn't send any funkey data
                    //this occurs more often than one would think when it constructs the whole string before sending
                    if (!joystickData[0].Contains("J") && !joystickData[1].Contains("J") && !joystickData[1].Contains("B"))
                    {
                        string joyStickY = joystickData[1];

                        Console.WriteLine("");

                        //convert (x,y) from string to ints
                        //Console.WriteLine("Joystick X = {0}", joystickData[0]);
                        int xValue = Convert.ToInt32(joystickData[0]);
                        //Console.WriteLine("Joystick Y = {0}", joystickData[1]);
                        int yValue = Convert.ToInt32(joyStickY);

                        //write to storage Vector2
                        lastJoystick = new Vector2(xValue, yValue);
                    }   
                }

                //this is completely broken

                //first thing first, we are changing the code on the arduino to
                //send all buttons at all times
                //it will send a 0 for 'not pressed', and the assoicated keycode (arduino is GXP.Key.code + 32!!!) for 'is pressed'
                //just modify the below code to check if a keycode sent is <= 0, 
                //and if not, copy the keycode to the correct position in the array we made :)
                //make sure to add that it should also copy non-presses, as otherwise, over time, 
                //the array will read all buttons as pressed
                //I think thats all
                //just ping me on discord if ya need help :D

                else if (inputData[0].Contains("B"))
                {
                    string keyCodeString = inputData[1];
                    string buttonInfo = inputData[0];

                    if (!keyCodeString.Contains("B"))
                    {
                        int keyCode = Convert.ToInt32(keyCodeString) - 32;

                        Keys key = (Keys)keyCode;
                        string keyString = key.ToString();

                        SendKeys.SendWait(keyString);
                    }
                }

            }
        }
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

        if(moveableToTracks.Contains(trackIndex + Mathf.Round(vDir)))
        {
            trackIndex += Mathf.Round(vDir);
            trackIndex = Mathf.Round(Mathf.Clamp(trackIndex, 0, trackHeights.Count - 1));
        }

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

        OffscreenPrevention();
    }

    private void OffscreenPrevention()
    {
        int minDistanceFromSide = 150;
        int slowingStart = 300;

        float sideMax = (x - game.width / 2 < 0 ? 0 : game.width);

        float distanceToSlow = Mathf.Abs(sideMax - slowingStart);
        float distanceToMin = Mathf.Abs(sideMax - minDistanceFromSide);

        float progress = 1.0f;

        if ((x < distanceToSlow && sideMax < game.width/2) || (x > distanceToSlow && sideMax > game.width/2))
        {
            float vTerminal = Mathf.Sqrt(moveForce / friction);
            

            if(sideMax < game.width / 2)
            {
                progress = 1.0f - (Mathf.Abs(x - distanceToSlow) / distanceToMin);
            }
            else
            {
                progress = 1.0f - (Mathf.Abs(x - distanceToSlow) / (game.width - distanceToMin));
            }

            float vMax = vTerminal * progress;
            float absoluteSpeed = (float)velocity.Magnitude();
            int directionality = Math.Sign(velocity.x);
            float adjustedSpeed = Mathf.Clamp(absoluteSpeed, 0, vMax) * directionality;

            velocity = new Vector2Double(adjustedSpeed, velocity.y);
        }

        //Console.WriteLine("trainSlow: {0} \ntrainMin: {1}", trainToSlow, trainToMin);
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
            int buffer = 300;

            int spacing = (game.height - buffer) / (nTracks);

            for (int i = 0; i < nTracks; i++)
            {
                trackHeights.Add(spacing * i + spacing / 2 + buffer);
            }
            generatedTracks = true;
        }
    }
}
