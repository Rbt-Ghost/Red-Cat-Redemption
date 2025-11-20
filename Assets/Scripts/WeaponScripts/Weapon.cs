using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer weaponSpriteRenderer; 
    [SerializeField]
    private Bullet bulletPrefab;
    
    [SerializeField]
    private float damage = 10f;
    
    [SerializeField]
    private float range = 10f;
    
    [SerializeField]
    private float fireTime = 0.5f; // Time between shots
    
    [SerializeField]
    private float recoilDistance = 0.5f; 
    
    [SerializeField]
    private float recoilDuration = 0.1f; 
    
    private bool canShoot = true;
    private Vector3 originalWeaponPosition;
    
    [SerializeField]
    private Transform firePoint;
    
    [SerializeField]
    private PlayerMovement playerMovement;

    void Awake()
    {
        // Get the player's SpriteRenderer to determine facing direction
        playerMovement = GetComponentInParent<PlayerMovement>();
        
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement not found on player! Bullet direction will not work correctly.");
        }
        
       //Save initial weapon's position
        originalWeaponPosition = transform.localPosition;
    }

    void Update()
    {
        if(playerMovement != null && playerMovement.IsFacingRight())
        {
            weaponSpriteRenderer.flipX = false;
        }
        else if(playerMovement != null && !playerMovement.IsFacingRight())
        {
            weaponSpriteRenderer.flipX = true;
        }
    }

    public void Shoot()
    {
        if (!canShoot)
        {
            return;
        }

        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab is not assigned to Weapon!");
            return;
        }

        // Determine spawn position
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;

        // Instantiate the bullet
        Bullet bulletInstance = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        // Determine direction based on player's sprite orientation
        float direction = playerMovement.IsFacingRight()? 1f : -1f;

        // Initialize the bullet with damage, range, and direction
        bulletInstance.Initialize(damage, range, direction);

        // Start the fire rate cooldown
        StartCoroutine(FireRateCooldown());
        
        // Start recoil effect
        StartCoroutine(RecoilEffect(direction));
    }

    private IEnumerator FireRateCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireTime);
        canShoot = true;
    }

    private IEnumerator RecoilEffect(float direction)
    {
        //Calculate recoil direction
        float recoilDirection = -direction;
        
        //Target position for recoil
        Vector3 recoilPosition = originalWeaponPosition + new Vector3(recoilDirection * recoilDistance, 0f, 0f);
        
        //Init rcoil time
        float elapsedTime = 0f;
        
        // Move the weapon in recoil position
        while (elapsedTime < recoilDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (recoilDuration / 2f);
            transform.localPosition = Vector3.Lerp(originalWeaponPosition, recoilPosition, t);
            yield return null;
        }
        
        // Move back to original position
        elapsedTime = 0f;
        while (elapsedTime < recoilDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (recoilDuration / 2f);
            transform.localPosition = Vector3.Lerp(recoilPosition, originalWeaponPosition, t);
            yield return null;
        }
        
        // Make sure that the weapon is exactly at original position
        transform.localPosition = originalWeaponPosition;
    }
}