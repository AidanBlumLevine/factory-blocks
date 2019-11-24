using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    float heavyProcessTally;
    public float totalHeavyProcess = 1.25f;
    public List<Level> levels { get; private set; }
    public static GameManager Instance;
    public GameObject generalPopupPrefab;
    public LoadingOverlay loadingOverlay;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        levels = new List<Level>();
        if (!Directory.Exists(Application.persistentDataPath + "/levels/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/levels/");
        }
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/levels/");
        FileInfo[] info = dir.GetFiles("*.json");
        foreach (FileInfo f in info)
        {
            string contents = File.ReadAllText(Application.persistentDataPath + "/levels/" + f.Name);
            levels.Add(JsonUtility.FromJson<Level>(contents));
        }
    }

    public bool LevelNameTaken(string n)
    {
        foreach(Level l in levels)
        {
            if (l.name.Equals(n))
            {
                return true;
            }
        }
        return false;
    }

    void LateUpdate()
    {
        heavyProcessTally = 0;
    }

    public bool ProcessHeavy(float cost)
    {
        if (heavyProcessTally + cost <= totalHeavyProcess)
        {
            heavyProcessTally += cost;
            return true;
        }
        return false;
    }

    public void LoadScene(int sceneIndex, Level l = null)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex,l));
    }

    IEnumerator LoadAsynchronously (int sceneIndex, Level level)
    {
        Vector2 dir = sceneIndex > SceneManager.GetActiveScene().buildIndex ? Vector2.left : Vector2.right;
        loadingOverlay.Show(dir);
        while (loadingOverlay.Moving)
        {
            yield return null;
        }

        float startTime = Time.time;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)// || Time.time - startTime < 1f (force load)
        {
            yield return null;
        }

        if (level != null)
        {
            if (sceneIndex == 1)
            {
                TileManager.Instance.LoadLevel(level);
            }
            if (sceneIndex == 2)
            {
                EditorManager.Instance.LoadLevel(level);
            }
        }
        loadingOverlay.Hide(dir);
    }

    public void SaveLevel(Level levelMap)
    {
        string contents = JsonUtility.ToJson(levelMap, true);
        string path = Application.persistentDataPath + "/levels/" + string.Format(@"{0}.json", Guid.NewGuid());
        if (LevelNameTaken(levelMap.name))
        {
            DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/levels/");
            FileInfo[] info = dir.GetFiles("*.json");
            foreach (FileInfo f in info)
            {
                string lc = File.ReadAllText(Application.persistentDataPath + "/levels/" + f.Name);
                if (JsonUtility.FromJson<Level>(contents).name.Equals(levelMap.name))
                {
                    path = f.FullName;
                    Level removeLevel = null;
                    foreach(Level l in levels)
                    {
                        if(l.name == levelMap.name)
                        {
                            removeLevel = l;
                        }
                    }
                    levels.Remove(removeLevel);
                    File.Delete(path);
                    break;
                }
            }
        }
        File.WriteAllText(path, contents);
        levels.Add(levelMap);
    }
}
