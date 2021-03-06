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

    bool consumed = false;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        // dumb bitshift because gameObject.layer returns the layer number and not a layer mask
        // but basically checks if the game object belongs to a layer in collisionMask
        if ( ((1 << collision.gameObject.layer) & collisionMask) != 0)
        {
            if (!consumed) GetConsumed();
        }
    }


    void GetConsumed()
    {

        consumed = true;

        // disappear

        // spawn new FX possibly


        // Tell GameManager to gain some ropes & return this pickup to the pool
        GameManager.GainRopes(value, this);
    }



    private void OnEnable()
    {
        consumed = false;
    }
}
