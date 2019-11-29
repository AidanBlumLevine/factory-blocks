using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<LevelLocation> levels { get; private set; }
    public static GameManager Instance;
    public GameObject generalPopupPrefab;
    public LoadingOverlay loadingOverlay;
    Level tempState;
    public struct LevelLocation
    {
        public Level level;
        public string path;
    }
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
        levels = new List<LevelLocation>();
        if (!Directory.Exists(Application.persistentDataPath + "/levels/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/levels/");
            Directory.CreateDirectory(Application.persistentDataPath + "/thumbnails/");
        }
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/levels/");
        FileInfo[] info = dir.GetFiles("*.json");
        foreach (FileInfo f in info)
        {
            string contents = File.ReadAllText(Application.persistentDataPath + "/levels/" + f.Name);
            levels.Add(new LevelLocation {
                level = JsonUtility.FromJson<Level>(contents),
                path = f.FullName
            });
        }
    }

    public bool LevelNameTaken(string n)
    {
        foreach(LevelLocation l in levels)
        {
            if (l.level.name.Equals(n))
            {
                return true;
            }
        }
        return false;
    }

    public void LoadScene(int sceneIndex, Level l = null)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex,l));
    }

    IEnumerator LoadAsynchronously (int sceneIndex, Level level)
    {
        Vector2 dir = TransitionDir(SceneManager.GetActiveScene().buildIndex, sceneIndex);
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

    Vector2 TransitionDir(int from, int to)
    {
        float f = (from == 3) ? .5f : from;
        float t = (to == 3) ? .5f : to;
        return (t > f) ? Vector2.left : Vector2.right;
    }

    public void SaveLevel(Level levelMap)
    {
        string contents = JsonUtility.ToJson(levelMap, true);
        string path = Application.persistentDataPath + "/levels/" + string.Format(@"{0}.json", Guid.NewGuid());
        if (LevelNameTaken(levelMap.name))
        {
            DeleteLevel(levelMap);
        }
        File.WriteAllText(path, contents);
        levels.Add(new LevelLocation {
            level = levelMap,
            path = path
        });
    }

    public void SetTempState(Level levelMap)
    {
        tempState = levelMap;
    }

    public void SaveTempState(bool clear = false)
    {
        string path = Application.persistentDataPath + "/tempState.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        if (!clear)
        {
            string contents = JsonUtility.ToJson(tempState, true);
            File.WriteAllText(path, contents);
        }
    }

    public void Continue()
    {
        LoadScene(1,TempState());
    }

    Level TempState()
    {
        string path = Application.persistentDataPath + "/tempState.json";
        if (File.Exists(path))
        {
            string contents = File.ReadAllText(path);
            return JsonUtility.FromJson<Level>(contents);
        }
        return null;
    }

    public bool CanContinue()
    {
        Level t = TempState();
        if (t == null) { return false; }

        return LevelNameTaken(t.realLevelName);
    }

    public void DeleteLevel(Level level)
    {
        LevelLocation toRemove = levels.First(element => element.level.name == level.name);
        levels.Remove(toRemove);
        if (File.Exists(toRemove.path))
        {
            File.Delete(toRemove.path);
        }
        if (File.Exists(Application.persistentDataPath + "/thumbnails/" + toRemove.level.name + ".png"))
        {
            File.Delete(Application.persistentDataPath + "/thumbnails/" + toRemove.level.name + ".png");
        }
        if (TempState() != null && TempState().realLevelName.Equals(toRemove.level.name))
        {
            File.Delete(Application.persistentDataPath + "/tempState.json");
        }
    }

    public void UpdateBestMoves(int moves, String levelName)
    {
        LevelLocation editedLevelInfo = levels.First(element => element.level.name == levelName);
        editedLevelInfo.level.bestMoves = moves;
        File.Delete(editedLevelInfo.path);
        File.WriteAllText(editedLevelInfo.path, JsonUtility.ToJson(editedLevelInfo.level, true));
    }
}
