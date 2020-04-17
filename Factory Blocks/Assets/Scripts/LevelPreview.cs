using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPreview : MonoBehaviour
{
    Level level;
    public Image preview;
    public Text title;
    public GameObject edit;
    public Text best;
    public GameObject coins;
    public Sprite emptyCoin;
    Animator anim;
    public void Set(Level l)
    {
        level = l;
        title.text = level.name;
        edit.SetActive(!l.permanent);
        coins.SetActive(l.permanent);
        Image[] coinImg = coins.GetComponentsInChildren<Image>();
        int bestMoves = GameManager.Instance.BestMoves(level);
        best.text = bestMoves > 9999 ? "---" : bestMoves + "";
        if (bestMoves > level.stars[0])
        {
            coinImg[1].sprite = emptyCoin;
        }
        if (bestMoves > level.stars[1])
        {
            coinImg[2].sprite = emptyCoin;
        }
        if (bestMoves > level.stars[2])
        {
            coinImg[3].sprite = emptyCoin;
        }
        preview.sprite = Loader.Instance.PreviewLevel(l);
        anim = GetComponent<Animator>();
    }

    public void PlayLevel()
    {
        GameManager.Instance.LoadScene(1, level);
        //anim.SetTrigger("Open");
    }

    public void EditLevel()
    {
        GameManager.Instance.LoadScene(2, level);
    }

    public void Delete()
    {
        Instantiate(GameManager.Instance.generalPopupPrefab, LevelSelectorManager.Instance.canvasRoot.transform).GetComponent<Popup>()
                .Set("Delete level", "Are you sure?", "Yes", "No", DeleteLevel);
    }

    void DeleteLevel()
    {
        GameManager.Instance.DeleteLevel(level);
        Destroy(gameObject);
    }
}
