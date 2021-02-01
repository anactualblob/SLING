using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Rope : MonoBehaviour
{

    LineRenderer lineRenderer;
    List<RopeSegment> ropeSegments = new List<RopeSegment>();


    [Header("Physics")]
    [SerializeField] float gravityForce = 1;
    Vector2 gravityVector
    {
        get { return Vector2.down * gravityForce; }
    }
    
    [Space]
    [SerializeField] int constraintsIteration = 100;


    [Header("Rope Parameters")]
    [SerializeField] float ropeLength = 1.0f;
    [SerializeField] int numberOfSegments = 35;
    float ropeSegmentLength
    {
        get { return ropeLength / numberOfSegments; }
    }



    [HideInInspector]
    public Vector3 ropeStart = Vector3.zero;
    [HideInInspector]
    public Vector3 ropeEnd = Vector3.zero;
    
    public Vector2 attachPoint
    {
        get { return ropeSegments[numberOfSegments - 1].posNow; }
    }



    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // create the rope
        for (int i = 0; i < numberOfSegments; ++i)
        {
            ropeSegments.Add(new RopeSegment(ropeStart + Vector3.down * i * ropeSegmentLength)); ;
        }


        gameObject.SetActive(false);
    }



    void FixedUpdate()
    {
        SimulateRope();
        ConstrainSegments(false);
        DrawRope();
    }
    


    void SimulateRope()
    {
        Vector2 velocity;
        for (int i = 0; i < ropeSegments.Count; i++)
        {
            RopeSegment seg = ropeSegments[i];

            velocity = seg.posNow - seg.posOld;
            seg.posOld = seg.posNow;
            velocity += gravityVector * Time.deltaTime;
            seg.posNow += velocity;

            ropeSegments[i] = seg;
        }
    }

    void ConstrainSegments(bool endConnected)
    {
        // first point always follows ropeStart
        RopeSegment first = ropeSegments[0];
        first.posNow = ropeStart;
        ropeSegments[0] = first;

        

        // repeat this constraining multiple times for accurate results
        for (int j = 0; j < constraintsIteration; j++)
        { 
            // move rope points closer if they're too far or farther if they're too close
            for (int i = 0; i < numberOfSegments-1; i++)
            {
                RopeSegment seg = ropeSegments[i];
                RopeSegment nextSeg = ropeSegments[i + 1];


                // get the normalized to get the magnitude using the dot trick
                //    (if a vector is aligned with a unit vector, its projection 
                //    on that unit vector is equal to its length)
                // this allows to only use one sqrt
                Vector3 dir = (seg.posNow - nextSeg.posNow).normalized;
                float dist = Vector3.Dot((seg.posNow - nextSeg.posNow), dir);
                float error = dist - ropeSegmentLength;
                //
                //// the dir is normalized and points to the next segment by default. 
                //// if dist is bigger than the segment length, error is negative
                Vector2 change = dir * error;

                if (i == 0)
                {
                    // if we're on the first point, it shouldn't move so the next 
                    //    point will do all of the change
                    nextSeg.posNow += change;
                }
                else
                {
                    // both points do half the change
                    seg.posNow -= change * 0.5f;
                    nextSeg.posNow += change * 0.5f;
                }


                // update the segments in the list
                ropeSegments[i] = seg;
                ropeSegments[i + 1] = nextSeg;
            }

            if (endConnected)
            {
                // last point always follows ropeEnd
                RopeSegment last = ropeSegments[numberOfSegments - 1];
                last.posNow = ropeEnd;
                ropeSegments[numberOfSegments - 1] = last;
            }
            

        }


        
    }

    void DrawRope()
    {
        Vector3[] ropePositions = new Vector3[numberOfSegments];

        for (int i = 0; i < numberOfSegments; ++i)
        {
            ropePositions[i] = ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = numberOfSegments;
        lineRenderer.SetPositions(ropePositions);
    }






    public void InitializeRope(Vector3 ropeStartPos, Vector3 ropeEndPos)
    {
        ropeStart = ropeStartPos;
        ropeEnd = ropeEndPos;

        // set positions of the rope so that it's vertical
        for (int i = 0; i < ropeSegments.Count; i++)
        {
            RopeSegment seg = ropeSegments[i];

            seg.posNow = ropeStartPos + Vector3.down * i;
            seg.posOld = Vector3.zero;

            ropeSegments[i] = seg;
        }

        // apply constraints with and make sure the end of the rope is pinned to ropeEnd
        ConstrainSegments(true);
    }





    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }
}
