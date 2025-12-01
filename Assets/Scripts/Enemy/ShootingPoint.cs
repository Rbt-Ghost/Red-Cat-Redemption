using UnityEngine;

public class ShootingPoint : MonoBehaviour
{

    [SerializeField] private float normalXPosition = 1.3f;
    [SerializeField] private float flippedXPosition = -1.3f;

    
    [SerializeField] private EnemyMovement enemyMovement;
    private SpriteRenderer enemySpriteRenderer;
    private bool wasFlipped = false;

    void Start()
    {
        // Reference to EnemyMovement if not set in inspector
        if (enemyMovement == null)
        {
            enemyMovement = GetComponentInParent<EnemyMovement>();
        }

        if (enemyMovement == null)
        {
            Debug.LogError("EnemyMovement not found! Make sure this script is on a child of an object with EnemyMovement.");
            return;
        }

        // Get SpriteRenderer from EnemyMovement's GameObject
        enemySpriteRenderer = enemyMovement.GetComponent<SpriteRenderer>();

        if (enemySpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the enemy!");
        }

        // Initialize the shooting point position
        UpdateShootingPointPosition();
    }

    void Update()
    {
        if (enemySpriteRenderer == null) return;

        // Verify if the flip state has changed
        if (enemySpriteRenderer.flipX != wasFlipped)
        {
            UpdateShootingPointPosition();
            wasFlipped = enemySpriteRenderer.flipX;
        }
    }

    void UpdateShootingPointPosition()
    {
        if (enemySpriteRenderer == null) return;

        // Set the target X position based on flip state
        float targetX = enemySpriteRenderer.flipX ? flippedXPosition : normalXPosition;

        // Update local position
        Vector3 localPosition = transform.localPosition;
        localPosition.x = targetX;
        transform.localPosition = localPosition;
    }

 
    public void ResetPosition()
    {
        UpdateShootingPointPosition();
        wasFlipped = enemySpriteRenderer != null ? enemySpriteRenderer.flipX : false;
    }


    public void SetPositions(float normalPos, float flippedPos)
    {
        normalXPosition = normalPos;
        flippedXPosition = flippedPos;
        UpdateShootingPointPosition();
    }
}