using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SkinnedMeshCombiner
{
    private static List<CombineInstance> combineInstances = new List<CombineInstance>();
    private static List<SkinnedMeshRenderer> smRenderers = new List<SkinnedMeshRenderer>();

    // Dictionaries to store combined data by material
    private static Dictionary<Material, List<CombineInstance>> materialToCombineInstances = new Dictionary<Material, List<CombineInstance>>();
    private static Dictionary<Material, List<BoneWeight>> materialToBoneWeights = new Dictionary<Material, List<BoneWeight>>();
    private static Dictionary<Material, List<Matrix4x4>> materialToBindPoses = new Dictionary<Material, List<Matrix4x4>>();
    private static Dictionary<Material, List<Transform>> materialToBones = new Dictionary<Material, List<Transform>>();


    public static List<GameObject>objecttodestroy = new List<GameObject>();
    public static SkinnedMeshRenderer CombineMeshes(GameObject o, List<SkinnedMeshRenderer> renderers, List<MeshFilter> meshFilters)
    {
        // Clear previous data
        combineInstances.Clear();
        materialToCombineInstances.Clear();
        materialToBoneWeights.Clear();
        materialToBindPoses.Clear();
        materialToBones.Clear();

        int boneOffset = 0;

        // Group skinned meshes by material
        foreach (var smr in renderers)
        {
            Material[] smrMaterials = smr.sharedMaterials;

            foreach (var material in smrMaterials)
            {
                if (!materialToCombineInstances.ContainsKey(material))
                {
                    materialToCombineInstances[material] = new List<CombineInstance>();
                    materialToBoneWeights[material] = new List<BoneWeight>();
                    materialToBindPoses[material] = new List<Matrix4x4>();
                    materialToBones[material] = new List<Transform>();
                }
            }

            // Process each submesh for the renderer
            for (int m = 0; m < smrMaterials.Length; m++)
            {
                Material material = smrMaterials[m];
                List<CombineInstance> instances = materialToCombineInstances[material];
                List<BoneWeight> weights = materialToBoneWeights[material];
                List<Matrix4x4> poses = materialToBindPoses[material];
                List<Transform> materialBones = materialToBones[material];

                BoneWeight[] meshBoneWeights = smr.sharedMesh.boneWeights;
                Vector3[] vertices = smr.sharedMesh.vertices;

                if (vertices.Length == 0)
                {
                    Debug.LogWarning($"Skipping {smr.name} because it has no vertices.");
                    continue;
                }

                int vertexOffset = weights.Count;  // Track the vertex offset for each mesh

                // Correctly assign bone weights relative to the combined mesh
                for (int i = 0; i < meshBoneWeights.Length; i++)
                {
                    BoneWeight bWeight = meshBoneWeights[i];
                    bWeight.boneIndex0 += boneOffset;
                    bWeight.boneIndex1 += boneOffset;
                    bWeight.boneIndex2 += boneOffset;
                    bWeight.boneIndex3 += boneOffset;
                    weights.Add(bWeight);
                }

                Transform[] smrBones = smr.bones;
                for (int i = 0; i < smrBones.Length; i++)
                {
                    materialBones.Add(smrBones[i]);
                    poses.Add(smr.sharedMesh.bindposes[i] * smr.transform.worldToLocalMatrix);
                }

                boneOffset += smrBones.Length;

                CombineInstance ci = new CombineInstance
                {
                    mesh = smr.sharedMesh,
                    subMeshIndex = m,
                    transform = smr.transform.localToWorldMatrix
                };
                instances.Add(ci);
            }

            //  GameObject.DestroyImmediate(smr.gameObject);
        }

        // Set up the SkinnedMeshRenderer for the combined mesh
        SkinnedMeshRenderer r = o.GetComponent<SkinnedMeshRenderer>();
        if (r == null)
        {
            r = o.AddComponent<SkinnedMeshRenderer>();
        }

        List<Material> finalMaterials = new List<Material>();
        List<Transform> finalBones = new List<Transform>();
        List<BoneWeight> finalBoneWeights = new List<BoneWeight>();
        List<Matrix4x4> finalBindPoses = new List<Matrix4x4>();

        foreach (var kvp in materialToCombineInstances)
        {
            Material mat = kvp.Key;
            List<CombineInstance> instances = kvp.Value;
            List<BoneWeight> weights = materialToBoneWeights[mat];
            List<Matrix4x4> poses = materialToBindPoses[mat];
            List<Transform> materialBones = materialToBones[mat];

            // Check if there are instances to combine
            if (instances.Count == 0)
            {
                Debug.LogWarning($"No meshes to combine for material {mat.name}.");
                continue;
            }

            // Create a combined mesh for the material
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(instances.ToArray(), true, false);  // Combine without merging submeshes to keep bone weights accurate

            // Ensure bone weights and bind poses match vertex count
            if (weights.Count != combinedMesh.vertexCount)
            {
                Debug.LogError($"Mismatch between bone weights count ({weights.Count}) and vertex count ({combinedMesh.vertexCount}) for material {mat.name}.");
                continue;
            }

            combinedMesh.boneWeights = weights.ToArray();
            combinedMesh.bindposes = poses.ToArray();

            CombineInstance finalCombineInstance = new CombineInstance
            {
                mesh = combinedMesh,
                transform = Matrix4x4.identity
            };

            combineInstances.Add(finalCombineInstance);

            finalMaterials.Add(mat);
            finalBoneWeights.AddRange(weights);
            finalBones.AddRange(materialBones);
            finalBindPoses.AddRange(poses);
        }

        // Combine all material meshes into one final mesh
        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(combineInstances.ToArray(), false, false);  // Combine into one mesh without merging submeshes

        // Ensure bone weights count matches the vertex count of the final mesh
        if (finalBoneWeights.Count != finalMesh.vertexCount)
        {
            Debug.LogError($"Final mismatch between bone weights count ({finalBoneWeights.Count}) and vertex count ({finalMesh.vertexCount}).");
            return null;
        }

        finalMesh.boneWeights = finalBoneWeights.ToArray();
        finalMesh.bindposes = finalBindPoses.ToArray();
        finalMesh.RecalculateBounds();

        r.sharedMesh = finalMesh;
        r.sharedMaterials = finalMaterials.ToArray();
        r.bones = finalBones.ToArray();

        // Clear resources to prevent memory leaks
        combineInstances.Clear();
        materialToCombineInstances.Clear();
        materialToBoneWeights.Clear();
        materialToBindPoses.Clear();
        materialToBones.Clear();

        return r;
    }

    public static void CombineMeshes(GameObject o)
    {
        smRenderers.Clear();
        o.GetComponentsInChildren<SkinnedMeshRenderer>(true, smRenderers);
        CombineMeshes(o, smRenderers, null);
    }


    public static void CombineAndStitchMeshes(GameObject parentObject, Transform rootBone)
    {
        if (parentObject == null)
        {
            Debug.LogError("Parent object is null. Cannot combine meshes.");
            return;
        }

        // Get all SkinnedMeshRenderers in children
        SkinnedMeshRenderer[] skinnedMeshes = parentObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (skinnedMeshes.Length == 0)
        {
            Debug.LogWarning("No SkinnedMeshRenderers found in children of the parent object.");
            return;
        }

        // Lists to hold combined mesh data
        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Material> combinedMaterials = new List<Material>();
        List<Transform> bones = new List<Transform>();
        List<Matrix4x4> bindPoses = new List<Matrix4x4>();
        List<BoneWeight> boneWeights = new List<BoneWeight>();

        int boneOffset = 0;
        int vertexOffset = 0;

        List<Vector3> combinedVertices = new List<Vector3>();
        List<Vector3> combinedNormals = new List<Vector3>();
        List<Vector4> combinedTangents = new List<Vector4>();
        List<Vector2> combinedUVs = new List<Vector2>();  // Preserve UVs for correct rendering
        List<int[]> subMeshTriangles = new List<int[]>(); // Store triangles for each sub-mesh

        foreach (var smr in skinnedMeshes)
        {
            Mesh mesh = smr.sharedMesh;

            if (mesh == null)
                continue;

            // Add mesh vertices, normals, tangents, and UVs to combined lists
            combinedVertices.AddRange(mesh.vertices);
            combinedNormals.AddRange(mesh.normals);
            combinedTangents.AddRange(mesh.tangents);
            combinedUVs.AddRange(mesh.uv);

            // Combine instance for mesh
            CombineInstance combineInstance = new CombineInstance
            {
                mesh = mesh,
                transform = smr.transform.localToWorldMatrix
            };
            combineInstances.Add(combineInstance);

            // Collect materials (deep copy to preserve material properties)
            foreach (var mat in smr.materials)
            {
                Material newMat = new Material(mat);
                newMat.CopyPropertiesFromMaterial(mat);  // Preserve all material properties
                combinedMaterials.Add(newMat);
            }

            // Collect sub-mesh triangles for material assignment
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] triangles = mesh.GetTriangles(i);
                for (int j = 0; j < triangles.Length; j++)
                {
                    triangles[j] += vertexOffset; // Adjust vertex indices
                }
                subMeshTriangles.Add(triangles);
            }

            // Collect bones and bind poses
            foreach (Transform bone in smr.bones)
            {
                bones.Add(bone);
            }

            foreach (Matrix4x4 bindPose in mesh.bindposes)
            {
                bindPoses.Add(bindPose);
            }

            // Collect bone weights and adjust indices based on the current offset
            foreach (BoneWeight bw in mesh.boneWeights)
            {
                BoneWeight adjustedWeight = bw;
                adjustedWeight.boneIndex0 += boneOffset;
                adjustedWeight.boneIndex1 += boneOffset;
                adjustedWeight.boneIndex2 += boneOffset;
                adjustedWeight.boneIndex3 += boneOffset;
                boneWeights.Add(adjustedWeight);
            }

            boneOffset += smr.bones.Length;
            vertexOffset += mesh.vertexCount;
        }

        // Create the new combined mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.vertices = combinedVertices.ToArray();
        combinedMesh.normals = combinedNormals.ToArray();
        combinedMesh.tangents = combinedTangents.ToArray();
        combinedMesh.uv = combinedUVs.ToArray();
        combinedMesh.boneWeights = boneWeights.ToArray();
        combinedMesh.bindposes = bindPoses.ToArray();

        // Assign triangles to each sub-mesh for proper material assignment
        combinedMesh.subMeshCount = subMeshTriangles.Count;
        for (int i = 0; i < subMeshTriangles.Count; i++)
        {
            combinedMesh.SetTriangles(subMeshTriangles[i], i);
        }

        // Create a new GameObject for the combined SkinnedMeshRenderer
        GameObject combinedObject = new GameObject("CombinedMesh");
       

        // Set up the new SkinnedMeshRenderer
        SkinnedMeshRenderer newSkinnedMeshRenderer = combinedObject.AddComponent<SkinnedMeshRenderer>();
        newSkinnedMeshRenderer.sharedMesh = combinedMesh;
        newSkinnedMeshRenderer.materials = combinedMaterials.ToArray();
        newSkinnedMeshRenderer.bones = bones.ToArray();
        newSkinnedMeshRenderer.rootBone = rootBone; // Set root bone to the passed rootBone

        // Ensure SkinnedMeshRenderer updates with the animator
        newSkinnedMeshRenderer.updateWhenOffscreen = true; // Allows proper updating when not in view
        SkinnedMeshRenderer rends = skinnedMeshes[0];
        // Disable only the SkinnedMeshRenderer components of original objects, keep the GameObjects active
        foreach (var smr in skinnedMeshes)
        {
            if (rends != null)
            {
                if (smr.sharedMesh.vertexCount < rends.sharedMesh.vertexCount) {
                    rends = smr;
                };
            }
            else
            {
                if (smr != null) continue; else rends = smr;
            }
        
            smr.enabled = false; // Disable only the SkinnedMeshRenderer component

        }
        rends.enabled = true;
       // combinedObject.isStatic = true;
        // Use the Stitcher class to stitch the combined SkinnedMeshRenderer to the original skeleton
        Stitcher stitcher = new Stitcher();
        stitcher.Stitch(combinedObject, parentObject);
        objecttodestroy.Add(combinedObject);
        Debug.Log("Meshes combined and stitched successfully with correct bone weights, material assignment, and skeleton binding!");
    }
}
