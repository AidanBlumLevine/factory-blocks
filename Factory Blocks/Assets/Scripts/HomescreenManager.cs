using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HomescreenManager : MonoBehaviour
{
    public static HomescreenManager Instance;
    public Button continueButton;
    public GameObject continueOverlay;
    public GameObject continueTextbox;

    public Text continueName;
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
        continueButton.interactable = gm.CanContinue();
        continueOverlay.SetActive(!gm.CanContinue());
        continueTextbox.SetActive(gm.CanContinue());
        continueName.text = gm.ContinueName();
    }

    public void Play()
    {
        gm.LoadScene(3);
    }

    public void Continue()
    {
        gm.Continue();
    }

    public void Edit()
    {
        gm.LoadScene(2);
    }
}
