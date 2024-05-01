using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOver : MonoBehaviour
{
    MeshRenderer cubeColor;

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
        cubeColor.material.color = new Color(0.1f, 0.1f, 0.1f, 1f);
    }

    private IEnumerator Glittering()
    {
        float cnt = 0;
        while (cnt < 1) {
            cnt += 0.1f;
            cubeColor.material.color = new Color(0.1f, 0.1f, 0.1f, cnt);
            yield return new WaitForFixedUpdate();

        }
    }
}
