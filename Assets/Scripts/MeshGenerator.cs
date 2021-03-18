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

            if (meshFilter == null)
                Debug.LogError("MeshGenerator.cs : Cannot create an obstacle from the given game object because it is missing a mesh filter.", source);

            if (edgeCollider == null)
                Debug.LogError("MeshGenerator.cs : Cannot create an obstacle from the given game object because it is missing an EdgeCollider2D.", source);
            

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
    
    /// <summary>
    /// number of chunks of an obstacle at any given time
    /// </summary>
    int nbChunks
    {
        get { return (int)((cameraSize.y*2 / (float)chunkSize)+ 1) + 1; }
    }


    float buildHeight;


    // state from game manager
    [HideInInspector] public float cameraCurrentHeight;
    [HideInInspector] public float startingHeight;
    [HideInInspector] public Vector2 cameraSize;


    [Space]
    [SerializeField]
    GameObject leftObstacleHolder = null;
    [SerializeField]
    GameObject rightObstacleHolder = null;

    [Header("Height Values")]

    [SerializeField, Range(0, 0.9f)] float startChunkHeight = 0.1f;
    [SerializeField, Range(0, 0.9f)] float startBumpHeight = 0.5f;
    [Space]
    [SerializeField, Range(0, 0.9f)] float maxChunkHeight = 0.6f;
    [SerializeField, Range(0, 0.9f)] float maxBumpHeight = 0.15f;
    


    


    float scale = 2.0f;
    
    int octaves = 1;
    float lacunarity = 2.0f;
    float persistence = 0.5f;







    void Start()
    {
        left = new Obstacle(leftObstacleHolder, chunkSize * verticesPerUnit);
        right = new Obstacle(rightObstacleHolder, chunkSize * verticesPerUnit);

        noiseGenerator = GetComponent<NoiseGenerator>();


        SetupFirstObstacles();
    }


    private void Update()
    {
        // if the bottom edge of the camera is higher than the bottom chunk of the obstacles, build new obstacles
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


    
    public Vector2 GetPointBetweenObstacles(float height, float interpolator = 0.5f)
    {
        Vector2 pos;

        // find index using left obstacle values, colliderPoints should be the same length on both                
        int index = left.colliderPoints.Count-1; // index points to the top of the colliders
        if (height >= buildHeight)
        {
            // log an error but pos will be between the top points of the colliders
            Debug.LogWarning("MeshGenerator.cs : Cannot find a point between obstacles at height " + height + " because obstacles haven't been built this high yet.");
        }
        else
        {
            float diff = buildHeight - height;
            index -= (int)(diff * verticesPerUnit);
            // index points to the point closest to height in the colliders
        }

        // get a point between the points in left and right collider at index with a lerp
        Vector2 leftPos = left.colliderPoints[index];
        Vector2 rightPos = right.colliderPoints[index];


        pos = Vector2.Lerp(leftPos, rightPos, interpolator);

        return pos;
    }



    void GetAndProcessNoise(ref float[] buffer, float scale, float offset = 0.0f)
    {
        noiseGenerator.GenerateNoiseChunk(ref buffer, scale, offset, octaves,lacunarity, persistence);

        bool bump = Random.Range(0, 6) == 0;

        // process noise in buffer
        for (int i = 0; i < buffer.Length; i++)
        { 

            float heightNormalized = Mathf.Clamp01(Mathf.InverseLerp(0, 300, buildHeight));

            
            float cosWeight = Mathf.Lerp(maxBumpHeight * cameraSize.x, startBumpHeight * cameraSize.x, 1 - heightNormalized);
            float height = Mathf.Lerp(startChunkHeight * cameraSize.x, maxChunkHeight * cameraSize.x, heightNormalized);

            float pad = 0.1f * cameraSize.x;
            
            // chunk height
            buffer[i] *= height;
            buffer[i] += pad;
            
            // generate bump
            if (bump)
            {
                buffer[i] += Cos01((float)i / (float)buffer.Length) * cosWeight;
            }
            
        }
    }



    /// <summary>
    /// Cosine function that oscillates between 0 and 1 over a period of 1.
    /// </summary>
    float Cos01(float x)
    {
        return (-Mathf.Cos(x * Mathf.PI * 2.0f) + 1.0f) / 2.0f;
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
            }
            else
            {
                obs.vertices.Add(offset + new Vector3(0, i / (float)verticesPerUnit));

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
    }



}
