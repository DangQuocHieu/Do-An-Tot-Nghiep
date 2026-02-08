using UnityEngine;

public class SliceableObject : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public MeshFilter MeshFilter;
    public MeshCollider MeshCollider;
    public Rigidbody Rb;

    public void ReplaceMesh(Mesh mesh, Material[] mats)
    {
        if(MeshRenderer == null) MeshRenderer = GetComponent<MeshRenderer>();
        if(MeshFilter == null) MeshFilter = GetComponent<MeshFilter>(); 
        if(MeshCollider == null) MeshCollider = GetComponent<MeshCollider>();

        MeshFilter.mesh = mesh;
        MeshRenderer.materials = mats;
        MeshCollider.sharedMesh = mesh;
        MeshCollider.convex = true;

    }

}
