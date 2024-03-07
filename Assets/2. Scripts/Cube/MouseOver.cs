using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOver : MonoBehaviour
{
    Renderer cubeColor;

    private void Start()
    {
        cubeColor = gameObject.GetComponent<Renderer>();
    }

    private void OnMouseEnter()
    {
        cubeColor.material.color = Color.blue;
    }

    private void OnMouseExit()
    {
        cubeColor.material.color = Color.black;
    }

}
