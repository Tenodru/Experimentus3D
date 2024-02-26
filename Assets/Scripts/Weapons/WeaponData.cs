using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public string name;
    public int damage;

    public WeaponData(string name, int damage)
    {
        this.name = name;
        this.damage = damage;
    }
}

public class RangedWeaponData : WeaponData
{
    public float spread;
    public float range;
    public float reloadTime;
    public int magazineSize;
    public int bulletsPerShot;
    public FireMode fireMode;

    public RangedWeaponData(string name, int damage, float spread, float range, float reloadTime, int magazineSize, int bulletsPerShot, FireMode fireMode) : base(name, damage)
    {
        this.spread = spread;
        this.range = range;
        this.reloadTime = reloadTime;
        this.magazineSize = magazineSize;
        this.bulletsPerShot = bulletsPerShot;
        this.fireMode = fireMode;
    }
}
