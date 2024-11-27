using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Scenes;
using Unity.Collections;
using System.Collections;
using System.Threading.Tasks;

public class GameFlow : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameClearPanel;
    [SerializeField] private SubScene subScene;
    private EntityQuery gameStatusQuery;
    private void OnEnable()
    {
        subScene.gameObject.SetActive(true);
    }
    void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world != null)
        {
            var entityManager = world.EntityManager;
            gameStatusQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<GameMgrComponent>());
        }
    }
    void Update()
    {
        if (gameStatusQuery == null) return;

        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;

        if (gameStatusQuery.TryGetSingleton(out GameMgrComponent gameStatus))
        {
            if (gameStatus.gameOver)
            {
                gameOverPanel.SetActive(true);
            }
            else if (gameStatus.gameClear)
            {
                gameClearPanel.SetActive(true);
            }
        }
    }
    public async void Restart()
    {
        subScene.gameObject.SetActive(false);
        await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
