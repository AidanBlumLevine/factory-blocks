using System.Collections.Generic;
using UnityEngine;

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


    int levelSize = 6;
    int nextID = 0;
    List<Vector2> selection = new List<Vector2>();

    TileManager tm;
    void Start()
    {
        tm = TileManager.Instance;
        SetWall(levelSize, levelSize);
    }

    void Update()
    {
        //TODO selection
    }

    public void Save(string fileName)
    {
        tm.SaveLevel(Application.persistentDataPath + "/" + fileName + ".json");
    }

    void SetWall(int width, int height)
    {
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
                    pos = new Vector2(i, firstEdge * (height-1))
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
                    pos = new Vector2(firstEdge * (width-1) ,i)
                };
                tm.AddTile(b);
                nextID++;
            }
        }

        CameraController.Instance.SetPosition(width);
    }

    void ClearWall(int width, int height)
    {
        for (int firstEdge = 0; firstEdge <= 1; firstEdge++)
        {
            for (int i = 0; i < width; i++)
            {
                tm.RemoveTile(tm.GetTile(new Vector2(i, firstEdge * (height - 1))));
            }
            for (int i = 1; i < height - 1; i++)
            {
                tm.RemoveTile(tm.GetTile(new Vector2(firstEdge * (width - 1), i)));
            }
        }
    }

    public void GrowWall()
    {
        if (levelSize < 20)
        {
            ClearWall(levelSize, levelSize);
            levelSize++;
            SetWall(levelSize, levelSize);
        }
    }

    public void ShrinkWall()
    {
        if (levelSize > 3)
        {
            ClearWall(levelSize,levelSize);
            levelSize--;
            SetWall(levelSize, levelSize);
        }
    }

    public void FillSelection(int tileID)
    {
        //TODO
    }
}
