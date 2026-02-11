using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform target;
    [SerializeField] private float smooth = 0f;

    [Header("Pixel Snap")]
    [SerializeField] private bool snapToPixelGrid = true;
    [SerializeField] private int pixelsPerUnit = 32;

    private Vector3 velocity;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(target.position.x, target.position.y, transform.position.z);

        if (smooth > 0f)
        {
            // smooth 값이 클수록 더 부드럽게 따라감
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, 1f / smooth);
        }
        else
        {
            transform.position = desired;
        }

        if (snapToPixelGrid && pixelsPerUnit > 0)
        {
            float unit = 1f / pixelsPerUnit;
            float x = Mathf.Round(transform.position.x / unit) * unit;
            float y = Mathf.Round(transform.position.y / unit) * unit;
            transform.position = new Vector3(x, y, transform.position.z);
        }
    }
}
