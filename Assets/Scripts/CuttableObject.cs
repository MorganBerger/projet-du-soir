using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CuttableObject : MonoBehaviour
{
    public CuttingPositions cuttingPositions;

    public List<Vector3> possibleCuts = new List<Vector3>();

    private Collider _collider;

    void OnValidate()
    {
        _collider = GetComponent<Collider>();
        possibleCuts = cuttingPositions.positions;
    }

    void Awake()
    {
        _collider = GetComponent<Collider>();
        possibleCuts = cuttingPositions.positions;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Cut();
        }
    }

    private void Cut()
    {
        if (possibleCuts.Count > 0) 
        {
            int randomIndex = Random.Range(0, possibleCuts.Count);

            currentCutStart = possibleCuts[randomIndex];


            Vector3 center = Vector3.zero;
            // Debug.Log("cut start: " + currentCutStart);

            Vector3 dir = new Vector3(currentCutStart.x, currentCutStart.y, 0);

            float horizontalStep = 0.15f;

            if (currentCutStart.x > 0)
            {
                dir.x -= horizontalStep;
            }
            else
            {
                dir.x += horizontalStep;
            }

            currentCutEnd = dir;

            var vector = currentCutEnd - currentCutStart;

            var rand = Random.Range(-10, 10f);

            if (rand < 0)
            {
                rand -= 5;
            } else {
                rand += 5;
            }

            var angle = Quaternion.Euler(0, 0, rand);

            angledVector = angle * vector;

            possibleCuts.RemoveAt(randomIndex);

            var defaultPos = currentCutStart + angledVector;
        }
    }

    private Vector3 currentCutStart;
    private Vector3 currentCutEnd;

    private Vector3 angledVector;

    void OnDrawGizmos()
    {
        if (possibleCuts == null)
            return;


        if (_collider != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_collider.bounds.center, 0.025f);
        }

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.red;
        foreach (var pos in possibleCuts)
        {
            Gizmos.DrawSphere(pos, 0.025f);
        }

        if (currentCutStart != null && currentCutEnd != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(currentCutStart, currentCutEnd);
        }

        if (currentCutStart != null && angledVector != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(currentCutStart, currentCutStart + angledVector);
        }

        // if (currentCutStart != Vector3.zero)
        // {
        //     // Draw First Plane (Horizontal)
            // Gizmos.color = new Color(0, 1, 1, 0.5f); 

        //     DrawPlaneGizmo(currentCutStart, transform.up);

        //     // Draw Second Plane (Vertical/Exit)
        //     Gizmos.color = new Color(1, 0, 1, 0.5f);
            
        //     Vector3 secondPos = currentCutStart;

        //     var secondNormal = cutRotation * transform.up;

        //     DrawPlaneGizmo(secondPos, secondNormal);
        // }

        Gizmos.matrix = Matrix4x4.identity;
    }


    [SerializeField] private bool showGizmos = true;
    [SerializeField] private float gizmoPlaneSize = 0.5f;
    private Quaternion cutRotation = Quaternion.Euler(0, 0, -20);

    private void DrawPlaneGizmo(Vector3 localPos, Vector3 localNormal)
    {
        // Preserve the object's localToWorld matrix
        Matrix4x4 objectMatrix = Gizmos.matrix;

        // Create rotation based on local normal
        Quaternion rot = Quaternion.LookRotation(localNormal, Vector3.forward);
        Matrix4x4 planeMatrix = Matrix4x4.TRS(localPos, rot, Vector3.one);

        // Multiply matrices to nest the plane under the object's transform
        Gizmos.matrix = objectMatrix * planeMatrix;

        // Draw the visualization cube
        Vector3 size = new Vector3(gizmoPlaneSize / 4f, gizmoPlaneSize / 2f, 0.005f);
        Vector3 offset = new Vector3(gizmoPlaneSize / 8f, 0, 0);
        Gizmos.DrawCube(offset, size);

        // Revert to object matrix for next calls
        Gizmos.matrix = objectMatrix;
    }
}
