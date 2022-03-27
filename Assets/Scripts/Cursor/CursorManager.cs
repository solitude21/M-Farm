using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MFarm.Map;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;

    private Sprite currentSprite;

    private Image cursorImage;

    private RectTransform cursorCanvas;
    
    private Camera mainCamera;

    private Grid currentGrid;

    private Vector3 mouseWorldPos;

    private Vector3Int mouseGridPos;

    /// <summary>
    /// 只有在选中物品时，cursorEnable才为true（切换场景时也是false）
    /// </summary>
    private bool cursorEnable = false; 

    private bool cursorPositionValid;

    private ItemDetails currentItem;

    private Transform playerTransform => FindObjectOfType<Player>().transform;

    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        currentSprite = normal;
        SetCursorImage(normal);

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (cursorCanvas == null) return;

        cursorImage.transform.position = Input.mousePosition;

        // ��ѡ����Ʒ �� ����UI����
        if (!InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid(); // ����ܷ���grid����
            CheckPlayerInput(); // ����Ƿ��°�ť
        }
        else
        {
            SetCursorImage(normal);
        }
    }

    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }


    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        //throw new System.NotImplementedException();
        cursorEnable = false;
    }

    private void OnAfterSceneLoadEvent()
    {
        //throw new System.NotImplementedException();
        currentGrid = FindObjectOfType<Grid>();
        //cursorEnable = true;
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {

        if (!isSelected)
        {
            currentItem = null;
            
            currentSprite = normal;

            cursorEnable = false;
        }
        else
        {
            currentItem = itemDetails;
            

            // TODO[epic=ItemType] 补充不同类型物品对应不同的cursor样式
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Commodity => item,
                _ => normal
            };

            cursorEnable = true;
        }
    }

    /// <summary>
    /// ֻ����cursorEnableΪtrueʱ�Ż���ã��ж�cursor�Ƿ����
    /// </summary>
    private void CheckCursorValid() 
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        //Debug.Log("WorldPos: " + mouseWorldPos + " GridPos: " + mouseGridPos);
        var playerGridPos = currentGrid.WorldToCell(playerTransform.position);

        // 如果鼠标超出作用范围，直接invalid
        if (Mathf.Abs(playerGridPos.x - mouseGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(playerGridPos.y - mouseGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInvalid();
            return;
        }

        // 获取当前鼠标所指tile
        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

        if (currentTile != null)
        {
            switch(currentItem.itemType)  
            {
                // TODO[epic=ItemType] 补充不同种类的物品对应不同的cursor表现
                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped) SetCursorValid(); else SetCursorInvalid();
                    break;
                case ItemType.Seed:
                    if (currentTile.canDropItem && currentItem.canDropped) SetCursorValid(); else SetCursorInvalid();
                    break;
                case ItemType.HoeTool:
                    if (currentTile.canDig) SetCursorValid(); else SetCursorInvalid();
                    break;
                default:
                    SetCursorInvalid();
                    break;
            }
        }
        else
        {
            SetCursorInvalid(); // !默认样式
        }
    }

    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }

    #region 设置鼠标UI, 以及鼠标是否可用
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    private void SetCursorInvalid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.5f);
    }
    #endregion



}
