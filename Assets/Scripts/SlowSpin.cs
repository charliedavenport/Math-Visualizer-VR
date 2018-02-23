using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowSpin : MonoBehaviour {

    public float spinRate = 1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        Vector3 rot = this.transform.rotation.eulerAngles;
        rot.y += Mathf.Rad2Deg * spinRate * Time.deltaTime;
        this.transform.rotation = Quaternion.Euler(rot);
        
        
    }
}
