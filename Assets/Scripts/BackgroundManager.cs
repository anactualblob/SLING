using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{

    [SerializeField]
    Transform[] bg = { };

    // index of the lower BG
    int _lowerIndex = 0;

    int lower
    {
        get { return _lowerIndex; }
        set
        {
            if (value > 1 || value < 0) Debug.LogError("BackgroundManager.cs : Trying to set variable 'lower' to an invalid value.", this);
            else _lowerIndex = value;
        }
    }
    int higher
    {
        get { return _lowerIndex == 1 ? 0 : 1; }
    }

    [SerializeField]
    float textureSize = 10.24f;


    void Start()
    {
        if (bg.Length < 2)
            Debug.LogError("BackgroundManager.cs : Missing background game object(s). Please assign them in the inspector.", this);

        InitBackgrounds();
    }


    // place the backgrounds at y = 0 and y = 0 + textureSize
    // called when starting and restarting a game
    public void InitBackgrounds()
    {
        bg[0].position = Vector3.zero + Vector3.forward * 10;
        bg[1].position = bg[0].position + Vector3.up * textureSize;
        lower = 0;
    }


    void Update()
    {
        // if we're in the right state
        if (GameManager.State == GameManager.GameState.notTouching || GameManager.State == GameManager.GameState.touching)
        {
            if (transform.position.y > (bg[lower].position.y + textureSize))
            {
                bg[lower].position = bg[lower].position + Vector3.up * textureSize * 2;
                lower = (lower == 0) ? 1 : 0;
            }
        }
        


        //if (transform.position.y < bg[lower].transform.position.y)// + textureSize/2)
        //{
        //    bg[0].position = Vector3.zero + Vector3.forward * 10;
        //    bg[1].position = bg[0].position + Vector3.up * textureSize;
        //    lower = 0;
        //}
    }
}
