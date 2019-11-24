using System.IO;
using UnityEngine;

public class HomescreenManager : MonoBehaviour
{
    public static HomescreenManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    GameManager gm;
    public GameObject levelList,levelPlayPrefab;

    void Start()
    {
        gm = GameManager.Instance;
        
        foreach (Level l in gm.levels)
        {
            Instantiate(levelPlayPrefab, levelList.transform).GetComponent<LevelPreview>().Set(l);
        }
    }

    public void PlayLevel(Level l)
    {
        gm.LoadScene(1, l);
    }

    public void Edit()
    {
        gm.LoadScene(2);
    }
}
