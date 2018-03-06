using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamVRWorldMouse : WorldMouse {
	public SteamVR_TrackedObject myController;
	public GameObject laserPointerPrefab;
	GameObject laserPointer;

	void Start()
	{
		laserPointer = Instantiate<GameObject>(laserPointerPrefab);
		laserPointer.transform.SetParent(this.transform);
	}
	protected override void Update()
	{
		base.Update();
		laserPointer.transform.localScale = new Vector3(1, 1, rayDistance);
	}
	public override bool pressDown()
	{
		if (myController.index >= 0)
		{
			return SteamVR_Controller.Input((int)myController.index).GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
		}
		return false;
	}

	public override bool pressUp()
	{
		if (myController.index >= 0)
		{
			return SteamVR_Controller.Input((int)myController.index).GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
		}
		return false;
	}
}
