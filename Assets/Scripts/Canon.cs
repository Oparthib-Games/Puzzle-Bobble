using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Canon : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float angleOffset = 90f;

    public BubbleTypes[] bubbleTypes = { BubbleTypes.RED, BubbleTypes.YELLOW, BubbleTypes.BLUE };
    public int currBubbleType = 0;
    [HideInInspector] public BubbleTriangle bubbleTriangle;

    [SerializeField] private Transform canonTip;
    [SerializeField] private GameObject bubbleProjectile;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject targetCell;
    [SerializeField] private GameObject positionHelper;

    #region Ray Casting
    private Vector3 raycast1Direction;
    private Vector3 raycast2Direction;
    private float raycastDistance = Mathf.Infinity;
    private Vector3 rayHitPoint1;
    private Vector3 rayHitPoint2;
    private Vector2 hitEndPoint;
    public LayerMask ignoreLayer;
    private bool has2Ray;
    #endregion

    Vector3 mousePos;
    bool isClicking = false;
    
    

    GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>().gameObject.GetComponent<GameManager>();
    }

    void Update()
    {
        mousePos = Mouse.current.position.ReadValue();

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
                    Debug.Log($"Hit Point: {rayHitPoint1}");
                }
            }
        } else
        {
            isClicking = false;
        }
    }

    void FixedUpdate()
    {
        RayCasting();
        CellFindingRayCasting();
        LineRedenring();
        HelperPositioning();
    }

    void HelperPositioning()
    {
        if (targetCell == null)
            return;

        SpriteRenderer positionHelperSpriteRendered = positionHelper.GetComponent<SpriteRenderer>();
        Color color = gameManager.GetBubbleColor(bubbleTypes[currBubbleType]);
        color.a = 0.3f;
        positionHelperSpriteRendered.color = color;
        positionHelper.SetActive(isClicking);
        positionHelper.transform.position = targetCell.transform.position;
    }

    void LineRedenring()
    {
        lineRenderer.startColor = gameManager.GetBubbleColor(bubbleTypes[currBubbleType]);

        float magnitude = (rayHitPoint1 - canonTip.position).magnitude;

        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, new Vector3(0, magnitude, 0));

        lineRenderer.enabled = isClicking;
    }

    void Shoot()
    {
        Vector3 dir = rayHitPoint1 - transform.position;

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

        bubbleTriangle.Shuffle();
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

    
    

    void RayCasting()
    {
        raycast1Direction = canonTip.up;

        RaycastHit2D hit = Physics2D.Raycast(canonTip.position, raycast1Direction, raycastDistance, ~ignoreLayer);
        rayHitPoint1 = hit.point;

        //if (hit.collider && hit.collider.tag == "platform")
        //{
        //    has2Ray = true;

        //    var deflectRotation = Quaternion.FromToRotation(-raycast1Direction, hit.normal);
        //    var raycast2Direction = deflectRotation * hit.normal;

        //    RaycastHit2D hit2 = Physics2D.Raycast(rayHitPoint1, raycast2Direction, raycastDistance, ~ignoreLayer);
        //    rayHitPoint2 = hit2.point;
        //}
        //else
        //{
        //    has2Ray = false;
        //}

        hitEndPoint = has2Ray ? rayHitPoint2 : rayHitPoint1;
    }

    private void CellFindingRayCasting()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(hitEndPoint, 1f);
        float minDist = Mathf.Infinity;
        foreach (Collider2D collider in colliders)
        {
            //Debug.Log("Collider found: " + collider.name);

            if (collider.gameObject.tag == "cell")
            {
                BubbleCell targetCellSC = collider.GetComponent<BubbleCell>();

                if (targetCellSC.isEmpty)
                {
                    float dist = Vector2.Distance(hitEndPoint, collider.transform.position);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        targetCell = collider.gameObject;
                    }
                }
            }
        }
    }

    //public void OnMouseDown()
    //{
    //    ToggleRotateCircle();
    //}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(canonTip.position, canonTip.position + raycast1Direction * 500);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayHitPoint1, rayHitPoint1 + raycast2Direction * 500);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rayHitPoint1, 0.3f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rayHitPoint2, 0.3f);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(hitEndPoint, 1f);

        if(targetCell != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(targetCell.transform.position, 0.2f);
        }
    }
}
