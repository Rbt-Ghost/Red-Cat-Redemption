using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private Weapon weapon;
    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private AudioSource shootAudioSource;
    private float pitchRange;

    void Update()
    {
        if (playerStats.IsAlive()) GetInput();
    }

    private void GetInput()
    {
        // Check if Space key is pressed OR if Mobile Button is held
        bool isShooting = Input.GetKey(KeyCode.Space);

        if (MobileInputManager.Instance != null && MobileInputManager.Instance.IsShooting())
        {
            isShooting = true;
        }

        if (isShooting)
        {
            if (weapon != null && weapon.isActiveAndEnabled && weapon.getCanShoot())
            {
                pitchRange = Random.Range(0.8f, 1.2f);
                shootAudioSource.pitch = pitchRange;
                shootAudioSource.Play();
                weapon.Shoot();
            }
        }
    }
}