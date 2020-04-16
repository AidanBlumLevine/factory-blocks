using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameWonPopup : MonoBehaviour
{
    public GameObject coins;
    public Button next;
    public Sprite emptyCoin;
    Sprite coin;
    string nextName;
    float width, height;
    Vector2 center;
    public float easeDuration = .5f;

    void Start()
    {
        width = Screen.width;
        height = Screen.height;
        center = new Vector2(width/2.0f,height/2.0f);
        transform.position = new Vector2(center.x, center.y + height);
        coin = coins.GetComponentInChildren<Image>().sprite;
    }

    public void Won(Level level, int moves)
    {
        next.gameObject.SetActive(level.permanent);
        if (level.permanent)
        {
            nextName = (int.Parse(level.name) + 1) + "";
        }
        coins.SetActive(level.permanent);
        Image[] coinImg = coins.GetComponentsInChildren<Image>();
        if (moves > level.stars[0])
        {
            coinImg[0].sprite = emptyCoin;
        }else{
            coinImg[0].sprite = coin;
        }
        if (moves > level.stars[1])
        {
            coinImg[1].sprite = emptyCoin;
        }else{
            coinImg[1].sprite = coin;
        }
        if (moves > level.stars[2])
        {
            coinImg[2].sprite = emptyCoin;
        }else{
            coinImg[2].sprite = coin;
        }
        Show(gameObject, new Vector2(0, -1));
    }

    public void Replay()
    {
        TileManager.Instance.ReloadLevel();
        Hide(gameObject, new Vector2(0, 1));
    }

    public void Back()
    {
        //anim.SetBool("Open", false);
        GameManager.Instance.LoadScene(3);
    }

    public void Next()
    {
        GameManager.Instance.LoadScene(1, GameManager.Instance.permanentLevels.First(e => e.name.Equals(nextName)));
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
