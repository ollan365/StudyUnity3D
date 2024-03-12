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
        Debug.Log("Mouse Enter");
        cubeColor.material.color = Color.blue;
    }
     
    void OnMouseExit()
    {
        Debug.Log("Mouse Exit");
        cubeColor.material.color = Color.black;
    }

    private void OnMouseUp()
    {
        Debug.Log("Mouse Clicked");
        cubeColor.material.color = Color.black;
    }

}
