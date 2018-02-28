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

    private Vector3 normalize_pos(Vector3 pos)
    {
        return new Vector3(pos.x / Mathf.Max(Mathf.Abs(x_max), Mathf.Abs(x_min)) * scale_factor,
            pos.y / Mathf.Max(Mathf.Abs(y_max), Mathf.Abs(y_min)) * scale_factor,
            pos.z / Mathf.Max(Mathf.Abs(z_max), Mathf.Abs(z_min)) * scale_factor);
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

        int i = 0;
        for (int x = 0; x < n_vectors_x; x++)
        {
            y_val = y_min;
            for(int y = 0; y < n_vectors_y; y++)
            {
                z_val = z_min;
                for (int z = 0; z < n_vectors_z; z++)
                {
                    vectors[i].position = normalize_pos(new Vector3(x_val, y_val, z_val));
                    vectors[i].startSize = .5f;
                    z_val += incr;
                    i++;
                }

                y_val += incr;
            }
            x_val += incr;
        }

        ps.SetParticles(vectors, vectors.Length);

    }//Awake()

    // Update is called once per frame
    void Update () {
		
	}
}
