using System.Collections;
using UnityEngine;

public class SmoothMove : MonoBehaviour
{
    public bool IsMoving { get { return isMoving; } }

    [SerializeField] float animationSpeed; // Duration of the movement in seconds

    Vector3 targetPosition;
    bool isMoving;

    /// <summary>
    /// Moves the gameObject to the specified position within the time specified.
    /// </summary>
    /// <param name="position">Vector3 position to move to.</param>
    /// <param name="moveTime">Time in seconds to complete the movement.</param>
    public void MoveTo(Vector3 position, float moveTime = 0)
    {
        targetPosition = position;

        if (!isMoving)
            StartCoroutine(Move(moveTime));
    }

    IEnumerator Move(float time)
    {
        isMoving = true;

        float _animationSpeed = time == 0 ? animationSpeed : time;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < _animationSpeed)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / _animationSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }
}
