﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public VRPlayer player;
    public Vector3 controllerVelocity;
    public Vector3 controllerAngularVelocity;

    public Rigidbody intersected;
    public bool isTeleporting = false;
    public bool isTeleLocationValid = false;
    public Transform teleporterBase;
    public float teleporterArcSpeed = 10.0f;
    public Vector3 teleporterArcGravity = new Vector3(0, -9.8f, 0);
    public float maxDistance = 100.0f;

    public GameObject telePrefab;
    public List<GameObject> telePoints = new List<GameObject>(); // used to keep track of game objects
    public GameObject teleHitVisual;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnTriggerStay(Collider other)
    {
        if (intersected == null)
        {
            intersected = other.attachedRigidbody;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        intersected = null;
    }

    // visualization of teleporter
    public void joystick(Vector2 direction)
    {
        float mag = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        //angle = 0 means fully pushed up, 90/-270 means pushed to the left
        Vector3 hitPos;
        int currTelePoint = 0;
        teleHitVisual.SetActive(true);

        for (int i = 0; i < telePoints.Count; i++)
        {
            telePoints[i].SetActive(false);
        } 

        if (isTeleporting)
        {
           
            //calculate arc points/distance
            float distanceTraveled = 0.0f;
            Vector3 currentPos = teleporterBase.position;
            Vector3 currentVel = teleporterBase.forward * teleporterArcSpeed;
            float deltaTime = teleporterArcSpeed / 500.0f;

            while (distanceTraveled < maxDistance)
            {
                //draw ray
                if (telePoints.Count <= currTelePoint)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(telePrefab);
                    go.transform.SetParent(this.transform);
                    telePoints.Add(go);
                } // add points to teleporter if it's not big enough
                Vector3 nextPos = currentPos + currentVel * deltaTime + .5f * teleporterArcGravity * deltaTime * deltaTime;
                Vector3 nextVel = currentVel + teleporterArcGravity * deltaTime; // needed because acceleration
                Vector3 between = nextPos - currentPos;

                telePoints[currTelePoint].SetActive(true);
                telePoints[currTelePoint].transform.position = currentPos;
                telePoints[currTelePoint].transform.forward = between.normalized;

                
                //time to form our raycast

                RaycastHit hit;
                if (Physics.Raycast(new Ray(currentPos, between.normalized), out hit, between.magnitude))
                { // vector from current pos towards the next one will go as far as distance between
                    hitPos = hit.point;
                    isTeleLocationValid = true;
                    teleHitVisual.SetActive(true);
                    teleHitVisual.transform.position = hitPos;
                    teleHitVisual.transform.forward = new Vector3(teleporterBase.forward.x, 0, teleporterBase.forward.y);
                    //orients hit visusalization 
                    break; // break to not continue once something is hit
                } //enters if something is hit with raycast

                currTelePoint++;
                currentPos = nextPos;
                currentVel = nextVel;
                distanceTraveled += between.magnitude;
            }

        } // do the drawing and physics raycasting







        if (mag > .1f && !isTeleporting)  
        {
            isTeleporting = true;
        } else if (mag < .05f && isTeleporting && isTeleLocationValid)
        {
            isTeleporting = false;
        }

    }

}
