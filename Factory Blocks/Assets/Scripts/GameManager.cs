using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public Animator loadingAnim;

    int gameState;

    void Start()
    {
        loadingAnim.SetBool("Open", false);
    }

    public void LoadScene(int sceneIndex)
    {
        loadingAnim.SetBool("Open", true);
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        //yield return new WaitForSeconds(3);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            yield return null;
        }
        loadingAnim.SetBool("Open", false);
        if(sceneIndex == 1)
        {
            TileManager.Instance.LoadLevel(Application.persistentDataPath + "/" + "level.json");
        }
    }
}
