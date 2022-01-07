using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesLists : MonoBehaviour
{
    [SerializeField] private List<Sprite> mobSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> attackValueSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> hpValueSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> attributeSprites = new List<Sprite>();
    // Start is called before the first frame update
    public Sprite GetMobSprite(int index)
    {
        
        return mobSprites[index];
    }

    public Sprite GetAttackValueSprite(int index)
    {
        return attackValueSprites[index];
    }

    public Sprite GetHpValueSprite(int index)
    {
        return hpValueSprites[index];
    }

    public Sprite GetAttributeSprite(int index)
    {
        return attributeSprites[index];
    }
}
