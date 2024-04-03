using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOver : MonoBehaviour
{
    MeshRenderer cubeColor;
    //true: black, false: origin color
    private bool isGlittering = true;


    private void Awake()
    {
        cubeColor = gameObject.GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(Glittering());
    }

    private void OnMouseOver()
    {
        cubeColor.material.color = Color.blue;
    }

    private void OnMouseExit()
    {
        if (isGlittering)
            cubeColor.material.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);
        else
            cubeColor.material.color = new Color(0.1f, 0.1f, 0.1f, 0);
    }

    private IEnumerator Glittering()
    {
        int cnt = 0;
        while (cnt < 100) {
            
            if (isGlittering)
            {
                cubeColor.material.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);
                isGlittering = false;
            }
            else
            {
                cubeColor.material.color = new Color(0.1f, 0.1f, 0.1f, 0);
                isGlittering = true;
            }
            yield return new WaitForSecondsRealtime(0.7f);
            cnt++;
        }
    }
}
