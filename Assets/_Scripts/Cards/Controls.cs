using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Controls : MonoBehaviour
{
    [SerializeField] Transform selectedCardPreviousParent;
    [SerializeField] Card selectedCard, target, hoverOverCard;
    //
    [SerializeField] Vector2 mouseClickCoords;
    [SerializeField] float clickHoldDistanceTrigger;
    [SerializeField] float magnifyCardTimerHand, magnifyCardTimerBoard;
    [SerializeField] GameObject cardMagnifiedPrefab;
    [Range(1, 20)][SerializeField] int touchscreenPrecissionMultiplier;

    DeckController deckController;

    GameObject cardMagnifiedGameObject;
    float magnifyCardTimerStart;
    bool clickHoldDistanceSurpassed;

    void Start()
    {
        deckController = FindObjectOfType<DeckController>();

        deckController.CheckCardsInHandDeployReadiness();
    }

    void Update()
    {
        Vector2 mousePosScreen = Input.mousePosition;
        // Distance from camera
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosScreen.x,
                                                                                mousePosScreen.y,
                                                                                10.21f)); 

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var mouseOverObject = Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity);
        
        // Card magnification
        if (hoverOverCard != null && hit.collider.gameObject != hoverOverCard.gameObject)
        {
            hoverOverCard = null;
            ToggleCardMagnify(false);
            magnifyCardTimerStart = 0;
        }
        if (!Input.GetKeyDown(0)
            && mouseOverObject
            && hit.collider.gameObject.layer == 0
            && hit.collider.CompareTag("card"))
        {
            hoverOverCard = hit.collider.GetComponent<Card>();

            // TODO: take into account who is playing, to be implemented with multiplayer
            if (hoverOverCard.Owner == Players.main || hoverOverCard.Owner != Players.main && hoverOverCard.DeployedOnBoard)
            {
                if (magnifyCardTimerStart == 0)
                {
                    magnifyCardTimerStart = Time.time;
                }
                else if (magnifyCardTimerStart + magnifyCardTimerHand < Time.time
                         && !hoverOverCard.DeployedOnBoard
                         || magnifyCardTimerStart + magnifyCardTimerBoard < Time.time
                         && hoverOverCard.DeployedOnBoard)
                {
                    ToggleCardMagnify(true);
                }
            }
        }

        // Move cards and actions
        if (Input.GetMouseButtonDown(0))
        {
            mouseClickCoords = mousePosScreen;

            // If a card is selected and no other card is currently selected
            if (selectedCard == null && hit.collider.CompareTag("card"))
            {
                Card card = hit.collider.gameObject.GetComponent<Card>();

                if (card.ReadyToDeploy || card.DeployedOnBoard)
                {
                    selectedCard = card;
                    card.gameObject.layer = 2;
                }
            }
        }
        else if (Input.GetMouseButton(0) && selectedCard != null && mouseOverObject)
        {
            // Checks if the mouse has moved outside of the initial click area range, to trigger movement of the card
            if (clickHoldDistanceSurpassed
                || Vector3.Distance(mousePosScreen, mouseClickCoords) > TouchscreenPrecissionMultiplier(clickHoldDistanceTrigger))
            {
                clickHoldDistanceSurpassed = true;

                // Card must not be in use or spent already in order to move it
                if (selectedCard.ReadyToDeploy || selectedCard.AvailableToUse)
                {
                    // Sets the card parent to gameObject.transform
                    if (selectedCard.transform.parent != gameObject.transform)
                    {
                        selectedCard.transform.SetParent(gameObject.transform);
                        selectedCard.GetComponent<SmoothMove>().MoveTo(new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, -1), .12f);
                    }
                    else // Moves selectedCard to mouse position
                    {
                        selectedCard.GetComponent<SmoothMove>().MoveTo(new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, -1), .025f);
                    }
                }

                //If targetCard has card tag, is set as target, else target is null
                GameObject targetCard = hit.collider.gameObject;
                target = targetCard.CompareTag("card") ? targetCard.GetComponent<Card>() : null;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (selectedCard != null)
            {
                Card _card = selectedCard.GetComponent<Card>();

                // Activates ONLY brotherhood, Deployed and Available checks done in ToggleActivateBrotherhood
                if (!clickHoldDistanceSurpassed)
                {
                    _card.ToggleActivateBrotherhood();
                    deckController.CountBrotherhoodPoints();
                    deckController.CheckCardsInHandDeployReadiness();
                }

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
                    else // Return to hand
                    {
                        selectedCard.transform.SetParent(deckController.CorrectHand(_card));
                    }
                }

                _card.ShowCardCanBeDeployed(); // Removes visual feedback after deploy
                selectedCard.gameObject.layer = 0; // Set the card layer to "Default"
            }

            // The following must be clear before a new click action begins
            clickHoldDistanceSurpassed = false;
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
        return mouseClampedYPosition <= screenMaxHeight * _distance;
    }

    float TouchscreenPrecissionMultiplier(float number)
    {
        if (Input.touchCount == 1)
        {
            return number * touchscreenPrecissionMultiplier;
        }
        else
        {
            return number;
        }
    }

    /// <summary>
    /// Instantiates a MagnifiedCard game object or Destroys it depending on parameters.
    /// </summary>
    /// <param name="_display">Magnified card is diplayed or not depending on this.</param>
    void ToggleCardMagnify(bool _display)
    {
        if (!_display)
        {
            Destroy(cardMagnifiedGameObject);
            cardMagnifiedGameObject = null;
            return;
        }
        else if (cardMagnifiedGameObject != null)
            return;

        // Instantiates magnified card and passes on hoverCard information to it.
        cardMagnifiedGameObject = Instantiate(cardMagnifiedPrefab);
        CardMagnified _cardMagnified = cardMagnifiedGameObject.GetComponent<CardMagnified>();
        _cardMagnified.Card = hoverOverCard;
    }

    void ClearSelected()
    {
        selectedCard = null;
        target = null;
    }
}