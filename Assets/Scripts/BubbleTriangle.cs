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

    private void Start()
    {
        canonSC = canon.GetComponent<Canon>();
    }

    private void Update()
    {
        RotateCircle();
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
