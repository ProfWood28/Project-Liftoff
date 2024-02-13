using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;

public class Weapon : Sprite
{
    public enum ProjectileType
    {
        Bullet,
        Buckshot,
        Missile
    }

    public String weaponName;
    public int ammoCapacity;
    public float knockbackFactor;
    public float projectileSpeed;
    public float projectileAccuracy;
    public ProjectileType projectileType;

    public Weapon(string name, int ammoCount, float knockbackFactor, float projectileSpeed, float projectileAccuracy, ProjectileType projectileType, string fileName) : base(fileName, false)
    {
        this.weaponName = name;
        this.ammoCapacity = ammoCount;
        this.knockbackFactor = knockbackFactor;
        this.projectileSpeed = projectileSpeed;
        this.projectileAccuracy = projectileAccuracy;
        this.projectileType = projectileType;
    }
}
