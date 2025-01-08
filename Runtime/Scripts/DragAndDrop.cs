using Codice.Client.Commands;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DragAndDrop : MonoBehaviour
{
    bool held = false;  // Flag to see if the object is held.
    float zDistance;    // Cached z-distance from the camera.
    Camera mainCamera;  // Cache the main camera reference.

    // The destination of the object and the distance necessary.
    [SerializeField] Transform target;
    [SerializeField] float distanceThreshold = 1f;

    // Whether or not the object should return to its position.
    [SerializeField] bool returnOnRelease = false;
    Vector3 startPosition;

    Vector3 offset;     // Offset between the touch position and object position.

    [SerializeField] ScrapPuzzles puzzle;

    void Start()
    {
        if (target != null && returnOnRelease) startPosition = transform.position;

        // Cache the camera and Z distance.
        mainCamera = Camera.main;
        zDistance = transform.position.z - mainCamera.transform.position.z;
    }

    void Update()
    {
        if (Input.touchCount == 0) return; // Exit early if no touch input.

        Touch touch = Input.GetTouch(0);  // Get the first touch input.
        Vector3 touchPos = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, zDistance));

        switch (touch.phase)
        {
            case TouchPhase.Began: // Raycast to check if the touch is on this object.
                if (Physics2D.OverlapPoint(touchPos) == GetComponent<Collider2D>())
                {
                    held = true;
                    offset = transform.position - touchPos; // Calculate the offset.
                }
                break;

            case TouchPhase.Moved:  // Update position only if it has changed.
                if (held)
                {
                    Vector3 newPos = touchPos + offset; // Apply the offset.
                    if (transform.position != newPos) transform.position = newPos;
                }
                CheckRelease(false);
                break;

            case TouchPhase.Ended:  // Release the object when touch ends.
            case TouchPhase.Canceled: // Release the object when touch is canceled.
                held = false;
                CheckRelease(true);
                break;
        }
    }

    void CheckRelease(bool released)
    {
        // If there is a target destination...
        if (target != null)
        {
            // Compare the distance of the object to its target destination.
            if (Vector3.Distance(transform.position, target.position) <= distanceThreshold)
            {
                if (held) held = false;

                // Set the position to the target's.
                transform.position = target.position;

                puzzle.Resolve();

                // Remove the components.
                Destroy(this);
                Destroy(GetComponent<Collider2D>());
            }

            // Otherwise, return to the original position, if necessary.
            else if (returnOnRelease && released) transform.position = startPosition;
        }
    }
}
