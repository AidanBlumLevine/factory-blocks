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

    /*List<TileTexturePackage> tileTextures = new List<TileTexturePackage>();
    struct TileTexturePackage
    {
        public int tileType;
        public NamedTexture[] textures;
    }*/
    Dictionary<string, Sprite> tileSpriteList = new Dictionary<string, Sprite>();
    Dictionary<string, Sprite> spriteList = new Dictionary<string, Sprite>();
    /*struct NamedTexture
    {
        public Texture2D tex;
        public string name;
    }*/

    

    public Sprite PreviewLevel(Level l)
    {
        if (spriteList.ContainsKey(l.name + l.permanent))
        {
            return spriteList[l.name + l.permanent];
        }
        if (l.permanent)
        {
            Sprite s = Resources.Load<Sprite>("Levels/"+l.name);
            spriteList.Add(l.name + l.permanent, s);
            return s;
        }
        else if (File.Exists(Application.persistentDataPath + "/thumbnails/" + l.name + ".png"))
        {
            Texture2D tex = LoadPNG(Application.persistentDataPath + "/thumbnails/" + l.name + ".png");
            Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5f, .5f), tex.width);
            spriteList.Add(l.name + l.permanent, s);
            return s;
        }
        return null;
    }

    public Sprite TileSprite(int type, string imgName)
    {
        if (tileSpriteList.ContainsKey(type + imgName))
        {
            return tileSpriteList[type + imgName];
        }
        Sprite s = Resources.Load<Sprite>("Tiles/tile" + type + "/" + imgName);
        if (s == null)
        {
            print("no sprite found at " + "Tiles/tile" + type + "/" + imgName);
        }
        tileSpriteList.Add(type + imgName, s);
        return s;
    }

    public void ClearLevelPreview(string name)
    {
        if (spriteList.ContainsKey(name))
        {
            spriteList.Remove(name);
        }
    }

   /* public Texture2D FetchTexture(int type, string name)
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

    NamedTexture[] FetchTileTextures(int type)
    {
        NamedTexture[] textures = GetFetchedTextures(type);
        if(textures == null && Directory.Exists("tile" + type + "/"))
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
    }*/

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
