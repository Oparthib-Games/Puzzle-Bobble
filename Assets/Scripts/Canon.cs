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

    #region Ray Casting
    private Vector3 raycastDirection;
    private float raycastDistance = Mathf.Infinity;
    private Vector3 rayHitPoint;
    public LayerMask layerMask;
    #endregion

    Vector3 mousePos;
    bool isClicking = false;
    bool doCircleRotation = false;
    float currCircleRotation = 0;


    void Start()
    {
        
    }

    void Update()
    {
        mousePos = Mouse.current.position.ReadValue();

        RotateCircle();

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

    void LineRedenring()
    {
        lineRenderer.startColor = GameManager.GetBubbleColor(bubbleTypes[currBubbleType]);

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

        RaycastHit2D hit = Physics2D.Raycast(canonTip.position, raycastDirection, raycastDistance);
        //Debug.Log("Hit point: " + hit.point);
        rayHitPoint = hit.point;
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
    }
}
