using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDip : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    AudioSource audio;

    GameObject[] toDip;
    float dipAmount = 5;
    bool down;
    void Start()
    {
        audio = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
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

        audio.PlayOneShot(GameManager.Instance.buttonDownSound, GameManager.Instance.volume * .5f);
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
