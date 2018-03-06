using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour {

	public bool isVectorMode = false;
	public ParticleGraph particleGraph;
	public VectorField vectorField;
    public GUIController gui;

	// Use this for initialization
	void Start () {
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
	void setMode(bool isVec) {
		isVectorMode = isVec;
		if (isVectorMode) {
			particleGraph.gameObject.SetActive(false);
			vectorField.gameObject.SetActive(true);
		}
		else {
			particleGraph.gameObject.SetActive(true);
			vectorField.gameObject.SetActive(false);
		}

	}

	// Update is called once per frame
	void Update () {
		
	}

    // theta is in degrees
    public void rotateGraph(float theta) {
        Vector3 current_rot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(current_rot.x, current_rot.y + theta, current_rot.z);
        if (isVectorMode) vectorField.generate();
        else particleGraph.generate();

        //set rotation in gui
        gui.current_rot = current_rot.y + theta;
    }

}
