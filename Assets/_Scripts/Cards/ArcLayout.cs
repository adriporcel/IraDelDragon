using UnityEngine;

public class ArcLayout : MonoBehaviour
{
    [SerializeField] float arcAngle, radius, yOffset; //  values 250, 0.15, -.75

    void Update()
    {
        int numItems = transform.childCount;
        float angleBetweenItems = arcAngle / (numItems - 1);

        for (int i = 0; i < numItems; i++)
        {
            Transform item = transform.GetChild(i);

            float angle = angleBetweenItems * i - arcAngle / 2;
            float y = Mathf.Cos(angle * Mathf.Deg2Rad) * radius + yOffset;

            if (!float.IsNaN(y))
            {
                item.localPosition = new Vector3(item.localPosition.x, y, 0);
            }
        }
    }
}