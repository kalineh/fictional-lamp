using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GhostController
    : MonoBehaviour
{
    public GameObject origin;
    public GameObject character;
    public GameObject head;
    public GameObject virtualHead;

    public bool fixedRadiusStyle;

    public void Update()
    {
        var indexLeft = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
        var indexRight = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);

        var touchpad = Vector2.zero;
        var touchpadDown = false;
        var gripDown = false;

        if (indexLeft != -1)
        {
            var left = SteamVR_Controller.Input(indexLeft);
            touchpad += left.GetAxis();
            touchpadDown = touchpadDown || left.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
            gripDown = gripDown || left.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip);
        }

        if (indexRight != -1)
        {
            var right = SteamVR_Controller.Input(indexRight);
            touchpad += right.GetAxis();
            touchpadDown = touchpadDown || right.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
            gripDown = gripDown || right.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip);
        }

        var planeX = head.transform.right;
        var planeZ = head.transform.forward;

        planeX.y = 0.0f;
        planeZ.y = 0.0f;

        planeX.Normalize();
        planeZ.Normalize();

        if (fixedRadiusStyle)
        {
            var range = 4.0f;

            var touchX = Mathf.Sign(touchpad.x) * Mathf.Pow(Mathf.Abs(touchpad.x), 0.9f);
            var touchY = Mathf.Sign(touchpad.y) * Mathf.Pow(Mathf.Abs(touchpad.y), 0.9f);

            transform.position = new Vector3(
                origin.transform.position.x,
                transform.position.y,
                origin.transform.position.z
            );
            transform.position += planeX * touchX * range;
            transform.position += planeZ * touchY * range;
        }
        else
        {
            var moved = Vector3.zero;
            var speed = 3.5f;

            var touchX = Mathf.Sign(touchpad.x) * Mathf.Pow(Mathf.Abs(touchpad.x), 0.6f);
            var touchY = Mathf.Sign(touchpad.y) * Mathf.Pow(Mathf.Abs(touchpad.y), 0.6f);

            moved += planeX * touchX * Time.deltaTime * speed;
            moved += planeZ * touchY * Time.deltaTime * speed;

            transform.position += moved;
        }

        var height = virtualHead.transform.localPosition.y;
        var info = new RaycastHit();
        var mask = LayerMask.GetMask("Default");

        var hit = Physics.Raycast(virtualHead.transform.position, Vector3.down, out info, height, mask, QueryTriggerInteraction.Ignore);

        //Debug.DrawLine(virtualHead.transform.position, virtualHead.transform.position + Vector3.down * height, hit ? Color.green : Color.red, 0.2f);

        if (hit)
        {
            var penetrate = info.distance - height;

            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y - penetrate,
                transform.localPosition.z);
        }
        else
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y - 0.10f,
                transform.localPosition.z);

            var hit2 = Physics.Raycast(virtualHead.transform.position, Vector3.down, out info, height, mask, QueryTriggerInteraction.Ignore);

            if (hit2)
            {
                var penetrate2 = info.distance - height;

                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    transform.localPosition.y - penetrate2,
                    transform.localPosition.z);
            }
        }

        if (gripDown)
            origin.transform.DOMove(transform.position, 0.09f);
        //if (touchpadDown)
            //origin.transform.DOMove(character.transform.position, 0.09f);
            //transform.DOMove(origin.transform.position, 0.07f);

        var characterBody = character.GetComponentInChildren<Rigidbody>();
        var characterOfs = virtualHead.transform.position - characterBody.position;

        if (characterOfs.sqrMagnitude > 0.01f)
        {
            var characterDir = characterOfs.normalized;
            var characterMove = characterOfs * 5000.0f * Time.deltaTime;

            characterBody.AddForce(characterMove);
        }
    }
}
