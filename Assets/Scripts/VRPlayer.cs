using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;

public class VRPlayer : NetworkBehaviour
{
    public Transform SteamVR_Rig;
    public SteamVR_TrackedObject hmd;
    public SteamVR_TrackedObject controllerLeft;
    public SteamVR_TrackedObject controllerRight;
    public GameManager gm;
    public GUIController gui; // pass controller input into this
    public InteractionZone interactionZone; // and this (Charlie)

    public Transform head;
    public HandController handLeft;
    public HandController handRight;
    public Transform feet;
    float saveMaxLeft;
    float saveMaxRight;
    //public enum LocomotionMode { TELEPORT, FLYING};
    //public LocomotionMode locomotionMode = LocomotionMode.FLYING;
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

    void Update()
    {

    }
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
                    hmd = gm.hmd;
                    controllerLeft = gm.controllerLeft;
                    controllerRight = gm.controllerRight;
                }
                //move the SteamVR_rig to the player's position
                copyTransform(this.transform, SteamVR_Rig.transform);
                //the controllers are the easy ones, just move them directly
                copyTransform(controllerLeft.transform, handLeft.transform);
                copyTransform(controllerRight.transform, handRight.transform);
                //now move the head to the HMD position, this is actually the eye position
                copyTransform(hmd.transform, head);

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
            CmdSyncPlayer(head.transform.position, head.transform.rotation, handLeft.transform.position, handLeft.transform.rotation, handRight.transform.position, handRight.transform.rotation);
        }
        else
        {
            //runs on all other clients and  the server
            //move to the syncvars
            head.position = Vector3.Lerp(head.position, headPos, .2f);
            head.rotation = Quaternion.Slerp(head.rotation, headRot, .2f);
            handLeft.transform.position = leftHandPos;
            handLeft.transform.rotation = leftHandRot;
            handRight.transform.position = rightHandPos;
            handRight.transform.rotation = rightHandRot;

        }

    }

    [Command]
    public void CmdGetAuthority(NetworkIdentity id)
    {
        id.AssignClientAuthority(this.connectionToClient);
    }
    [Command]
    void CmdSyncPlayer(Vector3 pos, Quaternion rot, Vector3 lhpos, Quaternion lhrot, Vector3 rhpos, Quaternion rhrot)
    {
        head.transform.position = pos;
        head.transform.rotation = rot;
        handLeft.transform.position = lhpos;
        handRight.transform.position = rhpos;
        handLeft.transform.rotation = lhrot;
        handRight.transform.rotation = rhrot;
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
    private void handleControllerInputs()
    {
        int indexLeft = (int)controllerLeft.index;
        int indexRight = (int)controllerRight.index;


        handLeft.controllerVelocity = getControllerVelocity(controllerLeft);
        handRight.controllerVelocity = getControllerVelocity(controllerRight);
        handLeft.controllerAngularVelocity = getControllerAngularVelocity(controllerLeft);
        handRight.controllerAngularVelocity = getControllerAngularVelocity(controllerRight);

        float triggerLeft = getTrigger(controllerLeft);
        float triggerRight = getTrigger(controllerRight);
        bool a_btn = SteamVR_Controller.Input(indexRight).GetPressDown(Valve.VR.EVRButtonId.k_EButton_A);
        interactionZone.a_btn = a_btn;

        Vector2 joyLeft = getJoystick(controllerLeft);
        gui.handleInput(joyLeft);
        Vector2 joyRight = getJoystick(controllerRight);
        fly(joyRight);


    }
    private float getTrigger(SteamVR_TrackedObject controller)
    {
        return controller.index >= 0 ? SteamVR_Controller.Input((int)controller.index).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).magnitude : 0.0f;
    }

    private Vector2 getJoystick(SteamVR_TrackedObject controller)
    {
        return controller.index >= 0 ? SteamVR_Controller.Input((int)controller.index).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) : Vector2.zero;
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

    public void fly(Vector2 rightJoystick)
    {

        float rightSpeed = Mathf.Clamp(rightJoystick.y, 0, 1); //y^ x> 
        Vector3 rightDirection = handRight.transform.forward;
        Vector3 displacement = (rightDirection * rightSpeed) * Time.deltaTime;
        this.transform.Translate(displacement, Space.World);

        float rightSpeed2 = Mathf.Clamp(-(rightJoystick.y), 0, 1); //y^ x> 
        Vector3 rightDirection2 = -(handRight.transform.forward);
        Vector3 displacement2 = (rightDirection2 * rightSpeed2) * Time.deltaTime;
        this.transform.Translate(displacement2, Space.World);

        float rightSpeed3 = Mathf.Clamp(rightJoystick.x, 0, 1); //y^ x> 
        Vector3 rightDirection3 = handRight.transform.right;
        Vector3 displacement3 = (rightDirection3 * rightSpeed3) * Time.deltaTime;
        this.transform.Translate(displacement3, Space.World);

        float rightSpeed4 = Mathf.Clamp(-(rightJoystick.x), 0, 1); //y^ x> 
        Vector3 rightDirection4 = -(handRight.transform.right);
        Vector3 displacement4 = (rightDirection4 * rightSpeed4) * Time.deltaTime;
        this.transform.Translate(displacement4, Space.World);
    }


}
