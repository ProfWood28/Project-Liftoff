using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;
using GXPEngine;

public class InputBuffer
{
    private List<InputEvent> buffer = new List<InputEvent>();

    public void AddKeyInput(int keyID)
    {
        InputEvent input = new InputEvent
        {
            type = InputEvent.InputType.KeyPress,
            keyID = keyID,
            timestamp = Time.time
        };

        buffer.Add(input);
    }

    public void AddMouseInput(Vector2 mousePosition, int mouseButton)
    {
        InputEvent input = new InputEvent
        {
            type = InputEvent.InputType.MouseClick,
            mousePosition = mousePosition,
            mouseButton = mouseButton,
            timestamp = Time.time
        };

        buffer.Add(input);
    }

    public void AddAxisInput(int axisOut, int axisID)
    {
        InputEvent input = new InputEvent
        {
            type = InputEvent.InputType.Axis,
            keyID = axisID,
            axisValue = axisOut,
            timestamp = Time.time
        };

        buffer.Add(input);
    }

    public void ProcessInputs()
    {
        // Clear the buffer after processing inputs
        buffer.Clear();
    }

    public List<InputEvent> GetKeyInputs()
    {
        List<InputEvent> keyInputs = new List<InputEvent>();

        foreach (InputEvent input in buffer)
        {
            if (input.type == InputEvent.InputType.KeyPress)
            {
                keyInputs.Add(input);
            }
        }

        return keyInputs;
    }

    public List<InputEvent> GetMouseInputs()
    {
        List<InputEvent> mouseInputs = new List<InputEvent>();

        foreach (InputEvent input in buffer)
        {
            if (input.type == InputEvent.InputType.MouseClick)
            {
                mouseInputs.Add(input);
            }
        }

        return mouseInputs;
    }

    public List<InputEvent> GetAxisInputs()
    {
        List<InputEvent> axisInputs = new List<InputEvent>();

        foreach (InputEvent input in buffer)
        {
            if (input.type == InputEvent.InputType.Axis)
            {
                axisInputs.Add(input);
            }
        }

        return axisInputs;
    }

    private void ProcessInput(InputEvent input)
    {
        // Example: Perform actions based on input type
        switch (input.type)
        {
            case InputEvent.InputType.KeyPress:
                Console.WriteLine($"Key {input.keyID} pressed at time {input.timestamp}");
                break;
            case InputEvent.InputType.MouseClick:
                Console.WriteLine($"Mouse clicked at position {input.mousePosition} with button {input.mouseButton} at time {input.timestamp}");
                break;
            case InputEvent.InputType.Axis:
                Console.WriteLine($"Axis input detected with keyID {input.keyID} at time {input.timestamp}");
                break;
                // Add additional cases for other input types as needed
        }
    }
}
