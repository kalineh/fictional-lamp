using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CustomController
    : SteamVR_TrackedController
{
    public override void OnTriggerClicked(ClickedEventArgs e)
    {
        base.OnTriggerClicked(e);

        var fwd = transform.forward;
        var hitInfo = new RaycastHit();
        var hit = Physics.Raycast(transform.position, transform.forward, out hitInfo);
        if (hit)
        {
            transform.parent.DOMove(hitInfo.point, 0.12f).SetEase(Ease.Linear);
        }
    }

    //public void Update()
    //{
        //Debug.DrawLine(transform.position, transform.position + transform.forward * 20.0f, Color.red);
    //}
}
