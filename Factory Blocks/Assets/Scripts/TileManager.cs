using System;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            tilePrefabs = Resources.LoadAll<GameObject>("Prefabs/Tiles");
        }
        else
        {
            Destroy(this);
        }
    }

    GameObject[] tilePrefabs;

    int width, height;
    List<Tile> level = new List<Tile>();
    Level levelMap;

    public void LoadLevel(string path)
    {
        string contents = System.IO.File.ReadAllText(path);
        levelMap = JsonUtility.FromJson<Level>(contents);
        width = levelMap.width;
        height = levelMap.height;
        foreach (Level.block b in levelMap.tiles)
        {
            AddTile(b);
        }
        foreach (Tile t in level)
        {
            Level.block b = Array.Find(levelMap.tiles, element => element.tileID == t.ID);
            if (b.isMaster)
            {
                foreach (int s in b.slavesIDs)
                {
                    t.AddSlave(level.Find(element => element.ID == s));
                }
            }
            else
            {
                t.SetMaster(level.Find(element => element.ID == b.masterID));
            }
        }
        foreach (Tile t in level)
        {
            t.SetSprite();
        }

        CameraController.Instance.SetPosition(width);
    }

    public void AddTile(Level.block b)
    {
        Tile t = Instantiate(Array.Find(tilePrefabs, element => element.GetComponent<Tile>().type == b.type)).GetComponent<Tile>();
        t.Set(b.tileID, b.pos, b.isMaster);
        level.Add(t);
    }

    public void RemoveTile(Tile t)
    {
        level.Remove(t);
        if (!t.BlockData().isMaster){ level.Find(element => element.ID == t.BlockData().masterID).RemoveReferences(t); }
        Destroy(t.gameObject);
    }

    public void SaveLevel(string path)
    {
        levelMap = new Level();
        levelMap.width = width;
        levelMap.height = height;
        levelMap.tiles = new Level.block[level.Count];
        for (int i = 0; i < level.Count; i++)
        {
            print(JsonUtility.ToJson(level[i].BlockData()));
            levelMap.tiles[i] = level[i].BlockData();
        }
        string contents = JsonUtility.ToJson(levelMap, true);
        System.IO.File.WriteAllText(path, contents);
    }

    public void Slide(Vector2 dir)
    {
        foreach(Tile t in level)
        {
            if (!t.IsStill())
            {
                return;
            }
        }
        foreach (Tile t in level)
        {
            t.InformBlockades(dir);
        }
        foreach (Tile t in level)
        {
            t.Push(dir);
        }
    }

    public Tile[] GetNeighbors(Vector2 pos)
    {
        Tile[] neighbors = new Tile[4];

        Vector2[] dirs = new Vector2[] { new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1) };

        for(int i = 0; i < 4; i++)
        {
            Vector2 searchPos = pos + dirs[i];
            if (InLevel(searchPos)){
                neighbors[i] = GetTile(searchPos);
            }
        }

        return neighbors;
    }

    public Tile GetTile(Vector2 pos)
    {
        return level.Find(element => element.pos == pos);
    }

    bool InLevel(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public Tile GetFirstTile(Vector2 pos, Vector2 dir)
    {
        for (int i = 1; i < width + height; i++) // the width + height is just to stop from freezing if there isnt a tile ever
        {
            Tile t;
            if((t = GetTile(pos + i * dir)) != null)
            {
                return t;
            }
        }
        return null;
    }

    public int GetSpace(Vector2 pos, Vector2 dir)
    {
        for (int i = 1; i < width + height; i++) // the width + height is just to stop from freezing if there isnt a tile ever
        {
            Tile t;
            if ((t = GetTile(pos + i * dir)) != null)
            {
                return i-1;
            }
        }
        return 0; //should never happen
    }
}
