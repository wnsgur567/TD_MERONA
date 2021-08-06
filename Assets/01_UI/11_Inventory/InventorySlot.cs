using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[System.Serializable]
public struct InventorySlotInfo
{
    public bool isOccupied;
    public int index;
  
    public Tower tower;
}

public class InventorySlot : MonoBehaviour
{
    public delegate void InfoChangedHandler();
    public event InfoChangedHandler OnInfoChangedEvent;
    public delegate void PositionChangedHandler(Vector3 pos);
    public event PositionChangedHandler OnPositionChangedEvent;

    [SerializeField] InventorySlotInfo m_info;
    public bool IsOccupied { get { return m_info.isOccupied; } }

    private void Start()
    {
        OnInfoChangedEvent += OnInfoChanged;
    }

    public void __Indexing(int index)
    {
        m_info.index = index;
    }

    public void UpdateTowerPosOnThisSlot()
    {
        // TODO : this.transform.position �� �������� �����ϱ� ...
    }

    public void SetTowerPos(Vector3 mousePos)
    {
        // TODO : mouse -> world ��ȯ �� ����
    }

    public void SetTower(Tower tower)
    {
        if (tower == null)
            m_info.isOccupied = false;
        else
            m_info.isOccupied = true;

        m_info.tower = tower;

        OnInfoChangedEvent?.Invoke();
    }
   
    public Tower GetTower()
    {        
        return m_info.tower;
    }
    
    void OnInfoChanged()
    {
        if(IsOccupied)
        {   // ���� ���Կ� Ÿ���� ������...

        }
        else
        {   // ���� ���Կ� Ÿ���� ������...

        }        
    }
}
