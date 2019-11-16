using UnityEngine;

public class HomescreenManager : MonoBehaviour
{
    public static HomescreenManager Instance;
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
    }

    public void Play()
    {
        gm.LoadScene(1);
    }

    public void Edit()
    {
        gm.LoadScene(2);
    }
}
