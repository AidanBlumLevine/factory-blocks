using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameWonPopup : MonoBehaviour
{
    Animator anim;
    public GameObject coins;
    public Button next;
    public Sprite emptyCoin;
    string nextName; 
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Won(Level level)
    {
        next.enabled = level.permanent;
        if (level.permanent)
        {
            nextName = (int.Parse(level.name) + 1) + "";
        }
        coins.SetActive(level.permanent);
        Image[] coinImg = coins.GetComponentsInChildren<Image>();
        if (level.bestMoves > level.stars[0])
        {
            coinImg[0].sprite = emptyCoin;
        }
        if (level.bestMoves > level.stars[1])
        {
            coinImg[1].sprite = emptyCoin;
        }
        if (level.bestMoves > level.stars[2])
        {
            coinImg[2].sprite = emptyCoin;
        }
        anim.SetBool("Open", true);
    }

    public void Replay()
    {
        TileManager.Instance.ReloadLevel();
        anim.SetBool("Open", false);
    }

    public void Back()
    {
        //anim.SetBool("Open", false);
        GameManager.Instance.LoadScene(3);
    }

    public void Next()
    {
        GameManager.Instance.LoadScene(1, GameManager.Instance.levels.First(e => e.level.name.Equals(nextName)).level);
    }
}
