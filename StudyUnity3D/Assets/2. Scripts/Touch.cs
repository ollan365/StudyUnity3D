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

            // Ray�� 3D plane�� �����ϴ��� Ȯ��
            if (Physics.Raycast(ray, out hit))
            {
                // ���⼭ hit.collider.gameObject�� Ŭ���� ��ü�� ��Ÿ���ϴ�.
                // ���� Ŭ���� ��ü�� 3D plane�̶��, Debug.log("click!"); ȣ��
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

            // Ray�� 3D plane�� �����ϴ��� Ȯ��
            if (Physics.Raycast(ray, out hit))
            {
                // ���⼭ hit.collider.gameObject�� Ŭ���� ��ü�� ��Ÿ���ϴ�.
                // ���� Ŭ���� ��ü�� 3D plane�̶��, Debug.log("click!"); ȣ��
                if (hit.collider.gameObject == gameObject)
                {
                    CM.Turn(touchPlaneColor, 1);
                }
            }
        }
    }
}
