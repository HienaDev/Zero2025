using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DeformableQuad : MonoBehaviour
{
    [SerializeField] private GameObject[] pointsObject = new GameObject[4]; // Assign 4 empty GameObjects in the inspector
    public Vector3[] points = new Vector3[4]; // 4 corners (local space)
    private Mesh mesh;

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Initialize points in a square
        points[0] = pointsObject[0].transform.position; // bottom-left
        points[1] = pointsObject[1].transform.position;  // bottom-right
        points[2] = pointsObject[2].transform.position;  // top-left
        points[3] = pointsObject[3].transform.position;   // top-right

        UpdateMesh();
    }

    void Update()
    {
        UpdateMesh(); // update every frame (if points move)
    }

    void UpdateMesh()
    {
        mesh.Clear();

        points[0] = pointsObject[0].transform.position; // bottom-left
        points[1] = pointsObject[1].transform.position;  // bottom-right
        points[2] = pointsObject[2].transform.position;  // top-left
        points[3] = pointsObject[3].transform.position;   // top-right

        // Set vertices
        mesh.vertices = new Vector3[]
        {
            points[0],
            points[1],
            points[2],
            points[3]
        };

        // Define triangles (two per quad)
        mesh.triangles = new int[]
        {
            0, 2, 1, // first triangle
            2, 3, 1  // second triangle
        };

        // Simple UV mapping
        mesh.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
