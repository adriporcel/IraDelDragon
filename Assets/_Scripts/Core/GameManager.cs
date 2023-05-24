using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool DebugMode { get { return debugMode; } }
    public int MainPlayerHealth { get { return mainPlayerHealth; } }
    public int SecondaryPlayerHealth { get { return secondaryPlayerHealth; } }

    public static GameManager instance;

    public GameState gameState;
    public GameObject selectedCard;
    public int mainPlayerInitialHealth, secondaryPlayerInitialHealth; // Game default is 21

    [SerializeField] bool debugMode;
    [SerializeField] DeckController deckController;
    [SerializeField] int mainPlayerHealth, secondaryPlayerHealth;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Application.targetFrameRate = 144; // TODO: create setting
        if (debugMode)
        {
            UnityEditor.EditorWindow.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }

        print($"Debug mode set to: {debugMode}"); // DEBUG
        gameState = GameState.start; // DEBUG
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.start:
                GameStartSetHealth(); // Set players max health
                deckController.InitialDeal(); // Deal initial number of cards to players hands
                gameState = GameState.mainPlayerTurn; // main player will begin his turn
                break;

            case GameState.mainPlayerTurn:
                GameOverCheck();
                break;

            case GameState.secondPlayerTurn:
                GameOverCheck();
                break;

            case GameState.endGame:
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Called once at the start of the game, set's player starting health points
    /// </summary>
    void GameStartSetHealth()
    {
        mainPlayerHealth = mainPlayerInitialHealth;
        secondaryPlayerHealth = secondaryPlayerInitialHealth;
    }

    void GameOverCheck()
    {
        if (MainPlayerHealth <= 0)
        {
            print($"{Players.main} lost the game");
            gameState = GameState.endGame;
        }
        else if (SecondaryPlayerHealth <= 0)
        {
            print($"{Players.secondary} lost the game");
            gameState = GameState.endGame;
        }
    }

    public void SelectCard(GameObject card)
    {
        selectedCard = card;
    }
}
