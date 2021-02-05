using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TrailRenderer))]
public class Ball : MonoBehaviour
{
    Rigidbody2D rb;
    TrailRenderer trail;

    bool attached = false;
    Rope rope;

    Queue<Vector3> positionHistory = new Queue<Vector3>();

    [Tooltip("Determines how many frames the position history contains. \n" +
        "This is used when the ball detaches from the rope, because its velocity will be " +
        "the difference between its position then and the oldest position in the history.")]
    [SerializeField] int positionHistoryLimit = 5;







    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
    }


    void Update()
    {

        if (attached)
        {
            rb.MovePosition(rope.attachPoint);
            trail.enabled = false;
        }
        else
        {
            trail.enabled = true;
        }


        
    }


    private void LateUpdate()
    {
        // keep a history of positions from previous frames
        positionHistory.Enqueue(transform.position);
        while (positionHistory.Count > positionHistoryLimit)
        {
            positionHistory.Dequeue();
        }
    }


    public void AttachToRope(Rope rope)
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        rb.Sleep();
        this.rope = rope;
        attached = true;
    }

    public void DetachFromRope()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.WakeUp();
        rb.gravityScale = 1;

        rb.velocity = (transform.position - positionHistory.Peek()) / (Time.deltaTime*positionHistoryLimit);
        

        //Debug.DrawRay(transform.position, rb.velocity, Color.red, 10);

        attached = false;
    }

}
