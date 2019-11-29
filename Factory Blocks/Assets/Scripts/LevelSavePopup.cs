using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelSavePopup : MonoBehaviour
{
    public InputField levelName;
    public Text errorText;
    Animator anim;
    static LevelSavePopup ins;
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
    }
    void Start()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/levels/");
        levelName.text = EditorManager.Instance.GetLevelName();
        anim = GetComponent<Animator>();
        anim.SetBool("Open", true);
    }

    public void Save()
    {
        string name = levelName.text;
        if (levelName.text.Length == 0)
        {
            errorText.text = "Name cannot be empty";
            return;
        }

        if (GameManager.Instance.LevelNameTaken(name))
        {
            Instantiate(GameManager.Instance.generalPopupPrefab, EditorManager.Instance.canvasRoot.transform).GetComponent<Popup>()
                .Set("LEVEL NAME TAKEN", "A LEVEL WITH THAT NAME ALREADY EXISTS. DO YOU WANT TO OVERWRITE "+ levelName.text +"?", "Yes", "No", SaveLevel,300,250);
        }
        else
        {
            SaveLevel();
        }
    }

    void SaveLevel()
    {
        if (anim != null)
        {
            string name = levelName.text;
            EditorManager.Instance.ResumeEditing(TileManager.Instance.SaveLevel(name));
            anim.SetBool("Open", false);
            Destroy(gameObject, .34f);
        }
    }

    public void Cancel()
    {
        EditorManager.Instance.ResumeEditing(null);
        anim.SetBool("Open", false);
        Destroy(gameObject, .34f);
    }

    public void TextChanged()
    {
    }
}
