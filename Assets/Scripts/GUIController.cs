using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {

	//public Transform player;
	public Text mode_text;
	public Text rotate_text;
    public Text scale_text;
	public Text func_text;
    public Text func_index;
    public Image mode_select;
	public Image rotate_select;
    public Image scale_select;
    public Image function_select;
	public GraphManager mainGraph;

	private RectTransform rect;

	public bool isVectorMode; // true: vector field // false: particle graph
	private string current_func;
	private int current_func_index;
	public float current_rot;
    public float current_scale;

    private int selection; //between 0 and 2. 0 means 'Mode Text', 1 means 'Rotate Text', 2 means 'Function Text'
	private bool wait_for_reset; //used in handling controller input
	// Use this for initialization
	void Start () {
		//isVectorMode = false; // set by GraphManager.cs
		rect = GetComponent<RectTransform>();
		rect.rotation = Quaternion.Euler(45f, 0f, 0f);
		rect.Translate(0f, 0f, 0.15f); // 'recenter' GUI with controller

		wait_for_reset = false;

        selection = 0; // start out with the mode text selected
        updateSelection();

		current_func = mainGraph.getCurrentFunctionName();
		current_func_index = mainGraph.getCurrentFunctionIndex();
        current_scale = 1f;

        mode_select.gameObject.SetActive(true);
        rotate_select.gameObject.SetActive(false);
        scale_select.gameObject.SetActive(false);
        function_select.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		mode_text.text = "Mode: " + (isVectorMode? "Vector Field" : "3D Graph");
		rotate_text.text = "Rotation: " + current_rot.ToString("F");
        scale_text.text = "Scale: " + current_scale; 
		func_text.text = "" + current_func;
		func_index.text = "Function: " + current_func_index;
	}

	public void handleInput(Vector2 joyLeft)
	{
		float y_speed = Mathf.Clamp(joyLeft.y, -1, 1);
		float x_speed = Mathf.Clamp(joyLeft.x, -1, 1);

		if (!wait_for_reset)
		{
			// move select up/down before changing selected value 
			//(important to note for when the user pushes the stick diagonally)
			if (y_speed >= 0.8f)
			{
				moveSelectionUp();
				wait_for_reset = true;
			}
			else if (y_speed <= -0.8f)
			{
				moveSelectionDown();
				wait_for_reset = true;
			}
			else if (Mathf.Abs(x_speed) >= 0.8f)
			{
                bool pos = (x_speed > 0);
                wait_for_reset = true;
				switch (selection)
				{
					case 0: // Mode
						isVectorMode = !isVectorMode;
						mainGraph.setMode(isVectorMode);
						current_func = mainGraph.getCurrentFunctionName();
						current_func_index = mainGraph.getCurrentFunctionIndex();
						break;
					case 1: // Rotate
						if (!mainGraph.isRotating)
						{
							mainGraph.rotateGraph(pos ? 30 : -30);
							current_rot += (pos ? 30 : -30);
							if (current_rot >= 360f) current_rot -= 360f;
							else if (current_rot <= -360f) current_rot += 360f;
						}
						break;
                    case 2:
                        mainGraph.scaleGraph(pos ? 0.1f : -0.1f);
                        current_scale = mainGraph.transform.localScale.x;
                        break;
					case 3: // Function
						current_func = (pos) ? mainGraph.nextFunction() : mainGraph.prevFunction();
						current_func_index = mainGraph.getCurrentFunctionIndex();
						break;
					default: // ERROR
						Debug.LogError("GUIController.handleInput: selection out of bounds");
						break;
						
				}
			}

		}
		else if (Mathf.Abs(y_speed) < 0.75f && Mathf.Abs(x_speed) < 0.75f)
		{
			wait_for_reset = false;
		}
	}

    public void moveSelectionUp() {
        if (selection > 0) selection--;
        updateSelection();
    }

    public void moveSelectionDown() {
        if (selection < 4) selection++;
        updateSelection();
    }

    private void updateSelection() {
		//Debug.Log(selection);
        switch (selection) {
            case 0: // MODE
				mode_select.gameObject.SetActive(true);
				rotate_select.gameObject.SetActive(false);
                scale_select.gameObject.SetActive(false);
				function_select.gameObject.SetActive(false);
				break;
            case 1: // ROTATE
                mode_select.gameObject.SetActive(false);
                rotate_select.gameObject.SetActive(true);
                scale_select.gameObject.SetActive(false);
                function_select.gameObject.SetActive(false);
                break;
            case 2: // SCALE
                mode_select.gameObject.SetActive(false);
                rotate_select.gameObject.SetActive(false);
                scale_select.gameObject.SetActive(true);
                function_select.gameObject.SetActive(false);
                break;
            case 3: // FUNCTION
                mode_select.gameObject.SetActive(false);
                rotate_select.gameObject.SetActive(false);
                scale_select.gameObject.SetActive(false);
                function_select.gameObject.SetActive(true);
                break;
        }
    }
}
