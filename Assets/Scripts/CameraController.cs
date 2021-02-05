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

    [SerializeField]
    float hSize;


    Vector2 targetPos;


    void Start()
    {
        leftWall = transform.Find("leftWall").GetComponent<BoxCollider2D>();
        if (leftWall == null) Debug.LogError("CameraController.cs : Cannot find BoxCollider2D leftWall in children.", this);

        rightWall = transform.Find("rightWall").GetComponent<BoxCollider2D>();
        if (rightWall == null) Debug.LogError("CameraController.cs : Cannot find BoxCollider2D rightWall in children.", this);

        cam = GetComponent<Camera>();

        PlaceColliders();
    }

    
    void LateUpdate()
    {
        // update position according to targetPos  
        transform.position = (Vector3)targetPos - Vector3.forward * 10;
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
