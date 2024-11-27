using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneMgr : MonoBehaviour
{
    private async void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        await Task.Delay(8000);
        SceneManager.LoadScene(1);
    }
}
