﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    List<Tile> tiles = new List<Tile>();
    List<Tile> goalBlocks = new List<Tile>();
    bool lastStill = true;
    Tile[,][] level;
    public Level loadedLevel { get; private set; }
    public bool won = false, playing = true;

    void Update()
    {
        if(Still() && !lastStill && goalBlocks.Count > 0)
        {
            won = true;
            foreach(Tile t in goalBlocks)
            {
                if(GetTile(t.pos,new int[] { 2 }) == null)
                {
                    won = false;
                }
            }
        }
        lastStill = Still();
    }

    public void LoadLevel(Level levelMap)
    {
        loadedLevel = levelMap;

        width = levelMap.width;
        height = levelMap.height;

        while(tiles.Count > 0)
        {
            RemoveTile(tiles[0]);
        }
        level = new Tile[width, height][];
        goalBlocks.Clear();
        won = false;

        foreach (Level.block b in levelMap.tiles)
        {
            AddTile(b);
        }
        foreach (Tile t in tiles)
        {
            Level.block b = Array.Find(levelMap.tiles, element => element.tileID == t.ID);
            if (b.isMaster)
            {
                foreach (int s in b.slavesIDs)
                {
                    t.AddSlave(tiles.Find(element => element.ID == s));
                }
            }
            else
            {
                t.SetMaster(tiles.Find(element => element.ID == b.masterID));
            }
        }
        RefreshSprites();
        CameraController.Instance.SetPosition(width);
        playing = true;
    }

    public void ReloadLevel()
    {
        LoadLevel(loadedLevel);
    }

    public void RefreshSprites()
    {
        //TODO if changed is set, only do that neighborhood
        foreach (Tile t in tiles)
        {
            t.SetSprite();
        }
    }

    public void AddTile(Level.block b)
    {
        Tile t = Instantiate(Array.Find(tilePrefabs, element => element.GetComponent<Tile>().type == b.type)).GetComponent<Tile>();
        t.Set(b.tileID, b.pos, b.isMaster);
        AddMap(t.pos,t);
        tiles.Add(t);
        if(t.type == 2)
        {
            goalBlocks.Add(t);
        }
    }

    public void RemoveTile(Tile t)
    {
        if(t == null) { return; }
        RemoveMap(t.pos,t);
        tiles.Remove(t);
        if (t.type == 2)
        {
            goalBlocks.Add(t);
        }
        t.SetMaster(null);
        Destroy(t.gameObject);
    }

    public Level SaveLevel(string name)
    {
        Level levelMap = new Level();
        levelMap.width = width;
        levelMap.height = height;
        levelMap.name = name;

        int savedTiles = 0;
        foreach(Tile t in tiles){ if (t.ID >= 0) { savedTiles++; }}
        levelMap.tiles = new Level.block[savedTiles];

        int saveNumber = 0;
        foreach (Tile t in tiles) {
            if (t.ID >= 0){
                levelMap.tiles[saveNumber] = t.BlockData();
                saveNumber++;
            }
        }

        GameManager.Instance.SaveLevel(levelMap);
        return levelMap;
    }

    public void Slide(Vector2Int dir)
    {
        if (Still() && !won && playing)
        {
            foreach (Tile t in tiles)
            {
                if (t.type == 2 || t.type == 3) t.Push(dir);
            }
            foreach (Tile t in tiles)
            {
                if (t.type != 2 && t.type != 3) t.Push(dir);
            }
        }
    }

    bool Still()
    {
        foreach (Tile t in tiles)
        {
            if (!t.IsStill())
            {
                return false;
            }
        }
        return true;
    }

    public bool[] IsNeighbors(Vector2Int pos, int type)
    {
        bool[] neighbors = new bool[8];

        Vector2Int[] dirs = new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0),
            new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 0) };

        for(int i = 0; i < 8; i++)
        {
            Vector2Int searchPos = pos + dirs[i];
            if (InLevel(searchPos) && GetTileWithType(searchPos,type)){
                neighbors[i] = true;
            }
        }
        return neighbors;
    }

    public Tile GetTile(Vector2Int pos)
    {
        Tile[] slot = level[pos.x, pos.y];
        if (slot != null)
        {
            if (slot[0] != null)
            {
                return slot[0];
            }
            if (slot[1] != null)
            {
                return slot[1];
            }
        }
        return null;
    }
    public Tile GetBGTile(Vector2Int pos)
    {
        Tile[] slot = level[pos.x, pos.y];
        if (slot != null)
        {
            if (slot[2] != null)
            {
                return slot[2];
            }
        }
        return null;
    }
    public Tile GetTile(Vector2Int pos, int[] ignoredTypes)
    {
        Tile[] slot = level[pos.x, pos.y];
        if (slot != null)
        {
            if (slot[0] != null && !new List<int>(ignoredTypes).Contains(slot[0].type))
            {
                return slot[0];
            }
            if (slot[1] != null && !new List<int>(ignoredTypes).Contains(slot[1].type))
            {
                return slot[1];
            }
        }
        return null;
    }
    public Tile GetTileWithType(Vector2Int pos, int type)
    {
        Tile[] slot = level[pos.x, pos.y];
        if (slot != null)
        {
            if (slot[0] != null && type == slot[0].type)
            {
                return slot[0];
            }
            if (slot[1] != null && type == slot[1].type)
            {
                return slot[1];
            }
        }
        return null;
    }
    public Tile GetTile(Vector2Int pos, int[] ignoredTypes, List<Tile> ignoredTiles)
    {
        Tile[] slot = level[pos.x, pos.y];
        if (slot != null)
        {
            if (slot[0] != null && !new List<int>(ignoredTypes).Contains(slot[0].type) && !ignoredTiles.Contains(slot[0]))
            {
                return slot[0];
            }
            if (slot[1] != null && !new List<int>(ignoredTypes).Contains(slot[1].type) && !ignoredTiles.Contains(slot[1]))
            {
                return slot[1];
            }
        }
        return null;
    }

    public bool InLevel(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public Tile GetFirstTile(Vector2Int pos, Vector2Int dir, int[] ignoredTypes, List<Tile> ignoredTiles)
    {
        for (int i = 1; i < width + height; i++) // the width + height is just to stop from freezing if there isnt a tile ever
        {
            Tile t;
            if((t = GetTile(pos + dir * i, ignoredTypes, ignoredTiles)) != null)
            {
                return t;
            }
        }
        return null;
    }

    public void SetLevelSize(int w, int h)
    {
        level = new Tile[w,h][];
        width = w;
        height = h;
    }

    public void RemapTiles()
    {
        foreach (Tile t in tiles)
        {
            AddMap(t.pos, t);
        }
    }

    public void RemoveMap(Vector2Int pos, Tile t)
    {
        level[pos.x, pos.y][MapLayer(t)] = null;
    }

    public void AddMap(Vector2Int pos, Tile t)
    {
        if (level[pos.x, pos.y] == null)
        {
            level[pos.x, pos.y] = new Tile[3];
        }
        level[pos.x, pos.y][MapLayer(t)] = t;
    }

    int MapLayer(Tile t)
    {
        if(t.type == 2)
        {
            return 1;
        }
        if(t.type == 4)
        {
            return 2;
        }
        return 0;
    }

    public int GetLevelSize()
    {
        return width;
    }
}
