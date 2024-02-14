using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

class Train : Sprite
{
    public Vector2 movement = new Vector2(0, 0);

    public Vector2 velocity = new Vector2(0, 0);
    public Vector2 velocityMoment = new Vector2(0, 0);

    public Vector2 otherForces = new Vector2(0, 0);
    public Vector2 totalForces = new Vector2(0, 0);

    public float mass = 0;
    public bool grounded = false;
    public float friction = 0.1f;
    public float moveForce;

    public int fixedDeltaTime = 20;
    public float accumulatedTime = 0;

    private InputBuffer inputBuffer = new InputBuffer();
    Train(string fileName) : base(fileName)
    {

    }

    private void Update()
    {
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
        inputBuffer.AddAxisInput(Input.GetAxis(Key.A, Key.D), Key.A);
        inputBuffer.AddAxisInput(Input.GetAxis(Key.W, Key.S), Key.W);

    }
    public virtual void HandleBufferedInputs()
    {
        //controls
    }

    private void RunFixedUpdate(float deltaTime)
    {
        accumulatedTime += deltaTime;

        while (accumulatedTime >= fixedDeltaTime)
        {
            FixedUpdate();
            accumulatedTime -= fixedDeltaTime;
        }
    }
    private void ForceToVelocity()
    {
        totalForces = new Vector2(movement.x + otherForces.x, movement.y + otherForces.y);

        Vector2 accel = Physics.Accel(mass, totalForces);
        accel = new Vector2(accel.x * fixedDeltaTime, accel.y * fixedDeltaTime);
        velocity = new Vector2(velocity.x + accel.x, velocity.y + accel.y);
        velocity = new Vector2(velocity.x - velocity.x * friction, velocity.y - velocity.y * friction);
        velocityMoment = new Vector2(velocity.x * fixedDeltaTime, velocity.y * fixedDeltaTime);
    }
    private void ApplyVelocity()
    {
        x += velocityMoment.x;
        y += velocityMoment.y;

        movement = new Vector2(0, 0);
        otherForces = new Vector2(0, 0);
        totalForces = new Vector2(0, 0);
    }
    public void AddForce(Vector2 force)
    {
        otherForces = new Vector2(otherForces.x + force.x, otherForces.y + force.y);
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
}
