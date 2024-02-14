using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

public class Projectile
{
    public Vector2 position;
    public Vector2 velocity;
    public int damage;
    public float mass;

    public Projectile(Vector2 position, Vector2 velocity, int damage, float mass)
    {
        this.position = position;
        this.velocity = velocity;
        this.damage = damage;
        this.mass = mass;
    }
}


