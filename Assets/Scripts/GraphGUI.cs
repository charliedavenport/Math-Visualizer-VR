using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphGUI : MonoBehaviour {

    public GUIController playerGUI;
    public GraphManager mainGraph;
    public Text functionSig; // function signature. i.e. 'f(x,y,z) = ' or 'f(x,y) = '
    public Text functionDesc;

    public bool isVectorMode;
    private string signature;
    private string description;

	// Use this for initialization
	void Start () {
		
	}
	
	void LateUpdate () {
        isVectorMode = mainGraph.isVectorMode;
        if (isVectorMode) {
            functionSig.text = "f(x, y, z) = ";
            functionDesc.text = mainGraph.vectorField.function_descriptions[mainGraph.vectorField.current_func_index];
        }
        else {
            functionSig.text = "f(x, y) = ";
            functionDesc.text = "something";
        }

	}
}
