using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    Image sr;
    bool volumeOn = true;

    public Sprite On, Off;

    void Start()
    {
        sr = GetComponent<Image>();
        if (PlayerPrefs.HasKey("volumeOn")){
            volumeOn = PlayerPrefs.GetInt("volumeOn") == 1;
        }

        if (!volumeOn)
        {
            sr.sprite = Off;
            GameManager.Instance.volume = 0;
        }
        else
        {
            sr.sprite = On;
            GameManager.Instance.volume = 1;
        }
    }

    public void Pressed()
    {
        if (volumeOn)
        {
            sr.sprite = Off;
            GameManager.Instance.volume = 0;
            volumeOn = false;
        }
        else
        {
            sr.sprite = On;
            GameManager.Instance.volume = 1;
            volumeOn = true;
        }
        PlayerPrefs.SetInt("volumeOn", volumeOn == false ? 0 : 1);
    }
}
