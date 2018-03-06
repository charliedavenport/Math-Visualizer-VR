﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class VectorField : MonoBehaviour {


    public int resolution = 10;
    public int x_min, z_min, y_min, x_max, z_max, y_max;
    public float scale_factor;
    public float start_size;

    private int n_vectors_x, n_vectors_z, n_vectors_y;
    private int n_vectors;

    private ParticleSystem.Particle[] vectors;
    private ParticleSystem ps;

    private delegate Vector3 VectorFunc(Vector3 input);

    // identity function: f(x,y,z) = (x,y,z) 
    private static Vector3 identity(Vector3 input) {
        return input;
    }

    private static Vector3 spiral_up(Vector3 input)
    {
        return new Vector3(
            Mathf.Cos(input.x),
            0.5f,
            Mathf.Sin(input.z));
    }

    private Vector3 normalize_pos(Vector3 pos)
    {
        return new Vector3(pos.x / Mathf.Max(Mathf.Abs(x_max), Mathf.Abs(x_min)) * scale_factor,
            pos.y / Mathf.Max(Mathf.Abs(y_max), Mathf.Abs(y_min)) * scale_factor,
            pos.z / Mathf.Max(Mathf.Abs(z_max), Mathf.Abs(z_min)) * scale_factor);
    }

    private void generate(VectorFunc func)
    {
        float x_val = x_min,
            z_val = z_min,
            y_val = y_min;
        float incr = 1f / resolution;
        int i = 0;
        for (int x = 0; x < n_vectors_x; x++)
        {
            y_val = y_min;
            for (int y = 0; y < n_vectors_y; y++)
            {
                z_val = z_min;
                for (int z = 0; z < n_vectors_z; z++)
                {
                    Vector3 input_vec = new Vector3(x_val, y_val, z_val);
                    vectors[i].position = normalize_pos(input_vec);
                    Vector3 return_vec = func(input_vec);
                    float theta = Mathf.Atan2(return_vec.x, return_vec.z) * Mathf.Rad2Deg;
                    float phi = Mathf.Atan2(return_vec.y, Mathf.Sqrt(return_vec.x * return_vec.x + return_vec.z * return_vec.z)) * Mathf.Rad2Deg;
                    //Debug.Log("phi " + phi + ", theta " + theta);
                    vectors[i].rotation3D = new Vector3(-phi, theta, 0);
                    float vec_size = start_size * Vector3.Magnitude(return_vec);
                    vectors[i].startSize3D = new Vector3(vec_size, vec_size, vec_size);
                    vectors[i].startColor = Color.HSVToRGB(Vector3.Magnitude(return_vec) / (2f * y_max), 1, 1); // normalizing color values based on twice the y_max value
                    z_val += incr;
                    i++;
                }

                y_val += incr;
            }
            x_val += incr;
        }

        ps.SetParticles(vectors, vectors.Length);
    }

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        n_vectors_x = (x_max - x_min) * resolution + 1;
        n_vectors_y = (y_max - y_min) * resolution + 1;
        n_vectors_z = (z_max - z_min) * resolution + 1;
        n_vectors = n_vectors_x * n_vectors_y * n_vectors_z;

        Debug.Log("n_vectors: " + n_vectors);

        vectors = new ParticleSystem.Particle[n_vectors];

        VectorFunc func = spiral_up;

        generate(func); // set particles

    }//Awake()

}
