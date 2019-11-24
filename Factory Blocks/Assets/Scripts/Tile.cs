﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    static int nextID = 0;
    static List<CompiledSprite> spriteCache = new List<CompiledSprite>();
    static List<Tile> globalLinkGroup = new List<Tile>();
    struct CompiledSprite
    {
        public string situation; //8 binary digets and then tile type
        public Sprite sprite;
    }

    public Texture2D texture;
    public bool globalGroup = false, kinetic = true, rotateMid = false;
    public int type;
    float startingMoveSpeed = 10, acceleration = 50;
    public int[] ignoredCollisionTypes = new int[] { 4 };

    public Vector2Int pos { get; private set; }
    public int ID { get; private set; }

    TileManager tm;
    SpriteRenderer sr;

    Sprite[] sprites;
    bool isMaster = true;
    Tile master;
    List<Tile> slaves = new List<Tile>();
    float moveSpeed = 0;
    bool merging = false; //only one merge at once
    bool flipX = false;
    bool flipY = false;


    void Awake()
    {
        ID = nextID;
        nextID++;
        sprites = Resources.LoadAll<Sprite>("Sprites/" + texture.name);
        sr = GetComponent<SpriteRenderer>();
        tm = TileManager.Instance;
        if (rotateMid)
        {
            flipX = Random.value > .5;
            flipY = Random.value > .5;
        }
        if (globalGroup)
        {
            globalLinkGroup.Add(this);
        }
    }


    void Update()
    {
        if ((Vector2)transform.position != pos)
        {
            transform.position = Vector2.MoveTowards(transform.position, pos, Time.deltaTime * moveSpeed);
            moveSpeed += Time.deltaTime * acceleration;
        }
        if(type == 4)
        {
            if(master == null)
            {
                tm.RemoveTile(this);
            }
            //SPIKE
        }
    }

    public void Set(int ID, Vector2Int pos,bool isMaster)
    {
        this.ID = ID;
        this.pos = pos;
        transform.position = new Vector2(pos.x,pos.y);
        this.isMaster = isMaster;
    }

    public void Delete()
    {
        tm.RemoveMap(pos, this);
        Destroy(gameObject);
    }

    public void AddSlave(Tile t)
    {
        slaves.Add(t);
    }

    public void SetMaster(Tile t)
    {
        if(t == null)
        {
            if (!isMaster) {
                master.SetMaster(null);
            }
            else
            {
                foreach(Tile s in slaves)
                {
                    s.WipeMaster();
                }
                slaves.Clear();
            }
        }
        else
        {
            isMaster = false;
            master = t;
            if(type == 4)
            {
                SetSprite();
            }
        }
    }

    public void WipeMaster() {//called by the master on the slaves
        master = null;
        isMaster = true;
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

    public void Collision(Vector2Int hitDir)
    {
        //TODO
    }

    public void Push(Vector2Int dir)
    {
        if (isMaster && kinetic)
        {
            List<Tile> pushedTiles = GetConnected();

            bool foundMore = true;
            while (foundMore)
            {
                int dist = int.MaxValue;
                foundMore = false;
                for (int i = 0; i < pushedTiles.Count; i++)
                {
                    Tile t = pushedTiles[i];
                    Tile firstHit = tm.GetFirstTile(t.pos, dir, t.ignoredCollisionTypes, pushedTiles);
                    int hitDist = Mathf.Abs(t.pos.x - firstHit.pos.x) + Mathf.Abs(t.pos.y - firstHit.pos.y) - 1;
                    dist = Mathf.Min(hitDist, dist);
                    //if(GetConnected().Count==6) print("Tile id:" + t.ID + " hit tile:" + firstHit.ID + " at dist:" + dist);
                    if (firstHit.kinetic && hitDist == 0)
                    {
                        pushedTiles.AddRange(firstHit.GetConnected());
                        foundMore = true;
                    }
                }
                foreach (Tile t in pushedTiles)
                {
                    t.Move(dir, dist);
                }
                if(dist > 0)
                {
                    foundMore = true; //it doesnt add them if they are more than 1 away 
                }
            }
        }
        moveSpeed = startingMoveSpeed;
    }

    List<Tile> GetConnected()
    {
        if (isMaster)
        {
            List<Tile> connected = new List<Tile>(slaves);
            connected.Add(this);
            return connected;
        } else
        {
            return master.GetConnected();
        }
    }

    void Move(Vector2Int dir, int dist)
    {
        tm.RemoveMap(pos, this);
        pos += dir * dist;
        tm.AddMap(pos, this);
    }

    public bool IsStill()
    {
        return pos.x == transform.position.x && pos.y == transform.position.y;
    }

    public void SetSprite()
    {
        if (isMaster)
        {
            List<Tile> group = null;
            if (globalGroup)
            {
                group = globalLinkGroup;
            }
            else
            {
                group = new List<Tile>(slaves);
                group.Add(this);
            }
            SetSprite(group);
            foreach (Tile s in slaves)
            {
                s.SetSprite(group);
            }
        }
    }

    public void SetSprite(List<Tile> linked)
    {
        if(type == 1)
        {
            print(globalLinkGroup.Count);
        }
        if(type == 4)
        {
            if (master != null) //this could fire before it had a chanceto destroy itself
            {
                Vector2 dir = pos - master.pos;
                sr.sprite = sprites[(int)(Vector2.SignedAngle(Vector2.right, dir) / 90)+ 1];
            }
        }
        else
        {
            linked.RemoveAll(item => item == null);
            bool[] sides = new bool[8];
            Vector2Int[] dirs = new Vector2Int[] {new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1),
            new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0) };
            for (int i = 0; i < 8; i++)
            {
                Tile inPos = linked.FirstOrDefault(element => element.pos + dirs[i] == pos);
                sides[i] = (inPos != null);
                if (sides[i] && inPos.type == 4 && inPos.master != this)
                {
                    sides[i] = false;
                }
            }
            GetSprite(sides);
        }
    }

    void GetSprite(bool[] atp) //sets sprite, takes in 8 bools starting top left going clockwise for if there is a mergable tile
    {
        string situationName = type + "_";
        string imgName = "";

        List<int> spritesToMerge = new List<int>();
        spritesToMerge.Add(12);
        //directly adjacent
        if (!atp[1]) { spritesToMerge.Add(7); } else { spritesToMerge.Add(2); }
        if (!atp[3]) { spritesToMerge.Add(13); } else { spritesToMerge.Add(14); }
        if (!atp[5]) { spritesToMerge.Add(17); } else { spritesToMerge.Add(22); }
        if (!atp[7]) { spritesToMerge.Add(11); } else { spritesToMerge.Add(10); }
        //corners
        if (atp[7] && atp[1] && !atp[0]) { spritesToMerge.Add(0); }
        if (atp[7] && !atp[1]) { spritesToMerge.Add(5); }
        if (!atp[7] && atp[1]) { spritesToMerge.Add(1); }
        if (!atp[7] && !atp[1]) { spritesToMerge.Add(6); }
        if (atp[1] && atp[3] && !atp[2]) { spritesToMerge.Add(4); }
        if (atp[1] && !atp[3]) { spritesToMerge.Add(3); }
        if (!atp[1] && atp[3]) { spritesToMerge.Add(9); }
        if (!atp[1] && !atp[3]) { spritesToMerge.Add(8); }
        if (atp[3] && atp[5] && !atp[4]) { spritesToMerge.Add(24); }
        if (atp[3] && !atp[5]) { spritesToMerge.Add(19); }
        if (!atp[3] && atp[5]) { spritesToMerge.Add(23); }
        if (!atp[3] && !atp[5]) { spritesToMerge.Add(18); }
        if (atp[5] && atp[7] && !atp[6]) { spritesToMerge.Add(20); }
        if (atp[5] && !atp[7]) { spritesToMerge.Add(21); }
        if (!atp[5] && atp[7]) { spritesToMerge.Add(15); }
        if (!atp[5] && !atp[7]) { spritesToMerge.Add(16); }
        //Full Corners
        if (atp[7] && atp[1] && atp[0]) { spritesToMerge.Add(25); }
        if (atp[1] && atp[3] && atp[2]) { spritesToMerge.Add(26); }
        if (atp[3] && atp[5] && atp[4]) { spritesToMerge.Add(27); }
        if (atp[5] && atp[7] && atp[6]) { spritesToMerge.Add(28); }

        foreach (int b in spritesToMerge) { situationName += b+","; imgName += b + "-"; }

        CompiledSprite fetched = spriteCache.FirstOrDefault(element => element.situation.Equals(situationName));
        if (fetched.sprite != null)
        {
            sr.sprite = fetched.sprite;
        }
        else if(Loader.Instance.FetchTexture(type,imgName) != null)
        {
            Texture2D tex = Loader.Instance.FetchTexture(type, imgName);
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), tex.width);
            spriteCache.Add(new CompiledSprite
            {
                situation = situationName,
                sprite = sr.sprite
            });
        }
        else
        {
            if (!merging)
            {
                StartCoroutine(MergeSprites(spritesToMerge, situationName,imgName));
            }
        }
    }

    //TODO it saves the old merge not the new one so spamming will leave you with out of date sprites
    IEnumerator MergeSprites(List<int> spriteIndexes,string situationName,string imgName)
    {
        merging = true;

        Texture2D tex = new Texture2D((int)sprites[0].rect.width, (int)sprites[0].rect.height,TextureFormat.ARGB32,false);
        tex.filterMode = FilterMode.Point;
        Color32[] pixels = new Color32[tex.width * tex.height];
        bool done = false;
        foreach (int i in spriteIndexes)
        {
            done = false;
            while (!done)
            {
                if (GameManager.Instance.ProcessHeavy(.5f))
                {
                    for (int x = 0; x < sprites[i].rect.width; x++)
                    {
                        for (int y = 0; y < sprites[i].rect.height; y++)
                        {
                            int xpos = flipX&&i==12 ? (int)sprites[i].rect.width - x - 1 + (int)sprites[i].rect.x : x + (int)sprites[i].rect.x;
                            int ypos = flipY&&i==12 ? (int)sprites[i].rect.height - y - 1 + (int)sprites[i].rect.y : y + (int)sprites[i].rect.y;

                            Color32 pixel = sprites[i].texture.GetPixel(xpos, ypos);
                            if (pixel.a != 0)
                            {
                                pixels[x + tex.width * y] = pixel;
                            }
                        }
                    }
                    done = true;
                }
                yield return null;
            }

        }
        done = false;
        while (!done)
        {
            if (GameManager.Instance.ProcessHeavy(1))
            {
                tex.SetPixels32(pixels);
                tex.Apply();
                done = true;
            }
            yield return null;
        }
        string path = "tile" + type + "/" + imgName + ".png";
        if (!File.Exists(path))
        {
            File.WriteAllBytes(path, tex.EncodeToPNG());
        }
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), tex.width);
        spriteCache.Add(new CompiledSprite
        {
            situation = situationName,
            sprite = sr.sprite
        });
        merging = false;

    }
}
