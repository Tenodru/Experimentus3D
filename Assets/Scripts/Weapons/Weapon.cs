using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    [Header("Identification Info")]
    public string weaponName = "Weapon Name";

    [Header("Weapon Stats - Base")]
    public int damage;

    [Header("Weapon Stats - Ranged")]
    public float spread;
    public float range;
    public float reloadTime;
    public int magazineSize;
    public int bulletsPerShot;
    public TriggerType triggerType;
    public FireMode fireMode;

    [Header("References")]
    public Transform attackPoint;

    [Header("VFX")]
    public GameObject muzzleFlash;
    public GameObject hitDecal;

    [HideInInspector] public int bulletsFired;
    [HideInInspector] public int magazineLeft;

    /// <summary>
    /// Calculates and returns the x, y projectile spread of this weapon.
    /// </summary>
    /// <returns></returns>
    public (float, float) GetSpread()
    {
        if (spread == 0) return (0f, 0f);
        else return (Random.Range(-spread, spread), Random.Range(-spread, spread));
    }

    /// <summary>
    /// Creates the muzzle flash effect using this weapon's muzzleFlash VFX.
    /// </summary>
    public void DoMuzzleFlash()
    {
        Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity, attackPoint);
    }

    /// <summary>
    /// Applies this weapon's hit decal to the hit object.
    /// </summary>
    /// <param name="hit"></param>
    public void DoHitDecal(RaycastHit hit)
    {
        Instantiate(hitDecal, hit.point, Quaternion.Euler(0, 180, 0));
    }
}
