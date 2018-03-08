using System.Collections;
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

    public float graph_scale_factor;

    public ParticleSystem ps;

    public ParticleSystem.Particle[] particles;

    public float spinRate;

    public float start_size;

    private int n_particles_x, n_particles_z;
    private int n_particles;

    public delegate float GraphFunc(float x, float z);
    public GraphFunc current_func;
	public List<GraphFunc> functions;
	public List<string> function_names;
    public List<string> function_descriptions;
	public int current_func_index;

    private string timestamp;

	static float sin_xz(float x, float z) {
        return Mathf.Sin(x * z) / 3f;
    }
    static float xz(float x, float z) {
        return x * z / 6f;
    }
    static float cos_x_sin_z(float x, float z) {
        return Mathf.Cos(x) * Mathf.Sin(z);
    }
    static float normal_distr(float x, float z) {
        return Mathf.Exp(1 - x*x - z*z) / Mathf.Exp(1);
    }
    static float sphere_top(float x, float z) { 
        return Mathf.Sqrt(9 - x * x - z * z) / 3f;
    }
    static float sphere_bottom(float x, float z) {
        return -Mathf.Sqrt(9 - x * x - z * z) / 3f;
    }

    private ParticleSystem.Particle[] rotate_particles(ParticleSystem.Particle[] part, float rate) {
        int n_part = part.Length;
        for (int i = 0; i < n_part; i++) {
            Vector3 pos = part[i].position;
            float x = pos.x;
            float z = pos.z;
            float theta = rate * Time.deltaTime;
            pos.x = x * Mathf.Cos(theta) - z * Mathf.Sin(theta);
            pos.z = z * Mathf.Cos(theta) + x * Mathf.Sin(theta);
            part[i].position = pos;
        }
        return part;
    }

    private Vector3 normalize_pos(Vector3 pos) {
        return new Vector3(pos.x / Mathf.Max(Mathf.Abs(x_max), Mathf.Abs(x_min)) * graph_scale_factor,
            pos.y / Mathf.Max(Mathf.Abs(y_max), Mathf.Abs(y_min)) * graph_scale_factor,
            pos.z / Mathf.Max(Mathf.Abs(z_max), Mathf.Abs(z_min)) * graph_scale_factor);
    }

    IEnumerator spin_particles() {
        while (true) {
            ps.GetParticles(particles);
            n_particles = particles.Length;
            float interval = 0.02f; // seconds
            for (int i = 0; i < n_particles; i++) {
                Vector3 pos = particles[i].position;
                float x = pos.x;
                float z = pos.z;
                float theta = spinRate * interval;
                pos.x = x * Mathf.Cos(theta) - z * Mathf.Sin(theta);
                pos.z = z * Mathf.Cos(theta) + x * Mathf.Sin(theta);
                particles[i].position = pos;
            }
            ps.SetParticles(particles, n_particles);
            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator scale_particles() {
        float time = 0f;
        float interval = 0.02f;
        float scale_factor = 1f;
        while (true) {
            ps.GetParticles(particles);
            n_particles = particles.Length;
            for (int i = 0; i < n_particles; i++) {
                Vector3 pos = particles[i].position;
                scale_factor = (0.01f * Mathf.Sin(time)) + 1.0f;
                pos *= scale_factor;
                particles[i].position = pos;
            }
            ps.SetParticles(particles, n_particles);
            time += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    void Awake () {
        ps = GetComponent<ParticleSystem>();

        timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");

		functions = new List<GraphFunc>
		{
			sin_xz,
			xz,
			cos_x_sin_z,
			normal_distr,
			sphere_top,
			sphere_bottom
		};
		function_names = new List<string>
		{
			"sin_xz",
			"xz",
			"cos_x_sin_z",
			"normal_distr",
			"sphere_top",
			"sphere_bottom"
		};
        function_descriptions = new List<string> {
            "sin(xy)",
            "xy",
            "cos(x) + sin(y)",
            "exp(1 - x^2 - y^2)",
            "+sqrt(9 - x^2 - y^2)",
            "-sqrt(9 - x^2 - y^2)"
        };

        n_particles_x = (x_max - x_min) * resolution;
        n_particles_z = (z_max - z_min) * resolution;

        n_particles = n_particles_x * n_particles_z;

        Debug.Log("n_particles: " + n_particles);

        particles = new ParticleSystem.Particle[n_particles];

        

        current_func = cos_x_sin_z;
        //GraphFunc func_1 = sphere_bottom;

        generate();
        
    }//Awake()

    public void generate() {
        float x_val = x_min;
        float z_val = z_min;
        float incr = 1f / resolution;

        int i = 0;
        for (int z = 0; z < n_particles_z; z++) {
            x_val = x_min;
            for (int x = 0; x < n_particles_x; x++) {
                particles[i].position = normalize_pos(new Vector3(x_val, current_func(x_val, z_val), z_val));

                if (particles[i].position.y < y_min || particles[i].position.y > y_max) {
                    particles[i].startColor = Color.clear; // particles outside of y-bounds are invisible
                }
                else particles[i].startColor =
                        Color.HSVToRGB((particles[i].position.y - y_min) / (y_max - y_min), 1, 1); //normalize [ymin, ymax] to [0,1]
                //particles[i].startColor = new Color(particles[i].startColor.r, particles[i].startColor.g, particles[i].startColor.b, 0.5f);
                particles[i].startSize = start_size;
                x_val += incr;
                i++;
            }
            z_val += incr;
        }

        ps.SetParticles(particles, particles.Length);
    }

	public string nextFunction()
	{
		current_func_index++;
		if (current_func_index >= functions.Count)
		{
			current_func_index = 0;
		}
		current_func = functions[current_func_index];
		generate();
		return function_names[current_func_index];
	}

	public string prevFunction()
	{
		current_func_index--;
		if (current_func_index < 0)
		{
			current_func_index = functions.Count - 1;
		}
		current_func = functions[current_func_index];
		generate();
		return function_names[current_func_index];
	}

	private void Start() {
        //StartCoroutine(spin_particles());
        //StartCoroutine(scale_particles());
    }

    // Update is called once per frame
    void Update () {

    }
}
