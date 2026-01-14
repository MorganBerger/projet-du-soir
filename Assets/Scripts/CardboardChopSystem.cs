using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;

/// <summary>
/// This script handles the dynamic chopping logic for cardboard-style assets.
/// It uses EzySlice to divide the mesh at the impact point of the axe.
/// </summary>
public class CardboardChopSystem : MonoBehaviour
{
    [Header("Material Settings")]
    [SerializeField] private Material crossSectionMaterial; // The corrugated cardboard edge texture

    [Header("Cutting Settings")]
    [SerializeField] private LayerMask choppableLayer;
    [SerializeField] private float cutForce = 2.0f;
    [SerializeField] private float chipLifeTime = 3.0f;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Chop initiated.");
            PerformChop();
        }
    }

    /// <summary>
    /// Simulates the axe impact and calculates the slice plane.
    /// </summary>
    public void PerformChop()
    {
        // We cast a ray from the "axe" position forward
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.0f, choppableLayer))
        {
            Debug.Log($"Hit detected on object: {hit.collider.gameObject.name}");
            GameObject target = hit.collider.gameObject;

            // Define the slicing plane
            // Position: where the axe hit
            // Normal: Up or Right relative to the axe swing direction
            Vector3 slicePosition = hit.point;
            Vector3 sliceNormal = transform.up;

            SliceObject(target, slicePosition, sliceNormal);
        }
    }

    /// <summary>
    /// Slices the target object and manages the resulting hulls.
    /// </summary>
    private void SliceObject(GameObject obj, Vector3 position, Vector3 normal)
    {
        // Perform the slice using the EzySlice library
        SlicedHull hull = obj.Slice(position, normal, crossSectionMaterial);

        if (hull != null)
        {
            // Create the two new parts
            GameObject upperPart = hull.CreateUpperHull(obj, crossSectionMaterial);
            GameObject lowerPart = hull.CreateLowerHull(obj, crossSectionMaterial);

            // Configure the main part (usually the lower one for a tree)
            SetupHull(lowerPart, false);
            lowerPart.layer = obj.layer; // Keep it choppable

            // Configure the "chip" (the piece that falls off)
            SetupHull(upperPart, true);

            // Inherit the original object's transform properties
            lowerPart.transform.position = obj.transform.position;
            lowerPart.transform.rotation = obj.transform.rotation;
            lowerPart.transform.localScale = obj.transform.localScale;

            upperPart.transform.position = obj.transform.position;
            upperPart.transform.rotation = obj.transform.rotation;
            upperPart.transform.localScale = obj.transform.localScale;

            // Destroy the original mesh to replace it with the sliced versions
            Destroy(obj);
        }
    }

    /// <summary>
    /// Adds necessary components to the new hulls.
    /// </summary>
    private void SetupHull(GameObject hullObj, bool isChip)
    {
        // hullObj.AddComponent<MeshCollider>().convex = true;
        if (isChip)
        {
            hullObj.AddComponent<MeshCollider>().convex = true;
            Rigidbody rb = hullObj.AddComponent<Rigidbody>();
            // Add some force to make the chip fly away
            rb.AddExplosionForce(cutForce, hullObj.transform.position, 1.0f, 1.0f, ForceMode.Impulse);
            // Auto-destroy the small chip after a while
            Destroy(hullObj, chipLifeTime);
        }
        else
        {
            hullObj.AddComponent<MeshCollider>();
            // The main part of the tree should stay static or be kinematic
            // until it's fully cut down
            // rb.isKinematic = true; 
        }
    }

    void OnDrawGizmos()
    {
        // Visualize the cutting plane in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - transform.up, transform.position + transform.up);
        Gizmos.DrawLine(transform.position - transform.right, transform.position + transform.right);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }
}