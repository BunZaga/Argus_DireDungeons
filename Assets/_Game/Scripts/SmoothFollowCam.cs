using UnityEngine;

public class SmoothFollowCam
{
    private Transform target;
    private Transform camTrans;
    
    private Vector3 velocity = Vector3.zero;
    private Vector3 oldPosition = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;
    private float smoothTime;

    public SmoothFollowCam(Transform target, Transform camTrans)
    {
        this.target = target;
        this.camTrans = camTrans;
        oldPosition = target.position;
        camTrans.position = oldPosition;
        camTrans.rotation = target.rotation;
    }
    
    public void ManualInstantUpdate()
    {
        camTrans.position = target.position;
        camTrans.rotation = target.rotation;
    }

    public void ControlledLateUpdate()
    {
        targetPosition = target.position;
        var camPos = camTrans.position;
        var diff = targetPosition - camPos;
        diff.y = 0;
        var diffX = Mathf.Abs(diff.x);
        var diffZ = Mathf.Abs(diff.z);
        var distance = diffX > diffZ ? diffX : diffZ;
        var diffNormal = diff.normalized;

        var newPos = Vector3.Lerp(camPos, camPos + diffNormal * distance, Time.unscaledDeltaTime * 2);
        camTrans.position = newPos;
    }
}
