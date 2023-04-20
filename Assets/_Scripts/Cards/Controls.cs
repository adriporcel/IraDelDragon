using System;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [SerializeField] Transform selectedCardPreviousParent;
    [SerializeField] Card selectedCard, target;

    List<Collider> playAreaColliders = new();
    DeckController deckController;

    void Start()
    {
        deckController = FindObjectOfType<DeckController>();

        foreach (GameObject area in GameObject.FindGameObjectsWithTag("dropArea"))
        {
            playAreaColliders.Add(area.GetComponent<Collider>());
        }

        deckController.CheckCardsInHandDeployReadiness();
    }

    void Update()
    {
        Vector3 mousePosScreen = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosScreen.x,
                                                                                mousePosScreen.y,
                                                                                10.21f)); // Distance from camera

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Right click to activate card (Brotherhood in this case)
        if (Input.GetMouseButtonDown(1) && !Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) &&
                hit.collider.tag == "card")
            {
                Card card = hit.collider.gameObject.GetComponent<Card>();

                if (card.CardType == CardType.brotherhood) // If right click on Brotherhood, activate card
                {
                    card.ToggleActivateBrotherhood();
                    deckController.CountBrotherhoodPoints();
                    deckController.CheckCardsInHandDeployReadiness();
                }
            }
        }

        // Left click, move cards and actions
        if (Input.GetMouseButtonDown(0))
        {
            // If a card is selected and no other card is currently selected
            if (selectedCard == null
                && Physics.Raycast(ray, out hit, Mathf.Infinity)
                && hit.collider.tag == "card")
            {
                Card card = hit.collider.gameObject.GetComponent<Card>();

                if (!card.ActiveBrotherhood && (card.ReadyToDeploy || card.DeployedOnBoard))
                {
                    selectedCard = card;
                    card.gameObject.layer = 2;

                    selectedCardPreviousParent = card.transform.parent;
                    card.transform.SetParent(gameObject.transform);
                }
            }
        }
        else if (Input.GetMouseButton(0) && selectedCard != null && Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            /* If the left mouse button is held down and a collider is hit,
               moves the card to the mouse position and set the target to the collider that is hit */
            selectedCard.GetComponent<SmoothMove>().MoveTo(new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, -1), .05f);

            GameObject targetCard = hit.collider.gameObject;
            if (targetCard.tag == "card")
            {
                target = targetCard.GetComponent<Card>();
            }
            else
            {
                target = null;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (selectedCard != null)
            {
                Card _card = selectedCard.GetComponent<Card>();

                if (!ScreenInEdgeBottomDetection(0.15f)) // If card outside the bottom 15% of the screen
                {
                    if (!_card.DeployedOnBoard) // Deploy card on corresponding board
                    {
                        selectedCard.transform.SetParent(deckController.CorrectBoardArea(_card));
                        // TODO: Implement Items, Enchantments and Instant cards
                    }
                    else if (_card.DeployedOnBoard
                             && target != null
                             && target.Owner == Players.secondary) // Execute card action
                    {
                        // TODO
                    }
                    else if (_card.DeployedOnBoard) // && (target == null || target.Owner == Players.main)
                    {
                        selectedCard.transform.SetParent(deckController.CorrectBoardArea(_card));
                    }

                    deckController.SpendBrotherhoodsPoints(_card);
                    _card.ReadyToDeploy = false;
                    _card.DeployedOnBoard = true;
                }
                else // If card inside the bottom 15% of the screen
                {
                    if (selectedCard.DeployedOnBoard)
                    {
                        selectedCard.transform.SetParent(deckController.CorrectBoardArea(_card));
                    }
                    // Return to hand
                    else
                    {
                        selectedCard.transform.SetParent(deckController.CorrectHand(_card));
                    }
                }

                _card.ShowCardCanBeDeployed(); // Removes visual feedback after deploy
                selectedCard.gameObject.layer = 0; // Set the card layer to "Default"
            }

            ClearSelected(); // Reset selectedCard and target variables
            deckController.CheckCardsInHandDeployReadiness();
        }
    }

    /// <summary>
    /// Returns true if the mouse is within the specified part of the bottom of the screen.
    /// </summary>
    /// <param name="_distance">Percentage of bottom of screen.</param>
    bool ScreenInEdgeBottomDetection(float _distance)
    {
        float screenMaxHeight = Screen.safeArea.height;
        int mouseClampedYPosition = (int)Math.Clamp(Input.mousePosition.y, 0f, screenMaxHeight);

        // Returns true if the mouse is within the specified percentage of the bottom of the screen (_distance)
        return mouseClampedYPosition <= screenMaxHeight * _distance ? true : false;
    }

    void ClearSelected()
    {
        selectedCardPreviousParent = null;
        selectedCard = null;
        target = null;
    }
}