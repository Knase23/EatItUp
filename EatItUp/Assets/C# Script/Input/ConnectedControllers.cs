using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;

public class ConnectedControllers : MonoBehaviour
{
    public List<int> controllersConnected = new List<int>();
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Submit1") && !controllersConnected.Contains(1))
            controllersConnected.Add(1);
        if (Input.GetButtonDown("Submit2") && !controllersConnected.Contains(2))
            controllersConnected.Add(2);
        if (Input.GetButtonDown("Submit3") && !controllersConnected.Contains(3))
            controllersConnected.Add(3);
        if (Input.GetButtonDown("Submit4") && !controllersConnected.Contains(4))
            controllersConnected.Add(4);
        if (Input.GetButtonDown("Submit5") && !controllersConnected.Contains(5))
            controllersConnected.Add(5);
        if (Input.GetButtonDown("Cancel1") && controllersConnected.Contains(1))
            controllersConnected.Remove(1);
        if (Input.GetButtonDown("Cancel2") && controllersConnected.Contains(2))
            controllersConnected.Remove(2);
        if (Input.GetButtonDown("Cancel3") && controllersConnected.Contains(3))
            controllersConnected.Remove(3);
        if (Input.GetButtonDown("Cancel4") && controllersConnected.Contains(4))
            controllersConnected.Remove(4);
        if (Input.GetButtonDown("Cancel5") && controllersConnected.Contains(5))
            controllersConnected.Remove(5);
    }

}
