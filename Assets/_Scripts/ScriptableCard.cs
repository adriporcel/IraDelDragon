using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card")]
public class ScriptableCard : ScriptableObject
{
    public Sprite artwork;
    public Deck deck;
    public CardType cardType;

    public int redCost;
    public int greebCost;
    public int blueCost;
    public int greyCost;

    public new string name;
    public string description;

    public List<CardType> canTarget; // Determines which cards it can target
    public int magicalNumber; // Middle number at the bottom of the card in brackets

    public bool hidden;

    [Header("Creature specific")]
    public int attack;
    public int defense;

    public bool hands; // Determines if the creature can hold magical objects
    public bool fly;
    public bool nightCreature;

    public List<GameObject> objectsEquiped;

}