using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    Vector2 startPos;
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
            tm.Slide(new Vector2Int(-1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            tm.Slide(new Vector2Int(0, 1));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            tm.Slide(new Vector2Int(1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            tm.Slide(new Vector2Int(0, -1));
        }
        if(Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            startPos = Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            if (Vector3.Distance(startPos, (Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition)) > 30)
            {
                DragDirection((Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition) - startPos);
            }
        }
    }

    private void DragDirection(Vector3 dragVector)
    {
        float positiveX = Mathf.Abs(dragVector.x);
        float positiveY = Mathf.Abs(dragVector.y);
        if (positiveX > positiveY)
        {
            if (dragVector.x > 0) {
                tm.Slide(new Vector2Int(1, 0));
            } else {
                tm.Slide(new Vector2Int(-1, 0));
            }
        }
        else
        {
            if (dragVector.y > 0)
            {
                tm.Slide(new Vector2Int(0, 1));
            }
            else
            {
                tm.Slide(new Vector2Int(0, -1));
            }
        }
    }
}
