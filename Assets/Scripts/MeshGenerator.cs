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
                Debug.LogError("MeshGenerator.cs : Cannot create an obstacle from the given game object because it is missing an edge collider 2D.", source);

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
        get { return (int)((cameraSize.y / chunkSize * 2 + 2)+0.5); }
    }


    float currentHeight;

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





    void Start()
    {
        left = new Obstacle(leftObstacleHolder, chunkSize * verticesPerUnit);
        right = new Obstacle(rightObstacleHolder, chunkSize * verticesPerUnit);


        noiseGenerator = GetComponent<NoiseGenerator>();

        SetupFirstObstacles();
    }


    private void Update()
    {
        // if cameraCurrentHeight - cameraSize.y > currentHeight - (nbchunks - 1)*chunkSize
        // todo : figure out why it's "nbChunks + 1" instead of "nbChunks - 1"
        if (cameraCurrentHeight - cameraSize.y > currentHeight - (nbChunks +1)*chunkSize)
        {
            BuildObstacles(startingHeight, cameraSize);
        }
    }


    public void SetupFirstObstacles()
    {
        currentHeight = transform.position.y;
        left.Reset();
        right.Reset();

        for (int i = 0; i < nbChunks; i++)
        {
            BuildObstacles(startingHeight, cameraSize);
        }
    }



    public void BuildObstacles(float startingHeight, Vector2 screenSize)
    {
        // left obstacle mesh
        GetAndProcessNoise(ref left.noiseChunk, scale, left.randomNoiseOffset);
        AddVertices(ref left, Vector3.left*screenSize.x + Vector3.up * (currentHeight + startingHeight));

        if (left.vertices.Count > verticesInChunk * nbChunks)
            RemoveBottomVertices(ref left, verticesInChunk);

        RebuildTriangles(ref left, left.vertices.Count);
        AssignMesh(ref left);


        // right obstacle mesh
        GetAndProcessNoise(ref right.noiseChunk, scale, right.randomNoiseOffset);
        AddVertices(ref right, Vector3.right * screenSize.x + Vector3.up * (currentHeight + startingHeight), true);

        if (right.vertices.Count > verticesInChunk * nbChunks) 
            RemoveBottomVertices(ref right, verticesInChunk);

        RebuildTriangles(ref right, right.vertices.Count);
        AssignMesh(ref right);


        // new height
        currentHeight += chunkSize;
    }


    
    
    



    void GetAndProcessNoise(ref float[] buffer, float scale, float offset = 0.0f)
    {
        noiseGenerator.GenerateNoiseChunk(ref buffer, scale, offset, 3, 2, 0.5f);

        // process noise in buffer
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
        obs.colliderPoints.RemoveRange(0, count/2); // edge collider only has half the vertices because it's only curved edge
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
