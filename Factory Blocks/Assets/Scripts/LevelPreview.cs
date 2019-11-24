using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPreview : MonoBehaviour
{
    Level level;
    public void Set(Level l)
    {
        level = l;
        GetComponentInChildren<Text>().text = level.name.Replace(".json","");
    }

    public void PlayLevel()
    {
        HomescreenManager.Instance.PlayLevel(level);
    }
}
