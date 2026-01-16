using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EzySlice;

public class CuttableObject : MonoBehaviour
{
    public CuttingPositions cuttingPositions;

    public List<Vector3> possibleCuts = new List<Vector3>();

    private Collider _collider;

    public Material cutMaterial;

    private Vector3 currentCutStart;

    private Vector3 angledMiddleVector;
    private Vector3 cornerPos;

    private Vector3 cutPositivePos;
    private Vector3 cutNegativePos;

    private Vector3 normalPositive;
    private Vector3 normalNegative;

    private float cutForce = 1.5f;
    private float chipLifeTime = 4.0f;

    void OnValidate()
    {
        _collider = GetComponent<Collider>();
        UpdatePositions();
    }

    void Awake()
    {
        _collider = GetComponent<Collider>();
        UpdatePositions();
    }

    void UpdatePositions() 
    {
        if (cuttingPositions == null) return;
        possibleCuts = new List<Vector3>(cuttingPositions.positions);
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
        if (possibleCuts.Count == 0) return;

        int randomIndex = Random.Range(0, possibleCuts.Count);
        currentCutStart = possibleCuts[randomIndex];

        // 1. Calcul de la direction vers l'intérieur (basé sur le signe de X)
        float horizontalStep = 0.2f;
        Vector3 inwardDir = new Vector3(-Mathf.Sign(currentCutStart.x), 0, 0);
        Vector3 baseDepthVector = inwardDir * horizontalStep;

        // 2. Calcul du sommet (Corner) avec une inclinaison aléatoire (+/- 5 à 15 degrés)
        float randAngle = Random.Range(5f, 30f) * (Random.value > 0.5f ? 1f : -1f);
        Quaternion tilt = Quaternion.Euler(0, 0, randAngle);
    
        cornerPos = currentCutStart + (tilt * baseDepthVector);

        // 3. Calcul des points d'évasement (Positive/Negative) depuis le sommet
        // On repart du sommet vers le point de départ avec un angle de +/- 15 degrés
        Vector3 returnDir = currentCutStart - cornerPos;
        
        cutPositivePos = cornerPos + (Quaternion.Euler(0, 0, 15f) * returnDir);
        cutNegativePos = cornerPos + (Quaternion.Euler(0, 0, -15f) * returnDir);

        // 4. CALCUL DES NORMALES
        // On crée les vecteurs de segments
        Vector3 dirPos = (cutPositivePos - cornerPos).normalized;
        Vector3 dirNeg = (cutNegativePos - cornerPos).normalized;

        // Pour un plan XY, la normale est perpendiculaire au segment et à l'axe Z.
        // On utilise Vector3.forward (0,0,1) comme axe de référence.
        // L'ordre (A, B) vs (B, A) dans le Cross change le sens de la normale.
        normalPositive = Vector3.Cross(dirPos, Vector3.forward).normalized;
        normalNegative = Vector3.Cross(Vector3.forward, dirNeg).normalized;

        Vector3 midPos = Vector3.Lerp(cornerPos, cutPositivePos, 0.5f);
        Vector3 midNeg = Vector3.Lerp(cornerPos, cutNegativePos, 0.5f);

        possibleCuts.RemoveAt(randomIndex);

        ExecuteDoubleSlice(
            transform.TransformPoint(midPos), 
            transform.TransformDirection(normalPositive), 
            transform.TransformPoint(midNeg), 
            transform.TransformDirection(normalNegative)
        );
        
        Debug.Log($"Coupe générée à {currentCutStart}. Points restants : {possibleCuts.Count}");
    }

    public void ExecuteDoubleSlice(Vector3 p1, Vector3 n1, Vector3 p2, Vector3 n2)
    {
        Debug.Log("Chopping object: " + gameObject.name);

        // FIRST SLICE: Divide the object into Top and Bottom
        SlicedHull firstHull = gameObject.Slice(p1, n1, cutMaterial);

        if (firstHull != null)
        {
            Debug.Log("First slice successful.");
            
            GameObject lowerPart = firstHull.CreateLowerHull(gameObject, cutMaterial);
            GameObject upperPart = firstHull.CreateUpperHull(gameObject, cutMaterial);

            Debug.Log("Created upper and lower hulls.");
            SetupHull(upperPart, false, gameObject);
            Debug.Log("Performing second slice on upper part.");
            SlicedHull secondHull = upperPart.Slice(p2, n2, cutMaterial);

            if (secondHull != null)
            {
                Debug.Log("Second slice successful.");
                GameObject chipPart = secondHull.CreateUpperHull(upperPart, cutMaterial);
                GameObject lower2 = secondHull.CreateLowerHull(upperPart, cutMaterial);

                SetupHull(lowerPart, false, gameObject);
                SetupHull(lower2, false, gameObject);
                SetupHull(chipPart, true, gameObject);

                Destroy(upperPart);
            
                var combinedObj = CombineHulls(lowerPart, lower2, gameObject);
                combinedObj.transform.SetParent(transform.parent);

                var combinedObjCuttable = combinedObj.AddComponent<CuttableObject>();
                combinedObjCuttable.cuttingPositions = ScriptableObject.CreateInstance<CuttingPositions>();
                combinedObjCuttable.cuttingPositions.positions = new List<Vector3>(possibleCuts);
                combinedObjCuttable.cutMaterial = cutMaterial;
                combinedObjCuttable.UpdatePositions();
                
                // combinedObjCuttable.SetActive(true);

                Destroy(gameObject);
            }
            // else
            // {
            //     Debug.Log("Second slice failed.");
            //     SetupHull(lowerPart, false, obj);
            //     SetupHull(upperPart, true, obj);
            //     Destroy(obj);
            // }
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
            // hullObj.AddComponent<MeshCollider>().convex = true;


            rb.AddExplosionForce(cutForce, cornerPos, 2.0f, 1.0f, ForceMode.Impulse);
            // rb.AddExplosionForce()
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

        // this.target = combinedObj;

        GameObject.Destroy(partA);
        GameObject.Destroy(partB);

        return combinedObj;
    }

    // void OnDrawGizmos()
    // {
    //     if (possibleCuts == null)
    //         return;


    //     if (_collider != null)
    //     {
    //         Gizmos.color = Color.blue;
    //         Gizmos.DrawSphere(_collider.bounds.center, 0.025f);
    //     }



    //     Gizmos.matrix = transform.localToWorldMatrix;

    //     Gizmos.color = Color.red;
    //     foreach (var pos in possibleCuts)
    //     {
    //         Gizmos.DrawSphere(pos, 0.025f);
    //     }

    //     if (currentCutStart != null && currentCutStart != Vector3.zero && cornerPos != null && cornerPos != Vector3.zero)
    //     {
    //         Gizmos.color = Color.yellow;
    //         Gizmos.DrawLine(currentCutStart, cornerPos);
    //     }

    //     if (currentCutStart != null && angledMiddleVector != null)
    //     {
    //         Gizmos.color = Color.magenta;
    //         Gizmos.DrawLine(currentCutStart, currentCutStart + angledMiddleVector);
    //     }

    //     if (cornerPos != null && cornerPos != Vector3.zero)
    //     {
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawSphere(cornerPos, 0.02f);
    //     }

    //     if (cornerPos != null && cornerPos != Vector3.zero && 
    //     cutNegativePos != null && cutNegativePos != Vector3.zero && 
    //     cutPositivePos != null && cutPositivePos != Vector3.zero)
    //     {
    //         Gizmos.color = Color.cyan;
    //         Gizmos.DrawLine(cornerPos, cutPositivePos);
    //         Gizmos.DrawLine(cornerPos, cutNegativePos);
            
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawSphere(cutPositivePos, 0.02f);
    //         Gizmos.DrawSphere(cutNegativePos, 0.02f);
    //     }

    //     // Normale du segment Positif (part du milieu du segment)
    //     if (normalPositive != Vector3.zero)
    //     {
    //         Vector3 midPos = Vector3.Lerp(cornerPos, cutPositivePos, 0.5f);
    //         Gizmos.DrawRay(midPos, normalPositive * 0.1f);
    //         // Petite sphère pour indiquer le bout de la normale
    //         Gizmos.DrawSphere(midPos + normalPositive * 0.1f, 0.005f);

    //         // DrawPlaneGizmo(midPos, normalPositive);
    //         // Gizmos.DrawCube(midPos)
    //     }

    //     // Normale du segment Négatif (part du milieu du segment)
    //     if (normalNegative != Vector3.zero)
    //     {
    //         Vector3 midNeg = Vector3.Lerp(cornerPos, cutNegativePos, 0.5f);
    //         Gizmos.DrawRay(midNeg, normalNegative * 0.1f);
    //         // Petite sphère pour indiquer le bout de la normale
    //         Gizmos.DrawSphere(midNeg + normalNegative * 0.1f, 0.005f);

    //         // DrawPlaneGizmo(midNeg, normalNegative);
    //     }

    //     Gizmos.matrix = Matrix4x4.identity;
    // }


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
        Vector3 size = new Vector3(gizmoPlaneSize, gizmoPlaneSize, 0.005f);
        Vector3 offset = new Vector3(gizmoPlaneSize, 0, 0);
        Gizmos.DrawCube(offset, size);

        // Revert to object matrix for next calls
        Gizmos.matrix = objectMatrix;
    }
}
