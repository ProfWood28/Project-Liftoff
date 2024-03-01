using System;						// System contains a lot of default C# libraries 
using System.Collections.Generic;
using GXPEngine;					// GXPEngine contains the engine
using GXPEngine.Core;               // cannot be bothered to type Core.Vector2 every goddamn time
using System.IO.Ports;              // serial ports wooooooooooooooooooooooooooooooooooooooo
using System.Runtime;

public class MyGame : Game {
	// Declare the Sprite variables:
	EasyDraw background;

	LevelHandler levelHandler;
	Train train;

	public Sprite rails;

	MovingBackground bgBackground;
	MovingBackground bgForeground;
	MovingBackground bgSkybox;

	Menu mainMenu;
	Menu gameOver;

	EasyDraw textLayer;

	public static SerialPort port = new SerialPort();

	public int gameState = -1;
	public float menuMoveTime = 0;

	public MyGame() : base(1366, 768, false, false, -1, -1, true)     // Create a window that's 1366x768 and ISnt fullscreen
	{
		targetFps = 144; //needed fsr

		// Create a full screen canvas (EasyDraw):
		// (in MyGame, width and height refer to game.width and game.height: the window size)
		background = new EasyDraw(width, height);

		textLayer = new EasyDraw(width, height);
		textLayer.TextFont(Utils.LoadFont("Pixel-Western.ttf", 10));

		rails = new Sprite("Empty.png", false, false);

		train = new Train("train_sprite-Sheet_v2.png", 3, 3, 8, port);
		train.SetXY(width/2, height/2);
		train.SetScaleXY(0.66f, 0.66f);

		levelHandler = new LevelHandler(textLayer, train, this);

		bgSkybox = new MovingBackground("sky_background1.png", 1f, 0, -0.3f, levelHandler, train);
		bgBackground = new MovingBackground("BackgroundSand_background2.png", 1f, 0, -0.6f, levelHandler, train);
		bgForeground = new MovingBackground("foregroundSand_background3.png", 1f, 0, -1f, levelHandler, train);

		mainMenu = new Menu("MenuUi.png", 1, new string[] { "Start_Button_Highlight.png", "Score_Button_Highlight.png" }, new Vector2[] {new Vector2(width/2-255, 232), new Vector2(width / 2 - 255, 338) }, new int[] {0, 2}, new float[] {1, 1}, this);
		gameOver = new Menu("EndScreen.png", 1, new string[] { "ContinueButton.png" }, new Vector2[] { new Vector2(width / 2-255, 457) }, new int[] { -1 }, new float[] { 1 }, this);

		// Add all sprites to the engine, so that they will be displayed every frame:
		// (The order that we add them is the order that they will be drawn.)
		AddChild(background);

		AddChild(bgSkybox);
		AddChild(bgBackground);
		AddChild(bgForeground);

		AddChild(rails);

		AddChild(train);
		AddChild(levelHandler);

		AddChild(gameOver);
		AddChild(mainMenu);

		AddChild(textLayer);


		// Print some information to the console (behind the game window):
		Console.WriteLine("Scene successfully initialized");
	}

	// Update is called once per frame, by the engine, for each game object in the hierarchy
	// (including the Game itself)
	void Update() {

		MenuStuffs();

		background.ClearTransparent();
		textLayer.ClearTransparent();
		if (Time.deltaTime != 0)
		{
			textLayer.Text(String.Format("FPS: {0}", 1000 / Time.deltaTime), width - 100, 40);
			//background.Text(String.Format("levelSpeed: {0}", levelHandler.levelSpeed), 40, 40);
			textLayer.Text(String.Format("Score: {0}", Mathf.Round(levelHandler.levelDistance)), 10, 40);
		}
	}

	private void MenuStuffs()
	{ 
		if(gameState == -1) // main menu
        {
			mainMenu.visible = true;

			menuMoveTime = 0;
			gameOver.visible = false;
			gameOver.y = -height;
        }
        else if(gameState == 0) // game
        {
			mainMenu.visible = false;

			menuMoveTime = 0;
			gameOver.y = -height;
			gameOver.visible = false;
        }
        else if (gameState == 1) // retry
        {
			mainMenu.visible = false;

			float timeToReach = 0.75f;
			menuMoveTime += Time.deltaTime / (timeToReach*1000);
			Console.WriteLine("gameOver.y: {0}", gameOver.y);
			gameOver.y = Lerp(-height, 0, menuMoveTime);
			gameOver.visible = true;
        }
		else // highscores
        {
			menuMoveTime = 0;
			gameOver.y = -height;
			mainMenu.visible = false;
			gameOver.visible = false;
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

	public static float Lerp(float a, float b, float t)
	{
		t = Math.Max(0, Math.Min(1, t));
		return a + (b - a) * t;
	}
}