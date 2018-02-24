using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxesSpin : MonoBehaviour {

    public float spinRate = 1f;
    public Transform[] labels;

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
            float delta_theta = Mathf.Rad2Deg * spinRate * interval; // change in y-rotation
            rot.y += delta_theta;
            this.transform.rotation = Quaternion.Euler(rot);

            // unspin each label
            if (labels != null) {
                for (int i=0; i<labels.Length; i++) {
                    Vector3 localRot = labels[i].rotation.eulerAngles;
                    localRot.y -= delta_theta;
                    labels[i].transform.rotation = Quaternion.Euler(localRot);
                }
            }
            yield return new WaitForSeconds(interval);
        }
    }
}
