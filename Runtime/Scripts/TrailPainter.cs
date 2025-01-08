using System.Collections;
using UnityEngine;

public class NeonCursorManager : MonoBehaviour
{
    [SerializeField] GameObject penCursor;

    [SerializeField] float zDistance;

    [SerializeField] float smoothness;

    TrailRenderer trail;
    bool tapped;

    [SerializeField] Transform lineParent;
    [SerializeField] ScrapCursorManager manager;

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (!trail)
                {
                    trail = Instantiate(penCursor, lineParent).GetComponent<TrailRenderer>();
                }

                Vector3 touchPosition = touch.position;
                touchPosition.z = zDistance; // Set an appropriate Z distance

                Vector3 newPosition = Camera.main.ScreenToWorldPoint(touchPosition);

                trail.transform.position = newPosition;
                tapped = true;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                Vector3 touchPosition = touch.position;
                touchPosition.z = zDistance; // Set an appropriate Z distance

                Vector3 newPosition = Camera.main.ScreenToWorldPoint(touchPosition);

                trail.transform.position = Vector3.MoveTowards(trail.transform.position, newPosition, smoothness * Vector3.Distance(trail.transform.position, newPosition) * Time.deltaTime);
            }
        }
        else if (Input.touchCount == 0 && tapped) // No active touches but previously tapped
        {
            tapped = false;

            if (trail) { trail = null; }
        }

        if (Input.GetKeyDown(KeyCode.Backspace)) { ClearLines(); }
    }

    public void ClearLines()
    {
        foreach (Transform child in lineParent) Destroy(child.gameObject);
    }
}
