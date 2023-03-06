using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    Transform _selectedCardPreviousParent;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) &&
                hit.collider.tag == "card" &&
                GameManager.instance.selectedCard == null)
            {
                Card _card = hit.collider.gameObject.GetComponent<Card>();
                GameManager.instance.SelectCard(_card.gameObject);
                _card.gameObject.layer = 2;

                _card.transform.SetParent(gameObject.transform);
            }
        }
        else if (Input.GetMouseButton(0) && Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 _raycastHitPosition = hit.transform.position;
            GameManager.instance.selectedCard.transform.position = new Vector3(_raycastHitPosition.x,
                                                                               _raycastHitPosition.y,
                                                                               1);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Card _card = hit.collider.gameObject.GetComponent<Card>();
                if (hit.collider.tag == "card" && GameManager.instance.selectedCard != null)
                {
                    GameManager.instance.selectedCard.layer = 0;
                    _card.CardAction();
                }
                _card.transform.SetParent(_selectedCardPreviousParent);
                GameManager.instance.SelectCard(null);
            }
        }
    }
}

