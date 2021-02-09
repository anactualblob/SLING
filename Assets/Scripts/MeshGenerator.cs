using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter), typeof(EdgeCollider2D), typeof(MeshRenderer))]
[RequireComponent(typeof(NoiseGenerator))]
public class MeshGenerator : MonoBehaviour
{

    MeshFilter meshFilter;
    EdgeCollider2D edgeCollider;

    NoiseGenerator noiseGenerator;

    [SerializeField] int verticesPerUnit = 10;
    [SerializeField] int chunkSize = 10;

    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> colliderPoints = new List<Vector2>();
    List<int> triangles = new List<int>();

    float currentHeight;
    float[] noiseChunk = { };

    Mesh mesh;

    [SerializeField]
    GameObject leftObstacleHolder = null;
    [SerializeField]
    GameObject rightObstacleHolder = null;


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
    }

    Obstacle left, right;


    void Start()
    {
        left = new Obstacle(leftObstacleHolder, chunkSize * verticesPerUnit);
        right = new Obstacle(rightObstacleHolder, chunkSize * verticesPerUnit);


        //mesh = new Mesh();
        //mesh.name = "newer mesh";
        //
        //meshFilter = GetComponent<MeshFilter>();
        //edgeCollider = GetComponent<EdgeCollider2D>();

        noiseGenerator = GetComponent<NoiseGenerator>();
        currentHeight = transform.position.y;

        noiseChunk = new float[chunkSize * verticesPerUnit];


        //GenerateMesh(Vector3.zero, true);
        //GenerateMesh(Vector3.up * currentHeight, true);

        GetAndProcessNoise(ref left.noiseChunk, 10.0f, currentHeight);

        AddVertices(ref left, Vector3.left);
        RebuildTriangles(ref left, left.vertices.Count);
        AssignMesh(ref left);



        GetAndProcessNoise(ref right.noiseChunk, 10.0f, currentHeight);

        AddVertices(ref right, Vector3.right, true);
        RebuildTriangles(ref right, right.vertices.Count);
        AssignMesh(ref right);

        currentHeight += chunkSize;


    }
    
    
    void GetAndProcessNoise(ref float[] buffer, float scale, float startPoint = 0.0f)
    {
        noiseGenerator.GenerateNoiseChunk(ref buffer, startPoint, scale);

        // process noise
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

    void RemoveVertices(ref Obstacle obs, int count)
    {
        obs.vertices.RemoveRange(0, count);
        obs.colliderPoints.RemoveRange(0, count);
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

        obs.meshFilter.sharedMesh = obs.mesh;
        obs.edgeCollider.points = obs.colliderPoints.ToArray();
    }



}
