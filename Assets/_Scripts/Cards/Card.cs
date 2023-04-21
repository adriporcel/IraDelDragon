using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Card : MonoBehaviour
{
    public Players Owner { get; set; }
    public BoardPosition BoardPosition { get; set; }
    public ScriptableCard ScriptableCard { get; set; }

    public UnityEvent cardAction;
    public bool ActiveBrotherhood { get; set; }
    public bool DeployedOnBoard { get; set; }
    public bool ReadyToDeploy { get; set; }
    public bool AvailableToUse { get; set; }

    [SerializeField] GameObject _front;

    // Visual feedback
    [SerializeField] GameObject activeIndicator, usedIndicator, readyToDeployIndicator;
    bool availableToUseLastCheck = true;

    public string Name { get { return ScriptableCard.name; } }

    // Card cost
    public int RedCost { get { return ScriptableCard.redCost; } }
    public int GreenCost { get { return ScriptableCard.greenCost; } }
    public int BlueCost { get { return ScriptableCard.blueCost; } }
    public int GreyCost { get { return ScriptableCard.greyCost; } }

    // Card stats
    public int Attack { get { return ScriptableCard.attack; } }
    public int Defense { get { return ScriptableCard.defense; } }
    public int MagicalNumber { get { return ScriptableCard.magicalNumber; } }

    // Card properties and perks
    public bool Hidden { get { return ScriptableCard.hidden; } }
    public bool Hands { get { return ScriptableCard.hands; } }
    public bool NightCreature { get { return ScriptableCard.nightCreature; } }

    public Deck Deck { get { return ScriptableCard.deck; } }
    public CardType CardType { get { return ScriptableCard.cardType; } }
    public ObjectType ObjectType { get { return ScriptableCard.objectType; } }
    public List<CardType> CanTarget { get { return ScriptableCard.canTarget; } }


    void Start()
    {
        // Set card artwork
        Material material = new(Shader.Find("Standard"))
        {
            mainTexture = ScriptableCard.artwork
        };
        _front.GetComponent<MeshRenderer>().material = material;

        // Add script to card if specified
        if (ScriptableCard.script != null)
        { 
            gameObject.AddComponent(ScriptableCard.script.GetClass());
        }
    }
    
    private void Update()
    {
        if (DeployedOnBoard && AvailableToUse != availableToUseLastCheck)
        {
            usedIndicator.SetActive(!AvailableToUse);
            availableToUseLastCheck = AvailableToUse;
        }

        // DEBUG
        if (GameManager.instance.DebugMode  && GetComponentInChildren<TextMeshProUGUI>().enabled)
        {
            GetComponentInChildren<TextMeshProUGUI>().text = $"ActiveBrotherhood: {ActiveBrotherhood}\n" +
                                                             $"DeployedOnBoard: {DeployedOnBoard}\n" +
                                                             $"ReadyToDeploy: {ReadyToDeploy}\n" +
                                                             $"AvailableToUse: {AvailableToUse}\n";
        }
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

        ActiveBrotherhood = !roundReset && !ActiveBrotherhood;
        activeIndicator.SetActive(ActiveBrotherhood); // DEBUG: Temporary visual indicator (trello task)
    }

    /// <summary>
    /// Gives the player visual feedback of which cards in hand can be deployed depending on ReadyToDeploy variable
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