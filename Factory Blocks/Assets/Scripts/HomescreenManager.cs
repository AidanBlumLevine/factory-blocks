﻿using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HomescreenManager : MonoBehaviour
{
    public static HomescreenManager Instance;
    public GameObject continueButton;
    public Text continueName;
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

    void Start()
    {
        gm = GameManager.Instance;
        continueButton.SetActive(gm.CanContinue());
        continueName.text = gm.ContinueName();
    }

    public void Play()
    {
        gm.LoadScene(3);
    }

    public void Continue()
    {
        gm.Continue();
    }

    public void Edit()
    {
        gm.LoadScene(2);
    }
}
