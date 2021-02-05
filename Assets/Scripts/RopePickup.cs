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
        Debug.Log("Pickup consumed");
        consumed = true;

        // Tell GameManager to gain some ropes
        GameManager.GainRopes(value);

        // disappear

        // spawn new FX possibly
    }
}
