using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] ScriptableCard scriptableCard;
    
    [SerializeField] Sprite artwork;

    void Start()
    {
        artwork = scriptableCard.artwork;
    }
}
