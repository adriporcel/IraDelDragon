using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class CardMagnified : MonoBehaviour
{
    public Card Card { get; set; }

    [SerializeField] GameObject _front;
    [SerializeField] float targetScaleMultiplier; // TODO: Set scale multiplier in global settings
    [SerializeField] float scaleUpAnimationSpeed;
    [SerializeField] float initialAnimationOffset;

    Vector3 TargetScale
    {
        get
        {
            return new(transform.localScale.x * targetScaleMultiplier,
                       transform.localScale.y * targetScaleMultiplier,
                       1);
        }
    }

    void Start()
    {
        // Set card artwork
        Material material = new(Shader.Find("Standard"))
        {
            mainTexture = Card.Artwork
        };
        _front.GetComponent<MeshRenderer>().material = material;

        if (Card.DeployedOnBoard)
        {
            // Set initial position relative to Card with slight Z ofset
            float _cardWidth = Card.GetComponent<RectTransform>().rect.width;
            float _cardXScale = Card.transform.localScale.x;
            Vector3 _targetPos = new(Card.transform.position.x + (_cardXScale * _cardWidth + (_cardXScale * _cardWidth / 2)),
                                     Card.transform.position.y,
                                     Card.transform.position.z - .1f);
            transform.position = _targetPos;

            // Starts animation transition to TargetScale
            StartCoroutine(ScaleUp(TargetScale));
        }
        else
        {
            // Calculates target position with slight Z ofset
            float _cardHeight = GetComponent<RectTransform>().rect.height;
            float _cardYScale = transform.localScale.y;
            Vector3 _targetPos = new(Card.transform.position.x,
                                     Card.transform.position.y + (_cardHeight * _cardYScale + (_cardHeight * _cardYScale / 2)),
                                     Card.transform.position.z - .1f);

            // Set inital position and scale
            transform.localScale = TargetScale;
            transform.position = new(Card.transform.position.x,
                                     Card.transform.position.y + _cardHeight * _cardYScale / 2,
                                     Card.transform.position.z);

            GetComponent<SmoothMove>().MoveTo(_targetPos);
        }
    }

    IEnumerator ScaleUp(Vector3 _targetScale)
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, _targetScale, scaleUpAnimationSpeed);
        }
    }
}
