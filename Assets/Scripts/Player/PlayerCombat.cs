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
        if(playerStats.IsAlive()) GetInput();
    }

    private void GetInput()
    {
        // Check if Space key is pressed or held down
        if (Input.GetKey(KeyCode.Space))
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