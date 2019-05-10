using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log(collision.tag);
        if(collision.tag == "Pac")
            gameObject.SetActive(false);
    }
}
