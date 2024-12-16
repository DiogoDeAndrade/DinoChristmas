using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Grabber : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)] 
    private float extend = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float grab = 1.0f;
    [SerializeField] 
    private float minAmplitude = 20.0f;
    [SerializeField] 
    private float maxAmplitude = 20.0f;
    [SerializeField]
    private int   segmentCount = 5;
    [SerializeField]
    private float segmentLength = 25.0f;
    [SerializeField, MinMaxSlider(0.0f, 45.0f)]
    private Vector2 clawLimit = new Vector2(6.0f, 30.0f);
    [SerializeField]
    private Transform clawTip;
    [SerializeField]
    private Transform clawTop;
    [SerializeField]
    private Transform clawBottom;

    LineRenderer lineRenderer;

    void Start()
    {
    }
   
    void Update()
    {
        UpdateGrabber();
    }

    [Button("Update Grabber")]
    void UpdateGrabber()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null) return;
        }

        // Array for LineRenderer positions
        List<Vector3> positions = new();

        var prevPoint = GetPos(0);

        for (int j = 0; j < 2; j++)
        {
            // Initialize first position
            int nextIndex = (j + 1) % 2;
            var vectorDir = (j == 0) ? (Vector2.right) : (Vector2.left);

            positions.Add(prevPoint);

            for (int i = 1; i < segmentCount; i++)
            {
                var segmentStart = prevPoint; segmentStart.y = GetPos(nextIndex).y;

                prevPoint = GetPointOnLineWithDistance(prevPoint, segmentStart, vectorDir, segmentLength);
                nextIndex = (nextIndex + 1) % 2;

                positions.Add(prevPoint);
            }

            prevPoint.y = GetPos(nextIndex).y;
            nextIndex = (nextIndex + 1) % 2;

            if (j == 0)
            {
                clawTip.localPosition = new Vector3(prevPoint.x, 0.0f, 0.0f);
            }
        }

        // Update the LineRenderer
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());

        float angle = Mathf.Lerp(clawLimit.x, clawLimit.y, 1.0f - grab);
        clawTop.localRotation = Quaternion.Euler(0.0f, 0.0f, angle);
        clawBottom.localRotation = Quaternion.Euler(0.0f, 0.0f, -angle);
    }

    Vector2 GetPointOnLineWithDistance(Vector2 p, Vector2 lineStart, Vector2 lineDir, float segmentLength)
    {
        // Normalize the direction vector V
        Vector2 direction = lineDir.normalized;

        // Vector from A to B
        Vector2 AB = lineStart - p;

        // Quadratic equation coefficients
        float a = 1; // direction is normalized
        float b = 2 * Vector2.Dot(direction, AB);
        float c = AB.sqrMagnitude - (segmentLength * segmentLength);

        // Solve the quadratic equation for t
        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            Debug.LogError("No valid solution exists for the given segmentLength.");
            return Vector2.zero; // No solution
        }

        // Calculate the two possible solutions for t
        float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);

        // One of them will yield a negative t

        if (t1 >= 0.0f) return lineStart + t1 * direction;
        
        return lineStart + t2 * direction;
    }

    Vector3 GetPos(int index)
    {
        var ret = Vector3.zero + Vector3.up * ((maxAmplitude - minAmplitude) * extend + minAmplitude);
        
        if (index == 1) ret.y = -ret.y;

        return ret;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetPos(0), 10.0f);
        Gizmos.DrawWireSphere(GetPos(1), 10.0f);
    }
}
