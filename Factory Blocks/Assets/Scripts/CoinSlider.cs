using UnityEngine;
using UnityEngine.UI;

public class CoinSlider : MonoBehaviour
{
    public Image coin1, coin2, coin3;
    public RectTransform path;
    public Sprite failedCoin, coin;
    float pathMax = 0, step;
    int c1, c2, c3;
    bool visible = true;
    Vector3 basePos;

    public void Set(Level l)
    {
        GetComponent<CanvasGroup>().alpha = l.permanent ? 1 : 0;
        visible = l.permanent;
        if (visible)
        {
            if (pathMax == 0)
            {
                pathMax = path.rect.width;
                basePos = path.anchoredPosition3D - Vector3.right * (GetComponent<RectTransform>().rect.width / 2 + 10);
            }
            c1 = l.stars[0];
            c2 = l.stars[1];
            c3 = l.stars[2];
            step = pathMax / c1;
            coin1.sprite = coin;
            coin2.sprite = coin;
            coin3.sprite = coin;
            Set(0);

            coin3.transform.GetChild(0).gameObject.SetActive(false);
            coin2.transform.GetChild(0).gameObject.SetActive(false);
            coin1.transform.GetChild(0).gameObject.SetActive(false);
            if (l.bestMoves <= c1)
            {
                coin1.transform.GetChild(0).gameObject.SetActive(true);
            }
            else if (l.bestMoves <= c2)
            {
                coin2.transform.GetChild(0).gameObject.SetActive(true);
            }
            else if (l.bestMoves <= c3)
            {
                coin3.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    public void Set(int moves)
    {
        if (visible) { 
        Vector3 s = new Vector3(step, 0, 0);

        if (step * moves - .0001f > pathMax)
        {
            path.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pathMax);
            coin1.transform.localPosition = basePos + s * c1 * pathMax / (step * moves);
            coin2.transform.localPosition = basePos + s * c2 * pathMax / (step * moves);
            coin3.transform.localPosition = basePos + s * c3 * pathMax / (step * moves);
        }
        else
        {
            path.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(30 , step * moves));
            coin1.transform.localPosition = basePos + s * c1;
            coin2.transform.localPosition = basePos + s * c2;
            coin3.transform.localPosition = basePos + s * c3;
        }
        if (moves > c1) { coin1.sprite = failedCoin; }
        if (moves > c2) { coin2.sprite = failedCoin; }
        if (moves > c3) { coin3.sprite = failedCoin; }
        }
    }
}
