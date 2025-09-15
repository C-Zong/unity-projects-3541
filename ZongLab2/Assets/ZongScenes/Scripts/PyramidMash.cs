using UnityEngine;

// Generates a simple pyramid mesh and assigns it to the MeshFilter component.
public class PyramidMesh : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Pyramid";

        // Vertices
        Vector3[] vertices = {
            new Vector3(0, 1, 0),     // A
            new Vector3(-0.5f, 0, -0.5f), // B
            new Vector3(0.5f, 0, -0.5f),  // C
            new Vector3(0.5f, 0, 0.5f),   // D
            new Vector3(-0.5f, 0, 0.5f)   // E
        };

        // Triangles (defined clockwise/counter-clockwise)
        int[] triangles = {
            // Sides
            0,1,2,  // A-B-C
            0,2,3,  // A-C-D
            0,3,4,  // A-D-E
            0,4,1,  // A-E-B
            // Bottom
            3,2,1,  // D-C-B
            4,3,1   // E-D-B
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
