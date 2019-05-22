using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortraitsCanvas : MonoBehaviour
{
    public static PlayerPortraitsCanvas INSTANCE;
    public Dictionary<long, PortriaitPlayerView> pairedPortriats = new Dictionary<long, PortriaitPlayerView>();
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
            var portriaits = GetComponentsInChildren<PortriaitPlayerView>();
            foreach (var item in portriaits)
            {
                pairedPortriats.Add(item.id, item);
            }
        }
    }
}
