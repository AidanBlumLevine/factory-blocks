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
    public GameObject coins;
    public Sprite emptyCoin;
    public void Set(Level l)
    {
        level = l;
        title.text = level.name;
        edit.SetActive(!l.permanent);
        coins.SetActive(l.permanent);
        Image[] coinImg = coins.GetComponentsInChildren<Image>();
        if (level.bestMoves > level.stars[0])
        {
            coinImg[1].sprite = emptyCoin;
        }
        if (level.bestMoves > level.stars[1])
        {
            coinImg[2].sprite = emptyCoin;
        }
        if (level.bestMoves > level.stars[2])
        {
            coinImg[3].sprite = emptyCoin;
        }
        preview.sprite = Loader.Instance.PreviewLevel(l,(int)preview.rectTransform.rect.width);
        preview.transform.position = new Vector3((int)preview.transform.position.x, (int)preview.transform.position.y, (int)preview.transform.position.z);
    }

    public void PlayLevel()
    {
        GameManager.Instance.LoadScene(1, level);
    }

    public void EditLevel()
    {
        GameManager.Instance.LoadScene(2, level);
    }
}
