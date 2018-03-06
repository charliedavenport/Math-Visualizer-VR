using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class WorldMouse : MonoBehaviour {


	public float rayDistance;
	public float worldRayDistance;
	protected virtual void Update()
	{
		worldRayDistance = Mathf.Infinity;
		RaycastHit hit;
		if(Physics.Raycast(new Ray(transform.position, transform.forward),out hit))
		{
			worldRayDistance = hit.distance;
		}
	}


	public abstract bool pressDown();

	public abstract bool pressUp();
}
