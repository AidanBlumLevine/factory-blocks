using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SpriteFormer : EditorWindow
{
    Texture2D tex;
    int type;
    Sprite[] sprites;

    [MenuItem("Window/Sprite Former")]
    public static void ShowWindow()
    {
        GetWindow<SpriteFormer>(false, "Sprite Former", true);
    }
    
    void OnGUI()
    {
        tex = (Texture2D) EditorGUILayout.ObjectField("Image", tex, typeof(Texture2D), false);
        type = EditorGUILayout.IntField("Type",type);

        if (tex != null && !Directory.Exists("/Assets/Resources/Tiles/tile" + type + "/"))
        {
            if (GUILayout.Button("Go"))
            {
                Directory.CreateDirectory("/Assets/Resources/Tiles/tile" + type + "/");
                sprites = Resources.LoadAll<Sprite>(tex.name);
                GetSprite(new bool[] { true, true, true, true, true, true, true, true });
            }
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

        foreach (int b in spritesToMerge)
        {
            situationName += b+",";
            imgName += b + "-";
        }

        MergeSprites(spritesToMerge, situationName,imgName);
    }

    void MergeSprites(List<int> spriteIndexes, string situationName, string imgName)
    {
        Texture2D tex = new Texture2D((int)sprites[0].rect.width, (int)sprites[0].rect.height,TextureFormat.ARGB32,false);
        tex.filterMode = FilterMode.Point;
        Color32[] pixels = new Color32[tex.width * tex.height];
        foreach (int i in spriteIndexes)
        {
            for (int x = 0; x < sprites[i].rect.width; x++)
            {
                for (int y = 0; y < sprites[i].rect.height; y++)
                {
                    int xpos = /*flipX&&i==12*/false ? (int)sprites[i].rect.width - x - 1 + (int)sprites[i].rect.x : x + (int)sprites[i].rect.x;
                    int ypos = /*flipY&&i==12*/false ? (int)sprites[i].rect.height - y - 1 + (int)sprites[i].rect.y : y + (int)sprites[i].rect.y;

                    Color32 pixel = sprites[i].texture.GetPixel(xpos, ypos);
                    if (pixel.a != 0)
                    {
                        pixels[x + tex.width * y] = pixel;
                    }
                }
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        string path = "/Assets/Resources/Tiles/tile" + type + "/" + imgName + ".png";
        if (Directory.Exists("/Assets/Resources/Tiles/tile" + type + "/") && !File.Exists(path))
        {
            File.WriteAllBytes(path, tex.EncodeToPNG());
        }
    }
}
