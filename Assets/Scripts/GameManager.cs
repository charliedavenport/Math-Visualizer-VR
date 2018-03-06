using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
public class GameManager : MonoBehaviour
{

    public GameObject vrCameraRig;
    public GameObject nonVRCameraRig;
    //public Blinker hmdBlinker;
    public SteamVR_TrackedObject hmd2;
    public SteamVR_TrackedObject controllerLeft;
    public SteamVR_TrackedObject controllerRight;
    
    
    public void enableVR()
    {

        StartCoroutine(doEnableVR());


    }
    IEnumerator doEnableVR()
    {
        while (UnityEngine.XR.XRSettings.loadedDeviceName != "OpenVR")
        {
            UnityEngine.XR.XRSettings.LoadDeviceByName("OpenVR");
            yield return null;
        }
        UnityEngine.XR.XRSettings.enabled = true;
        vrCameraRig.SetActive(true);
        nonVRCameraRig.SetActive(false);
    }
}
