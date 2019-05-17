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

    /// <summary>
    /// Gives a list the list of Friends the Discord User have, is depending on the filther used in RelationUpdate
    /// </summary>
    /// <returns></returns>
    public RelationshipManager GetRelationshipManager()
    { 
        return relationshipManager;
    }
    /// <summary>
    /// Updates the filter for the of friends.
    /// </summary>
    public void RelationUpdate()
    {
        relationshipManager.Filter((ref Relationship relationship) =>
        {
            return relationship.Type == Discord.RelationshipType.Friend && (onlyOnline? relationship.Presence.Status == Status.Online:true);
        });
        
    }
    /// <summary>
    /// Sets so only Online Friend Users appears in the list, after a RelationUpdate()
    /// </summary>
    public void RelationUpdateOnlyOnline()
    {
        onlyOnline = true;
    }

    /// <summary>
    /// Sets so all Friend Users appears in the list, after a RelationUpdate()
    /// </summary>
    public void RelationUpdateAll()
    {
        onlyOnline = false;
    }
}
