using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Bools")]
    public bool isLevelStart;
    public bool isLevelDone;
    public bool isLevelFail;

    Transform Player;
    Transform target;

    [Header("Settings")]
    public Vector3 offSet;
    public Vector3 upOffset;

    public float smoothTime;


    Vector3 targetPosition;

    public float rotateSpeed;

    private Vector3 velocity = Vector3.zero;

    public static CameraController instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Player = PlayerController.instance.transform;
        target = Player;
    }

    private void LateUpdate()
    {
        if (isLevelStart && !isLevelDone && !isLevelFail)
        {
            targetPosition = target.transform.position + offSet;
            //targetPosition.x = 0;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            //transform.position = Vector3.Lerp(transform.position, targetPosition, smoothTime);            
        }
        else if (isLevelDone)
        {
            targetPosition = target.transform.position + offSet;
            transform.position = new Vector3(transform.position.x, Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime).y, transform.position.z);

            Vector3 targetRotation = target.position - transform.position + upOffset;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetRotation), Time.deltaTime * 3);

            transform.RotateAround(target.position, Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
    public void EndGameMovement()
    {
        offSet.y = 2;
        offSet.y = 2;
        isLevelDone = true;
    }
}
