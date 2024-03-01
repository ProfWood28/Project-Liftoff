using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

class Menu : Sprite
{
    List<Button> buttons = new List<Button>();
    MyGame gamm;

    public Menu(string fileName, float scale, string[] buttonFiles, Vector2[] buttonPositons, int[] buttonSends, float[] buttonScales, MyGame gaym) : base (fileName, false)
    {
        SetXY(0, 0);
        SetOrigin(0.5f, 0.5f);
        SetScaleXY(scale);

        gamm = gaym;

        for (int i = 0; i < buttonFiles.Length; i++)
        {
            Button newButton = new Button(buttonFiles[i], buttonPositons[i], buttonSends[i], buttonScales[i]);
            newButton.SetOrigin(0.5f, 0.5f);
            buttons.Add(newButton);
            AddChild(newButton);
        }

        Console.WriteLine("Finished Menu setup: {0} buttons created", buttons.Count);
    }

    private void Update()
    {
        CheckButtons();
    }

    private void CheckButtons()
    {
        foreach (Button button in buttons)
        {
            button.visible = this.visible;

            if(button.visible)
            {
                bool buttonHover = button.HitTestPoint(Input.mouseX, Input.mouseY);

                int highLight = buttonHover ? 1 : 0;
                button.SetFrame(highLight);

                if (buttonHover && Input.GetMouseButtonDown(0))
                {
                    gamm.gameState = button.send;
                }
            }    
        }
    }
}