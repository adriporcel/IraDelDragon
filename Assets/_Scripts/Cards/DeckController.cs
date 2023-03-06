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
        Card _cardComponent = newCard.GetComponent<Card>();

        _cardComponent.Owner = _player;
        _cardComponent.scriptableCard = availableCards[Random.Range(0, availableCards.Count)];

        if (_player == Players.main)
        {
            _cardComponent.BoardPosition = BoardPosition.handMain;
            newCard.transform.SetParent(_handMain.transform);
        }
        else
        {
            _cardComponent.BoardPosition = BoardPosition.handSecond;
            newCard.transform.Rotate(new Vector3(0,0,1), 180);
            newCard.transform.SetParent(_handSecond.transform);
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
