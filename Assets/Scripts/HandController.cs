using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HandController : MonoBehaviour
{

    public VRPlayer player;
    public Vector3 controllerVelocity;
    public Vector3 controllerAngularVelocity;

    private Collider lastIntersection;
    public float maxSpeed = 5.0f; //don't let the object move too fast between frames
    public bool teleporterActive = false;

    public Transform teleporterBase; //set in the inspector, z forward is the telporter starting trajectory
    public float teleporterArcSpeed = 10.0f;
    public Vector3 teleporterArcGravity = new Vector3(0, -9.8f, 0);
    public float arcMaxDistance = 100.0f;

    public GameObject teleporterPointPrefab;
    public GameObject teleporterHitVisualization; //set in the inspector
    public List<GameObject> teleporterPoints = new List<GameObject>();
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
        if (other.attachedRigidbody != null)
        {
            lastIntersection = other;
        }
    }
    private void OnTriggerExit()
    {
        lastIntersection = null;
    }




}
