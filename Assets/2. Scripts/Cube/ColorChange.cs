using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    MeshRenderer meshRend;
    public int stage;

    public Constants.Colors color;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(StaticManager.Instance);
        
        //cubeMaterialSet[stage][white]
        stage = StaticManager.Instance.Stage;
        
    //    gameObject.GetComponent<MeshRenderer>().material = StaticManager.Instance.cubeMaterialSet[StaticManager.Instance.Stage % 6][color.ToInt()];
    }

    // Update is called once per frame
    void Update()
    {
        
        if (stage != StaticManager.Instance.Stage)
        {
            stage = StaticManager.Instance.Stage;
            Debug.Log(StaticManager.Instance.cubeMaterialSet[1][color.ToInt()]);
            gameObject.GetComponent<MeshRenderer>().material = StaticManager.Instance.cubeMaterialSet[StaticManager.Instance.Stage % 6][color.ToInt()];
        }
    }
}
