using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private Weapon weapon;
    [SerializeField]
    private PlayerStats playerStats;
    void Update()
    {
        if(playerStats.IsAlive()) GetInput();
    }

    private void GetInput()
    {
        // Check if Space key is pressed or held down
        if (Input.GetKey(KeyCode.Space))
        {
            if (weapon != null && weapon.isActiveAndEnabled)
            {
                weapon.Shoot();
            }
        }
    }
}