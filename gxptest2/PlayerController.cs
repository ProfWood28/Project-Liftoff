using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

public class PlayerController : EntityController
{
    private InputBuffer inputBuffer = new InputBuffer();
    public PlayerController(string fileName) : base(fileName)
    {
        mass = 1000f;
        moveForce = 4f;
        jumpForce = 250f;
        airStrafeMod = 0.7f;
        friction = 0.1f;
    }

    public override void Update()
    {
        HandleInput();

        GenComponents();

        RunFixedUpdate(Time.deltaTime);
    }
    public virtual void HandleInput()
    {
        inputBuffer.AddAxisInput(Input.GetAxis(Key.A, Key.D), Key.A);

        if (Input.GetKeyDown(Key.SPACE))
        {
            inputBuffer.AddKeyInput(Key.SPACE);
        }
    }

    public override void FixedUpdate()
    {
        PlayerBasics();
    }

    private void PlayerBasics()
    {
        HandleBufferedInputs();

        ForceToVelocity();

        List<Vector2> tempV = GenerateNormals(lvlObjs, true);
        List<Vector2> tempH = GenerateNormals(lvlObjs, false);

        CollisionHandler(tempV, tempH);

        ApplyVelocity();

        inputBuffer.ProcessInputs();
    }

    public virtual void PlayerMovement(bool jumpedTrue, int moveAxis)
    {
        int jumped = jumpedTrue && grounded ? 1 : 0;
        movement = new Vector2(moveAxis * moveForce * (grounded ? 1f : airStrafeMod), jumped * -jumpForce);
    }

    public virtual void HandleBufferedInputs()
    {
        BasicPlatformerControls();
    }

    private void BasicPlatformerControls()
    {
        GetInputs(out List<InputEvent> keys, out List<InputEvent> axises, out List<InputEvent> mouse);

        bool jumped = false;

        foreach (InputEvent input in keys)
        {
            switch (input.keyID)
            {
                case (int)Key.SPACE:
                    jumped = true;
                    break;
            }
        }

        int moveDir = 0;

        foreach (InputEvent axis in axises)
        {
            switch (axis.keyID)
            {
                case (Key.A):
                    moveDir += axis.axisValue;
                    break;
            }
        }

        moveDir = Mathf.Round(Mathf.Clamp(moveDir, -1, 1));
        PlayerMovement(jumped, moveDir);
    }

    public void GetInputs(out List<InputEvent> keys, out List<InputEvent> axises, out List<InputEvent> mouse)
    {
        List<InputEvent> keyInputs = inputBuffer.GetKeyInputs();
        List<InputEvent> axisInputs = inputBuffer.GetAxisInputs();
        List<InputEvent> mouseInputs = inputBuffer.GetMouseInputs();

        keys = keyInputs;
        axises = axisInputs;
        mouse = mouseInputs;
    }
}
