using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Root Panel �� origin slot�� �������� ����
// Root Panel ���� CellSizeFitter �� �����ؾ���
public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] Tower_TableExcelLoader m_excel_towerdata_so;
    List<Tower_TableExcel> m_tower_data_list;
    [SerializeField] Shop_TableExcelLoader m_excel_shopdata_so;
    private Shop_TableExcel m_cur_data;         // ���� �ֱٿ� UserInfoManger �κ��� �ҷ��� ������ ��
    private List<float> m_rates = null;         // ���� data ���� Ȯ���� �̾Ƴ� ����Ʈ
    private List<int> m_costs = null;           // ���� data ���� cost�� �̾Ƴ� ����Ʈ

    // SetActive On Off ��
    [SerializeField] Image m_shop_panel;
    // root panel �� ������ slot �� ����
    [SerializeField] Image m_root_panel;
    // root panel �� size fitter
    // fitter ���� ������ slot ������ŭ ����
    private CellSizeFitter m_fitter;

    // ���� �ϴ� ������ Ȯ���� ����
    [SerializeField] ShopStatusUIController m_status_controller;

    [SerializeField] ShopSlot m_origin;
    [SerializeField] List<ShopSlot> m_slot_list;

    private void Awake()
    {
        __Initialize();
    }

    private void Start()
    {
        // ���� Status ����        
        OnLevelChanged(UserInfoManager.Instance.Level);
        // ���� ���� ����
        ShopReset();
        // lock button release
        ShopUnLock();
    }

    public void __Initialize()
    {
        // this memeber alloc
        m_cur_data = new Shop_TableExcel();
        m_rates = new List<float>();
        m_costs = new List<int>();

        // setting
        m_origin.gameObject.SetActive(false);

        m_fitter = m_root_panel.GetComponent<CellSizeFitter>();
        int total_alloc_count = m_fitter.CellCount.x * m_fitter.CellCount.y;

        // new slots Instantiate
        for (int i = 0; i < total_alloc_count; i++)
        {
            ShopSlot newSlot = GameObject.Instantiate<ShopSlot>(m_origin);
            m_slot_list.Add(newSlot);
            newSlot.__Indexing(i);
            newSlot.transform.SetParent(m_root_panel.transform);
            newSlot.gameObject.SetActive(true);
            newSlot.MoveRenderPosition(new Vector3(100 * i, 100 * i, 0));   // ������ ��ġ�� �޶�������
        }

        // sub 3 item form front ( except devil )
        m_tower_data_list = m_excel_towerdata_so.DataList.GetRange(
            3,
            m_excel_towerdata_so.DataList.Count - 3);

        // link callback
        UserInfoManager.Instance.OnLevelChanged += OnLevelChanged;
    }


    /*************************** UI Control ******************************/

    // ���� ����� ���� �ϴ� �г��� text ����
    public void OnLevelChanged(int level)
    {
        // user info �� �������� ������ ���� Ȯ���� ������
        int cur_user_level = UserInfoManager.Instance.Level;

        var data_list = m_excel_shopdata_so.DataList;

        m_cur_data = data_list.Find((item) => { return cur_user_level == item.User_Level; });

        
        m_rates.Clear();      
        m_rates.Add(m_cur_data.Tower_Rand1);
        m_rates.Add(m_cur_data.Tower_Rand2);
        m_rates.Add(m_cur_data.Tower_Rand3);
        m_rates.Add(m_cur_data.Tower_Rand4);
        m_rates.Add(m_cur_data.Tower_Rand5);

        m_costs.Clear();
        m_costs.Add(m_cur_data.Tower_Gold1);
        m_costs.Add(m_cur_data.Tower_Gold2);
        m_costs.Add(m_cur_data.Tower_Gold3);
        m_costs.Add(m_cur_data.Tower_Gold4);
        m_costs.Add(m_cur_data.Tower_Gold5);

        m_status_controller.SetRates(m_rates.ToArray());
        m_status_controller.SetLevel(cur_user_level);
    }

    /*************************** Mouse callback ******************************/
    // TODO : Ÿ�� �Ŵ����� ���� Ÿ�� ����
    // TODO : Inventory �� �߰�
    /*************************** button Process ******************************/

    [Space(30)]
    [SerializeField] bool m_isLocked = false;
    [SerializeField] Image m_LockButtonImage;
    [SerializeField] Sprite m_LockSprite;
    [SerializeField] Sprite m_UnLockSprite;

    private void ShopReset()
    {
        // rank ���� Ȯ��
        float rand_val = 0.0f;

        // tower data ����
        int total_towerType_count = m_tower_data_list.Count; 
        // tower type ���� Ȯ��        
        int rand_val_tower = 0;

        // slot ������ŭ �ݺ�
        for (int i = 0; i < m_slot_list.Count; i++)
        {
            rand_val = Random.Range(0.0f, 1.0f);
            rand_val_tower = Random.Range(0, total_towerType_count);    // index ����(No - 1)

            int rank_forCreate = 1;
            int cost = 0;
            float acc = 0.0f;
            // ���� ��ũ�� Ȯ������ ���
            for (int j = 0; j < m_rates.Count; j++)
            {
                acc += m_rates[j];
                if (rand_val <= acc)
                {
                    rank_forCreate = j + 1;
                    cost = m_costs[j];
                    break;
                }
            }

            Debug.Log($"rank : {rank_forCreate}");

            // rank �� �����ϴ� ������ �̱�
            var tower_data_list = m_tower_data_list.FindAll((item) => { return item.Rank == rank_forCreate; });

            // tower_data_list �߿� �ϳ�
            int selected_index = Random.Range(0, tower_data_list.Count);

            // ������ ������ �����ͷ� Shop Slot�� ���� ������Ʈ            
            var tower_data = tower_data_list[selected_index]; 
            m_slot_list[i].SetInfo(cost, tower_data);
        }
    }


    private void ShopLock()
    {
        m_LockButtonImage.sprite = m_LockSprite;
    }
    private void ShopUnLock()
    {
        m_LockButtonImage.sprite = m_UnLockSprite;
    }

    public bool PurchaseProcess(int slotIndex)
    {
        ShopSlot slot = m_slot_list[slotIndex];

        // TODO : UserInfoManager.Instance.Gold ���� price �� ������            
        if (false == InventoryManager.Instance.IsAllOccupied())
        {   // �κ��� �� ������ ������
            var info = slot.GetInfo();
            InventoryManager.Instance.AddNewTower(info.excel_data.Value);

            Debug.Log("Puchase!!");
            return true;
        }

        return false;
    }

    /*************************** button callback ******************************/

    public void __OnResetButtonClicked()
    {
        Debug.Log("ShopReset");

        if (false == m_isLocked)
        {
            ShopReset();
        }
    }

    // Lock �� ��� true
    // UnLock �� ��� false
    public void __OnLockButtonClicked()
    {
        m_isLocked = !m_isLocked;
        if (m_isLocked)
        {
            ShopLock();
            Debug.Log("ShopLock");
        }
        else
        {
            ShopUnLock();
            Debug.Log("ShopUnLock");
        }
    }
    public void __OnOpenButtonClicked()
    {
        m_shop_panel.gameObject.SetActive(true);
    }
    public void __OnCloseButtonClicked()
    {
        m_shop_panel.gameObject.SetActive(false);
    }
}
