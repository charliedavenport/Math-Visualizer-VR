using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void Update() {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int i = 0;
        while (i < vertices.Length) {
            vertices[i] += normals[i] * Mathf.Sin(Time.time);
            i++;
        }
        mesh.vertices = vertices;
    }
}
