using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleTriangle : MonoBehaviour
{
    bool doCircleRotation = false;
    float currCircleRotation = 0;

    [SerializeField] private GameObject canon;
    [SerializeField] private Transform canonCircle;

    Canon canonSC;

    public SpriteRenderer[] colorsOnTriangle;

    GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>().gameObject.GetComponent<GameManager>();
        canonSC = canon.GetComponent<Canon>();

        canonSC.bubbleTriangle = this;
    }

    private void Update()
    {
        RotateCircle();
    }

    public void Shuffle()
    {
        List<BubbleTypes> selectedColors = GetRandomColors(3);

        canonSC.bubbleTypes = new BubbleTypes[0];

        int i = 0;
        foreach(BubbleTypes bubbleType in selectedColors)
        {
            System.Array.Resize(ref canonSC.bubbleTypes, canonSC.bubbleTypes.Length + 1);
            canonSC.bubbleTypes[canonSC.bubbleTypes.Length - 1] = bubbleType;

            colorsOnTriangle[i].color = gameManager.GetBubbleColor(bubbleType);
            i++;
        }
    }

    List<BubbleTypes> GetRandomColors(int count)
    {
        if (count > System.Enum.GetValues(typeof(BubbleTypes)).Length)
        {
            Debug.LogError("Cannot select more colors than the total available.");
            return null;
        }

        List<BubbleTypes> availableColors = new List<BubbleTypes>((BubbleTypes[])System.Enum.GetValues(typeof(BubbleTypes)));
        List<BubbleTypes> selectedColors = new List<BubbleTypes>();

        System.Random random = new System.Random();

        for (int i = 0; i < count; i++)
        {
            int randomIndex = random.Next(availableColors.Count);
            BubbleTypes selectedColor = availableColors[randomIndex];
            selectedColors.Add(selectedColor);
            availableColors.Remove(selectedColor);
        }

        return selectedColors;
    }

    void RotateCircle()
    {
        if (doCircleRotation)
        {
            Quaternion targetRot = Quaternion.Euler(0f, 0f, currCircleRotation);
            Quaternion smoothRot = Quaternion.Slerp(canonCircle.rotation, targetRot, 5f * Time.deltaTime);
            canonCircle.rotation = smoothRot;

            if (Mathf.Abs(currCircleRotation - canonCircle.rotation.z) < 2)
            {
                doCircleRotation = false;
            }
        }
    }

    void ToggleRotateCircle()
    {
        currCircleRotation -= 120f;
        doCircleRotation = true;

        if (canonSC.currBubbleType < 2)
        {
            canonSC.currBubbleType++;
        }
        else
        {
            canonSC.currBubbleType = 0;
        }
    }

    public void OnMouseDown()
    {
        ToggleRotateCircle();
    }
}
