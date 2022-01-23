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
    
    [SerializeField]private bool canPlaceCard = false;
    [SerializeField] MobDeckNetwork mobDeckNetwork;//on our DeckCardBack GameObject
    [SerializeField] private int cardsInDeck = 30;
    [SerializeField] TurnManager turnManager;//handles everything related to managing whose turn it is and what phase someone is in
    // Start is called before the first frame update
    void Start()
    {
        //initialize/re-initialize variables
        //SpawnMeServerRpc();
        
    }

    public void StartGame()
    {
        cardsInDeck = 30;
        widthOfCardsContainer = cardsContainer.GetComponent<SpriteRenderer>().bounds.size.x;
        mobCardsWidth = mobCard.GetComponent<SpriteRenderer>().bounds.size.x;
        StartCoroutine("WaitToDraw");
    }

    [ServerRpc(RequireOwnership =false)]
    private void SpawnMeServerRpc()
    {
        if (!IsServer) { return; }
        //Debug.Log("We spawned our card controller");
        this.NetworkObject.SpawnWithOwnership(OwnerClientId);
    }

    private int CardsToDraw()
    {
        if (isFirstDraw)
        {
            //Debug.Log("this is our first draw");
            isFirstDraw = false;
            return cardsToDraw = 5;
        }
        else
        {
            //Debug.Log("This is not the first draw");
            return cardsToDraw = 1;
        }


    }

    IEnumerator WaitToDraw()
    {
        yield return new WaitForSeconds(startWait);
        //this is just for our first draw
        DrawCards(false);
        turnManager.InitializeTurnManagerServerRpc();
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
                    new Vector3(cardsContainer.transform.position.x + -widthOfCardsContainer / 1.7f + (mobCardsWidth / 3) +
                    ((mobCardsWidth + .1f) * i + widthOfCardsContainer / cardsInHandCount),
                    cardsContainer.transform.position.y, 0);
            }
        }
    }

    private void DrawCards(bool isDeactivated)
    {
        if (isDeactivated) { return; }
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
            bool isMob = true;//set to true by default
            int[] tempStatList = new int[5];
            int tempMobSpriteIndex = 0;
            int tempAttributeSpriteIndex = 0;
            int tempAbilityIndex = 0;
            int tempAbilityRankMod = 0;
            string tempName = "";
            mobDeckNetwork.setUpCardOnDraw(ref tempMobSprite, ref tempStatList, ref tempMobSpriteIndex,ref tempAttackAndHpValueSpritesList, ref tempAttributeSprite, ref tempAttributeSpriteIndex, ref isMob,
                ref tempAbilityIndex, ref tempAbilityRankMod, ref tempName);
            //Debug.Log("Prev cards in hand: " + prevCardsInHandCount);
            //Debug.Log("cards to draw: " + cardsToDraw);
            //Debug.Log("we are drawing our cards, card number being drawn: " + (cardsDrawn+1));
            GameObject myMobCardInstance = Instantiate(mobCardOffServer);
            myMobCardInstance.GetComponent<MobCard>().CreateMobCard(tempStatList[0], tempStatList[1], tempStatList[2], tempStatList[3], tempStatList[4], tempMobSprite,tempMobSpriteIndex, isPlayer1, isMob,
                tempAttackAndHpValueSpritesList[0], tempAttackAndHpValueSpritesList[1], tempAttackAndHpValueSpritesList[2],tempAttackAndHpValueSpritesList[3], tempAttackAndHpValueSpritesList[4],
                tempAttackAndHpValueSpritesList[5], tempAttributeSprite, tempAttributeSpriteIndex, tempAbilityIndex, tempAbilityRankMod, tempName);

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
                new Vector3(cardsContainer.transform.position.x + -widthOfCardsContainer / 1.7f + (mobCardsWidth / 3) +
                ((mobCardsWidth + .1f) * (cardsDrawn + prevCardsInHandCount) + widthOfCardsContainer / cardsInHandCount),
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
                selectedCard.GetComponent<MobCard>().mobCardBorderRenderer.color = Color.black;
            }
            selectedCard = hit.collider.gameObject;
            selectedCard.GetComponent<MobCard>().mobCardBorderRenderer.color = Color.green;
        }
    }

    private bool CheckForDeckClick()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (Input.GetKeyDown(KeyCode.Mouse0) && hit.collider && hit.collider.tag == "Deck")
        {
            //Debug.Log("We hit the deck");
            return true;
        }
        return false;
    }

    private bool CheckForAbilityCard()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider && hit.collider.tag == "AbilityCard" )
        {
            
            hit.collider.GetComponent<NetworkObject>().GetComponent<MobCardNetwork>().abilityAttackServerRpc();
            //destroy the card afterwards
            hit.collider.GetComponent<MobCardNetwork>().DestroyNetworkObjectServerRpc();
            return true;
        }
        return false;
        
        
    }

    private IEnumerator WaitAfterAbilityCardCheck()
    {
        Debug.Log("started post ability waiter");
        yield return new WaitForSeconds(.1f);
        
    }

    private bool CheckForTile()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        if (hit.collider && hit.collider.tag == "BoardTile")
        {
            boardManager.SetSelectedTileAfterAbilityCardActivated(hit.collider);
            //Debug.Log("This ran");
            return true;
        }
        return false;
    }
    
    private bool PlaceSelectedCard()
    {

        
        CheckForTile();
        Vector3 cardPlacementPosition = boardManager.GetSelectedBoardCoordinates();
        int cardPlacementBoardIndex = boardManager.GetSelectedBoardIndex();

        
        
        
        //Debug.Log(cardPlacementBoardIndex + " is our cardPlacementBoardIndex");
        if(cardPlacementPosition.x != 9999 && cardPlacementBoardIndex != 9999)//our invalid placement tile position that our function returns if not hitting a boardtile
        {
            //selectedCard.GetComponent<SpriteRenderer>().color = Color.gray;
            selectedCard.transform.position = cardPlacementPosition;
            int[] tempStats = selectedCard.GetComponent<MobCard>().GetStats();
            bool tempIsPlayer1 = selectedCard.GetComponent<MobCard>().GetPlayer1Owned();
            bool tempIsMob = selectedCard.GetComponent<MobCard>().GetIsMob();
            int tempMobSpriteIndexRef = selectedCard.GetComponent<MobCard>().GetMobSpriteIndex();
            int tempAttributeSpriteIndexRef = selectedCard.GetComponent<MobCard>().GetAttributeSpriteIndex();
            int tempAbilityIndex = selectedCard.GetComponent<MobCard>().GetAbilityIndex();
            int tempAbilityRankMod = selectedCard.GetComponent<MobCard>().GetAbilityRankMod();
            string tempName = selectedCard.GetComponent<MobCard>().GetMobName();
            selectedCard.GetComponent<MobCard>().isInHand = false;
            SpawnMobCardOnBoardServerRpc(cardPlacementPosition, cardPlacementBoardIndex, tempStats, tempIsPlayer1, tempIsMob, tempMobSpriteIndexRef, tempAttributeSpriteIndexRef,
                tempAbilityIndex, tempAbilityRankMod, tempName);

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
            //Debug.Log(selectedCard);
            cardsInHand = tempList;//update our cardsInHand List to be the new list without the placed card
            //Debug.Log(cardsInHand.Count);
            ReOrganizeHand();//reorganize / relayout cards in hand
            cardsInHandCount = cardsInHand.Count;
            return true;
            
        }
        //return false otw so we can know not to update the turnManager
        return false;
    }

    [ServerRpc (RequireOwnership = false)]
    private void SpawnMobCardOnBoardServerRpc(Vector3 spawnPos,int cardPlacementBoardIndex, int[] tempStats, bool isPlayer1, bool tempIsMob, int mobSpriteIndexRef, int attributeSpriteIndexRef,
        int tempAbilityIndex, int tempAbilityRankMod, string tempName)
    {
        //we can't do this if we aren't the server
        //if (!IsServer) { Debug.Log(" we got stuck in the not server if statement"); return; }

        NetworkObject myMobCardInstance = Instantiate(mobCard,spawnPos , new Quaternion(0,0,0,0));

        myMobCardInstance.SpawnWithOwnership(OwnerClientId);

        int playerOwnerIndex = isPlayer1 ? 1 : 2;

        myMobCardInstance.GetComponent<MobCardNetwork>().CreateMobCardServerRpc(tempStats[0],tempStats[1],tempStats[2],tempStats[3],tempStats[4],playerOwnerIndex, tempIsMob, mobSpriteIndexRef,
            attributeSpriteIndexRef, cardPlacementBoardIndex, tempAbilityIndex, tempAbilityRankMod, tempName);

        
       
        int[] tempMobStats = myMobCardInstance.GetComponent<MobCardNetwork>().GrabStats();

        //Debug.Log("Network mob card stats: " + tempMobStats[0] + " " + tempMobStats[1] + " " + tempMobStats[4]);
        //spawn our mob card with ownership
        
    }

    // Update is called once per frame
    void Update()
    {
        //only the owner should be doing this
        if (!IsClient) { return; }
        //draw checks first
        if (Input.GetKeyDown(KeyCode.Alpha1) || CheckForDeckClick())
        {
            if (IsHost && turnManager.GetIsPlayer1Turn() && turnManager.GetTurnActionIndex() == 0 ||
               !IsHost && IsClient && !turnManager.GetIsPlayer1Turn() && turnManager.GetTurnActionIndex() == 0)
            {
                DrawCards(turnManager.GetWaitingForPhaseChangeStatus());
                StartCoroutine(turnManager.WaitToEndPhase());
            }
        }
        //next try card selection
        else if (Input.GetMouseButtonDown(0))
        {
            if(IsHost && turnManager.GetIsPlayer1Turn() && turnManager.GetTurnActionIndex() == 1  || 
                !IsHost && IsClient && !turnManager.GetIsPlayer1Turn() && turnManager.GetTurnActionIndex() == 1)
            {
                //Debug.Log("we met the conditions");
                SelectCard();
            }
            //Debug.Log("we are not meeting the conditions to select a card obviously");
            //Debug.Log(IsHost + "host bool");
            //Debug.Log(turnManager.GetIsPlayer1Turn() + " isplayer1bool value");
            //Debug.Log(turnManager.GetTurnActionIndex() + " turnactionindex value");
        }
        //lastly see if we can place the card selected on the field and also if we have
        else if (Input.GetMouseButtonDown(1) && selectedCard )
        {
            canPlaceCard = CheckForAbilityCard();
            //if false we can place the card normally
            if (!canPlaceCard)
            {
                if (PlaceSelectedCard())
                {
                    //handle turn updates to move to next phase since we successfully placed our card
                    StartCoroutine(turnManager.WaitToEndPhase());
                }
            }
            return;
            
        }
        //if we had an ability card in the spot last frame we will run the place card this frame2
        else if (canPlaceCard)
        {
            if (PlaceSelectedCard())
            {
                //handle turn updates to move to next phase since we successfully placed our card
                canPlaceCard = false;
                StartCoroutine(turnManager.WaitToEndPhase());
            }
        }
        



    }
}
