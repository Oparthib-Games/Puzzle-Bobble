using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bubble : MonoBehaviour
{
    [SerializeField] private float shootingForce = 3f;

    Rigidbody2D RB2D;
    SpriteRenderer spriteRenderer;

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

    }

    public void SetMyType(BubbleTypes type)
    {
        myType = type;
    }

    public void Shoot(Vector3 dir)
    {
        RB2D.AddForce(dir * shootingForce, ForceMode2D.Impulse);
    }
}
