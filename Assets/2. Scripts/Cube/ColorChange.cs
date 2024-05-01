using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    MeshRenderer meshRend;
    int stage;

    public Constants.Colors color;
    // Start is called before the first frame update
    void Start()
    {
        //cubeMaterialSet[stage][white]
        stage = 0;
        
        gameObject.GetComponent<MeshRenderer>().material = StaticManager.Instance.cubeMaterialSet[StaticManager.Instance.Stage % 6][color.ToInt()];
    }

    // Update is called once per frame
    void Update()
    {
        if (stage != StaticManager.Instance.Stage)
        {
            stage = StaticManager.Instance.Stage;

            gameObject.GetComponent<MeshRenderer>().material = StaticManager.Instance.cubeMaterialSet[StaticManager.Instance.Stage % 6][color.ToInt()];
        }
    }
}
