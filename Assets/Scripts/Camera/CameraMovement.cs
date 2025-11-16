using UnityEngine;

public class CameraMovement : MonoBehaviour
{
   //Reference to the player's transform
    [SerializeField]
    private Transform playerTransform;

    //Offset of the camera relative to the player
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 0f, -10f); 

    
    void LateUpdate()
    {
        //Verify that playerTransform is assigned
        if (playerTransform != null)
        {
            //Set camera position to player's position plus offset
            transform.position = playerTransform.position + offset;
        }
        else
        {
            Debug.LogWarning("Player Transform not assigned to CameraFollow script on " + gameObject.name);
        }
    }
}
