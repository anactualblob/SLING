using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    BoxCollider2D leftWall;
    BoxCollider2D rightWall;

    [HideInInspector]
    public Camera cam;

    float hSize;


    Vector2 targetPos;

    [SerializeField] private float maxDistancePerFrame;


    void Start()
    {
        leftWall = transform.Find("leftWall").GetComponent<BoxCollider2D>();
        if (leftWall == null) Debug.LogError("CameraController.cs : Cannot find BoxCollider2D leftWall in children.", this);

        rightWall = transform.Find("rightWall").GetComponent<BoxCollider2D>();
        if (rightWall == null) Debug.LogError("CameraController.cs : Cannot find BoxCollider2D rightWall in children.", this);

        cam = GetComponent<Camera>();

        PlaceColliders();
    }

    
    void FixedUpdate()
    {
        // update position according to targetPos  
        //transform.position = (Vector3)targetPos - Vector3.forward * 10;

        Vector2 posToTarget = targetPos - (Vector2)transform.position;

        if (posToTarget.magnitude < maxDistancePerFrame)
        {
            transform.position = targetPos;
        }
        else
        {
            transform.position += Vector3.up * maxDistancePerFrame;
        }
        //transform.position = Vector3.MoveTowards(transform.position, (Vector3)targetPos, 0.1f);


        // ensure we're moved back 10 units
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }



    void PlaceColliders()
    {
        hSize = cam.orthographicSize * cam.aspect;

        leftWall.transform.position = transform.position - Vector3.right * (hSize + (leftWall.size.x / 2));
        rightWall.transform.position = transform.position + Vector3.right * (hSize + (rightWall.size.x / 2));
    }

    public void SetTargetPosition(Vector2 pos)
    {
        targetPos = pos;
    }
}
