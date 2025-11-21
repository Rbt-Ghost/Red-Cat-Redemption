using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private Weapon weapon;

    void Update()
    {
        GetInput();
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
            else
            {
                Debug.LogWarning("Weapon is not assigned to PlayerCombat!");
            }
        }
    }
}