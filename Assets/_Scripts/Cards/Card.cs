using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Players Owner { get; set; }
    public BoardPosition BoardPosition { get; set; }
    public ScriptableCard scriptableCard { get; set; }

    public UnityEvent cardAction;

    [SerializeField] GameObject _front;

    public string Name { get { return scriptableCard.name; } }
    public int RedCost { get { return scriptableCard.redCost; } }
    public int GreenCost { get { return scriptableCard.greenCost; } }
    public int BlueCost { get { return scriptableCard.blueCost; } }
    public int GreyCost { get { return scriptableCard.greyCost; } }
    public int Attack { get { return scriptableCard.attack; } }
    public int Defense { get { return scriptableCard.defense; } }
    public int MagicalNumber { get { return scriptableCard.magicalNumber; } }
    public bool Hidden { get { return scriptableCard.hidden; } }
    public bool Hands { get { return scriptableCard.hands; } }
    public bool NightCreature { get { return scriptableCard.nightCreature; } }
    public Deck Deck { get { return scriptableCard.deck; } }
    public CardType CardType { get { return scriptableCard.cardType; } }
    public ObjectType ObjectType { get { return scriptableCard.objectType; } }
    public List<CardType> CanTarget { get { return scriptableCard.canTarget; } }

    void Start()
    {
        //gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(.59f, .89f);

        Material material = new Material(Shader.Find("Standard"));
        material.mainTexture = scriptableCard.artwork;
        _front.GetComponent<MeshRenderer>().material = material;

        if (scriptableCard.script != null)
            gameObject.AddComponent(scriptableCard.script.GetClass());

        //Debug.Log($"{scriptableCard.name}, {scriptableCard.artwork.name}");
    }

    public void CardAction()
    {
        cardAction.Invoke();
    }
}
