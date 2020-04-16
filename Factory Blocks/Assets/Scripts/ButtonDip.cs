using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonDip : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    GameObject[] toDip;
    float dipAmount = 5;
    bool down;
    void Start()
    {
        toDip = new GameObject[transform.childCount];
        for(int i =0; i < transform.childCount; i++)
        {
            toDip[i] = transform.GetChild(i).gameObject;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        foreach (GameObject g in toDip)
        {
            g.transform.position -= new Vector3(0, dipAmount);
        }
        down = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (down)
        {
            foreach (GameObject g in toDip)
            {
                g.transform.position += new Vector3(0, dipAmount);
            }
            down = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (down)
        {
            foreach (GameObject g in toDip)
            {
                g.transform.position += new Vector3(0, dipAmount);
            }
            down = false;
        }
        EventSystem.current.SetSelectedGameObject(null);
    }
}
