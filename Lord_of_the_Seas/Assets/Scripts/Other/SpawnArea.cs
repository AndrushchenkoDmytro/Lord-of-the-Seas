using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]

public class SpawnArea : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Mesh mesh;
    private LineRenderer lineRenderer;

    [SerializeField] public Vector3[] verticesPos = new Vector3[4];

    [SerializeField] private Side side;

    bool selected = false;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        lineRenderer = GetComponent<LineRenderer>();

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];

        for (int i = 0; i < 4; i++)
        {
            vertices[i] = verticesPos[i];
        }

        lineRenderer.SetPosition(0, verticesPos[0]);
        lineRenderer.SetPosition(1, verticesPos[1]);
        lineRenderer.SetPosition(2, verticesPos[3]);
        lineRenderer.SetPosition(3, verticesPos[2]);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 1;
        triangles[4] = 3;
        triangles[5] = 2;

        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        meshRenderer.enabled = false;
        //lineRenderer.enabled = false;
    }

    void Start()
    {
        
    }

    /*public void ExpandArea()
    {
        if(side == Side.Player)
        {
            Vector3[] vertices = mesh.vertices;
            vertices[2].z += verticesPos[2].z;
            vertices[3].z += verticesPos[3].z;
            mesh.vertices = vertices;
            lineRenderer.SetPosition(2, vertices[3]);
            lineRenderer.SetPosition(3, vertices[2]);
        }
        else
        {
            Vector3[] vertices = mesh.vertices;
            vertices[2].z += verticesPos[2].z;
            vertices[3].z += verticesPos[3].z;
            mesh.vertices = vertices;
            lineRenderer.SetPosition(2, vertices[3]);
            lineRenderer.SetPosition(3, vertices[2]);
        }
        mesh.RecalculateBounds();
        meshCollider.sharedMesh = mesh;
    }
    public void ReducArea()
    {
        if (side == Side.Player)
        {
            Vector3[] vertices = mesh.vertices;
            vertices[2].z -= verticesPos[2].z;
            vertices[3].z -= verticesPos[3].z;
            mesh.vertices = vertices;
            lineRenderer.SetPosition(2, vertices[3]);
            lineRenderer.SetPosition(3, vertices[2]);
        }
        else
        {
            Vector3[] vertices = mesh.vertices;
            vertices[2].z -= verticesPos[2].z;
            vertices[3].z -= verticesPos[3].z;
            mesh.vertices = vertices;
            lineRenderer.SetPosition(2, vertices[3]);
            lineRenderer.SetPosition(3, vertices[2]);
        }
        mesh.RecalculateBounds();
        meshCollider.sharedMesh = mesh;
    }*/

    public void SelectSpawnArea()
    {
        if(selected == false)
        {
            meshRenderer.enabled = true;
            selected = true;
        }
    }

    public void DeselectSpawnArea()
    {
        if(selected == true)
        {
            meshRenderer.enabled = false;
            selected = false;
        }
    }
}
