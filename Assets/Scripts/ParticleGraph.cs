﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(ParticleSystem))]
public class ParticleGraph : MonoBehaviour {

    public int resolution = 10; // no. of particles between whole numbers
    public int x_min = -5,
        z_min = -5,
        y_min = -2,
        x_max = 5,
        z_max = 5,
        y_max = 2; // bounds on our graph axes

    public ParticleSystem ps;

    public ParticleSystem.Particle[] particles;

    public float spinRate;

    private int n_particles_x, n_particles_z;
    private int n_particles;

    public delegate float GraphFunc(float x, float z);


    static float sin_xz(float x, float z) {
        return Mathf.Sin(x * z);
    }
    static float xz(float x, float z) {
        return x * z;
    }
    static float cos_x_sin_z(float x, float z) {
        return Mathf.Cos(x) * Mathf.Sin(z);
    }
    static float normal_distr(float x, float z) {
        return Mathf.Exp(1 - x*x - z*z);
    }
    static float sphere_top(float x, float z) {
        return Mathf.Sqrt(1 - x * x - z * z);
    }
    static float sphere_bottom(float x, float z) {
        return -Mathf.Sqrt(4 - x * x - z * z);
    }


    void Awake () {
        ps = GetComponent<ParticleSystem>();

        n_particles_x = (x_max - x_min) * resolution;
        n_particles_z = (z_max - z_min) * resolution;

        n_particles = n_particles_x * n_particles_z;

        Debug.Log("n_particles: " + n_particles);

        particles = new ParticleSystem.Particle[n_particles * 2];

        float x_val = x_min;
        float z_val = z_min;
        float incr = 1f / resolution;

		GraphFunc func = sin_xz;
        //GraphFunc func_1 = sphere_bottom;

        // all this needs to happen on the GPU. oh boy
        // TODO: write vertex shader for computing positions of particles
        int i = 0;

            for (int z = 0; z < n_particles_z; z++) {
                x_val = x_min;
                for (int x = 0; x < n_particles_x; x++) {
                    particles[i].position = new Vector3(x_val / x_max, 
                        func(x_val, z_val) / y_max,
                        z_val / z_max);
                    if (particles[i].position.y < y_min || particles[i].position.y > y_max) {
                        particles[i].startColor = Color.clear; // particles outside of y-bounds are invisible
                    }
                    else particles[i].startColor =
                            Color.HSVToRGB((particles[i].position.y / 10) + 0.2f, 1, 1);
                    particles[i].startSize = 0.1f;
                    x_val += incr;
                    i++;
                }
                z_val += incr;
            }

        ps.SetParticles(particles, particles.Length);
    }//Awake()
	
	// Update is called once per frame
	void Update () {

        //perform a slow spinning
        ps.GetParticles(particles);
        for (int i=0; i<n_particles; i++) {
            Vector3 pos = particles[i].position;
            float x = pos.x;
            float z = pos.z;
            float theta = spinRate * Time.deltaTime;
            pos.x = x * Mathf.Cos(theta) - z * Mathf.Sin(theta);
            pos.z = z * Mathf.Cos(theta) + x * Mathf.Sin(theta);
            particles[i].position = pos;
        }
        ps.SetParticles(particles, particles.Length);
    }
}
