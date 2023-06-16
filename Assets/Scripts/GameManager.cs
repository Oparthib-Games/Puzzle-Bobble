using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BubbleTypes { RED, YELLOW, BLUE, GREEN, BLACK, PINK, SKY, VIOLET };

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioClip popSound;

    bool isPlayingAudio;

    [SerializeField] private Color red;
    [SerializeField] private Color yellow;
    [SerializeField] private Color blue;
    [SerializeField] private Color green;
    [SerializeField] private Color black;
    [SerializeField] private Color pink;
    [SerializeField] private Color sky;
    [SerializeField] private Color violet;

    [SerializeField] private Animator doorAnim;

    // Start is called before the first frame update
    void Start()
    {
        doorAnim.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color GetBubbleColor(BubbleTypes type)
    {
        Color color = Color.white;

        switch (type)
        {
            case BubbleTypes.RED:
                color = red;
                break;
            case BubbleTypes.YELLOW:
                color = yellow;
                break;
            case BubbleTypes.BLUE:
                color = blue;
                break;
            case BubbleTypes.GREEN:
                color = green;
                break;
            case BubbleTypes.BLACK:
                color = black;
                break;
            case BubbleTypes.PINK:
                color = pink;
                break;
            case BubbleTypes.SKY:
                color = sky;
                break;
            case BubbleTypes.VIOLET:
                color = black;
                break;
            default:
                color = Color.magenta;
                break;
        }

        return color;
    }

    public void PlayPopSound()
    {
        StartCoroutine(IEPlayPopSound());
    }

    public void CheckAliveBubbles()
    {
        StartCoroutine(IECheckAliveBubbles());
    }

    IEnumerator IECheckAliveBubbles()
    {
        yield return new WaitForSeconds(1f);

        Bubble[] aliveBubbles = GameObject.FindObjectsOfType<Bubble>();

        Debug.Log("Alive Bubbles: " + aliveBubbles.Length);
        if (aliveBubbles.Length <= 0)
        {
            Debug.Log("Game Over");
            doorAnim.SetTrigger("Close");
        }
    }

    IEnumerator IEPlayPopSound()
    {
        if (!isPlayingAudio)
        {
            isPlayingAudio = true;
            AudioSource.PlayClipAtPoint(popSound, Camera.main.transform.position);

        }
        yield return new WaitForSeconds(popSound.length);
        isPlayingAudio = false;
    }
}
