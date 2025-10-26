using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Transform stand; // 🎯 Stand referansı
    public float smoothSpeed = 1f;
    public Vector3 offset;

    private Camera cam;
    private float targetSize;
    private float minSize = 5f;
    private float maxSize = 9f;
    private float zoomSmooth = 2f;

    public float minX = 0f;
    public float maxX = 30f;
    public float minY = -45f;
    public float maxY = 0f;
    public SpriteRenderer homeRenderer;
    public SpriteRenderer map;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraFollow: Ana kamera componenti bulunamadı!");
        }
        targetSize = cam.orthographicSize;
    }

    private void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothed.x, smoothed.y, transform.position.z);
        float clampedX = Mathf.Clamp(smoothed.x, minX, maxX);
        float clampedY = Mathf.Clamp(smoothed.y, minY, maxY);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);

        if (stand != null)
        {
            float distanceToStand = Vector2.Distance(target.position, stand.position);
            Debug.Log("distanceToStand: " + distanceToStand);
            targetSize = distanceToStand <= 9f ? minSize : maxSize;
            if (distanceToStand <= 9f)
            {
                float targetAlpha = Mathf.InverseLerp(2f, 0, distanceToStand);
                SetHomeAlpha(targetAlpha);
            }
            else if (distanceToStand > 9f)
            {
                float targetAlpha = Mathf.InverseLerp(0,2.5f, distanceToStand);
                SetHomeAlpha(targetAlpha);
            }
        }

        // 🔄 Smooth zoom geçişi
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSmooth);
    }
    private void SetHomeAlpha(float targetAlpha)
    {
        if (homeRenderer != null)
        {
            Color c = homeRenderer.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * 5f);
            homeRenderer.color = c;
        }
        if (map != null)
        {
            Color c = map.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * 5f);
            map.color = c;
        }
    }

}
