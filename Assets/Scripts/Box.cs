using UnityEngine;

public class Box : MonoBehaviour
{
    private Vector2 objectCenter;
    private bool isMouseDown;
    private bool isMouseOut;
    private Vector2 direction;

    void Update()
    {
        if (isMouseDown && isMouseOut)
        {
            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 directionVector = cursorPosition - objectCenter;

            if (Mathf.Abs(directionVector.x) > Mathf.Abs(directionVector.y))
            {
                direction = directionVector.x > 0 ? Vector2.right : Vector2.left;
            }
            else
            {
                direction = directionVector.y > 0 ? Vector2.up : Vector2.down;
            }
        }
    }

    private void OnMouseDown()
    {
        isMouseDown = true;
        isMouseOut = false;
        objectCenter = transform.position;
    }

    private void OnMouseUp()
    {
        if (isMouseOut)
        {
            Debug.Log($"Direction: {direction}");
        }

        isMouseDown = false;
        isMouseOut = false;
    }

    private void OnMouseExit()
    {
        if (isMouseDown)
        {
            isMouseOut = true;
        }
    }
}
