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

        InitBall();
    }

    // initial state of the ball (in the center, immovable, waiting for touch to attach to a rope)
    public void InitBall()
    {
        transform.position = Vector3.zero;
        positionHistory.Clear();
        rb.bodyType = RigidbodyType2D.Kinematic;
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
        this.rope = rope;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        rb.Sleep();
        rb.MovePosition(rope.attachPoint);

        attached = true;
    }

    public void DetachFromRope()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.WakeUp();
        rb.gravityScale = 1;

        rb.velocity = (transform.position - positionHistory.Peek()) / (Time.deltaTime*positionHistoryLimit);
        

        attached = false;
    }


}
