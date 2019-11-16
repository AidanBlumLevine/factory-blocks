using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Texture2D texture;
    public bool connectToClones = false, kinetic = true;
    public int type;

    TileManager tm;
    SpriteRenderer sr;

    static int nextID = 0;
    public int ID { get; private set; }
    Sprite[] sprites;
    Vector2 visualPos;
    public Vector2 pos { get; private set; }
    bool isMaster = true;
    Tile master;
    List<Tile> slaves = new List<Tile>();
    float moveSpeed = 0;

    List<Tile> dependants = new List<Tile>(); //temporary for moving

    void Awake()
    {
        ID = nextID;
        nextID++;
        sprites = Resources.LoadAll<Sprite>("Sprites/"+texture.name);
        sr = GetComponent<SpriteRenderer>();
        tm = TileManager.Instance;
    }

    void Update()
    {
        visualPos = Vector2.MoveTowards(visualPos, pos, Time.deltaTime * moveSpeed);
        transform.position = visualPos;
        moveSpeed += Time.deltaTime * 50;
    }

    public void Set(int ID, Vector2 pos,bool isMaster)
    {
        this.ID = ID;
        this.pos = pos;
        visualPos = pos;
        transform.position = pos;
        this.isMaster = isMaster;
    }

    public void SetSprite() 
    {
        if (isMaster)
        {
            if (connectToClones)
            {
                Tile[] neighbors = tm.GetNeighbors(pos);
                sr.sprite = GetSprite(neighbors[0] != null && neighbors[0].type == type,
                    neighbors[1] != null && neighbors[1].type == type,
                    neighbors[2] != null && neighbors[2].type == type,
                    neighbors[3] != null && neighbors[3].type == type);
            }
            else
            {
                SetLinkedSprite(slaves);
                foreach(Tile s in slaves)
                {
                    List<Tile> wholeLink = new List<Tile>(slaves);
                    wholeLink.Add(this);
                    s.SetLinkedSprite(wholeLink);
                }
            }       
        }
    }

    public void SetLinkedSprite(List<Tile> linked)
    {
        bool left = linked.Where(element => element.pos + new Vector2(1,0) == pos).Count() == 1;
        bool up = linked.Where(element => element.pos + new Vector2(0, -1) == pos).Count() == 1;
        bool right = linked.Where(element => element.pos + new Vector2(-1, 0) == pos).Count() == 1;
        bool down = linked.Where(element => element.pos + new Vector2(0, 1) == pos).Count() == 1;
        sr.sprite = GetSprite(left,up,right,down);
    }

    Sprite GetSprite(bool cL, bool cU, bool cR, bool cD)
    {
        int count = (cL ? 1 : 0) + (cR ? 1 : 0) + (cU ? 1 : 0) + (cD ? 1 : 0); // the number of connected sides

        if (count >= 3) { return sprites[0]; }

        if(count == 2)
        {
            if (cU && cR) { return sprites[1]; }
            if (cR && cD) { return sprites[2]; }
            if (cD && cL) { return sprites[3]; }
            if (cL && cU) { return sprites[4]; }

            if ((cU && cD) || (cL && cR)){
                return sprites[0];
            }
        }

        if (count == 1)
        {
            if (cU) { return sprites[5]; }
            if (cR) { return sprites[6]; }
            if (cD) { return sprites[7]; }
            if (cL) { return sprites[8]; }
        }
        return sprites[9]; // count must be 0
    }

    public void AddSlave(Tile t)
    {
        slaves.Add(t);
    }

    public void SetMaster(Tile t)
    {
        master = t;
    }

    public Level.block BlockData()
    {
        Level.block b = new Level.block();
        b.type = type;
        b.tileID = ID;
        b.pos = pos;
        b.isMaster = isMaster;
        b.masterID = master == null ? -1 : master.ID;
        b.slavesIDs = new int[slaves.Count]; 
        for (int i = 0; i < slaves.Count; i++)
        {
            b.slavesIDs[i] = slaves[i].ID;
        }
        return b;
    }

    public void Collision(Vector2 hitDir)
    {
        //TODO
    }

    public void InformBlockades(Vector2 dir)
    {
        if (isMaster && kinetic)
        {
            Tile d = tm.GetFirstTile(pos, dir);
            if (!slaves.Contains(d))
            {
                d.AddDependant(this);
            }
            foreach(Tile t in slaves)
            {
                d = tm.GetFirstTile(t.pos, dir);
                if (!slaves.Contains(d) && d != this)
                {
                    d.AddDependant(this);
                }
            }
        }
    }

    public void Push(Vector2 dir)
    {
        if (isMaster && kinetic)
        {
            int dist = int.MaxValue;

            if(!slaves.Contains(tm.GetFirstTile(pos, dir)))
            {
                dist = Mathf.Min(tm.GetSpace(pos, dir), dist);
            }

            foreach (Tile t in slaves)
            {
                Tile f = tm.GetFirstTile(t.pos, dir);
                if (!slaves.Contains(f) && f != this)
                {
                    dist = Mathf.Min(tm.GetSpace(t.pos, dir), dist);
                }
            }

            Move(dir, dist);

            foreach (Tile t in slaves)
            {
                t.Move(dir, dist);
            }

        }
    }

    void Move(Vector2 dir, float dist)
    {
        pos += dist * dir;
        UpdateDependants(dir);
        ClearDependants();
        moveSpeed = 10;
    }

    public void UpdateDependants(Vector2 dir)
    {
        foreach (Tile t in dependants)
        {
            t.Push(dir);
        }
    }

    public void AddDependant(Tile toAdd)
    {
        dependants.Add(toAdd);
    }

    public void ClearDependants()
    {
        dependants.Clear();
    }

    public bool IsStill()
    {
        return pos == visualPos;
    }

    public void RemoveReferences(Tile t)
    {
        if (slaves.Contains(t)){ slaves.Remove(t); }
    }
}
