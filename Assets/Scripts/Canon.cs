using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Canon : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float angleOffset = 90f;

    [SerializeField] private BubbleTypes[] bubbleTypes = { BubbleTypes.RED, BubbleTypes.YELLOW, BubbleTypes.BLUE };
    [SerializeField] private int currBubbleType = 0;

    [SerializeField] private Transform canonTip;
    [SerializeField] private Transform canonCircle;
    [SerializeField] private GameObject bubbleProjectile;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject targetCell;
    [SerializeField] private GameObject positionHelper;

    #region Ray Casting
    private Vector3 raycastDirection;
    private float raycastDistance = Mathf.Infinity;
    private Vector3 rayHitPoint;
    public LayerMask ignoreLayer;
    #endregion

    Vector3 mousePos;
    bool isClicking = false;
    bool doCircleRotation = false;
    float currCircleRotation = 0;

    GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>().gameObject.GetComponent<GameManager>();
    }

    void Update()
    {
        mousePos = Mouse.current.position.ReadValue();

        RotateCircle();
        HelperPositioning();

        if (mousePos.y > 250)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                isClicking = true;
                ControlCanon();
            }
            else
            {
                if (isClicking)
                {
                    isClicking = false;
                    Shoot();
                    Debug.Log($"Hit Point: {rayHitPoint}");
                }
            }
        }
    }

    void FixedUpdate()
    {
        RayCasting();
        LineRedenring();
    }

    void HelperPositioning()
    {
        if (targetCell == null)
            return;

        SpriteRenderer positionHelperSpriteRendered = positionHelper.GetComponent<SpriteRenderer>();
        Color color = gameManager.GetBubbleColor(bubbleTypes[currBubbleType]);
        color.a = 0.2f;
        positionHelperSpriteRendered.color = color;
        positionHelper.SetActive(isClicking);
        positionHelper.transform.position = targetCell.transform.position;
    }

    void LineRedenring()
    {
        lineRenderer.startColor = gameManager.GetBubbleColor(bubbleTypes[currBubbleType]);

        float magnitude = (rayHitPoint - canonTip.position).magnitude;

        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, new Vector3(0, magnitude, 0));

        lineRenderer.enabled = isClicking;
    }

    void Shoot()
    {
        Vector3 dir = rayHitPoint - transform.position;

        GameObject projectileGO = Instantiate(bubbleProjectile, canonTip.position, Quaternion.identity) as GameObject;
        Bubble bubbleSC = projectileGO.GetComponent<Bubble>();

        bubbleSC.SetMyType(bubbleTypes[currBubbleType]);
        bubbleSC.Shoot(dir);

        //GameObject[] bubbles = GameObject.FindGameObjectsWithTag("bubble");
        //foreach (GameObject bubble in bubbles)
        //{
        //    bubble.SendMessage("RayCasting6Direction", SendMessageOptions.DontRequireReceiver);
        //}

        if (targetCell != null)
        {
            bubbleSC.SetTargetCell(targetCell);
        }
    }

    void ControlCanon()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector3 dir = mouseWorldPos - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - angleOffset;
        Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
        Quaternion smoothRot = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        smoothRot.z = Mathf.Clamp(smoothRot.z, -0.50f, 0.50f);
        transform.rotation = smoothRot;
    }

    void ToggleRotateCircle()
    {
        currCircleRotation -= 120f;
        doCircleRotation = true;

        if(currBubbleType < 2)
        {
            currBubbleType++;
        } else
        {
            currBubbleType = 0;
        }
    }
    void RotateCircle()
    {
        if (doCircleRotation)
        {
            Quaternion targetRot = Quaternion.Euler(0f, 0f, currCircleRotation);
            Quaternion smoothRot = Quaternion.Slerp(canonCircle.rotation, targetRot, 5f * Time.deltaTime);
            canonCircle.rotation = smoothRot;

            if(Mathf.Abs(currCircleRotation - canonCircle.rotation.z) < 2)
            {
                doCircleRotation = false;
            }
        }
    }

    void RayCasting()
    {
        raycastDirection = canonTip.up;

        RaycastHit2D hit = Physics2D.Raycast(canonTip.position, raycastDirection, raycastDistance, ~ignoreLayer);
        rayHitPoint = hit.point;



        Collider2D[] colliders = Physics2D.OverlapCircleAll(rayHitPoint, 1f);
        float minDist = Mathf.Infinity;
        foreach (Collider2D collider in colliders)
        {
            //Debug.Log("Collider found: " + collider.name);

            if(collider.gameObject.tag == "cell")
            {
                BubbleCell targetCellSC = collider.GetComponent<BubbleCell>();

                if(targetCellSC.isEmpty)
                {
                    float dist = Vector2.Distance(rayHitPoint, collider.transform.position);

                    if(dist < minDist)
                    {
                        minDist = dist;
                        targetCell = collider.gameObject;
                    }
                }
            }
        }
    }

    public void OnMouseDown()
    {
        ToggleRotateCircle();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(canonTip.position, canonTip.position + raycastDirection * 500);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rayHitPoint, 0.3f);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(rayHitPoint, 1f);

        if(targetCell != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(targetCell.transform.position, 0.2f);
        }
    }
}
