using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    public List<ScriptableCard> availableCards;

    [SerializeField] GameObject baseCard;

    [Header("Board Areas")]
    [SerializeField] GameObject brotherhoodAreaSecond;
    [SerializeField] GameObject playAreaSecond;
    [SerializeField] GameObject handSecond;
    [SerializeField] GameObject brotherhoodsAreaMain;
    [SerializeField] GameObject playAreaMain;
    [SerializeField] GameObject handMain;

    [Header("Game Settings")]
    [SerializeField] int initialNumberOfCards;

    [Header("Game Stats")]
    [SerializeField] int redBrotherhoodPoints;
    [SerializeField] int greenBrotherhoodPoints;
    [SerializeField] int blueBrotherhoodPoints;
    [SerializeField] int greyBrotherhoodPoints;
    [SerializeField] int magicalBrotherhoodPoints;

    GameObject newCard;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            print("Dealing cards");
            DealCard(Players.main);
            DealCard(Players.secondary);
        }
        if (Input.GetKeyDown(KeyCode.R)) // DEBUG: remove, this must happen at the beginning of each round
        {
            RoundStartSetCardsToAvailable();
        }
    }

    /// <summary>
    /// Initial game deal, happens only once per game at the beginning.
    /// </summary>
    public void InitialDeal()
    {
        for (int i = 0; i < initialNumberOfCards; i++)
        {
            DealCard(Players.main);
            DealCard(Players.secondary);
        }
    }

    /// <summary>
    /// Checks if the player has enough resources to play a card, including both color brotherhood points and magical brotherhood points.
    /// </summary>
    /// <param name="_card">The card to be played.</param>
    /// <returns>True if the player has enough resources to play the card, false otherwise.</returns>
    public bool CheckAvailableBrotherhoods(Card _card)
    {
        if (_card.DeployedOnBoard) // Card cost has already been covered
            return true;

        if (redBrotherhoodPoints >= _card.RedCost
            && greenBrotherhoodPoints >= _card.GreenCost
            && blueBrotherhoodPoints >= _card.BlueCost
            && greyBrotherhoodPoints >= _card.GreyCost)
        {
            return true;
        }
        else if (magicalBrotherhoodPoints > 0)
        {
            int missingPoints = 0;

            if (redBrotherhoodPoints < _card.RedCost)
            {
                missingPoints += _card.RedCost - redBrotherhoodPoints;
            }
            if (greenBrotherhoodPoints < _card.GreenCost)
            {
                missingPoints += _card.GreenCost - greenBrotherhoodPoints;
            }
            if (blueBrotherhoodPoints < _card.BlueCost)
            {
                missingPoints += _card.BlueCost - blueBrotherhoodPoints;
            }
            if (greyBrotherhoodPoints < _card.GreyCost)
            {
                missingPoints += _card.GreyCost - greyBrotherhoodPoints;
            }

            if (magicalBrotherhoodPoints >= missingPoints)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Removes Brotherhood points according to the cost of the card played.
    /// The magical Brotherhood points are used if necessary, only once.
    /// </summary>
    /// <param name="_card">The card to be played.</param>
    public void UseUpActiveBrotherhoods(Card _card)
    {
        if (_card.DeployedOnBoard) // Card cost has already been covered
            return;

        redBrotherhoodPoints -= _card.RedCost;
        greenBrotherhoodPoints -= _card.GreenCost;
        blueBrotherhoodPoints -= _card.BlueCost;
        greyBrotherhoodPoints -= _card.GreyCost;

        ChangeBrotherhoodsToSpent(Deck.red, _card.RedCost);
        ChangeBrotherhoodsToSpent(Deck.green, _card.GreenCost);
        ChangeBrotherhoodsToSpent(Deck.blue, _card.BlueCost);
        ChangeBrotherhoodsToSpent(Deck.grey, _card.GreyCost);

        if (redBrotherhoodPoints < 0)
        {
            magicalBrotherhoodPoints += redBrotherhoodPoints;
            redBrotherhoodPoints = 0;
        }
        if (greenBrotherhoodPoints < 0)
        {
            magicalBrotherhoodPoints += greenBrotherhoodPoints;
            greenBrotherhoodPoints = 0;
        }
        if (blueBrotherhoodPoints < 0)
        {
            magicalBrotherhoodPoints += blueBrotherhoodPoints;
            blueBrotherhoodPoints = 0;
        }
        if (greyBrotherhoodPoints < 0)
        {
            magicalBrotherhoodPoints += greyBrotherhoodPoints;
            greyBrotherhoodPoints = 0;
        }

        if (magicalBrotherhoodPoints > 0)
        {
            int missingPoints = 0;

            if (redBrotherhoodPoints < _card.RedCost)
            {
                missingPoints += _card.RedCost - redBrotherhoodPoints;
            }
            if (greenBrotherhoodPoints < _card.GreenCost)
            {
                missingPoints += _card.GreenCost - greenBrotherhoodPoints;
            }
            if (blueBrotherhoodPoints < _card.BlueCost)
            {
                missingPoints += _card.BlueCost - blueBrotherhoodPoints;
            }
            if (greyBrotherhoodPoints < _card.GreyCost)
            {
                missingPoints += _card.GreyCost - greyBrotherhoodPoints;
            }

            if (magicalBrotherhoodPoints >= missingPoints)
            {
                ChangeBrotherhoodsToSpent(Deck.magical, missingPoints);
                magicalBrotherhoodPoints -= missingPoints;
            }
        }

        _card.DeployedOnBoard = true;
    }

    public void CheckCardsInHandDeployReadiness()
    {
        foreach (Transform child in handMain.transform)
        {
            Card _card = child.GetComponent<Card>();

            if (CheckAvailableBrotherhoods(_card) && !_card.DeployedOnBoard)
            {
                _card.ReadyToDeploy = true;
            }
            else
            {
                _card.ReadyToDeploy = false;
            }

            _card.ShowCardCanBeDeployed();
        }

    }

    public void CountBrotherhoodPoints()
    {
        // Resets all points before counting again
        redBrotherhoodPoints = 0;
        greenBrotherhoodPoints = 0;
        blueBrotherhoodPoints = 0;
        greyBrotherhoodPoints = 0;
        magicalBrotherhoodPoints = 0;

        foreach (Transform child in brotherhoodsAreaMain.transform)
        {
            Card card = child.GetComponent<Card>();

            if (card.ActiveBrotherhood && card.AvailableToUse)
            {
                switch (card.Deck)
                {
                    case Deck.red:
                        redBrotherhoodPoints++;
                        break;
                    case Deck.green:
                        greenBrotherhoodPoints++;
                        break;
                    case Deck.blue:
                        blueBrotherhoodPoints++;
                        break;
                    case Deck.grey:
                        greyBrotherhoodPoints++;
                        break;
                    case Deck.magical:
                        magicalBrotherhoodPoints++;
                        break;
                }
            }
        }
    }

    void RoundStartSetCardsToAvailable()
    {
        foreach (Transform child in brotherhoodsAreaMain.transform)
        {
            Card _card = child.GetComponent<Card>();

            _card.AvailableToUse = true;
            _card.ToggleActivateBrotherhood(true);
        }

        foreach (Transform child in playAreaMain.transform)
        {
            child.GetComponent<Card>().AvailableToUse = true;
        }

        CountBrotherhoodPoints();
        CheckCardsInHandDeployReadiness();
    }

    void ChangeBrotherhoodsToSpent(Deck _deck, int multiplier)
    {
        foreach (Transform child in brotherhoodsAreaMain.transform)
        {
            if (multiplier <= 0)
                return;

            Card card = child.GetComponent<Card>();

            if (card.ActiveBrotherhood && card.Deck == _deck)
            {
                card.AvailableToUse = false;
                multiplier--;
            }
        }
    }

    void DealCard(Players _player)
    {
        newCard = Instantiate(baseCard);
        Card _cardComponent = newCard.GetComponent<Card>();

        _cardComponent.Owner = _player;
        _cardComponent.scriptableCard = availableCards[UnityEngine.Random.Range(0, availableCards.Count)];

        if (_player == Players.main)
        {
            _cardComponent.BoardPosition = BoardPosition.handMain;
            newCard.transform.SetParent(handMain.transform);
        }
        else
        {
            _cardComponent.BoardPosition = BoardPosition.handSecond;
            newCard.transform.Rotate(new Vector3(0, 0, 1), 180);
            newCard.transform.SetParent(handSecond.transform);
        }
    }

    void LoadAllGameCards()
    {
        ScriptableCard[] objects = Resources.LoadAll<ScriptableCard>("Decks/Blue");
        foreach (ScriptableCard obj in objects)
        {
            availableCards.Add(obj);
        }
    }
}
