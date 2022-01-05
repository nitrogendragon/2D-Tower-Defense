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

    
    public void CreateMobCard(int initTopStat, int initBottomStat, int initLeftStat, int initRightStat, int initHitPoints, Sprite mobSprite, bool isPlayer1)
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
        mobSpriteRenderer.sprite = mobSprite;
        mobBackgroundRenderer.color = isPlayer1 ? player1mobBackgroundColor : player2mobBackgroundColor;

    }
}
