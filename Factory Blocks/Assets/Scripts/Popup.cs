using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public Text title, body, button1text, button2text;
    public delegate void TestDelegate();
    TestDelegate confirm;
    Animator anim;
    static Popup ins;
    void Awake()
    {
        if (ins == null)
        {
            ins = this;
        }
        else
        {
            Destroy(gameObject);
        }
        anim = GetComponent<Animator>();
    }
    public void Set(string t, string b, string b1, string b2, TestDelegate b1Callback, int width = 200, int height = 200)
    {
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        title.text = t;
        body.text = b;
        button1text.text = b1;
        button2text.text = b2;
        confirm = b1Callback;
        anim.SetBool("Open", true);
    }

    public void Cancel()
    {
        anim.SetBool("Open", false);
        Destroy(gameObject, .34f);
    }

    public void Confirm()
    {
        confirm();
        Cancel();
    }
}
