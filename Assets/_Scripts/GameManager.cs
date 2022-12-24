using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool debugMode;

    void Start()
    {
        Application.targetFrameRate = 144; // TODO: create setting
        if (debugMode)
        {
            UnityEditor.EditorWindow.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }
    }
}
