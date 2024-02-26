using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class WeaponOld : ScriptableObject
{
    [Header("Identification Info")]
    public string name = "Weapon Name";

    [Header("Weapon Stats")]
    public int damage;

    [Header("References")]
    public GameObject weaponModel;

    public abstract float GetSpread();
    public abstract float GetRange();
    public abstract float GetReloadTime();
    public abstract int GetMagazineSize();
    public abstract int GetBulletsPerShot();
    public abstract TriggerType GetTriggerType();
    public abstract FireMode GetFireMode();
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/RangedWeapon", order = 1)]
public class WeaponRanged : WeaponOld
{
    [Header("Weapon Stats")]
    public float spread;
    public float range;
    public float reloadTime;
    public int magazineSize;
    public int bulletsPerShot;
    public TriggerType triggerType;
    public FireMode fireMode;

    public override float GetSpread()
    {
        return spread;
    }
    public override float GetRange()
    {
        return range;
    }
    public override float GetReloadTime()
    {
        return reloadTime;
    }
    public override int GetMagazineSize()
    {
        return magazineSize;
    }
    public override int GetBulletsPerShot()
    {
        return bulletsPerShot;
    }
    public override TriggerType GetTriggerType()
    {
        return triggerType;
    }
    public override FireMode GetFireMode()
    {
        return fireMode;
    }
}


