using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] ScriptableCard scriptableCard;
    [SerializeField] GameObject front;

    void Start()
    {
        Material material = new Material(Shader.Find("Standard"));
        material.mainTexture = scriptableCard.artwork;

        front.GetComponent<MeshRenderer>().material = material;
    }
}
