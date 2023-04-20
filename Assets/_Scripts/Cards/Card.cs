using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Players Owner { get; set; }
    public BoardPosition BoardPosition { get; set; }
    public ScriptableCard scriptableCard { get; set; }

    public UnityEvent cardAction;
    public bool ActiveBrotherhood { get; set; }
    public bool DeployedOnBoard { get; set; }
    public bool ReadyToDeploy { get; set; }
    public bool AvailableToUse { get; set; }

    [SerializeField] GameObject _front;
    // Visual feedback
    [SerializeField] GameObject activeIndicator, readyToDeployIndicator;

    public string Name { get { return scriptableCard.name; } }
    // Card cost
    public int RedCost { get { return scriptableCard.redCost; } }
    public int GreenCost { get { return scriptableCard.greenCost; } }
    public int BlueCost { get { return scriptableCard.blueCost; } }
    public int GreyCost { get { return scriptableCard.greyCost; } }
    // Card stats
    public int Attack { get { return scriptableCard.attack; } }
    public int Defense { get { return scriptableCard.defense; } }
    public int MagicalNumber { get { return scriptableCard.magicalNumber; } }
    // Card properties and perks
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
    }

    private void Update()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = $"ActiveBrotherhood: {ActiveBrotherhood}\n" +
                                                         $"DeployedOnBoard: {DeployedOnBoard}\n" +
                                                         $"ReadyToDeploy: {ReadyToDeploy}\n" +
                                                         $"AvailableToUse: {AvailableToUse}\n";
    }

    /// <summary>
    /// Activates or deactivates the brotherhood.
    /// If the roundReset parameter is true, the brotherhood is deactivated.
    /// </summary>
    /// <param name="roundReset">True if the round is being reset, false otherwise.</param>
    public void ToggleActivateBrotherhood(bool roundReset = false)
    {
        if (CardType != CardType.brotherhood || !DeployedOnBoard || !AvailableToUse)
            return;

        ActiveBrotherhood = roundReset ? false : !ActiveBrotherhood;
        activeIndicator.SetActive(ActiveBrotherhood); // DEBUG: Temporary visual indicator (trello task)
    }

    /// <summary>
    /// Gives the player visual feedback of which cards in hand can be deployed
    /// </summary>
    public void ShowCardCanBeDeployed()
    {
        readyToDeployIndicator.SetActive(ReadyToDeploy);
    }

    public void CardAction()
    {
        cardAction.Invoke();
    }
}
