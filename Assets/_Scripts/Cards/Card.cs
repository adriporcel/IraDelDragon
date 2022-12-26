using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public ScriptableCard scriptableCard { get; set; }
    [SerializeField] GameObject front;

    void Start()
    {
        Material material = new Material(Shader.Find("Standard"));
        material.mainTexture = scriptableCard.artwork;

        front.GetComponent<MeshRenderer>().material = material;

        if (scriptableCard.script != null)
            gameObject.AddComponent(scriptableCard.script.GetClass());

        Debug.Log($"{scriptableCard.name}, {scriptableCard.artwork.name}");
    }
}
