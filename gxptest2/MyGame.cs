using System;                   // System contains a lot of default C# libraries 
using System.Collections.Generic;
using GXPEngine;                // GXPEngine contains the engine
using GXPEngine.Core;

public class MyGame : Game {
	// Declare the Sprite variables:
	EasyDraw background;

	Player player;

	PlayerMK2 pMK2;

	Sprite platform;
	Sprite wall;
	Level level;

	// Declare other variables:
	SoundChannel soundTrack;

	public MyGame() : base(1366, 768, false, true)     // Create a window that's 1200x800 and NOT fullscreen
	{
		targetFps = 20;

		// Create a full screen canvas (EasyDraw):
		// (in MyGame, width and height refer to game.width and game.height: the window size)
		background = new EasyDraw(width, height);
		level = new Level();

		pMK2 = new PlayerMK2("square.png", new Vector2(width - 300, 200), level);
		pMK2.SetOrigin(pMK2.width / 2, pMK2.height / 2);

		player = new Player(width / 2, 200, 1000f, level);
		player.SetOrigin(player.width / 2, player.height / 2);

		platform = new Sprite("checkers.png", false, true);
		platform.SetXY(width / 2, height - 50);
		platform.SetOrigin(platform.width / 2, platform.height / 2);
		platform.SetScaleXY(20, 0.5f);
		level.Add(platform);

		wall = new Sprite("colors.png", false, true);
		wall.SetOrigin(wall.width / 2, wall.height / 2);
		wall.SetXY(300, height / 2);
		wall.SetScaleXY(0.5f, 10);
		level.Add(wall);

		// Add all sprites to the engine, so that they will be displayed every frame:
		// (The order that we add them is the order that they will be drawn.)
		
		AddChild(platform);
		AddChild(wall);
		AddChild(player);
		AddChild(pMK2);
		AddChild(background);

		// Play a sound track, looping and streaming, and keep a reference to it such that
		// we can change the volume:
		// (The .ogg file is in bin/Debug. ogg, mp3 and wav files are supported)
		soundTrack = new Sound("The_Endless_Journey.ogg", true, true).Play(true);

		// Print some information to the console (behind the game window):
		Console.WriteLine("Scene successfully initialized");
	}

	void FillBackground() {
		for (int i = 0; i < 100; i++) {
			// Set the fill color of the canvas to a random color:
			background.Fill(Utils.Random(100, 255), Utils.Random(100, 255), Utils.Random(100, 255));
			// Don't draw an outline for shapes:
			background.NoStroke();
			// Choose a random position and size:
			float px = Utils.Random(0, width);
			float py = Utils.Random(0, height);
			float size = Utils.Random(2, 5);
			// Draw a small circle shape on the canvas:
			background.Ellipse(px, py, size, size);
		}
	}

	// Update is called once per frame, by the engine, for each game object in the hierarchy
	// (including the Game itself)
	void Update() {

		background.ClearTransparent();
		background.Text(String.Format("Player Position: ({0}, {1})", Mathf.Round(pMK2.x), Mathf.Round(pMK2.y)), 50,50);
		background.Text(String.Format("Player Velocity: ({0}, {1})", Math.Round(pMK2.velocity.x, 2), Math.Round(pMK2.velocity.y, 2)), width-300, 50);
		background.Text(String.Format("Target FPS: {0}", Mathf.Round(targetFps)), width-300, 100);
		if (Time.deltaTime != 0)
		{
			background.Text(String.Format("Current FPS: {0}", 1000 / Time.deltaTime), width - 300, 150);
		}
	}

	// Main is the first method that's called when the program is run
	static void Main() {
		// Create a "MyGame" and start it:
		new MyGame().Start();
	}
}