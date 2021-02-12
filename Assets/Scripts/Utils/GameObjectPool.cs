//#define DEBUG_POOL
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    [SerializeField]
    GameObject objectToPool = null;
    [Space]
    [SerializeField]
    bool setupPoolAtStart = true;
    [SerializeField] [Tooltip("Only used if setupPoolAtStart is true.")]
    int poolSize = 0;


    GameObject[] pool = { };
    int index = 0;



    private void Start()
    {
        if (setupPoolAtStart) SetupPool(poolSize);
    }


    #region Pool functions
    /// <summary>
    /// Create a pool of a given GameObject.
    /// </summary>
    /// <param name="size"></param>
    public void SetupPool(int size)
    {
        pool = new GameObject[size];

        GameObject current; 
        for (int i = 0; i < size; ++i)
        {
            current = null;
            current = Instantiate(objectToPool, Vector3.zero, Quaternion.identity);
            current.transform.SetParent(transform);

            current.SetActive(false);

            pool[i] = current;
        }

        index = 0;

    }


    /// <summary>
    /// Get a GameObject from this pool if there are any available.
    /// </summary>
    /// <param name="activate">Whether or not the returned GameObject should already be active. Set to true by default.</param>
    /// <returns>A GameObject from the pool, or null if there are none available.</returns>
    public GameObject TakeFromPool(bool activate = true)
    {
        if (index >= pool.Length)
        {
            Debug.LogError("GameObjectPool.cs : Trying to get a new object from pool but there is no more.");
            return null;
        }

        GameObject ret = pool[index];
        ++index;

        if (ret == null)
        {
            Debug.LogError("GameObjectPool.cs : Can't take from pool because the GameObject at the given index is null. Has the pool been properly initialized?");
            return null;
        }

        ret.SetActive(activate);

        return ret;
    }


    /// <summary>
    /// Return an object to the pool, making it available again. Resets its parent transform and deactivates it.
    /// </summary>
    /// <param name="returnedObj">The object to return to the pool.</param>
    /// <param name="checkIfBelongsToPool">Whether or not to check if returnedObj belongs to the pool.</param>
    public void ReturnToPool(GameObject returnedObj, bool checkIfBelongsToPool = false)
    {
        if (returnedObj == null)
        {
            Debug.LogError("GameObjectPool.cs : Trying to return a null GameObject to the pool.");
            return;
        }

        if (checkIfBelongsToPool)
        {
            bool present = false;
            for (int i = 0; i < poolSize; ++i)
            {
                if (pool[i] == returnedObj)
                {
                    present = true;
                    break;
                }
            }

            if (!present)
            {
                Debug.LogError("GameObjectPool.cs : Trying to return a GameObject to the pool but the Gameobject doesn't belong to the pool.");
                return;
            }
        }


        returnedObj.SetActive(false);
        --index;
    }
    #endregion


    #region Debug Context Methods
#if DEBUG_POOL
    List<GameObject> objectsTakenOut = new List<GameObject>();


    [ContextMenu("DEBUG: Get New Object")]
    void GetNewObject()
    {
        objectsTakenOut.Add(TakeFromPool());
        Debug.Log("index: " + index);
    }


    [ContextMenu("DEBUG: Get All Objects")]
    void GetAllObjects()
    {
        for (int i = index; i < poolSize; ++i)
        {
            objectsTakenOut.Add(TakeFromPool());
        }
        Debug.Log("index: " + index);
    }


    [ContextMenu("DEBUG: Return Object")]
    void ReturnObject()
    {
        GameObject obj = objectsTakenOut[objectsTakenOut.Count - 1];
        ReturnToPool(obj, true);
        objectsTakenOut.Remove(obj);

        Debug.Log("index: " + index);
    }


    [ContextMenu("DEBUG: Return Invalid Object")]
    void ReturnInvalidObject()
    {
        GameObject obj = this.gameObject;
        ReturnToPool(obj, true);
        objectsTakenOut.Remove(obj);

        Debug.Log("index: " + index);
    }


    [ContextMenu("DEBUG: Return All Objects")]
    void ReturnAllObjects()
    {
        foreach (GameObject go in objectsTakenOut)
        {
            ReturnToPool(go, true);
        }
        objectsTakenOut.Clear();

        Debug.Log("index: " + index);
    }

#endif
    #endregion
}
