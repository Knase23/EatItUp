using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortraitsCanvas : MonoBehaviour
{
    public static PlayerPortraitsCanvas INSTANCE;
    public List<PortriaitPlayerView> portriaits = new List<PortriaitPlayerView>();
    private void Awake()
    {
        if(INSTANCE)
        {
            Destroy(gameObject);
        }
        else
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
            portriaits.AddRange(GetComponentsInChildren<PortriaitPlayerView>());
        }
    }
}
