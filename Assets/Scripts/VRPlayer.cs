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
    //public Transform hmd;
    //public Transform leftController;
    //public Transform rightController;
    public Transform feet;
    public SteamVR_TrackedObject hmd2;
    public SteamVR_TrackedObject controllerLeft;
    public SteamVR_TrackedObject controllerRight;

    public Rigidbody leftHeldObj;
    public Rigidbody rightHeldObj;

    public enum LocomotionMode { TELEPORT };
    public LocomotionMode locomotionMode = LocomotionMode.TELEPORT;

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
    {/*
        //int leftIndex = (int)leftController.GetComponent<SteamVR_TrackedObject>().index;
        //int rightIndex = (int)rightController.GetComponent<SteamVR_TrackedObject>().index;
        int leftIndex = (int)controllerLeft.index;
        int rightIndex = (int)controllerRight.index;

        // RIGHT HAND
        if (rightIndex >= 0)
		{


            Vector2 joyRight = getJoystick(controllerRight.transform);//rightController);
			rightHand.joystick(joyRight);

			float rightTrigger = SteamVR_Controller.Input(rightIndex).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).magnitude;

			bool a_btn = SteamVR_Controller.Input(rightIndex).GetPressDown(Valve.VR.EVRButtonId.k_EButton_A);
			


		} // RIGHT HAND

		// LEFT HAND
		if (leftIndex >= 0)
		{
            Vector2 joyLeft = getJoystick(controllerLeft.transform);//leftController);
			leftHand.joystick(joyLeft);

			float leftTrigger = SteamVR_Controller.Input(leftIndex).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).magnitude;


		} // LEFT HAND
        */
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
                //moves SteamVR_rig to player's position
                copyTransform(this.transform, SteamVR_Rig.transform);
                //moves controllers directly
                copyTransform(controllerLeft.transform, leftHand.transform);
                copyTransform(controllerRight.transform, rightHand.transform);
                //moves head to the HMD position / eye position
                copyTransform(hmd2.transform, head);

                //move the feet to be in the tracking space, but on the ground (maybe do this with physics to ensure a good foot position later)
                feet.position = Vector3.Scale(head.position, new Vector3(1, 0, 1)) + Vector3.Scale(SteamVR_Rig.position, new Vector3(0, 1, 0));
                handleControllerInputs();

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

    /**
     * Assigns network id 
     */
    [Command]
    public void CmdGetAuthority(NetworkIdentity id)
    {
        id.AssignClientAuthority(this.connectionToClient);
    }
    /**
     *Syncs players 
     */
     [Command]
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

    /**
     * Copies Transform
     */
    private void copyTransform(Transform origT, Transform newT)
    {
        newT.position = origT.position;
        newT.rotation = origT.rotation;
    }

    private void handleControllerInputs()
    {
        int indexLeft = (int)controllerLeft.index;
        int indexRight = (int)controllerRight.index;

        /*if (indexRight >= 0)
        {
            if (SteamVR_Controller.Input(indexRight).GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
            {
                CmdSpawnCube();
            }
        }*/

        leftHand.controllerVelocity = getControllerVelocity(controllerLeft);
        rightHand.controllerVelocity = getControllerVelocity(controllerRight);
        leftHand.controllerAngularVelocity = getControllerAngularVelocity(controllerLeft);
        rightHand.controllerAngularVelocity = getControllerAngularVelocity(controllerRight);

        float triggerLeft = getTrigger(controllerLeft);
        float triggerRight = getTrigger(controllerRight);

        Vector2 joyLeft = getJoystick(controllerLeft);
        Vector2 joyRight = getJoystick(controllerRight);
        //leftHand.squeeze(triggerLeft);
        //rightHand.squeeze(triggerRight);

        switch (locomotionMode)
        {
            /*case LocomotionMode.FLYING:
                {
                    fly(joyLeft, joyRight);
                    break;
                }*/
            case LocomotionMode.TELEPORT:
                {
                    leftHand.joystick(joyLeft);
                    rightHand.joystick(joyRight);
                    break;
                }
        }
    }


    private float getTrigger(SteamVR_TrackedObject controller)
    {
        if (controller.index >= 0)
        {
            return SteamVR_Controller.Input((int)controller.index).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).magnitude;
        } else {
            return 0.0f;
        }
    }

    private Vector2 getJoystick(SteamVR_TrackedObject controller)
        {
            /*if ((int)controller.GetComponent<SteamVR_TrackedObject>().index >= 0)
                return SteamVR_Controller.Input((int)controller.GetComponent<SteamVR_TrackedObject>().index).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
            else
                return Vector2.zero;*/
            
                if (controller.index >= 0)
                {
                    return SteamVR_Controller.Input((int)controller.index).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);             
                }else
                {
                    return Vector2.zero;
                }
        }
    private Vector3 getControllerVelocity(SteamVR_TrackedObject controller)
    {
        Vector3 controllerVelocity = controller.index >= 0 ? SteamVR_Controller.Input((int)controller.index).velocity : Vector3.zero;
        return SteamVR_Rig.localToWorldMatrix.MultiplyVector(controllerVelocity.normalized) * controllerVelocity.magnitude;
    }

    private Vector3 getControllerAngularVelocity(SteamVR_TrackedObject controller)
    {
        Vector3 angularVelocity = controller.index >= 0 ? SteamVR_Controller.Input((int)controller.index).angularVelocity : Vector3.zero;
        return SteamVR_Rig.localToWorldMatrix.MultiplyVector(angularVelocity.normalized) * angularVelocity.magnitude;
    }

    public void teleport(Vector3 pos, Vector3 forward)
    {
        //hmdBlinker.blink(.1f);
        // Vector3 facingDirection = new Vector3(head.forward.x, 0, head.forward.z);
        // float angleBetween = Vector3.SignedAngle(facingDirection, forward, Vector3.up);
        //   this.transform.Rotate(Vector3.up, angleBetween, Space.World);
        // Vector3 offset = pos - feet.position;
        //  this.transform.Translate(offset, Space.World);

        Vector3 offset = pos - feet.position;
        SteamVR_Rig.Translate(offset, Space.World);
        Vector3 facingDirection = new Vector3(head.forward.x, 0, head.forward.z);
        float angleBetween = Vector3.SignedAngle(facingDirection, forward, Vector3.up);
        SteamVR_Rig.Rotate(Vector3.up, angleBetween, Space.World);
    }
    /*public void fly(Vector2 leftJoystick, Vector2 rightJoystick)
    {

        float leftSpeed = Mathf.Clamp(leftJoystick.y, 0, 1);
        float rightSpeed = Mathf.Clamp(rightJoystick.y, 0, 1);
        Vector3 leftDirection = leftHand.transform.forward;
        Vector3 rightDirection = rightHand.transform.forward;
        Vector3 displacement = (leftDirection * leftSpeed + rightDirection * rightSpeed) * Time.deltaTime;
        this.transform.Translate(displacement, Space.World);
        int index = (int)controller.GetComponent<SteamVR_TrackedObject>().index;
        if (index >= 0)
			return SteamVR_Controller.Input(index).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
		else
			return Vector2.zero;
    }*/
}
