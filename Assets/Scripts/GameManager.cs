using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private LayerMask hitPlaneLayer;
    private LayerMask hitBoxLayer;
    private bool isMouseDown;
    private Vector3 initialMousePosition;
    private Vector2 normalizedDirection;
    private GameObject hitBox;
    private bool isAnimating;

    public float stageOneSpeed = 7f;
    public float stageTwoSpeed = 7f;
    public float arrowRotateSpeed = 10f;
    public float distanceBetweenMouse = 50f;

    public GameObject arrow;

    SoundManager soundManager;

    public CanvasGroup nextLevelMenu;

    // Start is called before the first frame update
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();

        hitPlaneLayer = LayerMask.GetMask("Plane");
        hitBoxLayer = LayerMask.GetMask("Box");
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimating) return;

        if (!isAnimating)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hitBoxLayer))
            {
                if (hit.transform.CompareTag("Box") || hit.transform.CompareTag("Alien"))
                {
                    foreach (MovableObject m in FindObjectsOfType<MovableObject>())
                    {
                        m.DisableOutlines();
                    }

                    MovableObject mo = hit.transform.GetComponent<MovableObject>();
                    if (mo.moveable)
                    {
                        mo.EnableOutlines();
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hitBoxLayer))
            {
                if (!hit.transform.CompareTag("Wall"))
                {
                    if (hit.transform.GetComponent<MovableObject>().moveable)
                    {
                        isMouseDown = true;
                        initialMousePosition = hit.point;
                        hitBox = hit.collider.gameObject;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isMouseDown)
        {
            isMouseDown = false;
            Debug.Log($"Normalized Direction: {normalizedDirection}");
            if (hitBox != null)
            {
                foreach (MovableObject m in FindObjectsOfType<MovableObject>())
                {
                    m.DisableOutlines();
                }

                Debug.Log($"Box to Move: {hitBox.name}");
                //hitBox.transform.position = new Vector3(hitBox.transform.position.x+normalizedDirection.x, hitBox.transform.position.y + normalizedDirection.y, hitBox.transform.position.z);
                if (hitBox.CompareTag("Alien"))
                {
                    StartCoroutine(MoveBox(hitBox, normalizedDirection, stageOneSpeed, stageTwoSpeed, resetRotation: false, alien: true));
                }else if (hitBox.CompareTag("Box"))
                {
                    StartCoroutine(MoveBox(hitBox, normalizedDirection, stageOneSpeed, stageTwoSpeed));
                }
            }
            hitBox = null;
        }

        if (isMouseDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hitPlaneLayer))
            {
                Vector3 currentMousePosition = hit.point;
                Vector2 direction = currentMousePosition - initialMousePosition;

                if (Vector3.Distance(currentMousePosition, initialMousePosition) > distanceBetweenMouse)
                {
                    arrow.GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    arrow.GetComponent<SpriteRenderer>().enabled = false;
                }

                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    normalizedDirection = direction.x > 0 ? Vector2.right : Vector2.left;
                }
                else
                {
                    normalizedDirection = direction.y > 0 ? Vector2.up : Vector2.down;
                }

                arrow.transform.position = hitBox.transform.position;
                float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle-90f);
                arrow.transform.rotation = Quaternion.Slerp(arrow.transform.rotation, targetRotation, Time.deltaTime * arrowRotateSpeed);
            }
        }
        else
        {
            arrow.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private IEnumerator MoveBox(GameObject box, Vector2 direction, float stageOneSpeed, float stageTwoSpeed, bool resetRotation = true, bool alien = false)
    {
        isAnimating = true;

        float randomTorque = Random.Range(100f, 160f);
        int randomDirection = Random.value < 0.5f ? -1 : 1;

        Vector3 direction3D = new Vector3(direction.x, direction.y, 0f);
        Ray ray = new Ray(box.transform.position, direction3D);

        string hitTag = "";

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hitBoxLayer))
        {
            hitTag = hit.transform.tag;

            Vector3 targetPosition = hit.point - (direction3D * (box.transform.localScale.x / 2));
            float distance = Vector3.Distance(box.transform.position, targetPosition);
            float stageOneTime = distance / stageOneSpeed;
            float startTime = Time.time;
            Vector3 startPosition = box.transform.position;
            Quaternion startRotation = box.transform.rotation;
            while (Time.time < startTime + stageOneTime)
            {
                float t = (Time.time - startTime) / stageOneTime;
                box.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                float angle = randomTorque * Time.deltaTime * randomDirection;
                box.transform.Rotate(Vector3.forward, angle);

                yield return null;
            }
            box.transform.position = targetPosition;
        }

        // SECOND PART OF ANIMATION

        switch (hitTag)
        {
            case "Box":
                if (box.CompareTag("Box"))
                {
                    soundManager.PlayWoodHit();
                }
                else
                {
                    soundManager.PlaySqueakyToyHit();
                }
                break;
            case "Alien":
                soundManager.PlaySqueakyToyHit();
                break;
            case "Wall":
                if (box.CompareTag("Box"))
                {
                    soundManager.PlayWoodHit();
                }
                else
                {
                    soundManager.PlaySqueakyToyHit();
                }
                break;
        }

        float zRotation = box.transform.eulerAngles.z;
        float closest90Degree = Mathf.Round(zRotation / 90f) * 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, closest90Degree);

        Vector3 finalPosition = box.transform.position;
        finalPosition.x = Mathf.Round(finalPosition.x);
        finalPosition.y = Mathf.Round(finalPosition.y);
        finalPosition.x = Mathf.Clamp(finalPosition.x, -2f, 2f);
        finalPosition.y = Mathf.Clamp(finalPosition.y, -2f, 2f);

        float distanceStageTwo = Vector3.Distance(box.transform.position, finalPosition);
        float stageTwoTime = distanceStageTwo / stageTwoSpeed;
        float stageTwoStartTime = Time.time;
        Vector3 stageTwoStartPosition = box.transform.position;
        Vector3 originalScale = box.transform.localScale;
        Vector3 bounceScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z);

        while (Time.time < stageTwoStartTime + stageTwoTime)
        {
            float t = (Time.time - stageTwoStartTime) / stageTwoTime;
            box.transform.position = Vector3.Lerp(stageTwoStartPosition, finalPosition, t);
            if (resetRotation) {
                box.transform.rotation = Quaternion.Slerp(box.transform.rotation, targetRotation, t);
            }

            float bounceValue = Mathf.Sin(t * Mathf.PI);
            box.transform.localScale = Vector3.Lerp(originalScale, bounceScale, bounceValue);

            yield return null;
        }

        box.transform.position = finalPosition;
        box.transform.localScale = originalScale;


        if (alien && box.transform.position == new Vector3(0, 0, -1))
        {
            float animationDuration = 3f;
            float animationStartTime = Time.time;
            float rotationSpeed = 10f;
            Vector3 initialScale = box.transform.localScale;
            Vector3 finalScale = Vector3.zero;
            Vector3 initialPosition = box.transform.position;
            finalPosition = initialPosition + new Vector3(0, 0, 5);

            while (Time.time < animationStartTime + animationDuration)
            {
                float elapsed = Time.time - animationStartTime;
                float t = elapsed / animationDuration;
                float acceleratedRotationSpeed = rotationSpeed * (1 + elapsed * 10 * elapsed * elapsed);

                box.transform.Rotate(0, 0, acceleratedRotationSpeed * Time.deltaTime);
                box.transform.localScale = Vector3.Lerp(initialScale, finalScale, t);
                box.transform.position = Vector3.Lerp(initialPosition, finalPosition, t);

                yield return null;
            }

            box.transform.localScale = finalScale;
            box.transform.position = finalPosition;

            nextLevelMenu.alpha = 1;
            nextLevelMenu.blocksRaycasts = true;
        }
        else
        {
            isAnimating = false;
        }
    }
}
