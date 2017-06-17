using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GhostController
    : MonoBehaviour
{
    public GameObject origin;
    public GameObject character;

    public void Update()
    {
        var indexLeft = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
        var indexRight = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);

        var touchpad = Vector2.zero;

        if (indexLeft != -1)
            touchpad += SteamVR_Controller.Input(indexLeft).GetAxis();
        if (indexRight != -1)
            touchpad += SteamVR_Controller.Input(indexRight).GetAxis();

        var planeX = origin.transform.right;
        var planeZ = origin.transform.forward;

        planeX.y = 0.0f;
        planeZ.y = 0.0f;

        planeX.Normalize();
        planeZ.Normalize();

        var moved = Vector3.zero;

        moved += planeX * touchpad.x * Time.deltaTime * 1.5f;
        moved += planeZ * touchpad.y * Time.deltaTime * 1.5f;

        transform.position += moved;
    }
}
