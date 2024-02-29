using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButton : MonoBehaviour
{
    public GameObject inventory;
    private bool state;

    private void Start()
    {
        state = false;
        inventory.SetActive(state);
    }

    public void InventoryONOFF()
    {
        if (state)
        {
            inventory.SetActive(false);
            state = false;
        }
        else
        {
            inventory.SetActive(true);
            state = true;   
        }
    }
}
