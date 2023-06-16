using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bubble : MonoBehaviour
{
    [SerializeField] private float shootingForce = 3f;

    Rigidbody2D RB2D;
    SpriteRenderer spriteRenderer;

    public GameObject targetCell;

    [SerializeField] private BubbleTypes myType = BubbleTypes.RED;

    void Awake()
    {
        RB2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        spriteRenderer.color = GameManager.GetBubbleColor(myType);
    }

    void Update()
    {
        if(targetCell != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetCell.transform.position, 10f * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RB2D.velocity = Vector2.zero;
        RB2D.angularVelocity = 0f;
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
}
