using UnityEngine.SceneManagement;
using UnityEngine;

public class LobbyLogic : MonoBehaviour
{
    public void OnClickGameStartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
