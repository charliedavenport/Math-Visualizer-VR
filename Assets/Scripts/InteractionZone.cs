using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractionZone : MonoBehaviour {

	public Transform rightController;
	public Transform start_pos_indicator;
	public GraphManager mainGraph;
	public VRPlayer player;

	public bool a_btn;

	private bool set;

	// Use this for initialization
	void Start () {
		set = false;
		a_btn = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

	private void OnTriggerEnter(Collider other)
	{
		set = false;
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
			if (!set)
			{
				start_pos_indicator.position = Vector3.Lerp(start_pos_indicator.position, other.transform.position, 0.5f);
				if (a_btn)
				{
					set = true;
					mainGraph.vectorField.new_solution_curve(start_pos_indicator.localPosition);
					Debug.Log("drawing new solution curve");
				}
			}
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
