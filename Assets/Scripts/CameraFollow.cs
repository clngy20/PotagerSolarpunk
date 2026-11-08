using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; //drone
    [SerializeField] private Vector2 mapMin = new Vector2(-10f, -10f); //coin bas gauchez du sol
    [SerializeField] private Vector2 mapMax = new Vector2(10f, 10f); //coin haut droit du sol
    [SerializeField] private float smoothTime = 0.15f;

    private Camera cam;
    private Vector3 velocity;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Moitié de la zone visible, en unités du monde
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        // Si la vue est plus grande que la map, on centre sur la map
        float minX = mapMin.x + halfWidth;
        float maxX = mapMax.x - halfWidth;
        float minZ = mapMin.y + halfHeight;
        float maxZ = mapMax.y - halfHeight;

        float x = minX > maxX ? (mapMin.x + mapMax.x) / 2f : Mathf.Clamp(target.position.x, minX, maxX);
        float z = minZ > maxZ ? (mapMin.y + mapMax.y) / 2f : Mathf.Clamp(target.position.z, minZ, maxZ);

        Vector3 desired = new Vector3(x, transform.position.y, z);
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((mapMin.x + mapMax.x) / 2f, 0f, (mapMin.y + mapMax.y) / 2f);
        Vector3 size = new Vector3(mapMax.x - mapMin.x, 0.1f, mapMax.y - mapMin.y);
        Gizmos.DrawWireCube(center, size);
    }
}
