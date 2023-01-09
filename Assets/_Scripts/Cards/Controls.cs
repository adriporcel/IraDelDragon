using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "card")
                {
                    print(hit.collider.gameObject.GetComponent<Card>().Name);
                    GameManager.instance.SelectCard(hit.collider.gameObject);
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            GameManager.instance.SelectCard(null);
        }
    }
}

