using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractionZone : MonoBehaviour {

	public Transform rightController;
	public Transform start_pos_indicator;
	public GraphManager mainGraph;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	private void OnTriggerEnter(Collider other)
	{
		//Debug.Log("InteractionZone: OnTriggerEnter()");
		if (other.gameObject.tag == "rightController" && mainGraph.isVectorMode)
		{
			start_pos_indicator.gameObject.SetActive(true);
		}

	}

	private void OnTriggerStay(Collider other)
	{
		//Debug.Log("InteractionZone: OnTriggerStay()");
		if (other.gameObject.tag == "rightController" && mainGraph.isVectorMode)
		{
			// lerp sphere to controller location
			start_pos_indicator.position = Vector3.Lerp(start_pos_indicator.position, other.transform.position, 0.5f);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "rightController")
		{
			start_pos_indicator.localPosition = mainGraph.vectorField.start_pos;
		}

	}

}
