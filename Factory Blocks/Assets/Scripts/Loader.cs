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
