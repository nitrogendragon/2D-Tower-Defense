using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsController : MonoBehaviour
{
    private bool startFirstDraw = false;
    private float startWait = 1f;
    protected bool isFirstDraw  = true;
    protected int cardsToDraw;
    private int cardsInHandCount = 0;
    public GameObject cardsContainer;
    public GameObject mobCard;
    private List<GameObject> cardsInHand = new List<GameObject>();
    private float widthOfCardsContainer;
    private float mobCardsWidth;
    private float timePassed = 0;
    // Start is called before the first frame update
    void Start()
    {
        widthOfCardsContainer = cardsContainer.GetComponent<SpriteRenderer>().bounds.size.x;
        mobCardsWidth = mobCard.GetComponent<SpriteRenderer>().bounds.size.x;
        StartCoroutine("WaitToDraw");
    }

    private int CardsToDraw()
    {
        if (isFirstDraw)
        {
            Debug.Log("this is our first draw");
            isFirstDraw = false;
            return cardsToDraw = 2;
        }
        else
        {
            Debug.Log("This is not the first draw");
            return cardsToDraw = 1;
        }


    }

    IEnumerator WaitToDraw()
    {
        yield return new WaitForSeconds(startWait);
        startFirstDraw = true;
    }

    private void DrawCards()
    {
        int cardsToDraw = CardsToDraw();
        int prevCardsInHandCount = cardsInHandCount; // for keeping track of which card we are on between the for loops
        //Debug.Log("The number of cards to be drawn is " + cardsToDraw);
        cardsInHandCount += cardsToDraw;
        if (cardsInHand.Count != 0)
        {
            //Debug.Log("we have cards in our hand so we are reorganizing");
            for(int i = 0; i < cardsInHand.Count; i++)
            {
                //very fancy but should adjust to fill space as cards are drawn, will likely use for adjusting when playing cards too
                cardsInHand[i].transform.position =
                    new Vector3(cardsContainer.transform.position.x + -widthOfCardsContainer / 1.7f + (mobCardsWidth / 2) +
                    ((mobCardsWidth + .1f) * i + .1f + widthOfCardsContainer / cardsInHandCount),
                    cardsContainer.transform.position.y, 0);
            }
        }

        for (int cardsDrawn = 0; cardsDrawn < cardsToDraw; cardsDrawn++)
        {
            //Debug.Log("we are drawing our cards, card number being drawn: " + (cardsDrawn+1));
            GameObject myMobCard = Instantiate(mobCard);
            cardsInHand.Add(myMobCard);
            myMobCard.transform.position =
                new Vector3(cardsContainer.transform.position.x + -widthOfCardsContainer / 1.7f + (mobCardsWidth / 2) +
                ((mobCardsWidth + .1f) * (cardsDrawn + prevCardsInHandCount) + .1f + widthOfCardsContainer / cardsInHandCount),
                        cardsContainer.transform.position.y, 0);
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startFirstDraw)
        {
            startFirstDraw = false;
            DrawCards();
        }
        else if (Input.GetMouseButtonDown(0))//just for testing purposes, will remove later or at least adjust
        {
            DrawCards();
        }

    }
}
