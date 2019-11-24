using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWonPopup : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!anim.GetBool("Open") && TileManager.Instance.won)
        {
            anim.SetBool("Open", true);
        }
    }

    public void Replay()
    {
        TileManager.Instance.ReloadLevel();
        anim.SetBool("Open", false);
    }

    public void Home()
    {
        anim.SetBool("Open", false);
        GameManager.Instance.LoadScene(0);
    }
}
