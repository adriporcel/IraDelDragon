using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState gameState;
    public GameObject selectedCard;

    [SerializeField] bool _debugMode;
    [SerializeField] DeckController deckController;

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
        if (_debugMode)
        {
            UnityEditor.EditorWindow.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }
    }

    private void Update()
    {
        return; // TODO: implement states
        switch (gameState)
        {
            case GameState.start:
                deckController.InitialDeal();
                break;
            case GameState.mainPlayerTurn:
                break;
            case GameState.secondPlayerTurn:
                break;
            case GameState.endGame:
                break;
            default:
                break;
        }
    }

    public void SelectCard(GameObject card)
    {
        selectedCard = card;
    }
}