using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SurfaceMesh : MonoBehaviour {

    public MeshFilter mf;
    public MeshRenderer mr;

    public Material mat;

    public List<Vector3> vertices;
    public int[] indices;
    public List<Vector3> normals;

    public float size_x = 1.0f; // scale (x_min, x_max) to (0, size.x)
    public float size_y = 1.0f; // scale (y_min, y_max) to (0, size.y) --> normalize

    public int x_min = 0;
    public int x_max = 5;
    public int y_min = 0;
    public int y_max = 5; // for axes
    public int resolution = 1; // no. of 'ticks' per whole number on the axis

    private Mesh mesh;

    private int n_quads;
    private int n_vertices;

    void Awake() {
        StartCoroutine(Generate());
    }



    private IEnumerator Generate() {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();
        mr.sharedMaterial = mat;

        mesh = new Mesh();
        mf.mesh = mesh;
        //vertices = mesh.vertices;

        //mf.mesh = mesh;

        // create a vertex for every point in our coordinate system
        int n_vertices_x = (x_max - x_min) * resolution;
        int n_vertices_y = (y_max - y_min) * resolution;
        n_vertices = n_vertices_x * n_vertices_y;
        vertices = new List<Vector3>();
        int i = 0;
        float y_val = y_min;
        float x_val = x_min;
        float incr = 1f / (float)resolution;
        for (int y = 0; y < n_vertices_y; y++) {
            x_val = x_min;
            for (int x = 0; x < n_vertices_x; x++) {
                // y coordinate starts at zero for every vertex in a flat plane
                vertices.Insert(i, new Vector3(x_val, 0f, y_val));
                i++; // increment index
                     // increment position values for our index

                x_val += incr;

            }
            y_val += incr;
        }

        //set indices
        n_quads = (n_vertices_x - 1) * (n_vertices_y - 1);
        indices = new int[n_quads * 4]; // 4 indices per quad
        i = 0;
        for (int y = 0; y < (n_vertices_y - 1); y++) {
            for (int x = 0; x < (n_vertices_x - 1); x++) {
                indices[i] = (y * n_vertices_y) + x;           // bottom-left
                indices[i + 1] = ((y + 1) * n_vertices_y) + x;     // top-left
                indices[i + 2] = ((y + 1) * n_vertices_y) + x + 1; // top-right
                indices[i + 3] = (y * n_vertices_y) + x + 1;       // bottom-right
                i += 4; // next quad in array
            }
        }

        // set all normals to up vector
        normals = new List<Vector3>();
        for (i = 0; i < n_vertices; i++) {
            normals.Insert(i, Vector3.up); // all normals pointing up to start
        }

        // apply function to vertices
        List<Vector3> vertices_xy = function_xy(vertices);
        List<Vector3> vertices_sin_xy = function_sin_xy(vertices);

        mesh.SetVertices(vertices_sin_xy);
        mesh.SetNormals(normals);
        mesh.SetIndices(indices, MeshTopology.Quads, 0);

        mf.mesh = mesh;
        yield return null;
    }



    /*
     * Returns vertices with the y-coord set to x * z
     * y in math coordinates is z in unity coordinates
    */
    List<Vector3> function_xy(List<Vector3> vertices) {
        int len = vertices.Count;
        List<Vector3> new_vert = new List<Vector3>();        
        for (int i=0; i<len; i++) {
            float y_val = vertices[i].x * vertices[i].z;
            y_val /= 10f;
            new_vert.Add(new Vector3(vertices[i].x, y_val, vertices[i].z));
        }
        return new_vert;
    }

    List<Vector3> function_sin_xy(List<Vector3> vertices) {
        int len = vertices.Count;
        List<Vector3> new_vert = new List<Vector3>();
        for (int i = 0; i < len; i++) {
            float y_val = Mathf.Sin(vertices[i].x * vertices[i].z);
            y_val /= 10f;
            new_vert.Add(new Vector3(vertices[i].x, y_val, vertices[i].z));
        }
        return new_vert;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

