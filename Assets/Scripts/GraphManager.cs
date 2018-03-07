using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour {

	public bool isVectorMode = false;
	public ParticleGraph particleGraph;
	public VectorField vectorField;
    public GUIController gui;

	public bool isRotating;

	// Use this for initialization
	void Start () {
		isRotating = false;

		if (isVectorMode) {
			particleGraph.gameObject.SetActive(false);
			vectorField.gameObject.SetActive(true);
		}
		else {
			particleGraph.gameObject.SetActive(true);
			vectorField.gameObject.SetActive(false);
		}

        gui.current_rot = transform.rotation.eulerAngles.y;
        gui.isVectorMode = this.isVectorMode;
	}
	
	// enable or disable grpahs
	public void setMode(bool isVec) {
		vectorField.start_pos_indicator.gameObject.SetActive(false);//reset whenever mode is changed
		isVectorMode = isVec;
		if (isVectorMode) {
			particleGraph.gameObject.SetActive(false);
			vectorField.gameObject.SetActive(true);
			vectorField.generate();
		}
		else {
			particleGraph.gameObject.SetActive(true);
			vectorField.gameObject.SetActive(false);
			particleGraph.generate();
		}
		
	}

    // theta is in degrees
    public void rotateGraph(float theta) {
        Vector3 current_rot = transform.rotation.eulerAngles;
        Quaternion end_rot = Quaternion.Euler(current_rot.x, current_rot.y + theta, current_rot.z);
		if (!isRotating)
		{
			StartCoroutine(lerp_rotation(transform.rotation, end_rot));
		}

        //set rotation in gui
        //gui.current_rot = current_rot.y + theta;
    }

    public void scaleGraph(float scaleRate) {
        transform.localScale += new Vector3(scaleRate, scaleRate, scaleRate);
        //vectorField.transform.localScale = this.transform.localScale;
        //particleGraph.transform.localScale = this.transform.localScale;
        vectorField.scale_factor += scaleRate * 0.5f;
        particleGraph.graph_scale_factor += scaleRate * 0.5f;
        if (isVectorMode)
            vectorField.generate();
        else
            particleGraph.generate();
    }

	//chan
	public string nextFunction()
	{
			return isVectorMode? vectorField.nextFunction() : particleGraph.nextFunction();
	}

	public string prevFunction()
	{
		return isVectorMode? vectorField.prevFunction() : particleGraph.prevFunction();
	}

	public string getCurrentFunctionName()
	{
		return isVectorMode ? 
			vectorField.function_names[vectorField.current_func_index]
			: particleGraph.function_names[particleGraph.current_func_index];

	}

	public int getCurrentFunctionIndex()
	{
		return isVectorMode ? vectorField.current_func_index : particleGraph.current_func_index;
	}

	IEnumerator lerp_rotation(Quaternion start_rot, Quaternion end_rot)
	{
		isRotating = true;
		float total_time = .25f;
		int n_iter = 10;
		float incr = total_time / (float)n_iter;
		for (float t = 0f; t <= total_time ; t += incr)
		{
			transform.rotation = Quaternion.Lerp(start_rot, end_rot, t / total_time);
			if (isVectorMode)
				vectorField.generate();
			else
				particleGraph.generate();
			//yield return new WaitForSeconds(incr);
			yield return null;
		}
		isRotating = false;
			
		
	}

}
