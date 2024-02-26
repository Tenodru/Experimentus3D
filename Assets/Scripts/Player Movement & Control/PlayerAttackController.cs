using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles weapon attacking controls.
/// </summary>
[RequireComponent(typeof(PlayerKeybinds))]
public class PlayerAttackController : MonoBehaviour
{
    [Header("References")]
    public Weapon activeWeapon;
    public Camera cam;
    public PlayerHUD playerHUD;

    [Header("Hit Detection")]
    public RaycastHit rayHit;
    public LayerMask enemyLayer;

    PlayerKeybinds keybinds;

    bool isShooting;
    bool readyToShoot;
    bool isReloading;

    int bulletsFired, magazineLeft;
    float fireCooldown = 0.1f;
    float timeBetweenBullets = 0f;

    private void Start()
    {
        keybinds = GetComponent<PlayerKeybinds>();
        if (activeWeapon) UpdateActiveWeapon();
        readyToShoot = true;
    }

    private void Update()
    {
        GetInput();
    }

    /// <summary>
    /// Gets player mouse input.
    /// </summary>
    void GetInput()
    {
        // Check weapon trigger type to determine input type.
        if (activeWeapon.GetType() == typeof(WeaponRanged))
        {
            if (activeWeapon.triggerType == TriggerType.Tap)
            {
                isShooting = Input.GetKeyDown(keybinds.primaryFireKey);
            }
        }
        else isShooting = Input.GetKey(keybinds.primaryFireKey);

        // Fire
        if (readyToShoot && isShooting && !isReloading && activeWeapon.magazineLeft > 0)
        {
            activeWeapon.bulletsFired = activeWeapon.bulletsPerShot;
            Fire();
        }

        // Reload
        if (Input.GetKeyDown(keybinds.reloadKey) && activeWeapon.magazineLeft < activeWeapon.magazineSize && !isReloading) Reload();
    }

    /// <summary>
    /// Fires the active weapon.
    /// Calculates fire direction, fires raycast for hit detection, 
    /// and updates weapon magazine if applicable.
    /// </summary>
    void Fire()
    {
        readyToShoot = false;

        // Handling spread
        float spreadX = activeWeapon.GetSpread().Item1;
        float spreadY = activeWeapon.GetSpread().Item2;

        // Calculate direction
        Vector3 direction = cam.transform.forward + new Vector3(spreadX, spreadY, 0f);

        // Fire shot via Raycast
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit, activeWeapon.range, enemyLayer)) 
        {
            Debug.Log("Hit " + rayHit.collider.name);
            // Do other on-hit stuff
        }
        // Do VFX
        activeWeapon.DoMuzzleFlash();
        // TODO: Weapon hit decal
        //activeWeapon.DoHitDecal(rayHit);

        // Update weapon magazine
        activeWeapon.bulletsFired--;
        activeWeapon.magazineLeft--;
        playerHUD.UpdateMagazineDisplay(activeWeapon.magazineLeft, activeWeapon.magazineSize);

        Invoke(nameof(ResetShot), fireCooldown);
        if (activeWeapon.bulletsFired > 0 && activeWeapon.magazineLeft > 0) Invoke(nameof(Fire), timeBetweenBullets);
    }

    /// <summary>
    /// Handles end-of-shot actions.
    /// </summary>
    void ResetShot()
    {
        readyToShoot = true;
    }

    /// <summary>
    /// Initiates weapon reloading.
    /// </summary>
    void Reload()
    {
        isReloading = true;
        // TODO: on-reload stuff
        Invoke(nameof(ReloadFinished), activeWeapon.reloadTime);
    }

    /// <summary>
    /// Marks the end of reloading.
    /// Updates weapon magazine.
    /// </summary>
    void ReloadFinished()
    {
        activeWeapon.magazineLeft = activeWeapon.magazineSize;
        playerHUD.UpdateMagazineDisplay(activeWeapon.magazineLeft, activeWeapon.magazineSize);
        isReloading = false;
    }

    /// <summary>
    /// Handles actions that should happen when we get a new active weapon.
    /// </summary>
    public void UpdateActiveWeapon()
    {
        activeWeapon.magazineLeft = activeWeapon.magazineSize;
    }
}
