using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameLogic gameLogic;

    public Transform head;
    public HandController leftHand;
    public HandController rightHand;

    public Transform hmd;
    public Transform leftController;
    public Transform rightController;

    public Rigidbody leftHeldObj;
    public Rigidbody rightHeldObj;

    public Rigidbody ball;

    public Transform cups;

    public CupsGUI gui;

    float saveMaxLeft;
    float saveMaxRight;

    int n_cups;

    Vector3 ball_start_pos;

    Vector3[,] cup_positions; // stores the starting position of the cups
    Quaternion[,] cup_rotations; // stores the starting rotation of the cups

    // Use this for initialization
    void Start()
    {
        n_cups = 18;
        //save ball transform
        ball_start_pos = new Vector3(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z);
        //save cups transforms
        cup_positions = new Vector3[3, 6];
        cup_rotations = new Quaternion[3, 6];
        for (int j = 0; j < 3; j++)
        {
            // each stack of cups is a child of the cups obj
            Transform cup_stack = cups.GetChild(j).transform;
            for (int i = 0; i < 6; i++)
            {
                cup_positions[j, i] = cup_stack.GetChild(i).transform.position;
                cup_rotations[j, i] = cup_stack.GetChild(i).transform.rotation;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        head.position = hmd.position;
        leftHand.transform.position = leftController.position;
        rightHand.transform.position = rightController.position;

        head.rotation = hmd.rotation;
        leftHand.transform.rotation = leftController.transform.rotation;
        rightHand.transform.rotation = rightController.transform.rotation;

        int leftIndex = (int)leftController.GetComponent<SteamVR_TrackedObject>().index;
        int rightIndex = (int)rightController.GetComponent<SteamVR_TrackedObject>().index;

        // RIGHT HAND
        if (rightIndex >= 0)
        {
            float rightTrigger = SteamVR_Controller.Input(rightIndex).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).magnitude;

            // reset ball - can happen as long as the game is not over
            bool a_btn = SteamVR_Controller.Input(rightIndex).GetPressDown(Valve.VR.EVRButtonId.k_EButton_A);
            if (a_btn && !gameLogic.gameOver)
            {
                ball.transform.position = ball_start_pos;
                ball.velocity = Vector3.zero;
                ball.angularVelocity = Vector3.zero;
                gameLogic.ball_reset(); // increments n_ball_resets
            }

            // reset game (cups, ball, score)
            bool b_btn = SteamVR_Controller.Input(rightIndex).GetPressDown(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu);
            if (b_btn)
            {
                // ball
                ball.transform.position = ball_start_pos;
                ball.velocity = Vector3.zero;
                ball.angularVelocity = Vector3.zero;
                //cups
                for (int j = 0; j < 3; j++)
                {
                    // each stack of cups is a child of the cups obj
                    Transform cup_stack = cups.GetChild(j).transform;
                    for (int i = 0; i < 6; i++)
                    {
                        Transform cup = cup_stack.GetChild(i);
                        cup.transform.position = cup_positions[j, i];
                        cup.transform.rotation = cup_rotations[j, i];
                        cup.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        cup.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                        cup.GetComponent<Cup>().knocked_over = false;
                    }
                }
                // everything else
                gameLogic.game_reset(); // sets score to 0
            }

            if (rightHand.intersected != null && rightTrigger > 0.2f)
            {
                // pick up right
                rightHeldObj = rightHand.intersected;
                saveMaxRight = rightHand.intersected.maxAngularVelocity;
                rightHand.intersected.maxAngularVelocity = Mathf.Infinity;
            }

            if (rightHeldObj != null && rightTrigger <= 0.2f)
            {
                // release right object
                rightHeldObj.velocity = SteamVR_Controller.Input(rightIndex).velocity;
                rightHeldObj.angularVelocity = SteamVR_Controller.Input(rightIndex).angularVelocity;
                rightHeldObj.maxAngularVelocity = saveMaxRight;
                rightHeldObj = null;
            }

            if (rightHeldObj != null)
            {
                // force object to follow right hand
                rightHeldObj.velocity = (rightHand.transform.position - rightHeldObj.position) / Time.deltaTime;
                // update rotation
                float angle;
                Vector3 axis;
                Quaternion q = rightHand.transform.rotation * Quaternion.Inverse(rightHeldObj.rotation);
                q.ToAngleAxis(out angle, out axis);
                rightHeldObj.angularVelocity = axis * angle * Mathf.Deg2Rad / Time.deltaTime;
            }

        } // RIGHT HAND

        // LEFT HAND
        if (leftIndex >= 0)
        {
            float leftTrigger = SteamVR_Controller.Input(leftIndex).GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).magnitude;

            if (leftHand.intersected != null && leftTrigger > 0.2f)
            {
                // pick up left
                leftHeldObj = leftHand.intersected;
                saveMaxLeft = leftHeldObj.maxAngularVelocity;
                leftHeldObj.maxAngularVelocity = Mathf.Infinity;
            }

            if (leftHeldObj != null && leftTrigger <= 0.2f)
            {
                // release left object
                leftHeldObj.velocity = SteamVR_Controller.Input(leftIndex).velocity;
                leftHeldObj.angularVelocity = SteamVR_Controller.Input(leftIndex).angularVelocity;
                leftHeldObj.maxAngularVelocity = saveMaxLeft;
                leftHeldObj = null;
            }

            if (leftHeldObj != null)
            {
                // force object to follow left hand
                leftHeldObj.velocity = (leftHand.transform.position - leftHeldObj.position) / Time.deltaTime;
                // update rotation
                float angle;
                Vector3 axis;
                Quaternion q = leftHand.transform.rotation * Quaternion.Inverse(leftHeldObj.rotation);
                q.ToAngleAxis(out angle, out axis);
                leftHeldObj.angularVelocity = axis * angle * Mathf.Deg2Rad / Time.deltaTime;
            }

        } // LEFT HAND

    } // LateUpdate


}
