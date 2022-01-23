using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCard : MonoBehaviour
{
    public bool isInHand = true;
    //all the impoortant stats for the mob to display and use for interactions
    private int topStat, bottomStat, leftStat, rightStat, curTopStat, curBottomStat, curLeftStat, curRightStat, curHitPoints, hitPoints, abilityIndex, abilityRankMod;
    private Color player1mobBackgroundColor = new Color(.6f, .3f, .3f);
    private Color player2mobBackgroundColor = new Color(.3f,.3f,.6f);
    public SpriteRenderer mobSpriteRenderer;
    public SpriteRenderer mobBackgroundRenderer;
    public SpriteRenderer mobCardBorderRenderer;
    public SpriteRenderer topStatRenderer, bottomStatRenderer, leftStatRenderer, rightStatRenderer, hitPointTensRenderer, hitPointOnesRenderer, attributeRenderer;
    private bool isPlayer1;
    private bool isMob;//are we a mob or are we an ability card?
    private int mobSpriteIndex; //will pull to figure out what sprite to place on the networkMobCard since i can't do it on the server.
    private int attributeSpriteIndex;
    private string mobName;
    
    public void CreateMobCard(int initLeftStat, int initRightStat, int initTopStat, int initBottomStat, int initHitPoints, Sprite mobSprite, int mobSprteIndex, bool isOwnedByPlayer1, bool initIsMob,
        Sprite leftStatSprite, Sprite rightStatSprite, Sprite topStatSprite, Sprite bottomStatSprite, Sprite hitPointTensSprite, Sprite hitPointOnesSprite, Sprite attributeSprite, int attrSpriteIndex,
        int initAbilityIndex, int initAbilityRankMod, string initMobName)
    {
        topStat = initTopStat;
        bottomStat = initBottomStat;
        rightStat = initRightStat;
        leftStat = initLeftStat;
        hitPoints = initHitPoints;
        curTopStat = topStat;
        curBottomStat = bottomStat;
        curLeftStat = leftStat;
        curRightStat = rightStat;
        curHitPoints = hitPoints;
        isPlayer1 = isOwnedByPlayer1;
        isMob = initIsMob;
        mobSpriteRenderer.sprite = mobSprite;
        mobSpriteIndex = mobSprteIndex;
        attributeSpriteIndex = attrSpriteIndex;
        mobBackgroundRenderer.color = isPlayer1 ? player1mobBackgroundColor : player2mobBackgroundColor;
        topStatRenderer.sprite = topStatSprite;
        bottomStatRenderer.sprite = bottomStatSprite;
        leftStatRenderer.sprite = leftStatSprite;
        rightStatRenderer.sprite = rightStatSprite;
        hitPointTensRenderer.sprite = hitPointTensSprite;
        hitPointOnesRenderer.sprite = hitPointOnesSprite;
        attributeRenderer.sprite = attributeSprite;
        abilityIndex = initAbilityIndex;
        abilityRankMod = initAbilityRankMod;
        mobName = initMobName;

    }

    public int[] GetStats()
    {
        int[] temp = new int[] { leftStat, rightStat, topStat, bottomStat, hitPoints };
        return temp;
    }

    public Sprite GetSprite()
    {
        return mobSpriteRenderer.sprite;
    }

    public int GetMobSpriteIndex()
    {
        return mobSpriteIndex;
    }

    public int GetAttributeSpriteIndex()
    {
        return attributeSpriteIndex;
    }

    public SpriteRenderer GetBackGround()
    {
        return mobBackgroundRenderer;
    }

    public bool GetPlayer1Owned()
    {
        return isPlayer1;
    }

    public bool GetIsMob()
    {
        return isMob;
    }

    public int GetAbilityIndex()
    {
        return abilityIndex;
    }

    public int GetAbilityRankMod()
    {
        return abilityRankMod;
    }
    public string GetMobName()
    {
        return mobName;
    }
}
