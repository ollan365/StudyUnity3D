using UnityEngine;

public class Touch : MonoBehaviour
{
    [SerializeField] private CubeManager.TouchPlaneColor touchPlaneColor;
    [SerializeField] private CubeManager CM;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray가 3D plane과 교차하는지 확인
            if (Physics.Raycast(ray, out hit))
            {
                // 여기서 hit.collider.gameObject는 클릭된 객체를 나타냅니다.
                // 만약 클릭된 객체가 3D plane이라면, Debug.log("click!"); 호출
                if (hit.collider.gameObject == gameObject)
                {
                    CM.Turn(touchPlaneColor, -1);
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Ray가 3D plane과 교차하는지 확인
            if (Physics.Raycast(ray, out hit))
            {
                // 여기서 hit.collider.gameObject는 클릭된 객체를 나타냅니다.
                // 만약 클릭된 객체가 3D plane이라면, Debug.log("click!"); 호출
                if (hit.collider.gameObject == gameObject)
                {
                    CM.Turn(touchPlaneColor, 1);
                }
            }
        }
    }
}
