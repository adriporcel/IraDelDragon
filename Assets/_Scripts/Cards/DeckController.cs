using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    public List<ScriptableCard> availableCards;

    [SerializeField] GameObject _baseCard;

    [Header("Board Areas")]
    [SerializeField] GameObject _playAreaSecond;
    [SerializeField] GameObject _brotherhoodAreaSecond;
    [SerializeField] GameObject _brotherhoodsAreaMain;
    [SerializeField] GameObject _playAreaMain;
    [SerializeField] GameObject _handSecond;
    [SerializeField] GameObject _handMain;

    [Header("Game Settings")]
    [SerializeField] int initialNumberOfCards;

    GameObject newCard;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            print("Dealing cards");
            DealCard(Players.main);
            DealCard(Players.secondary);
        }
    }

    public void InitialDeal()
    {
        for (int i = 0; i < initialNumberOfCards; i++)
        {
            DealCard(Players.main);
            DealCard(Players.secondary);
        }
    }

    void DealCard(Players _player)
    {
        newCard = Instantiate(_baseCard);
        newCard.GetComponent<Card>().scriptableCard = availableCards[Random.Range(0, availableCards.Count)];

        if (_player == Players.main)
        {

        }
        else
        {

        }
    }

    void PlaceCardOnBoard()
    {
        if (newCard.GetComponent<Card>().CardType == CardType.brotherhood)
        {
            newCard.transform.SetParent(_brotherhoodsAreaMain.transform, false);
        }
        else
        {
            newCard.transform.SetParent(_playAreaMain.transform, false);
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
