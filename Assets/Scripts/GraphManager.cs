using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour {

	public bool isVectorMode = false;
	public ParticleGraph particleGraph;
	public VectorField vectorField;

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
}
