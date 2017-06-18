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
        var touchpadPress = false;
        var gripPress = false;

        if (indexLeft != -1)
        {
            var left = SteamVR_Controller.Input(indexLeft);
            touchpad += left.GetAxis();
            touchpadPress = touchpadPress || left.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
            gripPress = gripPress || left.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip);
        }

        if (indexRight != -1)
        {
            var right = SteamVR_Controller.Input(indexRight);
            touchpad += right.GetAxis();
            touchpadPress = touchpadPress || right.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
            gripPress = gripPress || right.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip);
        }

        var planeX = origin.transform.right;
        var planeZ = origin.transform.forward;

        planeX.y = 0.0f;
        planeZ.y = 0.0f;

        planeX.Normalize();
        planeZ.Normalize();

        var moved = Vector3.zero;
        var speed = 1.0f;

        if (touchpadPress)
            speed = 3.5f;

        moved += planeX * touchpad.x * Time.deltaTime * speed;
        moved += planeZ * touchpad.y * Time.deltaTime * speed;

        transform.position += moved;

        if (gripPress)
            origin.transform.DOMove(transform.position, 0.12f);
    }
}
