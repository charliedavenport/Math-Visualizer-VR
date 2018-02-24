using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowSpin : MonoBehaviour {

    public float spinRate = 1f;

	// Use this for initialization
	void Start () {
        StartCoroutine(spin());
	}
	
	// Update is called once per frame
	void Update () {

    }

    IEnumerator spin() {
        while (true) {
            float interval = 0.02f; // seconds
            Vector3 rot = this.transform.rotation.eulerAngles;
            rot.y += Mathf.Rad2Deg * spinRate * interval;
            this.transform.rotation = Quaternion.Euler(rot);
            yield return new WaitForSeconds(interval);
        }
    }
}
