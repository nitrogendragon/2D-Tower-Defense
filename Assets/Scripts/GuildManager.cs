using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildManager : MonoBehaviour
{
    List<GameObject> guilds;
    
    public void AddGuild(GameObject guild)
    {
        guilds.Add(guild);
    }

    public void RemoveGuild(GameObject guild)
    {
        guilds.Remove(guild);
    }

    public GameObject GrabGuild()
    {
        GameObject guild = GameObject.Find("Guild(Clone)");
        
        if (guild)
        {
            return guild;
        }
        else return null;
       
    }
}
