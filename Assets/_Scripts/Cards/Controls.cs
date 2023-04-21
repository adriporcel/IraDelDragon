using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Controls : MonoBehaviour
{
    [SerializeField] Transform selectedCardPreviousParent;
    [SerializeField] Card selectedCard, target;
    //
    [SerializeField] Vector2 mouseClickCoords;
    [SerializeField] float clickHoldDistanceTrigger;
    [SerializeField] float clickHoldTimer;
    [Range(1, 20)][SerializeField] int touchscreenPrecissionMultiplier;

    List<Collider> playAreaColliders = new();
    DeckController deckController;

    bool clickHoldDistanceSurpassed;
    bool magnifiedCard;
    float clickStartTime;

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
        Vector2 mousePosScreen = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosScreen.x,
                                                                                mousePosScreen.y,
                                                                                10.21f)); // Distance from camera

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var mouseOverObject = Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity);

        if (Input.GetMouseButtonDown(1) && hit.collider.CompareTag("card"))
        {
            ToggleCardMagnify(true);
        }

        // Left click, move cards and actions
        if (Input.GetMouseButtonDown(0))
        {
            ToggleCardMagnify(false);
            mouseClickCoords = mousePosScreen;
            clickStartTime = Time.time;

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
            if (clickStartTime + clickHoldTimer < Time.time && !magnifiedCard)
            {
                ToggleCardMagnify(true);
            }
            // Checks if the mouse has moved outside of the initial click area range, to trigger movement of the card
            else if (clickHoldDistanceSurpassed
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

                if (!magnifiedCard)
                {
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
        return mouseClampedYPosition <= screenMaxHeight * _distance ? true : false;
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

    void ToggleCardMagnify(bool display)
    {
        magnifiedCard = display;
        print($"magnify card is active: {magnifiedCard}");
    }

    void ClearSelected()
    {
        selectedCard = null;
        target = null;
    }
}