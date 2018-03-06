using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {

	public Transform player;
	public Text mode_text;
	public Text rotate_text;
	public Text func_text;
    public Image select_img;

	private RectTransform rect;

	public bool isVectorMode; // true: vector field // false: particle graph
	private string current_func;
	public float current_rot;

    private int selection; //between 0 and 2. 0 means 'Mode Text', 1 means 'Rotate Text', 2 means 'Function Text'

	// Use this for initialization
	void Start () {
		//isVectorMode = false; // set by GraphManager.cs
		rect = GetComponent<RectTransform>();
		rect.rotation = Quaternion.Euler(45f, 0f, 0f);
		rect.Translate(0f, 0f, 0.15f); // 'recenter' GUI with controller

        selection = 0; // start out with the mode text selected
        updateSelection();
	}
	
	// Update is called once per frame
	void Update () {
		mode_text.text = "Mode: " + (isVectorMode? "Vector Field" : "3D Graph");
		rotate_text.text = "Rotation: " + current_rot.ToString("F");
		func_text.text = "Function: " + current_func;
	}

    public void moveSelectionUp() {
        if (selection > 0) selection--;
        updateSelection();
    }

    public void moveSelectionDown() {
        if (selection < 2) selection++;
        updateSelection();
    }

    private void updateSelection() {
        switch (selection) {
            case 0:
                select_img.rectTransform.position = mode_text.rectTransform.position;
                break;
            case 1:
                select_img.rectTransform.position = rotate_text.rectTransform.position;
                break;
            case 2:
                select_img.rectTransform.position = func_text.rectTransform.position;
                break;
        }
    }
}
