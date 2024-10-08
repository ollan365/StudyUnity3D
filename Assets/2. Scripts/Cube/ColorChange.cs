using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ColorChange : MonoBehaviour
{
    MeshRenderer meshRend;
    public int stage;
    public int materialSize;

    public Constants.Colors color;
    // Start is called before the first frame update
    void Start()
    {
        //cubeMaterialSet[stage][white]
        stage = StaticManager.Instance.Stage;
        materialSize = StaticManager.Instance.cubeMaterialSet.Length;
        gameObject.GetComponent<MeshRenderer>().material = StaticManager.Instance.cubeMaterialSet[stage % materialSize][color.ToInt()];
    }  

    // Update is called once per frame
    void Update()
    {
        if (stage != StaticManager.Instance.Stage)
        {
            stage = StaticManager.Instance.Stage;
            gameObject.GetComponent<MeshRenderer>().material = StaticManager.Instance.cubeMaterialSet[stage % materialSize][color.ToInt()];
        }
    }
}
