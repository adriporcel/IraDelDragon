using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [SerializeField] Transform selectedCardPreviousParent;
    [SerializeField] GameObject selectedCard, target;
    
    List<Collider> playAreaColliders = new();

    void Start()
    {
        foreach (GameObject area in GameObject.FindGameObjectsWithTag("dropArea"))
        {
            playAreaColliders.Add(area.GetComponent<Collider>());
        }
    }

    void Update()
    {
        Vector3 mousePosScreen = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosScreen.x,
                                                                                mousePosScreen.y,
                                                                                10.21f)); // Distance from camera

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            // If a card is selected and no other card is currently selected
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) &&
                hit.collider.tag == "card" &&
                selectedCard == null)
            {
                Card card = hit.collider.gameObject.GetComponent<Card>();
                selectedCard = card.gameObject;
                card.gameObject.layer = 2;

                selectedCardPreviousParent = card.transform.parent;
                card.transform.SetParent(gameObject.transform);
            }

            SetActivePlayAreaColliders(true); // Enable the play area colliders
        }
        else if (Input.GetMouseButton(0) && Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            /* If the left mouse button is held down and a collider is hit,
               moves the card to the mouse position and set the target to the colider that is hit */
            if (selectedCard != null)
            {
                selectedCard.GetComponent<SmoothMove>().MoveTo(new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, -1), .05f);

                var thisTarget = hit.collider.gameObject;
                if (thisTarget.tag == "dropArea")
                {
                    target = thisTarget;
                }
                else
                {
                    target = null;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (target != null)
            {
                selectedCard.layer = 0; // Set the card layer to "Default"
                selectedCard.transform.SetParent(target.transform); // Set the card's parent to the target object
            }
            else if (selectedCard != null)
            {
                selectedCard.layer = 0; // Set the card layer to "Default"
                selectedCard.transform.SetParent(selectedCardPreviousParent); // Set the card's parent back to its previous parent
            }

            ClearSelecteds(); // Reset selectedCard and target
            SetActivePlayAreaColliders(false); // Disable the play area colliders
        }
    }

    private void ClearSelecteds()
    {
        selectedCardPreviousParent = null;
        selectedCard = null;
        target = null;
    }

    void SetActivePlayAreaColliders(bool state)
    {
        foreach (var col in playAreaColliders)
        {
            col.enabled = state;
        }
    }
}