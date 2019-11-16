using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
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

    TileManager tm;

    void Start()
    {
        tm = TileManager.Instance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            tm.Slide(new Vector2(-1,0));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            tm.Slide(new Vector2(0, 1));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            tm.Slide(new Vector2(1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            tm.Slide(new Vector2(0, -1));
        }
    }
}
