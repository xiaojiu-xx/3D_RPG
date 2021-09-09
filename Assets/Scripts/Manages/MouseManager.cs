using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;
//[System.Serializable]
//public class EventVector3 : UnityEvent<Vector3> { }

public class MouseManager : Singleton<MouseManager>
{
    public Texture2D point, doorway, attack, target, arrow;

    RaycastHit hitinfo;
    public event Action<Vector3> OnMouseClick;
    public event Action<GameObject> OnEnemyClick;
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }


    private void Update()
    {
        SetCursorTexture();
        if(!InteractWithUI())
            MouseControl();
    }

    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);

        if (Physics.Raycast(ray, out hitinfo))
        {
            // TOOD 切换鼠标贴图
            switch (hitinfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;

                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitinfo.collider != null)
        {
            if (hitinfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClick?.Invoke(hitinfo.point);
            if (hitinfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClick?.Invoke(hitinfo.collider.gameObject);
            if (hitinfo.collider.gameObject.CompareTag("AttackAble"))
                OnEnemyClick?.Invoke(hitinfo.collider.gameObject);
            if (hitinfo.collider.gameObject.CompareTag("Portal"))   // 传送门
                OnMouseClick?.Invoke(hitinfo.point);
            if (hitinfo.collider.gameObject.CompareTag("Item"))
                OnMouseClick?.Invoke(hitinfo.point);
        }
    }

    bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        else
            return false;
    }
}
