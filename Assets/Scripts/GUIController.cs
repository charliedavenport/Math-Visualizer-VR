using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {

	public Transform player;
	public Text mode_text;
	public Text rotate_text;
	public Text func_text;

	private RectTransform rect;

	private bool isVectorMode; // true: vector field // false: particle graph
	private string current_func;
	private float current_rot;

	// Use this for initialization
	void Start () {
		isVectorMode = false;
		rect = GetComponent<RectTransform>();
		rect.rotation = Quaternion.Euler(45f, 0f, 0f);
		rect.Translate(0f, 0f, 0.15f); // 'recenter' GUI with controller
	}
	
	// Update is called once per frame
	void Update () {
		mode_text.text = "Mode: " + (isVectorMode? "Vector Field" : "3D Graph");
		rotate_text.text = "Rotation: " + current_rot.ToString("F");
		func_text.text = "Function: " + current_func;
	}
}
