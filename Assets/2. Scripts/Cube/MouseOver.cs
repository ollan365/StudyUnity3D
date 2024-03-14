using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOver : MonoBehaviour
{
    MeshRenderer cubeColor;
    

    private void Start()
    {
        cubeColor = gameObject.GetComponent<MeshRenderer>();
    }

    void OnMouseOver()
    {
        cubeColor.material.color = Color.blue;
    }
     
    void OnMouseExit()
    {
        cubeColor.material.color = Color.black;
    }

    private void OnMouseUp()
    {
        cubeColor.material.color = Color.black;
    }

}
