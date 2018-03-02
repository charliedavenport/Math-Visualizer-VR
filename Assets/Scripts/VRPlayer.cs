using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPlayer : MonoBehaviour
{
    //public GameLogic gameLogic;

    public Transform head;
    public HandController leftHand;
    public HandController rightHand;

    public Transform hmd;
    public Transform leftController;
    public Transform rightController;

    public Rigidbody leftHeldObj;
    public Rigidbody rightHeldObj;

    float saveMaxLeft;
    float saveMaxRight;

    // Use this for initialization
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
		int leftIndex = (int)leftController.GetComponent<SteamVR_TrackedObject>().index;
		int rightIndex = (int)rightController.GetComponent<SteamVR_TrackedObject>().index;

		// RIGHT HAND
		if (rightIndex >= 0)
		{


			Vector2 joyRight = getJoystick(rightController);
			rightHand.joystick(joyRight);

			float rightTrigger = SteamVR_Controller.Input(rightIndex).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).magnitude;

			bool a_btn = SteamVR_Controller.Input(rightIndex).GetPressDown(Valve.VR.EVRButtonId.k_EButton_A);
			


		} // RIGHT HAND

		// LEFT HAND
		if (leftIndex >= 0)
		{
			Vector2 joyLeft = getJoystick(leftController);
			leftHand.joystick(joyLeft);

			float leftTrigger = SteamVR_Controller.Input(leftIndex).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).magnitude;


		} // LEFT HAND
	}

    private void LateUpdate()
    {
        head.position = hmd.position;
        leftHand.transform.position = leftController.position;
        rightHand.transform.position = rightController.position;

        head.rotation = hmd.rotation;
        leftHand.transform.rotation = leftController.transform.rotation;
        rightHand.transform.rotation = rightController.transform.rotation;

        

    } // LateUpdate

    private Vector2 getJoystick(Transform controller)
    {
		if ((int)controller.GetComponent<SteamVR_TrackedObject>().index >= 0)
			return SteamVR_Controller.Input((int)controller.GetComponent<SteamVR_TrackedObject>().index).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
		else
			return Vector2.zero;
    }
}
