using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject healthPrefab;
    public GameObject ammoPrefab;

    public int healthCount = 5;
    public int ammoCount = 5;

    [Header("Area Settings")]
    public Vector2 areaSize = new Vector2(50, 50);
    public float rayHeight = 20f;
    public LayerMask groundLayer;

    private void Start()
    {
        SpawnItems(healthPrefab, healthCount);
        SpawnItems(ammoPrefab, ammoCount);
    }

    private void SpawnItems(GameObject prefab, int count)
    {
        if (prefab == null) return;

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = GetRandomPosition();
            if (randomPos != Vector3.zero)
            {
                Instantiate(prefab, randomPos + Vector3.up * 0.5f, Quaternion.identity);
            }
        }
    }

    private Vector3 GetRandomPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            float x = Random.Range(-areaSize.x / 2, areaSize.x / 2);
            float z = Random.Range(-areaSize.y / 2, areaSize.y / 2);

            Vector3 rayStart = new Vector3(x, rayHeight, z) + transform.position;

            RaycastHit hit;
            if (Physics.Raycast(rayStart, Vector3.down, out hit, 50f, groundLayer))
            {
                return hit.point;
            }
        }

        return Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, 10, areaSize.y));
    }
}