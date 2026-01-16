using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using System.Collections.Generic;

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
    [SerializeField] private float cutForce = 3.0f;
    [SerializeField] private float chipLifeTime = 4.0f;
    [SerializeField] private float notchSize = 0.5f; // How deep the "L" cut goes
    [SerializeField] private float maxReach = 1.0f;

    [Header("Debug Visualization")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private float gizmoPlaneSize = 0.5f;

    public GameObject target;

    private Quaternion cutRotation = Quaternion.Euler(0, 0, 20);

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
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxReach, choppableLayer))
        {
            // GameObject target = hit.collider.gameObject;

            // 1. Define the first plane (Horizontal cut)
            Vector3 firstPlanePos = hit.point;
            Vector3 firstPlaneNormal = transform.up;

            // 2. Define the second plane (Vertical cut to "exit" the mesh)
            // We shift the second plane deep into the mesh to create the corner
            Vector3 secondPlanePos = hit.point;// + (transform.forward * notchSize);
            // Vector3 secondPlaneNormal = transform.right;
            Vector3 secondPlaneNormal = cutRotation * transform.up;

            ExecuteDoubleSlice(target, firstPlanePos, firstPlaneNormal, secondPlanePos, secondPlaneNormal);
        }
    }

    private void ExecuteDoubleSlice(GameObject obj, Vector3 p1, Vector3 n1, Vector3 p2, Vector3 n2)
    {
        Debug.Log("Chopping object: " + obj.name);

        // FIRST SLICE: Divide the object into Top and Bottom
        SlicedHull firstHull = obj.Slice(p1, n1, crossSectionMaterial);

        if (firstHull != null)
        {
            Debug.Log("First slice successful.");
            
            GameObject lowerPart = firstHull.CreateLowerHull(obj, crossSectionMaterial);
            GameObject upperPart = firstHull.CreateUpperHull(obj, crossSectionMaterial);

            Debug.Log("Created upper and lower hulls.");
            // SetupHull(upperPart, false, obj);

            Debug.Log("Performing second slice on upper part.");
            SlicedHull secondHull = upperPart.Slice(p2, n2, crossSectionMaterial);

            if (secondHull != null)
            {
                Debug.Log("Second slice successful.");
                GameObject chipPart = secondHull.CreateLowerHull(upperPart, crossSectionMaterial);
                GameObject remainingUpperPart = secondHull.CreateUpperHull(upperPart, crossSectionMaterial);

                SetupHull(lowerPart, false, obj);
                SetupHull(remainingUpperPart, false, obj);
                SetupHull(chipPart, true, obj);

                Destroy(upperPart);
                Destroy(obj);

                CombineHulls(lowerPart, remainingUpperPart, obj);
            }
            else
            {
                Debug.Log("Second slice failed.");
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

        if (isChip)
        {
            // The small piece flies away
            Rigidbody rb = hullObj.AddComponent<Rigidbody>();
            rb.AddExplosionForce(cutForce, hullObj.transform.position, 2.0f, 1.0f, ForceMode.Impulse);
            Destroy(hullObj, chipLifeTime);
        }
        else
        {
            // The parts of the tree/object stay in place
            // rb.isKinematic = true;
        }
    }

    /// <summary>
    /// Merges two GameObjects into a single Mesh while preserving materials.
    /// </summary>
    private GameObject CombineHulls(GameObject partA, GameObject partB, GameObject original)
    {
        GameObject combinedObj = new GameObject(original.name);
        combinedObj.transform.position = original.transform.position;
        combinedObj.transform.rotation = original.transform.rotation;
        combinedObj.transform.localScale = original.transform.localScale;
        combinedObj.layer = original.layer;

        MeshFilter[] filters = { partA.GetComponent<MeshFilter>(), partB.GetComponent<MeshFilter>() };
        MeshRenderer[] renderers = { partA.GetComponent<MeshRenderer>(), partB.GetComponent<MeshRenderer>() };
        
        // We use the materials from partA as the reference (Original + Cross-section)
        Material[] sharedMaterials = renderers[0].sharedMaterials;
        int subMeshCount = sharedMaterials.Length;
        
        CombineInstance[] finalSubmeshCombine = new CombineInstance[subMeshCount];
        
        // Iterate through each material slot (submesh)
        for (int m = 0; m < subMeshCount; m++)
        {
            List<CombineInstance> subMeshGeometry = new List<CombineInstance>();
            
            for (int i = 0; i < filters.Length; i++)
            {
                Mesh mesh = filters[i].sharedMesh;
                // If the part actually has geometry for this material slot
                if (m < mesh.subMeshCount)
                {
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = mesh;
                    ci.subMeshIndex = m; // Extract ONLY this material's geometry
                    ci.transform = combinedObj.transform.worldToLocalMatrix * filters[i].transform.localToWorldMatrix;
                    subMeshGeometry.Add(ci);
                }
            }
            
            // Create a temporary mesh that merges all geometry for THIS specific material
            Mesh mergedSubMesh = new Mesh();
            mergedSubMesh.CombineMeshes(subMeshGeometry.ToArray(), true); // Merge into one single submesh
            
            finalSubmeshCombine[m].mesh = mergedSubMesh;
            finalSubmeshCombine[m].transform = Matrix4x4.identity;
        }

        // Final step: combine the individual material-specific meshes back into one multi-submesh mesh
        Mesh finalMesh = new Mesh();
        finalMesh.name = original.name;
        finalMesh.CombineMeshes(finalSubmeshCombine, false); // Keep as separate submeshes (1 per material)
        
        combinedObj.AddComponent<MeshFilter>().mesh = finalMesh;
        combinedObj.AddComponent<MeshRenderer>().sharedMaterials = sharedMaterials;

        combinedObj.AddComponent<MeshCollider>();

        this.target = combinedObj;

        Destroy(partA);
        Destroy(partB);

        return combinedObj;
    }


    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // Origin point for the visualization (simulating a hit point at a fixed distance if not hitting anything)
        Vector3 previewPos = transform.position + (transform.forward * 1.5f);
        
        // Raycast check to snap Gizmos to actual surface if possible
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxReach, choppableLayer))
        {
            previewPos = hit.point;
        }

        // Draw First Plane (Horizontal)
        Gizmos.color = new Color(0, 1, 1, 0.2f); // Cyan semi-transparent
        DrawPlaneGizmo(previewPos, transform.up);

        // Draw Second Plane (Vertical/Exit)
        Gizmos.color = new Color(1, 0, 1, 0.2f); // Magenta semi-transparent
        
        Vector3 secondPos = previewPos;

        var secondNormal = cutRotation * transform.up;

        DrawPlaneGizmo(secondPos, secondNormal);
    }
    
    private void DrawPlaneGizmo(Vector3 position, Vector3 normal)
    {
        Matrix4x4 rotationMatrix = Matrix4x4.LookAt(position, position + normal, Vector3.up);
        
        // We rotate it to be flat against the normal
        if (normal == transform.forward) rotationMatrix = Matrix4x4.LookAt(position, position + normal, Vector3.up);
        else rotationMatrix = Matrix4x4.LookAt(position, position + normal, transform.forward);

        Gizmos.matrix = rotationMatrix;
        // Gizmos.DrawWireCube(Vector3.zero, new Vector3(gizmoPlaneSize, gizmoPlaneSize, 0.005f));
        
        Vector3 center = new Vector3(0 + gizmoPlaneSize / 8, 0, 0);

        // Gizmos.DrawCube(Vector3.zero, new Vector3(gizmoPlaneSize / 2, gizmoPlaneSize / 2, 0.005f));
        Gizmos.DrawCube(center, new Vector3(gizmoPlaneSize / 4, gizmoPlaneSize / 2, 0.005f));
        Gizmos.matrix = Matrix4x4.identity;
    }
}