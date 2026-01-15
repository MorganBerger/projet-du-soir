using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

[RequireComponent(typeof(Collider))]
public class Cutable : MonoBehaviour
{
    [Min(0.001f)]
    public float verticalStep = 0.5f;
    [Min(0.001f)]
    public float horizontalStep = 0.5f;
    public float verticalMargin = 0.1f;
    public float horizontalMargin = 0.1f;

    public bool showGizmos = true;

    private Collider _collider;

    private List<Vector3> _cuttingPoints = new List<Vector3>();

    void OnValidate()
    {
        _collider = GetComponent<Collider>();
        SetupCuttingPoints();
    }

    private void Start()
    {
        _collider = GetComponent<Collider>();
        SetupCuttingPoints();
        SaveCuttingPoints();
    }

    private void SaveCuttingPoints()
    {
        // Create an instance of the ScriptableObject "in memory."
        var newSO = ScriptableObject.CreateInstance<CuttingPositions>();
    
        newSO.positions = _cuttingPoints;

        // Bundle that instance into an asset file and save it.
        AssetDatabase.CreateAsset(newSO, $"Assets/{gameObject.name}CuttingPositions.asset");
        AssetDatabase.SaveAssets();
    }

    private void SetupCuttingPoints()
    {
        _cuttingPoints.Clear();

        var countLimit = 1;

        Bounds bounds = _collider.bounds;

        Vector3 objCenter = transform.position;
        Vector3 start = transform.position - Vector3.forward;

        for (float y = bounds.min.y - verticalMargin; y <= bounds.max.y + verticalMargin; y += verticalStep)
        {
            var count = 0;

            for (float x = bounds.min.x - horizontalMargin; x <= bounds.max.x + horizontalMargin; x += horizontalStep) 
            {
                Vector3 newPos = new Vector3(x, y, objCenter.z);
                Vector3 dir = (newPos - start).normalized;
                RaycastHit hit;

                if (_collider.Raycast(new Ray(start, dir), out hit, 5))
                {
                    var point = transform.InverseTransformPoint(hit.point);
                    _cuttingPoints.Add(point);

                    count++;
                    if (count >= countLimit)
                    {
                        break;
                    }
                }
            }

            for (float x = bounds.max.x + horizontalMargin; x >= (objCenter.x - horizontalMargin); x -= horizontalStep) 
            {
                Vector3 newPos = new Vector3(x, y, objCenter.z);
                Vector3 dir = (newPos - start).normalized;
                RaycastHit hit;

                if (_collider.Raycast(new Ray(start, dir), out hit, 5))
                {

                    var point = transform.InverseTransformPoint(hit.point);
                    _cuttingPoints.Add(point);

                    count++;
                    if (count >= countLimit)
                    {
                        break;
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 start = transform.position - Vector3.forward;

        foreach (var pos in _cuttingPoints)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pos, 0.05f);
        }

        Gizmos.matrix = Matrix4x4.identity;
    }
}