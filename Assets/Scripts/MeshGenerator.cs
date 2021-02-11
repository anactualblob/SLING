using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(NoiseGenerator))]
public class MeshGenerator : MonoBehaviour
{
    protected class Obstacle
    {
        public MeshFilter meshFilter = null;
        public EdgeCollider2D edgeCollider = null;
        //public PolygonCollider2D polyCollider = null;
        //public MeshCollider meshCollider = null;

        public Mesh mesh = null;

        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector2> colliderPoints = new List<Vector2>();
        public List<int> triangles = new List<int>();

        public float[] noiseChunk = { };
        public float randomNoiseOffset = 0;

        public Obstacle(GameObject source, int chunkLength)
        {
            meshFilter = source.GetComponent<MeshFilter>();
            edgeCollider = source.GetComponent<EdgeCollider2D>();
            //polyCollider = source.GetComponent<PolygonCollider2D>();
            //meshCollider = source.GetComponent<MeshCollider>();

            if (meshFilter == null)
                Debug.LogError("MeshGenerator.cs : Cannot create an obstacle from the given game object because it is missing a mesh filter.", source);

            if (edgeCollider == null)
                Debug.LogError("MeshGenerator.cs : Cannot create an obstacle from the given game object because it is missing an EdgeCollider2D.", source);
            
            //if (polyCollider == null)
            //    Debug.LogError("MeshGenerator.cs : Cannot create an obstacle from the given game object because it is missing a PolygonCollider2D.", source);
            
            //if (meshCollider == null)
            //    Debug.LogError("MeshGenerator.cs : Cannot create an obstacle from the given game object because it is missing a MeshCollider.", source);

            randomNoiseOffset = Random.Range(0.0f, 100000.0f);
            mesh = new Mesh();
            mesh.name = "newerer mesh";

            noiseChunk = new float[chunkLength];
        }


        public void Reset()
        {
            randomNoiseOffset = Random.Range(0.0f, 100000.0f);
            mesh = new Mesh();
            mesh.name = "newerer mesh";

            vertices.Clear();
            triangles.Clear();
            colliderPoints.Clear();
        }
    }

    Obstacle left, right;

    NoiseGenerator noiseGenerator;


    [SerializeField] int verticesPerUnit = 10;
    [SerializeField] int chunkSize = 10;
    int verticesInChunk
    {
        get { return verticesPerUnit * chunkSize * 2; }
    }
    int nbChunks
    {
        get { return (int)((cameraSize.y*2 / (float)chunkSize)+ 1) + 1; }
    }


    float buildHeight;

    [HideInInspector] public float cameraCurrentHeight;
    [HideInInspector] public float startingHeight;
    [HideInInspector] public Vector2 cameraSize;


    [Space]
    [SerializeField]
    GameObject leftObstacleHolder = null;
    [SerializeField]
    GameObject rightObstacleHolder = null;

    [Header("Debug")]
    [SerializeField] int startChunks = 0;
    [Space]
    [SerializeField] float scale = 1.0f;
    [SerializeField] int octaves = 1;
    [SerializeField] float lacunarity = 1.0f;
    [SerializeField] float persistence = 0.5f;





    void Start()
    {
        left = new Obstacle(leftObstacleHolder, chunkSize * verticesPerUnit);
        right = new Obstacle(rightObstacleHolder, chunkSize * verticesPerUnit);


        noiseGenerator = GetComponent<NoiseGenerator>();

        SetupFirstObstacles();
    }


    private void Update()
    {
        //Debug.Log("bottom : " + (currentHeight - nbChunks * chunkSize));
        //Debug.Log("nbChunks: " + nbChunks);
        //Debug.Log("Current height: " + currentHeight);
        

        
        // if the bottom edge of the camera is higher than the bottom chunk of the obstacles
        if (cameraCurrentHeight - cameraSize.y > buildHeight - (nbChunks - 1) *chunkSize)
        {
            BuildObstacles();
        }
    }




    public void SetupFirstObstacles()
    {
        buildHeight = transform.position.y - cameraSize.y;
        left.Reset();
        right.Reset();

        for (int i = 0; i < nbChunks; i++)
        {
            BuildObstacles();
        }
    }



    public void BuildObstacles()
    {
        // left obstacle mesh
        GetAndProcessNoise(ref left.noiseChunk, scale, left.randomNoiseOffset);
        AddVertices(ref left, Vector3.left*cameraSize.x + Vector3.up * (buildHeight));

        if (left.vertices.Count > verticesInChunk * nbChunks)
            RemoveBottomVertices(ref left, verticesInChunk);

        RebuildTriangles(ref left, left.vertices.Count);
        AssignMesh(ref left);


        // right obstacle mesh
        GetAndProcessNoise(ref right.noiseChunk, scale, right.randomNoiseOffset);
        AddVertices(ref right, Vector3.right * cameraSize.x + Vector3.up * (buildHeight), true);

        if (right.vertices.Count > verticesInChunk * nbChunks) 
            RemoveBottomVertices(ref right, verticesInChunk);

        RebuildTriangles(ref right, right.vertices.Count);
        AssignMesh(ref right);


        // new height
        buildHeight += chunkSize;
    }


    
    
    



    void GetAndProcessNoise(ref float[] buffer, float scale, float offset = 0.0f)
    {
        noiseGenerator.GenerateNoiseChunk(ref buffer, scale, offset, octaves,lacunarity, persistence);

        // process noise in buffer
        for (int i = 0; i < buffer.Length; i++)
        {
            // TEMPORARY
            buffer[i] *= 0.5f + 1.5f * buildHeight / 100;
        }
    }


    void AddVertices(ref Obstacle obs, Vector3 offset, bool faceLeft = false)
    {
        for (int i = 0; i < obs.noiseChunk.Length; ++i)
        {
            if (faceLeft)
            {
                obs.vertices.Add(offset + new Vector3(-obs.noiseChunk[i], i / (float)verticesPerUnit));
                obs.colliderPoints.Add(offset + new Vector3(-obs.noiseChunk[i], i / (float)verticesPerUnit));

                obs.vertices.Add(offset + new Vector3(0, i / (float)verticesPerUnit));
                //obs.colliderPoints.Add(offset + new Vector3(0, i / (float)verticesPerUnit));
            }
            else
            {
                obs.vertices.Add(offset + new Vector3(0, i / (float)verticesPerUnit));
                //obs.colliderPoints.Add(offset + new Vector3(0, i / (float)verticesPerUnit));

                obs.vertices.Add(offset + new Vector3(obs.noiseChunk[i], i / (float)verticesPerUnit));
                obs.colliderPoints.Add(offset + new Vector3(obs.noiseChunk[i], i / (float)verticesPerUnit));
            }

        }
    }

    void RemoveBottomVertices(ref Obstacle obs, int count)
    {
        obs.vertices.RemoveRange(0, count);
        obs.colliderPoints.RemoveRange(0, count/2); // edge collider points is only the "noisy" vertices
    }


    void RebuildTriangles(ref Obstacle obs, int verticesCount)
    {
        obs.triangles.Clear();

        for (int i = 0; i < verticesCount - 2; i += 2)
        {
            obs.triangles.Add(i + 2);
            obs.triangles.Add(i + 1);
            obs.triangles.Add(i);

            obs.triangles.Add(i + 2);
            obs.triangles.Add(i + 3);
            obs.triangles.Add(i + 1);
        }

    }

    void AssignMesh(ref Obstacle obs)
    {
        obs.mesh.SetVertices(obs.vertices);
        obs.mesh.triangles = obs.triangles.ToArray();

        obs.mesh.RecalculateBounds();

        obs.meshFilter.sharedMesh = obs.mesh;
        obs.edgeCollider.points = obs.colliderPoints.ToArray();

        //obs.polyCollider.points = obs.colliderPoints.ToArray();
        //obs.meshCollider.sharedMesh = obs.mesh;
        //Vector2[] transformedColliderPoints = new Vector2[obs.colliderPoints.Count];
        //int startPointer = 0;
        //int endpointer = transformedColliderPoints.Length-1;
        //for (int i = 0; i < transformedColliderPoints.Length; i++)
        //{
        //    if (i%2 == 0)
        //    {
        //        transformedColliderPoints[startPointer++] = obs.colliderPoints[i];
        //        
        //    }
        //    else
        //    {
        //        transformedColliderPoints[endpointer--] = obs.colliderPoints[i];
        //    }
        //}
        //
        //obs.polyCollider.points = transformedColliderPoints;

        
    }



}
