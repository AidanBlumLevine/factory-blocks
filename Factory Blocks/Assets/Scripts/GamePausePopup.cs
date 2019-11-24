using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePausePopup : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Pause()
    {
        if (anim.GetBool("Open"))
        {
            UnPause();
        }
        else
        {
            anim.SetBool("Open", true);
            TileManager.Instance.playing = false;
        }
    }

    public void UnPause()
    {
        anim.SetBool("Open", false);
        TileManager.Instance.playing = true;
    }

    public void Reload()
    {
        TileManager.Instance.ReloadLevel();
        anim.SetBool("Open", false);
    }

    public void Home()
    {
        anim.SetBool("Open", false);
        GameManager.Instance.LoadScene(0);
    }

    public void Edit()
    {
        anim.SetBool("Open", false);
        GameManager.Instance.LoadScene(2, TileManager.Instance.loadedLevel);
    }
}
