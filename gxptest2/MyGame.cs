using System;                   // System contains a lot of default C# libraries 
using System.Collections.Generic;
using GXPEngine;                // GXPEngine contains the engine
using GXPEngine.Core;

public class MyGame : Game {
	// Declare the Sprite variables:
	EasyDraw background;

	// Declare other variables:
	SoundChannel soundTrack;

	Train train;

	public MyGame() : base(1366, 768, false, true)     // Create a window that's 1200x800 and NOT fullscreen
	{
		targetFps = 600;

		// Create a full screen canvas (EasyDraw):
		// (in MyGame, width and height refer to game.width and game.height: the window size)
		background = new EasyDraw(width, height);

		train = new Train("colors.png");
		train.SetXY(width/2, height/2);

		// Add all sprites to the engine, so that they will be displayed every frame:
		// (The order that we add them is the order that they will be drawn.)
		AddChild(background);
		AddChild(train);

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
		if (Time.deltaTime != 0)
		{
			background.Text(String.Format("FPS: {0}", 1000 / Time.deltaTime), width - 100, 40);
		}
	}

	// Main is the first method that's called when the program is run
	static void Main() {
		// Create a "MyGame" and start it:
		new MyGame().Start();
	}
}