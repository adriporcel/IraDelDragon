using UnityEngine;

public class ChildArranger : MonoBehaviour
{
    [SerializeField] float distanceBetweenChildren;

    int lastChildCount;

    void Start()
    {
        lastChildCount = transform.childCount;
        ArrangeChildren();
    }

    void Update()
    {
        if (lastChildCount != transform.childCount)
        {
            SetChildrenPosition();
            ArrangeChildren();
            lastChildCount = transform.childCount;
        }
    }

    void SetChildrenPosition()
    {
        int childCount = transform.childCount;

        if (childCount <= 1) return;

        Transform newChild = transform.GetChild(childCount - 1);

        for (int i = 0; i < childCount; i++)
        {
            Transform currentChild = transform.GetChild(i);

            if (currentChild.transform.position.x > newChild.transform.position.x)
            {
                newChild.SetSiblingIndex(i);
                return;
            }
        }
    }

    void ArrangeChildren()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        Vector3 parentPosition = transform.position;
        float halfDistance = (childCount - 1) * distanceBetweenChildren / 2f;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 childPosition = new Vector3(parentPosition.x + (i * distanceBetweenChildren) - halfDistance,
                                                parentPosition.y,
                                                parentPosition.z
                                                );
            
            child.GetComponent<SmoothMove>().MoveTo(childPosition);
        }
    }
}
