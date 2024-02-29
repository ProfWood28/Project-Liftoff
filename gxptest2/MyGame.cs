using System;						// System contains a lot of default C# libraries 
using System.Collections.Generic;
using GXPEngine;					// GXPEngine contains the engine
using GXPEngine.Core;               // cannot be bothered to type Core.Vector2 every goddamn time
using System.IO.Ports;

public class MyGame : Game {
	// Declare the Sprite variables:
	EasyDraw background;

	LevelHandler levelHandler;
	Train train;

	public Sprite rails;

	public static SerialPort port = new SerialPort();

	public MyGame() : base(1366, 768, false, false)     // Create a window that's 1366x768 and ISnt fullscreen
	{
		//test
		targetFps = 144; //needed fsr

		// Create a full screen canvas (EasyDraw):
		// (in MyGame, width and height refer to game.width and game.height: the window size)
		background = new EasyDraw(width, height);
		background.TextFont(Utils.LoadFont("Pixel-Western.ttf", 10));

		rails = new Sprite("Empty.png", false, false);

		train = new Train("train_sprite-Sheet_v2.png", 3, 3, 8, port);
		train.SetXY(width/2, height/2);
		train.SetScaleXY(0.66f, 0.66f);

		levelHandler = new LevelHandler(background, train);

		// Add all sprites to the engine, so that they will be displayed every frame:
		// (The order that we add them is the order that they will be drawn.)
		AddChild(background);
		AddChild(rails);
		AddChild(train);
		AddChild(levelHandler);


		// Print some information to the console (behind the game window):
		Console.WriteLine("Scene successfully initialized");
	}

	// Update is called once per frame, by the engine, for each game object in the hierarchy
	// (including the Game itself)
	void Update() {

		background.ClearTransparent();
		if (Time.deltaTime != 0)
		{
			background.Text(String.Format("FPS: {0}", 1000 / Time.deltaTime), width - 100, 40);
			background.Text(String.Format("levelSpeed: {0}", levelHandler.levelSpeed), 40, 40);
			background.Text(String.Format("levelDistance: {0}", levelHandler.levelDistance), 40, 80);
		}
	}

	// Main is the first method that's called when the program is run
	static void Main() {
		
		string[] portNames = SerialPort.GetPortNames();

        foreach (string portName in portNames)
        {
			port.PortName = portName;
			port.BaudRate = 9600;
			port.RtsEnable = true;
			port.DtrEnable = true;

			try
			{
				port.Open();
				Console.WriteLine("Opened port at {0}", portName);

				port.Write("1");
			}
			catch 
			{
				Console.WriteLine("Couldn't find open port at '{0}'", portName);
			}
		}
		// Create a "MyGame" and start it:
		new MyGame().Start();
	}
}