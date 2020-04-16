using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<LevelLocation> customLevels { get; private set; }
    public List<Level> permanentLevels { get; private set; }
    public List<Level> levels { get; private set; }
    Dictionary<Level, int> bestMoves = new Dictionary<Level, int>();

    public static GameManager Instance;
    public GameObject generalPopupPrefab;
    public LoadingOverlay loadingOverlay;
    Level tempState;
    BestMoves bestMovesLoadedState;
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

        levels = new List<Level>();
        //Load Custom Levels
        customLevels = new List<LevelLocation>();
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
            customLevels.Add(new LevelLocation {
                level = JsonUtility.FromJson<Level>(contents),
                path = f.FullName
            });
            levels.Add(customLevels[customLevels.Count - 1].level);
        }

        //Permanent levels
        permanentLevels = new List<Level>();
        TextAsset[] t = Resources.LoadAll<TextAsset>("Levels");
        foreach (TextAsset a in t)
        {
            permanentLevels.Add(JsonUtility.FromJson<Level>(a.text));
        }
        levels.AddRange(permanentLevels);



        //Best Moves
        if (!File.Exists(Application.persistentDataPath + "/progress.json"))
        {
            BestMoves toAdd = new BestMoves();
            string contents = JsonUtility.ToJson(toAdd, true);
            File.WriteAllText(Application.persistentDataPath + "/progress.json", contents);
        }
        //Load Best Moves
        string bestMovesText = File.ReadAllText(Application.persistentDataPath + "/progress.json");
        bestMovesLoadedState = JsonUtility.FromJson<BestMoves>(bestMovesText);
        foreach (Level l in levels)
        {
            int moves = 999999;
            foreach(BestMoves.LevelNum lm in bestMovesLoadedState.levels)
            {
                if(lm.permanent == l.permanent && lm.name.Equals(l.name))
                {
                    moves = lm.bestMoves;
                }
            }
            bestMoves.Add(l, moves);
        }
    }

    public bool LevelNameTaken(string n)
    {
        foreach(LevelLocation l in customLevels)
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
        customLevels.Add(new LevelLocation {
            level = levelMap,
            path = path
        });
        bestMoves.Add(levelMap, 999999);
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
        LevelLocation toRemove = customLevels.First(element => element.level.name == level.name);
        customLevels.Remove(toRemove);
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
        DeleteBestMoves(level);
    }

    public void UpdateBestMoves(int moves, Level l)
    {
        bestMoves[l] = moves;
        bool exists = false;
        for(int i = 0; i < bestMovesLoadedState.levels.Count(); i++) 
        { 
            BestMoves.LevelNum lm = bestMovesLoadedState.levels[i];
            if (lm.permanent == l.permanent && lm.name.Equals(l.name))
            {
                exists = true;
                bestMovesLoadedState.levels[i].bestMoves = moves;
            }
        }
        if (!exists)
        {
            List<BestMoves.LevelNum> temp = new List<BestMoves.LevelNum>(bestMovesLoadedState.levels);
            temp.Add(new BestMoves.LevelNum
            {
                bestMoves = moves,
                permanent = l.permanent,
                name = l.name
            });
            bestMovesLoadedState.levels = temp.ToArray();
        }
        File.Delete(Application.persistentDataPath + "/progress.json");
        string contents = JsonUtility.ToJson(bestMovesLoadedState, true);
        File.WriteAllText(Application.persistentDataPath + "/progress.json", contents);
    }

    void DeleteBestMoves(Level l)
    {
        List<BestMoves.LevelNum> temp = new List<BestMoves.LevelNum>(bestMovesLoadedState.levels);
        BestMoves.LevelNum toDelete = default(BestMoves.LevelNum);
        foreach (BestMoves.LevelNum lm in bestMovesLoadedState.levels)
        {
            if (lm.permanent == l.permanent && lm.name.Equals(l.name))
            {
                toDelete = lm;
            }
        }
        temp.Remove(toDelete);
        bestMoves.Remove(l);
        bestMovesLoadedState.levels = temp.ToArray();
        File.Delete(Application.persistentDataPath + "/progress.json");
        string contents = JsonUtility.ToJson(bestMovesLoadedState, true);
        File.WriteAllText(Application.persistentDataPath + "/progress.json", contents);
    }

    public int BestMoves(Level l)
    {
        return bestMoves[l];
    }
}
