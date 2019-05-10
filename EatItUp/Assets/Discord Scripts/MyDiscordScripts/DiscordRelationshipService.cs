using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
public class DiscordRelationshipService : MonoBehaviour
{
    public static DiscordRelationshipService INSTANCE;
    bool onlyOnline = false;
    RelationshipManager relationshipManager;

    private void Awake()
    {
        if (INSTANCE)
        {
            Destroy(this);
        }
        else
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        relationshipManager = DiscordManager.INSTANCE.GetDiscord().GetRelationshipManager();        
    }

    public RelationshipManager GetRelationshipManager()
    {
        return relationshipManager;
    }

    public void RelationUpdate()
    {
        relationshipManager.Filter((ref Relationship relationship) =>
        {
            return relationship.Type == Discord.RelationshipType.Friend && (onlyOnline? relationship.Presence.Status == Status.Online:true);
        });
        
    }
    public void RelationUpdateOnlyOnline()
    {
        onlyOnline = true;
    }
    public void RelationUpdateAll()
    {
        onlyOnline = false;
    }
}
