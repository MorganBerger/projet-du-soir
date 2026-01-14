using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class Cutable : MonoBehaviour
{
    [Min(0.001f)]
    public float scanStep = 0.5f;
    
    [Min(0f)]
    public float scanMargin = 0.1f;

    public bool showGizmos = true;

    private Collider _collider;

    private void OnValidate()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Bounds bounds = _collider.bounds;

        Gizmos.color = Color.red;

        Vector3 start = transform.localPosition - Vector3.forward;

        Vector3 objCenter = transform.localPosition;
        Vector3 left = objCenter + Vector3.left;

        for (float y = bounds.min.y - scanMargin; y <= bounds.max.y + scanMargin; y += scanStep)
        {
            var count = 0;

            for (float x = bounds.min.x - scanMargin; x <= (objCenter.x + scanMargin); x += scanStep) 
            {
                Vector3 newPos = new Vector3(x, y, objCenter.z);
                Gizmos.DrawLine(start, newPos);

                Vector3 dir = (newPos - start).normalized;

                // RaycastHit hit;
                // if (_collider.Raycast(new Ray(start, dir), out hit, 5) && hit.collider == _collider)
                // {
                //     Gizmos.color = Color.yellow;
                //     var point = transform.InverseTransformPoint(hit.point);
                //     Gizmos.DrawSphere(hit.point, 0.01f);

                //     count++;

                    // if (count > 3)
                    // {
                    //     break;
                    // }
                // }
            }

            Gizmos.color = new Color(1, 0, 1, 1f);
            for (float x = bounds.max.x + scanMargin; x >= (objCenter.x - scanMargin); x -= scanStep) 
            {
                Vector3 newPos = new Vector3(x, y, objCenter.z);
                Gizmos.DrawLine(start, newPos);
            }
        } 

        // Gizmos.color = new Color(0, 1, 0, .3f);
        // Gizmos.DrawCube(bounds.center, bounds.size);
    }
}
