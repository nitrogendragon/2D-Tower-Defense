using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class CardsControllerNetwork : NetworkBehaviour
{
    [SerializeField] private NetworkObject mobCard;
    public BoardManagerNetwork boardManager;
    private GameObject selectedCard;
    private bool startFirstDraw = false;
    //will/can be used to delay several actions like in the yugioh card games or other games
    private float startWait = 1f;
    protected bool isFirstDraw  = true;
    protected int cardsToDraw;
    private int cardsInHandCount = 0;
    public GameObject cardsContainer;
    public GameObject mobCardOffServer;//not to be instantiated on the server
    private List<GameObject> cardsInHand = new List<GameObject>();
    private float widthOfCardsContainer;
    private float mobCardsWidth;
    private float timePassed = 0;
    [SerializeField] MobDeckNetwork mobDeckNetwork;//on our DeckCardBack GameObject
    [SerializeField ]private int cardsInDeck = 30;
    //will be used to determine what a player should be doing, drawing, placing a card/cards, activating abilities/spells/traps/field cards, ending turn
    private NetworkVariable<int> turnActionIndex = new NetworkVariable<int>(0);
    //Network Specific variables below
    //who's turn is it? player1(host) or player2(client), determines if we should draw and can place cards and so on in the turn execution cycle
    private NetworkVariable<bool> isPlayer1Turn = new NetworkVariable<bool>();

    // Start is called before the first frame update
    void Start()
    {
        //initialize/re-initialize variables
        //SpawnMeServerRpc();
        
    }

    public void StartGame()
    {
        cardsInDeck = 30;
        startFirstDraw = false;
        widthOfCardsContainer = cardsContainer.GetComponent<SpriteRenderer>().bounds.size.x;
        mobCardsWidth = mobCard.GetComponent<SpriteRenderer>().bounds.size.x;
        StartCoroutine("WaitToDraw");
    }

    [ServerRpc(RequireOwnership =false)]
    private void SpawnMeServerRpc()
    {
        if (!IsServer) { return; }
        Debug.Log("We spawned our card controller");
        this.NetworkObject.SpawnWithOwnership(OwnerClientId);
    }

    private int CardsToDraw()
    {
        if (isFirstDraw)
        {
            Debug.Log("this is our first draw");
            isFirstDraw = false;
            return cardsToDraw = 5;
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

    public void ReOrganizeHand()
    {
        if (cardsInHand.Count != 0)
        {
            //Debug.Log("we have cards in our hand so we are reorganizing");
            for (int i = 0; i < cardsInHand.Count; i++)
            {
                //very fancy but should adjust to fill space as cards are drawn, will likely use for adjusting when playing cards too
                cardsInHand[i].transform.position =
                    new Vector3(cardsContainer.transform.position.x + -widthOfCardsContainer / 1.7f + (mobCardsWidth / 2) +
                    ((mobCardsWidth + .1f) * i + .1f + widthOfCardsContainer / cardsInHandCount),
                    cardsContainer.transform.position.y, 0);
            }
        }
    }

    private void DrawCards()
    {
        //make sure only the owner is doing this
        int cardsToDraw = CardsToDraw();
        int prevCardsInHandCount = cardsInHandCount; // for keeping track of which card we are on between the for loops
        //Debug.Log("The number of cards to be drawn is " + cardsToDraw);
        cardsInHandCount += cardsToDraw;
        ReOrganizeHand();
        bool isPlayer1;
        if (IsHost)
        {
            isPlayer1 = true;
        }
        else { isPlayer1 = false; }
        for (int cardsDrawn = 0; cardsDrawn < cardsToDraw; cardsDrawn++)
        {
            Sprite tempMobSprite = null;
            Sprite tempAttributeSprite = null;
            //in order, left, right, top, bottom, hptens, hpones
            Sprite[] tempAttackAndHpValueSpritesList = new Sprite[6];
            int[] tempStatList = new int[5];
            int tempMobSpriteIndex = 0;
            int tempAttributeSpriteIndex = 0;
            mobDeckNetwork.setUpCardOnDraw(ref tempMobSprite, ref tempStatList, ref tempMobSpriteIndex,ref tempAttackAndHpValueSpritesList, ref tempAttributeSprite, ref tempAttributeSpriteIndex);
            //Debug.Log("Prev cards in hand: " + prevCardsInHandCount);
            //Debug.Log("cards to draw: " + cardsToDraw);
            //Debug.Log("we are drawing our cards, card number being drawn: " + (cardsDrawn+1));
            GameObject myMobCardInstance = Instantiate(mobCardOffServer);
            myMobCardInstance.GetComponent<MobCard>().CreateMobCard(tempStatList[0], tempStatList[1], tempStatList[2], tempStatList[3], tempStatList[4], tempMobSprite,tempMobSpriteIndex, isPlayer1,
                tempAttackAndHpValueSpritesList[0], tempAttackAndHpValueSpritesList[1], tempAttackAndHpValueSpritesList[2],tempAttackAndHpValueSpritesList[3], tempAttackAndHpValueSpritesList[4],
                tempAttackAndHpValueSpritesList[5], tempAttributeSprite, tempAttributeSpriteIndex);

            cardsInHand.Add(myMobCardInstance);
            //special exception to formula when only 1 card because otherwise it shows up in a weird place, so we center here
            if (prevCardsInHandCount == 0 && cardsToDraw == 1)
            {
                //Debug.Log("we met the conditions");
                myMobCardInstance.transform.position = new Vector3(0, cardsContainer.transform.position.y, 0);
                return;
            }
            //Debug.Log("We didn't have zero cards in hand and are not the only card being drawn");
            myMobCardInstance.transform.position =
                new Vector3(cardsContainer.transform.position.x + -widthOfCardsContainer / 1.7f + (mobCardsWidth / 2) +
                ((mobCardsWidth + .1f) * (cardsDrawn + prevCardsInHandCount) + .1f + widthOfCardsContainer / cardsInHandCount),
                        cardsContainer.transform.position.y, 0);
            

        }
    }

    private void SelectCard()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider && hit.collider.tag == "MobCard" && hit.collider.GetComponent<MobCard>().isInHand)
        {
            if (selectedCard)
            {
                selectedCard.GetComponent<SpriteRenderer>().color = Color.gray;
            }
            selectedCard = hit.collider.gameObject;
            selectedCard.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    private void PlaceSelectedCard()
    {
        Vector3 cardPlacementPosition = boardManager.GetSelectedBoardCoordinates();
        int cardPlacementBoardIndex = boardManager.GetSelectedBoardIndex();
        if(cardPlacementPosition.x != 9999)//our invalid placement tile position that our function returns if not hitting a boardtile
        {
            //selectedCard.GetComponent<SpriteRenderer>().color = Color.gray;
            selectedCard.transform.position = cardPlacementPosition;
            int[] tempStats = selectedCard.GetComponent<MobCard>().GetStats();
            bool tempIsPlayer1 = selectedCard.GetComponent<MobCard>().GetPlayer1Owned();
            int tempMobSpriteIndexRef = selectedCard.GetComponent<MobCard>().GetMobSpriteIndex();
            int tempAttributeSpriteIndexRef = selectedCard.GetComponent<MobCard>().GetAttributeSpriteIndex();
            selectedCard.GetComponent<MobCard>().isInHand = false;
            SpawnMobCardOnBoardServerRpc(cardPlacementPosition, cardPlacementBoardIndex, tempStats, tempIsPlayer1,tempMobSpriteIndexRef, tempAttributeSpriteIndexRef);

            //update cards in hand list
            List<GameObject> tempList = new List<GameObject>();
            for(int i = 0; i < cardsInHand.Count; i++)
            {
                if(cardsInHand[i] != selectedCard)
                {
                    tempList.Add(cardsInHand[i]);
                }
            }
            Destroy(selectedCard);
            selectedCard = null;
            Debug.Log(selectedCard);
            cardsInHand = tempList;//update our cardsInHand List to be the new list without the placed card
            //Debug.Log(cardsInHand.Count);
            ReOrganizeHand();//reorganize / relayout cards in hand
            cardsInHandCount = cardsInHand.Count;
        }
        //do nothing otw
    }

    [ServerRpc (RequireOwnership = false)]
    private void SpawnMobCardOnBoardServerRpc(Vector3 spawnPos,int cardPlacementBoardIndex, int[] tempStats, bool isPlayer1, int mobSpriteIndexRef, int attributeSpriteIndexRef)
    {
        //we can't do this if we aren't the server
        if (!IsServer) { Debug.Log(" we got stuck in the not server if statement"); return; }

        NetworkObject myMobCardInstance = Instantiate(mobCard,spawnPos , new Quaternion(0,0,0,0));

        myMobCardInstance.SpawnWithOwnership(OwnerClientId);

        int playerOwnerIndex = isPlayer1 ? 1 : 2;

        myMobCardInstance.GetComponent<MobCardNetwork>().CreateMobCardServerRpc(tempStats[0],tempStats[1],tempStats[2],tempStats[3],tempStats[4],playerOwnerIndex,mobSpriteIndexRef, attributeSpriteIndexRef, cardPlacementBoardIndex);

        
       
        int[] tempMobStats = myMobCardInstance.GetComponent<MobCardNetwork>().GrabStats();

        Debug.Log("Network mob card stats: " + tempMobStats[0] + " " + tempMobStats[1] + " " + tempMobStats[4]);
        //spawn our mob card with ownership
        
    }

    // Update is called once per frame
    void Update()
    {
        //only the owner should be doing this
        
        if (startFirstDraw)
        {
            startFirstDraw = false;
            DrawCards();
        }
        else if (Input.GetMouseButtonDown(0))//just for testing purposes, will remove later or at least adjust
        {
            SelectCard();
        }
        else if (Input.GetMouseButton(1) && selectedCard)
        {

            PlaceSelectedCard();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DrawCards();//just for testing purposes
        }


    }
}
