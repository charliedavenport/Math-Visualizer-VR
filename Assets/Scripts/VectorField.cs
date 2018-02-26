using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class VectorField : MonoBehaviour {


    public int resolution = 10;
    public int x_min, z_min, y_min, x_max, z_max, y_max;
    public float scale_factor;

    private int n_vectors_x, n_vectors_z, n_vectors_y;
    private int n_vectors;

    private ParticleSystem.Particle[] vectors;
    private ParticleSystem ps;

    private delegate Vector3 VectorFunc(Vector3 input);

    // identity function: f(x) = x 
    private static Vector3 identity(Vector3 input) {
        return input;
    }

    private void Awake() {
        ps = GetComponent<ParticleSystem>();

        n_vectors_x = (x_max - x_min) * resolution;
        n_vectors_y = (y_max - y_min) * resolution;
        n_vectors_z = (z_max - z_min) * resolution;
        n_vectors = n_vectors_x * n_vectors_y * n_vectors_z;

        Debug.Log("n_vectors: " + n_vectors);

        vectors = new ParticleSystem.Particle[n_vectors];

        float x_val = x_min,
            z_val = z_min,
            y_val = y_min;
        float incr = 1f / resolution;

        VectorFunc func = identity;


    }

    // Update is called once per frame
    void Update () {
		
	}
}
