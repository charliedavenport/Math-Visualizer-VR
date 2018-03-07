using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem), typeof(LineRenderer))]
public class VectorField : MonoBehaviour {


    public int resolution = 10;
    public int x_min, z_min, y_min, x_max, z_max, y_max;
    public float scale_factor;
    public float start_size;

    private int n_vectors_x, n_vectors_z, n_vectors_y;
    private int n_vectors;

    private ParticleSystem.Particle[] vectors;
    private ParticleSystem ps;
    private LineRenderer lr;
	public Transform start_pos_indicator; //red sphere
	public Vector3 start_pos;

    public delegate Vector3 VectorFunc(Vector3 input);
    public VectorFunc current_func;
	public List<VectorFunc> functions;
	public List<string> function_names;
	public int current_func_index;

    // identity function: f(x,y,z) = (x,y,z) 
    private static Vector3 identity(Vector3 input) {
        return input;
    }

    private static Vector3 spiral_up(Vector3 input) {
        return new Vector3(
            -input.z,
            0.1f,
            input.x);
    }

    private static Vector3 hyperbolic(Vector3 input) {
        return new Vector3(
            input.y * input.z,
            input.x * input.z,
            input.y * input.x);
    }

    private static Vector3 fluid_flow(Vector3 input) {
        return new Vector3(
            Mathf.Sin(input.x) + Mathf.Sin(input.y) + Mathf.Sin(input.z),
            Mathf.Sin(input.x) - Mathf.Sin(input.y) + Mathf.Sin(input.z),
            Mathf.Sin(input.x) + Mathf.Sin(input.y) - Mathf.Sin(input.z));
    }

    private Vector3 normalize_pos(Vector3 pos) {
        return new Vector3(pos.x / Mathf.Max(Mathf.Abs(x_max), Mathf.Abs(x_min)) * scale_factor,
            pos.y / Mathf.Max(Mathf.Abs(y_max), Mathf.Abs(y_min)) * scale_factor,
            pos.z / Mathf.Max(Mathf.Abs(z_max), Mathf.Abs(z_min)) * scale_factor);
    }

    

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
        lr = GetComponent<LineRenderer>();
		//lr.positionCount = 20;

		functions = new List<VectorFunc>
		{
			spiral_up,
			hyperbolic,
			identity,
			fluid_flow,
		};
		function_names = new List<string>
		{
			"spiral_up",
			"hyperbolic",
			"identity",
			"fluid_flow",
		};
		current_func_index = 0;

		start_pos_indicator.gameObject.SetActive(false);
		start_pos = Vector3.zero;

        n_vectors_x = (x_max - x_min) * resolution + 1;
        n_vectors_y = (y_max - y_min) * resolution + 1;
        n_vectors_z = (z_max - z_min) * resolution + 1;
        n_vectors = n_vectors_x * n_vectors_y * n_vectors_z;

        Debug.Log("n_vectors: " + n_vectors);

        vectors = new ParticleSystem.Particle[n_vectors];

		current_func = functions[current_func_index];

        generate(); // set particles
        //StartCoroutine(draw_solution(new Vector3(1f, -1f, 0.8f)));
    }//Awake()
	

	public void generate() {

        float min_size = 0.05f,
            max_size = 1f;

        float x_val = x_min,
            z_val = z_min,
            y_val = y_min;
        float incr = 1f / resolution;
        int i = 0;
        for (int x = 0; x < n_vectors_x; x++) {
            y_val = y_min;
            for (int y = 0; y < n_vectors_y; y++) {
                z_val = z_min;
                for (int z = 0; z < n_vectors_z; z++) {
                    Vector3 input_vec = new Vector3(x_val, y_val, z_val);
                    vectors[i].position = normalize_pos(input_vec);
                    Vector3 return_vec = current_func(input_vec);
                    float theta = Mathf.Atan2(return_vec.x, return_vec.z) * Mathf.Rad2Deg;
                    float phi = Mathf.Atan2(return_vec.y, Mathf.Sqrt(return_vec.x * return_vec.x + return_vec.z * return_vec.z)) * Mathf.Rad2Deg;
                    //Debug.Log("phi " + phi + ", theta " + theta);
                    vectors[i].rotation3D = new Vector3(-phi, theta, 0);
                    float vec_size = Mathf.Clamp(start_size * Vector3.Magnitude(return_vec), min_size, max_size);
                    vectors[i].startSize3D = new Vector3(vec_size, vec_size, vec_size);
                    float hue = Mathf.Clamp(Vector3.Magnitude(return_vec) / (2f * y_max), 0f, 1f);
                    vectors[i].startColor = Color.HSVToRGB(hue, 1, 1); // normalizing color values based on twice the y_max value
                    z_val += incr;
                    i++;
                }

                y_val += incr;
            }
            x_val += incr;
        }

        ps.SetParticles(vectors, vectors.Length);
    }//generate

	public string nextFunction()
	{
		resetSolutionCurve();
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
		resetSolutionCurve();
		current_func_index--;
		if (current_func_index < 0)
		{
			current_func_index = functions.Count - 1;
		}
		current_func = functions[current_func_index];
		generate();
		return function_names[current_func_index];
	}

	public void new_solution_curve(Vector3 start)
	{
		StartCoroutine(draw_solution(start));
	}

	public void resetSolutionCurve()
	{
		lr.positionCount = 0;
	}

    IEnumerator draw_solution(Vector3 start) {
		start_pos = start;
		start_pos_indicator.gameObject.SetActive(true);
		start_pos_indicator.localPosition = start; //normalize_pos(start);
		if (lr.positionCount > 0) resetSolutionCurve();
        Vector3 pos = start;
        //Vector3[] positions = new Vector3[10];
        for (int i = 0; i < 200; i++) {
            //positions[i] = pos;
            lr.positionCount = i + 1;
            
            //Debug.Log(pos);
            lr.SetPosition(i, pos);
			pos += 0.05f * current_func(pos);

			//break out if out of bounds
			if (pos.x >= x_max * 1.5 || pos.x <= x_min * 1.5 || pos.y >= y_max * 1.5 || pos.y <= y_min * 1.5 || pos.z >= z_max * 1.5 || pos.z <= z_min * 1.5)
			{
				break;
			}
            //yield return new WaitForSeconds(0.05f);
            yield return null;
        }

    }

}