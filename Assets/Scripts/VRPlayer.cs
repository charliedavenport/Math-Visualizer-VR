using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;

/// <summary>
/// /////////////////
/// </summary>
public class VRPlayer : NetworkBehaviour
{
    //public GameLogic gameLogic;

    public Transform head;
    public HandController leftHand;
    public HandController rightHand;

    public Transform SteamVR_Rig;
    public Transform hmd;
    public Transform leftController;
    public Transform rightController;
    public Transform feet;
    public SteamVR_TrackedObject hmd2;
    public SteamVR_TrackedObject controllerLeft;
    public SteamVR_TrackedObject controllerRight;

    public Rigidbody leftHeldObj;
    public Rigidbody rightHeldObj;

    float saveMaxLeft;
    float saveMaxRight;

    // Use this for initialization
    [SyncVar]
    Vector3 headPos;
    [SyncVar]
    Quaternion headRot;
    [SyncVar]
    Vector3 leftHandPos;
    [SyncVar]
    Quaternion leftHandRot;
    [SyncVar]
    Vector3 rightHandPos;
    [SyncVar]
    Quaternion rightHandRot;
    void Start()
    {
        head.transform.position = new Vector3(0, 2, 0);
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

    //
    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if (UnityEngine.XR.XRSettings.enabled)
            {
                if (SteamVR_Rig == null)
                {
                    GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
                    SteamVR_Rig = gm.vrCameraRig.transform;
                    hmd2 = gm.hmd2;
                    controllerLeft = gm.controllerLeft;
                    controllerRight = gm.controllerRight;
                    //hmdBlinker = gm.hmdBlinker;
                }
                //move the SteamVR_rig to the player's position
                copyTransform(this.transform, SteamVR_Rig.transform);
                //the controllers are the easy ones, just move them directly
                copyTransform(controllerLeft.transform, leftHand.transform);
                copyTransform(controllerRight.transform, rightHand.transform);
                //now move the head to the HMD position, this is actually the eye position
                copyTransform(hmd2.transform, head);

                //move the feet to be in the tracking space, but on the ground (maybe do this with physics to ensure a good foot position later)
                feet.position = Vector3.Scale(head.position, new Vector3(1, 0, 1)) + Vector3.Scale(SteamVR_Rig.position, new Vector3(0, 1, 0));
               // handleControllerInputs();

            }
            else
            {


                float vertical = Input.GetAxis("Vertical");
                float horizontal = Input.GetAxis("Horizontal");
                transform.Translate(vertical * Time.fixedDeltaTime * (new Vector3(0, 0, 1)));
                transform.Translate(horizontal * Time.fixedDeltaTime * (new Vector3(1, 0, 0)));


            }
            CmdSyncPlayer(head.transform.position, head.transform.rotation, leftHand.transform.position, leftHand.transform.rotation, rightHand.transform.position, rightHand.transform.rotation);
        }
        else
        {
            //runs on all other clients and  the server
            //move to the syncvars
            head.position = Vector3.Lerp(head.position, headPos, .2f);
            head.rotation = Quaternion.Slerp(head.rotation, headRot, .2f);
            leftHand.transform.position = leftHandPos;
            leftHand.transform.rotation = leftHandRot;
            rightHand.transform.position = rightHandPos;
            rightHand.transform.rotation = rightHandRot;

        }
    }

    /*private void LateUpdate()
    {
        head.position = hmd.position;
        leftHand.transform.position = leftController.position;
        rightHand.transform.position = rightController.position;

        head.rotation = hmd.rotation;
        leftHand.transform.rotation = leftController.transform.rotation;
        rightHand.transform.rotation = rightController.transform.rotation;
    } // LateUpdate*/

    void CmdSyncPlayer(Vector3 pos, Quaternion rot, Vector3 lhpos, Quaternion lhrot, Vector3 rhpos, Quaternion rhrot)
    {
        head.transform.position = pos;
        head.transform.rotation = rot;
        leftHand.transform.position = lhpos;
        rightHand.transform.position = rhpos;
        leftHand.transform.rotation = lhrot;
        rightHand.transform.rotation = rhrot;
        headPos = pos;
        headRot = rot;
        leftHandPos = lhpos;
        leftHandRot = lhrot;
        rightHandPos = rhpos;
        rightHandRot = rhrot;

    }
    private void copyTransform(Transform from, Transform to)
    {
        to.position = from.position;
        to.rotation = from.rotation;
    }

    private Vector2 getJoystick(Transform controller)
    {
		if ((int)controller.GetComponent<SteamVR_TrackedObject>().index >= 0)
			return SteamVR_Controller.Input((int)controller.GetComponent<SteamVR_TrackedObject>().index).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
		else
			return Vector2.zero;
    }
}
