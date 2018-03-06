﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public VRPlayer player;
    public Vector3 controllerVelocity;
    public Vector3 controllerAngularVelocity;

	Transform attachPoint;
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
		/*
		 attachPoint = (new GameObject()).transform;
		 attachPoint.SetParent(this.transform);
		 attachPoint.localPosition = Vector3.zero;
		 attachPoint.localRotation = Quaternion.identity;
		  */

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
    /*  public void joystick(Vector2 direction)
      {
          float mag = direction.magnitude;
          float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
          //angle = 0 means fully pushed up, 90/-270 means pushed to the left
          // bool validLocation = false;
          Vector3 hitPos = Vector3.zero;
          Vector3 hitDir = Vector3.forward;
          int currTelePoint = 0;
          teleHitVisual.SetActive(true);

          for (int i = 0; i < telePoints.Count; i++)
          {
              telePoints[i].SetActive(false);
          }
          //  teleHitVisual.SetActive(false);

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

                  //draw ray


                  //time to form our raycast

                  RaycastHit hit;
                  if (Physics.Raycast(new Ray(currentPos, between.normalized), out hit, between.magnitude))
                  { // vector from current pos towards the next one will go as far as distance between

                      hitPos = hit.point;
                      isTeleLocationValid = true;
                      teleHitVisual.SetActive(true);
                      teleHitVisual.transform.position = hitPos;
                      hitDir = new Vector3(teleporterBase.forward.x, 0, teleporterBase.forward.z);
                      hitDir = Quaternion.AngleAxis(-angle, Vector3.up) * hitDir;
                      // rotates by hit directrion to allow angle of joystick to change facing direction
                      teleHitVisual.transform.forward = hitDir;
                      //orients hit visusalization 
                      break; // break to not continue once something is hit
                  } //enters if something is hit with raycast


                  currTelePoint++;
                  currentPos = nextPos;
                  currentVel = nextVel;
                  distanceTraveled += between.magnitude;

              }

          } // do the drawing and physics raycasting







          if (mag > .9f && !isTeleporting)
          {
              isTeleporting = true;
          }
          else if (mag < .85f && isTeleporting && isTeleLocationValid)
          {
              isTeleporting = false;
              player.teleport(hitPos, hitDir);
          }


      }

  */

    public void joystick(Vector2 direction)
    {

        float mag = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        //angle = 0 means fully pushed up, 90/-270 means pushed to the left

        bool validLocation = false;
        Vector3 hitPos = Vector3.zero;
        Vector3 hitDir = Vector3.forward;
        for (int i = 0; i < telePoints.Count; i++)
        {
            telePoints[i].SetActive(false);
        }
        teleHitVisual.SetActive(false);
        if (isTeleporting)
        {
            //compute arc points
            float distanceTraveled = 0.0f;
            Vector3 currentPos = teleporterBase.position;
            Vector3 currentVel = teleporterBase.forward * teleporterArcSpeed;

            int currentTeleporterPoint = 0;


            float dt = teleporterArcSpeed / 1000.0f;
            while (distanceTraveled < maxDistance)
            {


                Vector3 nextPos = currentPos + currentVel * dt + .5f * teleporterArcGravity * dt * dt;
                Vector3 nextVel = currentVel + teleporterArcGravity * dt;
                Vector3 between = nextPos - currentPos;

                //draw ray
                if (telePoints.Count <= currentTeleporterPoint)
                {
                    //add  a point to the telporter
                    GameObject go = GameObject.Instantiate<GameObject>(telePrefab);
                    go.transform.SetParent(this.transform);
                    telePoints.Add(go);

                }
                telePoints[currentTeleporterPoint].SetActive(true);
                telePoints[currentTeleporterPoint].transform.position = currentPos;
                telePoints[currentTeleporterPoint].transform.forward = between.normalized;

                //perform raycast

                RaycastHit hit;
                if (Physics.Raycast(new Ray(currentPos, between.normalized), out hit, between.magnitude))
                {
                    validLocation = true;
                    hitPos = hit.point;
                    hitDir = new Vector3(teleporterBase.forward.x, 0, teleporterBase.forward.z);
                    hitDir = Quaternion.AngleAxis(-angle, Vector3.up) * hitDir;
                    teleHitVisual.SetActive(true);
                    teleHitVisual.transform.position = hitPos;
                    teleHitVisual.transform.forward = hitDir;
                    break;
                }

                currentPos = nextPos;
                currentVel = nextVel;
                distanceTraveled += between.magnitude;
                currentTeleporterPoint++;
            }


        }


        if (mag > .9f && !isTeleporting)
        {
            isTeleporting = true;
        }
        else if (mag < .85f && isTeleporting && validLocation)
        {
            isTeleporting = false;
            player.teleport(hitPos, hitDir);
        }


    }

}

