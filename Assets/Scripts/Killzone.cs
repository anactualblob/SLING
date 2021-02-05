using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    [SerializeField]
    LayerMask collisionMask = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & collisionMask) != 0)
        {
            GameManager.GameOver();
        }
    }
}
