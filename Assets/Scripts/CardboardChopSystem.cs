using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

/// <summary>
/// This script handles the dynamic chopping logic for cardboard-style assets.
/// It uses EzySlice to divide the mesh at the impact point of the axe.
/// </summary>
public class CardboardChopSystem : MonoBehaviour
{
    // [Header("Material Settings")]
    // [SerializeField] private Material crossSectionMaterial; // The corrugated cardboard edge texture

    // [Header("Cutting Settings")]
    // [SerializeField] private LayerMask choppableLayer;
    // [SerializeField] private float cutForce = 2.0f;
    // [SerializeField] private float chipLifeTime = 3.0f;

    // void Update()
    // {
    //     if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
    //     {
    //         Debug.Log("Chop initiated.");
    //         PerformChop();
    //     }
    // }

    // /// <summary>
    // /// Simulates the axe impact and calculates the slice plane.
    // /// </summary>
    // public void PerformChop()
    // {
    //     // We cast a ray from the "axe" position forward
    //     RaycastHit hit;
    //     if (Physics.Raycast(transform.position, transform.forward, out hit, 1.0f, choppableLayer))
    //     {
    //         Debug.Log($"Hit detected on object: {hit.collider.gameObject.name}");
    //         GameObject target = hit.collider.gameObject;

    //         // Define the slicing plane
    //         // Position: where the axe hit
    //         // Normal: Up or Right relative to the axe swing direction
    //         Vector3 slicePosition = hit.point;
    //         Vector3 sliceNormal = transform.up;

    //         SliceObject(target, slicePosition, sliceNormal);
    //     }
    // }

    // /// <summary>
    // /// Slices the target object and manages the resulting hulls.
    // /// </summary>
    // private void SliceObject(GameObject obj, Vector3 position, Vector3 normal)
    // {
    //     // Perform the slice using the EzySlice library
    //     SlicedHull hull = obj.Slice(position, normal, crossSectionMaterial);

    //     if (hull != null)
    //     {
    //         // Create the two new parts
    //         GameObject upperPart = hull.CreateUpperHull(obj, crossSectionMaterial);
    //         GameObject lowerPart = hull.CreateLowerHull(obj, crossSectionMaterial);

    //         // Configure the main part (usually the lower one for a tree)
    //         SetupHull(lowerPart, false);
    //         lowerPart.layer = obj.layer; // Keep it choppable

    //         // Configure the "chip" (the piece that falls off)
    //         SetupHull(upperPart, true);

    //         // Inherit the original object's transform properties
    //         lowerPart.transform.position = obj.transform.position;
    //         lowerPart.transform.rotation = obj.transform.rotation;
    //         lowerPart.transform.localScale = obj.transform.localScale;

    //         upperPart.transform.position = obj.transform.position;
    //         upperPart.transform.rotation = obj.transform.rotation;
    //         upperPart.transform.localScale = obj.transform.localScale;

    //         // Destroy the original mesh to replace it with the sliced versions
    //         Destroy(obj);
    //     }
    // }

    // /// <summary>
    // /// Adds necessary components to the new hulls.
    // /// </summary>
    // private void SetupHull(GameObject hullObj, bool isChip)
    // {
    //     // hullObj.AddComponent<MeshCollider>().convex = true;
    //     if (isChip)
    //     {
    //         hullObj.AddComponent<MeshCollider>().convex = true;
    //         Rigidbody rb = hullObj.AddComponent<Rigidbody>();
    //         // Add some force to make the chip fly away
    //         rb.AddExplosionForce(cutForce, hullObj.transform.position, 1.0f, 1.0f, ForceMode.Impulse);
    //         // Auto-destroy the small chip after a while
    //         Destroy(hullObj, chipLifeTime);
    //     }
    //     else
    //     {
    //         hullObj.AddComponent<MeshCollider>();
    //         // The main part of the tree should stay static or be kinematic
    //         // until it's fully cut down
    //         // rb.isKinematic = true; 
    //     }
    // }

    [Header("Material Settings")]
    [SerializeField] private Material crossSectionMaterial; // The corrugated cardboard edge texture

    [Header("Cutting Settings")]
    [SerializeField] private LayerMask choppableLayer;
    [SerializeField] private float cutForce = 3.0f;
    [SerializeField] private float chipLifeTime = 4.0f;
    [SerializeField] private float notchSize = 0.5f; // How deep the "L" cut goes

    void Update()
    {
        // Detect Space bar press using the new Input System
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PerformNotchChop();
        }
    }

    /// <summary>
    /// Performs a complex "L-shaped" cut to remove a small piece of the mesh.
    /// </summary>
    public void PerformNotchChop()
    {
        RaycastHit hit;
        // Search for a cardboard object in front of the axe
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3.0f, choppableLayer))
        {
            GameObject target = hit.collider.gameObject;

            // 1. Define the first plane (Horizontal cut)
            Vector3 firstPlanePos = hit.point;
            Vector3 firstPlaneNormal = transform.up;

            // 2. Define the second plane (Vertical cut to "exit" the mesh)
            // We shift the second plane deep into the mesh to create the corner
            Vector3 secondPlanePos = hit.point + (transform.forward * notchSize);
            Vector3 secondPlaneNormal = transform.forward;

            ExecuteDoubleSlice(target, firstPlanePos, firstPlaneNormal, secondPlanePos, secondPlaneNormal);
        }
    }

    /// <summary>
    /// Slices the object twice to isolate a corner piece (the chip).
    /// </summary>
    private void ExecuteDoubleSlice(GameObject obj, Vector3 p1, Vector3 n1, Vector3 p2, Vector3 n2)
    {
        // FIRST SLICE: Divide the object into Top and Bottom
        SlicedHull firstHull = obj.Slice(p1, n1, crossSectionMaterial);

        if (firstHull != null)
        {
            GameObject lowerPart = firstHull.CreateLowerHull(obj, crossSectionMaterial);
            GameObject upperPart = firstHull.CreateUpperHull(obj, crossSectionMaterial);

            // SECOND SLICE: We slice the Upper Part again to create the "notch"
            // This isolates the "corner" from the rest of the upper mesh
            SlicedHull secondHull = upperPart.Slice(p2, n2, crossSectionMaterial);

            if (secondHull != null)
            {
                // The corner piece that will fall off
                GameObject chipPart = secondHull.CreateLowerHull(upperPart, crossSectionMaterial);
                // The remaining part of the upper mesh that stays attached
                GameObject remainingUpperPart = secondHull.CreateUpperHull(upperPart, crossSectionMaterial);

                // Setup the pieces
                SetupHull(lowerPart, false, obj);
                SetupHull(remainingUpperPart, false, obj);
                SetupHull(chipPart, true, obj);

                // Clean up temporary object
                Destroy(upperPart);
                // Destroy original object
                Destroy(obj);
            }
            else
            {
                // If the second slice fails, we fall back to a simple split
                SetupHull(lowerPart, false, obj);
                SetupHull(upperPart, true, obj);
                Destroy(obj);
            }
        }
    }

    /// <summary>
    /// Configures physics and transforms for the newly created mesh parts.
    /// </summary>
    private void SetupHull(GameObject hullObj, bool isChip, GameObject original)
    {
        // Copy transform data
        hullObj.transform.position = original.transform.position;
        hullObj.transform.rotation = original.transform.rotation;
        hullObj.transform.localScale = original.transform.localScale;
        hullObj.layer = original.layer;

        // Add physics
        hullObj.AddComponent<MeshCollider>().convex = true;
        Rigidbody rb = hullObj.AddComponent<Rigidbody>();

        if (isChip)
        {
            // The small piece flies away
            rb.AddExplosionForce(cutForce, hullObj.transform.position, 2.0f, 1.0f, ForceMode.Impulse);
            Destroy(hullObj, chipLifeTime);
        }
        else
        {
            // The parts of the tree/object stay in place
            rb.isKinematic = true;
        }
    }

    void OnDrawGizmos()
    {
        // Visualize the cutting plane in the editor
        Gizmos.color = Color.red;

        var start = transform.position;
        var bottomL = transform.position - transform.right;
        var topL = bottomL + transform.forward;

        var bottomR = transform.position + transform.right;
        var topR = bottomR + transform.forward;

        Gizmos.DrawLine(topL, topR);
        Gizmos.DrawLine(bottomL, bottomR);
        Gizmos.DrawLine(topL, bottomL);
        Gizmos.DrawLine(topR, bottomR);


        Gizmos.color = Color.blue;
        Gizmos.DrawLine(start, start + transform.forward);
    }
}