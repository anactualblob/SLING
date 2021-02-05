using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePickup : MonoBehaviour
{
    [SerializeField]
    LayerMask collisionMask = 0;
    [Space]
    [SerializeField]
    int value = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // dumb bitshift because gameObject.layer returns the layer number and not a layer mask
        // but basically checks if the game object belongs to a layer in collisionMask
        if ( ((1 << collision.gameObject.layer) & collisionMask) != 0)
        {
            GetConsumed();
        }
    }


    void GetConsumed()
    {
        Debug.Log("Pickup consumed");
        // Tell GameManager to add some ropes
        GameManager.GainRopes(value);

        // disappear

        // spawn new FX possibly
    }
}
