using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectorManager : MonoBehaviour
{
    public static LevelSelectorManager Instance;
    public static bool custom = false;
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

    public GameObject levelListCustom, levelList, levelPlayPrefab;
    public Animator customSwitch;
    GameManager gm;
    float width, height;
    Vector2 center;
    public float easeDuration = .5f;
    bool moving;

    void Start()
    {
        gm = GameManager.Instance;
        width = Screen.width;
        height = Screen.height;
        if (custom)
        {
            center = levelListCustom.transform.parent.parent.position;
            levelList.transform.parent.parent.position = new Vector2(center.x - width, center.y);
        }
        else
        {
            center = levelList.transform.parent.parent.position;
            levelListCustom.transform.parent.parent.position = new Vector2(center.x + width, center.y);
        }
        customSwitch.SetBool("customShown", custom);

        foreach (GameManager.LevelLocation l in gm.levels)
        {
            if (l.level.permanent)
            {
                Instantiate(levelPlayPrefab, levelList.transform).GetComponent<LevelPreview>().Set(l.level);
            }
            else
            {
                Instantiate(levelPlayPrefab, levelListCustom.transform).GetComponent<LevelPreview>().Set(l.level);
            }
        }
    }

    public void Back()
    {
        gm.LoadScene(0);
    }

    public void TypeFlip()
    {
        if (!moving)
        {
            custom = !custom;
            if (custom)
            {
                Show(levelListCustom.transform.parent.parent.gameObject, new Vector2(1, 0));
                Hide(levelList.transform.parent.parent.gameObject, new Vector2(1, 0));
            }
            else
            {
                Show(levelList.transform.parent.parent.gameObject, new Vector2(-1, 0));
                Hide(levelListCustom.transform.parent.parent.gameObject, new Vector2(-1, 0));
            }
            customSwitch.SetBool("customShown", custom);
        }
    }

    //EASE STUFF
    public void Show(GameObject g, Vector2 dir)
    {
        Vector2 startPos = center - new Vector2(dir.x * width, dir.y * height);
        StartCoroutine(EaseTo(g, startPos, center));
    }

    public void Hide(GameObject g, Vector2 dir)
    {
        Vector2 endPos = center + new Vector2(dir.x * width, dir.y * height);
        StartCoroutine(EaseTo(g, center, endPos));
    }

    IEnumerator EaseTo(GameObject g, Vector2 start, Vector2 end)
    {
        float time = 0;
        while (time < easeDuration)
        {
            time += Time.deltaTime;
            g.transform.position = Ease(start, end, time, easeDuration);
            yield return null;
        }
        g.transform.position = end;
        moving = false;
    }

    Vector2 Ease(Vector2 startPos, Vector2 endPos, float time, float duration)
    {
        Vector2 c = endPos - startPos;
        time /= duration / 2;
        if (time < 1) return c / 2 * time * time * time + startPos;
        time -= 2;
        return c / 2 * (time * time * time + 2) + startPos;
    }
}
