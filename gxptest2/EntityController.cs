using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

public class EntityController : Sprite
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
    public float airStrafeMod;
    public float jumpForce;

    public int fixedDeltaTime = 20;
    public float accumulatedTime = 0;

    public Level lvl;
    public List<Sprite> lvlObjs;

    public Sprite projection;

    public EntityController(string fileName) : base(fileName)
    {

    }

    public virtual void Update()
    {
        GenComponents();

        RunFixedUpdate(Time.deltaTime);
    }
    public virtual void FixedUpdate()
    {
        ForceToVelocity();

        List<Vector2> tempV = GenerateNormals(lvlObjs, true);
        List<Vector2> tempH = GenerateNormals(lvlObjs, false);

        CollisionHandler(tempV, tempH);

        ApplyVelocity();
    }

    public void RunFixedUpdate(float deltaTime)
    {
        accumulatedTime += deltaTime;

        while (accumulatedTime >= fixedDeltaTime)
        {
            FixedUpdate();
            accumulatedTime -= fixedDeltaTime;
        }
    }
    public void GenComponents()
    {
        if (projection == null)
        {
            GenerateProjection();
        }

        if (lvlObjs == null)
        {
            lvlObjs = lvl.GetAll();
        }
    }

    public void GenerateProjection()
    {
        projection = new Sprite(texture.filename, true, true);
        projection.alpha = 0f;
        projection.SetOrigin(projection.width / 2, projection.height / 2);
        projection.SetScaleXY(scaleX, scaleY);
        projection.SetXY(x, y);
        Console.WriteLine("Generated Entity Projection with filename '{0}'", projection.texture.filename);
    }
    public void ForceToVelocity()
    {
        totalForces = new Vector2(movement.x + otherForces.x, movement.y + otherForces.y);

        Vector2 accel = Physics.GravityAccel(mass, totalForces);
        accel = new Vector2(accel.x * fixedDeltaTime, accel.y * fixedDeltaTime);
        velocity = new Vector2(velocity.x + accel.x, velocity.y + accel.y);
        velocity = new Vector2(velocity.x - velocity.x * friction, velocity.y - velocity.y * friction);
        velocityMoment = new Vector2(velocity.x * fixedDeltaTime, velocity.y * fixedDeltaTime);
    }
    //Generate a list of collision normals for either horizontal or vertical position changes, using 
    public List<Vector2> GenerateNormals(List<Sprite> objects, bool isVertical)
    {
        //list of normals to be returned
        List<Vector2> normals = new List<Vector2>();

        //temp holding vector for the normal
        Vector2 normal;

        //apply velocity changes to the player projection with relavancy to direction
        projection.x = x + velocityMoment.x * (isVertical ? 0 : 1);
        projection.y = y + velocityMoment.y * (isVertical ? 1 : 0);

        //check for every object in the list provided...
        foreach (Sprite sprite in objects)
        {
            //if they overlap with the player projection
            if (projection.HitTest(sprite))
            {
                //if they do, calculate the offset vector
                Vector2 offset = new Vector2(sprite.x - x, sprite.y - y);

                //if the check is for vertical collision...
                if (isVertical)
                {
                    //add the normal change to the x - aka left - if negative
                    //or to the y - aka right - if positive
                    normal = new Vector2(offset.y < 0 ? 1 : 0, offset.y > 0 ? 1 : 0);
                }
                //else if the check is for horizontal collision...
                else
                {
                    //add the normal change to the x - aka up - if negative
                    //or to the y - aka down - if positive
                    normal = new Vector2(offset.x < 0 ? 1 : 0, offset.x > 0 ? 1 : 0);
                }
                //finally, add the created normal to the list
                normals.Add(normal);
            }
        }
        //reverts the projection to the player position
        projection.SetXY(x, y);

        //and then return the list of either vertical collision normals or horizontal collision normals
        return normals;
    }

    public void CollisionHandler(List<Vector2> vertical, List<Vector2> horizontal)
    {
        int changeX = 1;
        int changeY = 1;

        grounded = false;

        foreach (Vector2 v in vertical)
        {
            if (v.Magnitude() > 0)
            {
                changeY = 0;
            }

            if (v.y > 0)
            {
                grounded = true;
            }
        }

        foreach (Vector2 h in horizontal)
        {
            if (h.Magnitude() > 0)
            {
                changeX = 0;
            }
        }

        velocityMoment = new Vector2(velocityMoment.x * changeX, velocityMoment.y * changeY);
    }
    public void ApplyVelocity()
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
}
