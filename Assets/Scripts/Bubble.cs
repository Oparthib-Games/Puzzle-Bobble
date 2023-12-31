using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BubbleState { STATIC, MOVING };

public class Bubble : MonoBehaviour
{
    [SerializeField] private float shootingForce = 3f;
    [SerializeField] private BubbleTypes myType = BubbleTypes.RED;
    [SerializeField] private BubbleState myState = BubbleState.MOVING;
    [SerializeField] private LayerMask bubbleLayer;
    [SerializeField] private float rayDistance = 0.3f;
    [SerializeField] private Vector3[] sixDirection;
    [SerializeField] private GameObject[] connectedBubbles;
    [SerializeField] private GameObject[] sameColorConnectedBubbles;
    [SerializeField] private float sameColorConnected;
    [SerializeField] private GameObject explosion;

    Rigidbody2D RB2D;
    SpriteRenderer spriteRenderer;

    public GameObject targetCell;

    GameManager gameManager;

    bool isRaycasting;

    void Awake()
    {
        RB2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        FindMyTargetCell();
        SetSixDirections();

        gameManager = FindObjectOfType<GameManager>().gameObject.GetComponent<GameManager>();

        spriteRenderer.color = gameManager.GetBubbleColor(myType);
        explosion.GetComponent<SpriteRenderer>().color = gameManager.GetBubbleColor(myType);

        //StartCoroutine(IERayCasting6Direction());
    }

    void FindMyTargetCell()
    {
        if(myState == BubbleState.STATIC)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
            float minDist = Mathf.Infinity;
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.tag == "cell")
                {
                    BubbleCell targetCellSC = collider.GetComponent<BubbleCell>();

                    if (targetCellSC.isEmpty)
                    {
                        float dist = Vector2.Distance(transform.position, collider.transform.position);

                        if (dist < minDist)
                        {
                            minDist = dist;
                            targetCell = collider.gameObject;
                        }
                    }
                }
            }

            targetCell.GetComponent<BubbleCell>().isEmpty = false;
            //targetCell.GetComponent<SpriteRenderer>().color = Color.cyan;
        }
    }

    private void SetSixDirections()
    {
        sixDirection = new Vector3[6];
        sixDirection[0] = Vector2.left;
        sixDirection[1] = Vector2.right;
        sixDirection[2] = new Vector2(-1, 1); // Up-left
        sixDirection[3] = new Vector2(1, 1);  // Up-right
        sixDirection[4] = new Vector2(1, -1); // Down-right
        sixDirection[5] = new Vector2(-1, -1);// Down-left
    }

    void Update()
    {
        if(targetCell != null && myState == BubbleState.MOVING)
        {
            transform.position = Vector3.Lerp(transform.position, targetCell.transform.position, 10f * Time.deltaTime);

            float distance = Vector3.Distance(transform.position, targetCell.transform.position);
            if(distance <= 0.05f)
            {
                myState = BubbleState.STATIC;
                StartCoroutine("SendMessageToAllBubble");
                //targetCell.GetComponent<SpriteRenderer>().color = Color.cyan;
            }
        }
    }

    IEnumerator SendMessageToAllBubble()
    {
        yield return new WaitForSeconds(0.2f);
        GameObject[] bubbles = GameObject.FindGameObjectsWithTag("bubble");
        foreach (GameObject bubble in bubbles)
        {
            bubble.SendMessage("RayCasting6Direction", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //RB2D.velocity = Vector2.zero;
        //RB2D.angularVelocity = 0f;
        RB2D.bodyType = RigidbodyType2D.Static;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    public void SetMyType(BubbleTypes type)
    {
        myType = type;
    }

    public void SetTargetCell(GameObject cell)
    {
        targetCell = cell;

        targetCell.GetComponent<BubbleCell>().isEmpty = false;
    }

    public void Shoot(Vector3 dir)
    {
        //RB2D.AddForce(dir * shootingForce, ForceMode2D.Impulse);
    }

    IEnumerator IERayCasting6Direction()
    {
        yield return new WaitForSeconds(3f);
        RayCasting6Direction();
        StartCoroutine(IERayCasting6Direction());
    }

    public void RayCasting6Direction()
    {
        Debug.Log("Raycasting on 6 directions");

        connectedBubbles = new GameObject[0];
        sameColorConnectedBubbles = new GameObject[0];
        isRaycasting = true;
        sameColorConnected = 0;


        PerformRaycast(sixDirection[0]);
        PerformRaycast(sixDirection[1]);
        PerformRaycast(sixDirection[2]);
        PerformRaycast(sixDirection[3]);
        PerformRaycast(sixDirection[4]);
        PerformRaycast(sixDirection[5]);

        isRaycasting = false;
    }



    void PerformRaycast(Vector2 direction)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, rayDistance, bubbleLayer);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.transform.gameObject != this.gameObject)
            {
                //Debug.Log("Hit object: " + hit.collider.gameObject.name);
                //Debug.Log("Hit point: " + hit.point);
                //Debug.Log("Hit normal: " + hit.normal);

                System.Array.Resize(ref connectedBubbles, connectedBubbles.Length + 1);
                connectedBubbles[connectedBubbles.Length - 1] = hit.collider.gameObject;

                Bubble bubbleSC = hit.collider.GetComponent<Bubble>();
                if (bubbleSC.myType == this.myType)
                {
                    sameColorConnected += 1;

                    System.Array.Resize(ref sameColorConnectedBubbles, sameColorConnectedBubbles.Length + 1);
                    sameColorConnectedBubbles[sameColorConnectedBubbles.Length - 1] = hit.collider.gameObject;
                }
            }
        }

        if(sameColorConnected >= 2)
            CheckSameColorConnected();
    }

    public void CheckSameColorConnected()
    {
        Debug.Log("Boom for: " + this.gameObject.name);

        foreach (GameObject bubble in sameColorConnectedBubbles)
        {
            Bubble bubbleSC = bubble.GetComponent<Bubble>();

            bubbleSC.Boom();
        }
        Boom();
    }

    public void Boom()
    {
        if(targetCell != null)
        {
            targetCell.GetComponent<BubbleCell>().isEmpty = true;
        }
        gameManager.PlayPopSound();
        Instantiate(explosion, transform.position, Quaternion.identity);
        gameManager.CheckAliveBubbles();
        Destroy(this.gameObject, 0.1f);
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            Gizmos.color = gameManager.GetBubbleColor(myType);
            Gizmos.DrawLine(transform.position, transform.position + sixDirection[0] * rayDistance);
            Gizmos.DrawLine(transform.position, transform.position + sixDirection[1] * rayDistance);
            Gizmos.DrawLine(transform.position, transform.position + sixDirection[2] * rayDistance);
            Gizmos.DrawLine(transform.position, transform.position + sixDirection[3] * rayDistance);
            Gizmos.DrawLine(transform.position, transform.position + sixDirection[4] * rayDistance);
            Gizmos.DrawLine(transform.position, transform.position + sixDirection[5] * rayDistance);
        }
    }
}
