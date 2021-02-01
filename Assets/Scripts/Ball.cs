using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField]
    bool attached = false;

    [SerializeField]
    public Rope rope;

    Queue<Vector3> positionHistory = new Queue<Vector3>();

    [SerializeField]
    int positionHistoryLimit = 5;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {

        if (attached)
        {
            rb.MovePosition(rope.attachPoint);
        }


        
    }


    private void LateUpdate()
    {
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

        rb.velocity = (transform.position - positionHistory.Peek()) / (Time.deltaTime*positionHistoryLimit);
        

        //Debug.DrawRay(transform.position, rb.velocity, Color.red, 10);

        attached = false;
    }

}
