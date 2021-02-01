using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{

    Vector2 velocity;
    Vector3 previousPos;

    Rigidbody2D rb;

    [SerializeField]
    bool attached = false;

    [SerializeField]
    public Rope rope;

    Queue<Vector3> positionHistory = new Queue<Vector3>();

    [SerializeField]
    int positionHistoryLimit = 5;

    //float angularVelocity = 0.0f;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {

        if (attached)
        {
            rb.MovePosition(rope.attachPoint);
            //velocity = transform.position - previousPos;
            //angularVelocity = FindAngularVelocity();
        }


        
    }


    private void LateUpdate()
    {
        //previousPos = transform.position;
        positionHistory.Enqueue(transform.position);
        while (positionHistory.Count > positionHistoryLimit)
        {
            positionHistory.Dequeue();
        }
    }


    public void AttachToRope(Rope rope)
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        this.rope = rope;
        attached = true;
    }

    public void DetachFromRope()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;

        // velocity = angular velocity * radius * unit vector orthogonal to radius
        //Vector2 posToCenter = (Vector2)transform.position - GameManager.touchPosition;
        //float radius = posToCenter.magnitude;
        //Vector2 velocityDirection = Vector3.Cross(posToCenter, Vector3.back).normalized;
        //rb.velocity = (angularVelocity * radius * velocityDirection);

        rb.velocity = (transform.position - positionHistory.Peek()) / (Time.deltaTime*positionHistoryLimit);
        

        //Debug.DrawRay(transform.position, rb.velocity, Color.red, 10);

        attached = false;
    }



    //public float FindAngularVelocity()
    //{
    //    
    //    Vector2 prev = (Vector2)positionHistory.Peek() - GameManager.touchPosition;
    //    Vector2 curr = (Vector2)transform.position - GameManager.touchPosition;
    //
    //
    //    return Vector2.SignedAngle(prev, curr);
    //
    //}


    //private void OnGUI()
    //{
    //    GUILayout.Label("current angular velocity: " + angularVelocity);
    //    GUILayout.Label("position difference: " + (transform.position - positionHistory.Peek()) );
    //}

}
