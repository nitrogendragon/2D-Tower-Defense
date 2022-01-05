using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCard : MonoBehaviour
{
    public bool isInHand = true;
    //all the impoortant stats for the mob to display and use for interactions
    private int topStat, bottomStat, leftStat, rightStat, curTopStat, curBottomStat, curLeftStat, curRightStat, curHitPoints, hitPoints;
    private Color player1mobBackgroundColor = new Color(.4f, 0, 0);
    private Color player2mobBackgroundColor = new Color(.1f, .1f, 1);
    public SpriteRenderer mobSpriteRenderer;
    public SpriteRenderer mobBackgroundRenderer;
    private bool isPlayer1;
    private int mobSpriteIndex; //will pull to figure out what sprite to place on the networkMobCard since i can't do it on the server.

    
    public void CreateMobCard(int initTopStat, int initBottomStat, int initLeftStat, int initRightStat, int initHitPoints, Sprite mobSprite, int mobSprteIndex, bool isOwnedByPlayer1)
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
        mobSpriteRenderer.sprite = mobSprite;
        mobSpriteIndex = mobSprteIndex;
        mobBackgroundRenderer.color = isPlayer1 ? player1mobBackgroundColor : player2mobBackgroundColor;

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

    public SpriteRenderer GetBackGround()
    {
        return mobBackgroundRenderer;
    }

    public bool GetPlayer1Owned()
    {
        return isPlayer1;
    }
}
