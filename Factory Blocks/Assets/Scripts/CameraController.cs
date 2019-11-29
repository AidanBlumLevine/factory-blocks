using System.IO;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
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

    public int top = 100, bottom = 100, buffer = 20;
    Camera cam;
    Rect pixelPos;
    float screenSquareWidth, yShift;
    string screenshotPath = "";

    public void SetPosition(int levelSize)
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
            screenSquareWidth = Mathf.Min(Screen.width - buffer * 2, Screen.height - top - bottom - buffer * 2);
            yShift = (bottom + top) / 2.0f;
            pixelPos = new Rect(Screen.width / 2 - screenSquareWidth / 2, Screen.height / 2 - yShift - screenSquareWidth / 2, screenSquareWidth, screenSquareWidth);
        }
        //TODO fix
        for (int i = 0; i < 10; i++)
        {
            float pixelsPerUnit = cam.scaledPixelHeight / cam.orthographicSize;
            Vector3 centered = new Vector3(levelSize / 2.0f - .5f, (levelSize) / 2.0f - .5f, -10);
            cam.orthographicSize = ((levelSize / 2.0f * pixelsPerUnit) + Screen.height - screenSquareWidth) / pixelsPerUnit;
            pixelsPerUnit = cam.scaledPixelHeight / cam.orthographicSize;
            cam.transform.position = centered - new Vector3(0, yShift / pixelsPerUnit);
        }
    }

    public void Screenshot(string path)
    {
        screenshotPath = path;
    }

    public void OnPostRender()
    {
        if (!screenshotPath.Equals(""))
        {
            Vector2Int center = new Vector2Int(Screen.width / 2, Screen.height / 2);
            int width = (int)screenSquareWidth;
            int startX = center.x - width / 2;
            int startY = center.y - (int)(screenSquareWidth / 2 - yShift / 2);
            Texture2D tex = new Texture2D(width, width, TextureFormat.ARGB32, false);

            Rect rex = new Rect(startX, startY, width, width);
            tex.ReadPixels(rex, 0, 0);
            tex.Apply();

            byte[] bytes = tex.EncodeToPNG();
            Destroy(tex);

            Loader.Instance.ClearLevelPreview(TileManager.Instance.loadedLevel.name);
            File.WriteAllBytes(Application.persistentDataPath + "/thumbnails/" + screenshotPath + ".png", bytes);
            screenshotPath = "";
        }
    }
}
