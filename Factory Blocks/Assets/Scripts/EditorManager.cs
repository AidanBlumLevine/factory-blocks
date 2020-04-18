using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    public static EditorManager Instance;
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

    public GameObject selectionIndicator, savePopup, canvasRoot;
    public Image LinkToggle, SelectionToggle;
    public Sprite linkOther, selectionOther;
    public Animator SavePlay;
    public Text fileName;
    int levelSize = 6;
    int nextID = 0;
    List<Tile> selection = new List<Tile>();
    bool editing = true, selecting = true, linking = true, waitUntilRelease = false, saved = false;
    TileManager tm;
    Level level; //if it exists, it is saved and can be played
    void Start()
    {
        tm = TileManager.Instance;
        TileManager.Instance.SetLevelSize(levelSize, levelSize);
        SetWall(levelSize, levelSize);
    }

    void Update()
    {
        if (editing)
        {
            Vector3 mp = Input.touches.Count() > 0 ? (Vector3)Input.touches[0].position : Input.mousePosition;
            mp.z = 0;
            mp = Camera.main.ScreenToWorldPoint(mp);
            Vector2Int gamePos = new Vector2Int((int)(mp.x + .5), (int)(mp.y + .5));

            if (!waitUntilRelease && (Input.GetMouseButton(0) || Input.touches.Count() > 0) && gamePos.x > 0 && gamePos.x < levelSize - 1 && gamePos.y > 0 && gamePos.y < levelSize - 1)
            {
                if (selecting)
                {
                    if (selection.Where(element => element.pos == gamePos).Count() == 0)
                    {
                        selection.Add(Instantiate(selectionIndicator).GetComponentInChildren<Tile>());
                        selection[selection.Count - 1].Set(-1, gamePos, true);
                        UpdateSelectionSprites();
                        CalculateSelectionLinkStatus();
                    }
                }      
                else if (selection.Where(element => element.pos == gamePos).Count() != 0)
                {
                    Tile t = selection.First(element => element.pos == gamePos);
                    selection.Remove(t);
                    Destroy(t.gameObject);
                    UpdateSelectionSprites();
                    CalculateSelectionLinkStatus();
                    if(selection.Count == 0)
                    {
                        ToggleSelect();
                        waitUntilRelease = true;
                    }
                }
            }
            if (waitUntilRelease && (!Input.GetMouseButton(0) && Input.touches.Count() == 0))
            {
                waitUntilRelease = false;
            }
        }
        if(level == null)
        {
            fileName.text = "";
        }
        else
        {
            fileName.text = level.name + (saved ? "" : "*");
        }
        SavePlay.SetBool("SaveShown", !saved);
    }

    void CalculateSelectionLinkStatus()
    {
        bool setToLinking = true;
        foreach (Tile s in selection)
        {
            Tile t = tm.GetTile(s.pos);
            if (t != null && !t.BlockData().isMaster)
            {
                setToLinking = false;
            }
        }
        if(linking != setToLinking)
        {
            Sprite temp = LinkToggle.sprite;
            LinkToggle.sprite = linkOther;
            linkOther = temp;
        }
        linking = setToLinking;
    }

    public void ResumeEditing(Level savedLevel)
    {
        if (savedLevel != null) {
            level = savedLevel;
            saved = true;
        }
        ResumeEditing();
    }

    public void ResumeEditing()
    {
        editing = true;
        waitUntilRelease = true;
    }

    void UpdateSelectionSprites()
    {
        foreach(Tile t in selection)
        {
            t.SetSprite(selection);
        }
    }

    public void SaveOrPlay()
    {
        if (!saved)
        {
            Instantiate(savePopup, canvasRoot.transform);
            UnSelect();
            editing = false;
        } else
        {
            GameManager.Instance.LoadScene(1, level);
        }
    }

    public void UnSelect()
    {
        while (selection.Count > 0)
        {
            Tile temp = selection[0];
            selection.Remove(temp);
            Destroy(temp.gameObject);
        }
    }

    public void LoadLevel(Level l)
    {
        TileManager.Instance.LoadLevel(l);
        levelSize = TileManager.Instance.GetLevelSize();
        level = l;
        saved = true;
        SavePlay.SetBool("SaveShown", false);
        nextID = TileManager.Instance.LargestID() + 1;
    }

    void SetWall(int width, int height)
    {
        tm.SetLevelSize(width, height);
        for (int firstEdge = 0; firstEdge <= 1; firstEdge++)
        {
            for (int i = 0; i < width; i++)
            {
                Level.block b = new Level.block
                {
                    type = 1,
                    tileID = nextID,
                    isMaster = true,
                    masterID = -1,
                    slavesIDs = new int[] { },
                    pos = new Vector2Int(i, firstEdge * (height-1))
                };
                tm.AddTile(b);
                nextID++;
            }
            for (int i = 1; i < height - 1; i++)
            {
                Level.block b = new Level.block
                {
                    type = 1,
                    tileID = nextID,
                    isMaster = true,
                    masterID = -1,
                    slavesIDs = new int[] { },
                    pos = new Vector2Int(firstEdge * (width-1) ,i)
                };
                tm.AddTile(b);
                nextID++;
            }
        }

        CameraController.Instance.SetPosition(width);
        tm.RemapTiles();
        tm.RefreshSprites();
    }

    void ClearWall(int width, int height)
    {
        for (int firstEdge = 0; firstEdge <= 1; firstEdge++)
        {
            for (int i = 0; i < width; i++)
            {
                tm.RemoveTile(tm.GetTile(new Vector2Int(i, firstEdge * (height - 1))));
                tm.RemoveTile(tm.GetBGTile(new Vector2Int(i, firstEdge * (height - 1))));
            }
            for (int i = 1; i < height - 1; i++)
            {
                tm.RemoveTile(tm.GetTile(new Vector2Int(firstEdge * (width - 1), i)));
                tm.RemoveTile(tm.GetBGTile(new Vector2Int(firstEdge * (width - 1), i)));
            }
        }
    }

    void ClearSelectionFromWall(int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            Tile t = selection.FirstOrDefault(element => element.transform.position == new Vector3(i, height - 1));
            if (t != null)
            {
                selection.Remove(t);
                Destroy(t.gameObject);
            }
        }
        for (int i = 1; i < height - 1; i++)
        {
            Tile g = selection.FirstOrDefault(element => element.transform.position == new Vector3(width - 1, i));
            if (g != null)
            {
                selection.Remove(g);
                Destroy(g.gameObject);
            }
        }
        UpdateSelectionSprites();
    }

    public void GrowWall()
    {
        saved = false;
        if (levelSize < 20)
        {
            ClearWall(levelSize, levelSize);
            levelSize++;
            SetWall(levelSize, levelSize);
        }
    }

    public void ShrinkWall()
    {
        saved = false;
        if (levelSize > 3)
        {
            ClearWall(levelSize,levelSize);
            levelSize--;
            ClearSelectionFromWall(levelSize, levelSize);
            ClearWall(levelSize, levelSize); //this is important for clearing stuff
            SetWall(levelSize, levelSize);
        }
    }

    public void FillSelection(int tileType)
    {
        saved = false;
        foreach (Tile s in selection)
        {
            if (tm.GetTile(s.pos) == null && tm.GetBGTile(s.pos) == null)
            {
                Level.block b = new Level.block
                {
                    type = tileType,
                    tileID = nextID,
                    isMaster = true,
                    masterID = -1,
                    slavesIDs = new int[] { },
                    pos = s.pos
                };
                tm.AddTile(b);
                nextID++;
            }
        }
        tm.RefreshSprites();
    }

    public void FillSpikes()
    {
        saved = false;
        foreach (Tile s in selection)
        {
            if (tm.GetTile(s.pos) == null && tm.GetBGTile(s.pos) == null)
            {
                Vector2Int[] dirs = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1) };
                foreach (Vector2Int dir in dirs)
                {
                    if (tm.GetTile(s.pos + dir) != null && tm.GetTile(s.pos + dir).type == 1)
                    {
                        Level.block b = new Level.block
                        {
                            type = 4,
                            tileID = nextID,
                            isMaster = false,
                            masterID = tm.GetTile(s.pos + dir).ID,
                            slavesIDs = new int[] { },
                            pos = s.pos
                        };
                        tm.AddTile(b);
                        b.isMaster = false;
                        tm.GetBGTile(s.pos).SetMaster(tm.GetTile(s.pos + dir));
                        tm.GetTile(s.pos + dir).AddSlave(tm.GetBGTile(s.pos));
                        tm.GetTile(s.pos + dir).SetSprite();
                        nextID++;
                        break; //only add one spike per square
                    }
                }         
            }
        }
    }

    public void ClearSelection()
    {
        saved = false;
        foreach (Tile s in selection)
        {
            if (tm.GetTile(s.pos))
            {
                tm.RemoveTile(tm.GetTile(s.pos));
            }
            if (tm.GetBGTile(s.pos))
            {
                tm.RemoveTile(tm.GetBGTile(s.pos));
            }
        }
        tm.RefreshSprites();
    }

    public void ToggleSelect()
    {
        selecting = !selecting;
        Sprite temp = SelectionToggle.sprite;
        SelectionToggle.sprite = selectionOther;
        selectionOther = temp;
    }

    public void ToggleLink()
    {
        saved = false;
        if (linking)
        {
            LinkSelection();
        }
        else
        {
            UnlinkSelection();
        }
        CalculateSelectionLinkStatus();
    }

    void LinkSelection()
    {
        UnlinkSelection();
        List<Tile> selectedTiles = new List<Tile>();
        foreach (Tile s in selection)
        {
            Tile t = tm.GetTile(s.pos);
            if(t != null && t.kinetic) { selectedTiles.Add(t); }
        }

        if (selectedTiles.Count > 0)
        {
            Tile master = selectedTiles[0];
            foreach (Tile slave in selectedTiles)
            {
                if (slave.ID != master.ID&& slave.type == master.type)
                {
                    master.AddSlave(slave);
                    slave.SetMaster(master);
                }
            }
        }
        tm.RefreshSprites();
    }

    void UnlinkSelection()
    {
        foreach (Tile s in selection)
        {
            Tile t = tm.GetTile(s.pos);
            if (t != null)
            {
                t.SetMaster(null);
            }
        }
        tm.RefreshSprites();
    }

    public void Home()
    {
        if (!saved)
        {
            editing = false;
            Instantiate(GameManager.Instance.generalPopupPrefab,canvasRoot.transform).GetComponent<Popup>()
                .Set("Are you sure", "Level is not saved", "Yes", "No", QuitToHome, ResumeEditing);
        } else
        {
            QuitToHome();
        }
    }

    void QuitToHome()
    {
        GameManager.Instance.LoadScene(0, null);
    }

    public string GetLevelName()
    {
        if(level != null)
        {
            return level.name;
        }
        return "";
    }
}
