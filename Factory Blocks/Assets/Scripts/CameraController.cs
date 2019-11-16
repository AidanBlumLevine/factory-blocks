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

    public float scale = 1.0f;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }
    public void SetPosition(int levelSize)
    {
        cam.transform.position = new Vector3(levelSize / 2.0f, levelSize / 2.0f, -10);
        cam.orthographicSize = scale * levelSize;
    }
}
