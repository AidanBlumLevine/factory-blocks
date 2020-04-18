using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public Text title, body, button1text, button2text;
    public delegate void TestDelegate();
    TestDelegate confirm, cancel;
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
    public void Set(string t, string b, string b1, string b2, TestDelegate b1Callback, TestDelegate b2Callback = null, int width = 200, int height = 200)
    {
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        title.text = t;
        body.text = b;
        button1text.text = b1;
        button2text.text = b2;
        confirm = b1Callback;
        cancel = b2Callback;
        anim.SetBool("Open", true);
    }

    public void Cancel()
    {
        if(cancel != null)
        {
            cancel();
        }
        anim.SetBool("Open", false);
        Destroy(gameObject, .34f);
    }

    public void Confirm()
    {
        confirm();
        anim.SetBool("Open", false);
        Destroy(gameObject, .34f);
    }
}
