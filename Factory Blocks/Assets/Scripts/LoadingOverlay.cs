using System.Collections;
using UnityEngine;

public class LoadingOverlay : MonoBehaviour
{
    float width, height, loadingSpeed = 0;
    Vector2 center;
    public float easeDuration = 1;
    public bool Moving { get; private set; }
    Animator anim;

    void Start()
    {
        width = Screen.width;
        height = Screen.height;
        center = new Vector2(width / 2.0f, height / 2.0f);
        transform.position = new Vector2(center.x - width, center.y);
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetFloat("loading", loadingSpeed);
        loadingSpeed = Mathf.Lerp(loadingSpeed, (Vector2)transform.position == center ? 1 : 0, Time.deltaTime * 3);
    }

    public void Show(Vector2 dir)
    {
        anim.SetTrigger("reset");
        Vector2 startPos = center - new Vector2(dir.x * width,dir.y * height);
        StartCoroutine(EaseTo(startPos, center));
    }

    public void Hide(Vector2 dir)
    {
        Vector2 endPos = center + new Vector2(dir.x * width, dir.y * height);
        StartCoroutine(EaseTo(center, endPos));
    }

    IEnumerator EaseTo(Vector2 start, Vector2 end)
    {
        Moving = true;
        float time = 0;
        while(time < easeDuration)
        {
            time += Time.deltaTime;
            transform.position = Ease(start, end, time, easeDuration);
            yield return null;
        }
        transform.position = end;
        Moving = false;
    }

    Vector2 Ease(Vector2 startPos, Vector2 endPos, float time, float duration)
    {
        Vector2 c = endPos - startPos;
        time /= duration / 2;
        if (time < 1) return c / 2 * time * time * time + startPos;
        time -= 2;
        return c / 2 * (time * time * time + 2) + startPos;
    }
}
