using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public static Loader Instance;
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

    List<TileTexturePackage> tileTextures = new List<TileTexturePackage>();
    struct TileTexturePackage
    {
        public int tileType;
        public NamedTexture[] textures;
    }
    struct NamedTexture
    {
        public Texture2D tex;
        public string name;
    }

    [Header("Colors")]
    public Color32 BackgroundColor, CrateColor, GoalColor, WallColor, SpikeColor;

    public Texture2D FetchTexture(int type, string name)
    {
        foreach (NamedTexture n in FetchTileTextures(type))
        {
            if (n.name.Equals(name))
            {
                return n.tex;
            }
        }
        return null;
    }

    public Sprite PreviewLevel(Level l)
    {
        int tileWidth = 20;
        int gap = 3;

        bool[,] fillMap = new bool[l.width, l.height];
        Level.block[,] blockTypes = new Level.block[l.width,l.height];
        Dictionary<int, Level.block> idList = new Dictionary<int, Level.block>();
        foreach (Level.block b in l.tiles)
        {
            blockTypes[b.pos.x, b.pos.y] = b;
            fillMap[b.pos.x, b.pos.y] = true;
            idList.Add(b.tileID, b);
        }

        Texture2D tex = new Texture2D(-gap + (gap + tileWidth) * l.width, -gap + (gap + tileWidth) * l.height, TextureFormat.ARGB32, false);
        Color32[] pixels = new Color32[tex.width * tex.height];

        for(int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                if (fillMap[x / (gap + tileWidth), y / (gap + tileWidth)])
                {
                    Level.block b = blockTypes[x / (gap + tileWidth), y / (gap + tileWidth)];
                    int type = b.type;
                    bool[] sides = new bool[4];
                    if (b.type == 1) {
                        if (b.pos.x > 0) { sides[0] = blockTypes[b.pos.x - 1, b.pos.y].type == 1; }
                        if (b.pos.y > 0) { sides[1] = blockTypes[b.pos.x, b.pos.y - 1].type == 1; }
                        if (b.pos.x < l.width - 1) { sides[2] = blockTypes[b.pos.x + 1, b.pos.y].type == 1; }
                        if (b.pos.y < l.height - 1) { sides[3] = blockTypes[b.pos.x, b.pos.y + 1].type == 1; }
                    }
                    else
                    {
                        foreach(int id in b.slavesIDs)
                        {
                            Level.block t = idList[id];
                            if (t.pos.x == b.pos.x - 1 && t.pos.y == b.pos.y)
                            {
                                sides[0] = true;
                            }
                            if (t.pos.x == b.pos.x && t.pos.y == b.pos.y - 1)
                            {
                                sides[1] = true;
                            }
                            if (t.pos.x == b.pos.x - 1 && t.pos.y == b.pos.y)
                            {
                                sides[2] = true;
                            }
                            if (t.pos.x == b.pos.x && t.pos.y == b.pos.y + 1)
                            {
                                sides[3] = true;
                            }
                        }
                    }

                    if (type == 0 || type == 3)
                    {
                        pixels[x + tex.width * y] = CrateColor;
                    }
                    else if (type == 1)
                    {
                        pixels[x + tex.width * y] = WallColor;
                    }
                    else if (type == 2)
                    {
                        pixels[x + tex.width * y] = GoalColor;
                    }
                    if (type == 3)
                    {
                        if (x % (gap + tileWidth) >= tileWidth / 3 && x % (gap + tileWidth) <= tileWidth - tileWidth / 3 &&
                            y % (gap + tileWidth) >= tileWidth / 3 && y % (gap + tileWidth) <= tileWidth - tileWidth / 3)
                        {
                            pixels[x + tex.width * y] = BackgroundColor;
                        }
                    }
                    if (type == 4)
                    {
                        pixels[x + tex.width * y] = SpikeColor;
                    }
                }
                else
                {
                    pixels[x + tex.width * y] = BackgroundColor;
                }
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), tex.width);
    }

    NamedTexture[] FetchTileTextures(int type)
    {
        NamedTexture[] textures = GetFetchedTextures(type);
        if(textures == null)
        {
            DirectoryInfo dir = new DirectoryInfo("tile" + type + "/");
            FileInfo[] info = dir.GetFiles("*.png");
            textures = new NamedTexture[info.Length];
            for (int i = 0; i < info.Length; i++)
            {
                textures[i] = new NamedTexture
                {
                    tex = LoadPNG("tile" + type + "/" + info[i].Name),
                    name = info[i].Name.Replace(".png", "")
                };
            }
            tileTextures.Add(new TileTexturePackage
            {
                tileType = type,
                textures = textures
            });
        }
        return textures;
    }

    NamedTexture[] GetFetchedTextures(int type)
    {
        foreach(TileTexturePackage t in tileTextures)
        {
            if(t.tileType == type)
            {
                return t.textures;
            }
        }
        return null;
    }

    public static Texture2D LoadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        tex.filterMode = FilterMode.Point;
        return tex;
    }
}
