using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ����� ������ scroll view �� ũ�⿡ ���缭 �� �°� ��带 �����ϴ� Ŭ����
[ExecuteInEditMode]
public class ScrollViewController : MonoBehaviour
{
    RectTransform m_thisRectTransform;
    [SerializeField] bool m_panel_size_fitting; // cell ����� ���� fitting

    // ���� scroll view �����ϱ�
    [Space(10)]
    [SerializeField] ScrollRect m_scrollrect;       // scrollrect �� size �� ���缭 ��� ������ ����
    [SerializeField] GridLayoutGroup m_content;     // ����� �θ�

    [SerializeField] RectOffset m_padding;
    [SerializeField] Vector2Int m_cellCount;    // �ش� ��ũ�� �信 �Ҵ�� ��� ����
    [SerializeField] Vector2 m_cellsize;    // space �� padding ���� �ڵ� ������
    [SerializeField] Vector2 m_spacing;
    [SerializeField] TextAnchor m_childAlignment;
    [SerializeField] GridLayoutGroup.Constraint m_constraint;


    public void SetCellCount(int row, int col)
    {
        m_cellCount.x = row;
        m_cellCount.y = col;

        SetContentLayout();
    }

    private void Awake()
    {
        m_thisRectTransform = this.GetComponent<RectTransform>();
        
        SetContentLayout();
    }



    private void Update()
    {
#if UNITY_EDITOR
        SetContentLayout();
#endif
    }



    void SetContentLayout()
    {
        // ��� ����� �� ����
        float total_node_width = m_cellCount.x * m_cellsize.x;
        float total_node_height = m_cellCount.y * m_cellsize.y;

        // ���� �¿� ��� ������ �� ���� ���ϱ�
        // space �� ����� �������� �Ѱ� ����
        float total_space_width = (m_cellCount.x - 1) * m_spacing.x;
        float total_space_height = (m_cellCount.y - 1) * m_spacing.y;

        // panel ũ�� = ��� �� ���� + space �� ���� + �����¿� ����
        float total_width = total_node_width + total_space_width + m_padding.left + m_padding.right;
        float total_height = total_node_height + total_space_height + m_padding.top + m_padding.bottom;

        // �г� ũ�� ����
        if(m_panel_size_fitting)
        {
            m_thisRectTransform.sizeDelta = new Vector2(total_width, total_height);
            //Debug.LogFormat($"panel size delta : {total_width}, {total_height}");
        }



        // ���� ������Ʈ�� �⺻ ������ 1920 1080 ���� �ϰ�����
        // �ػ� ���� ������ �Ǵ� ������ ������ �ʿ���
        var _scale = m_scrollrect.GetComponent<RectTransform>().lossyScale;
        Scaling(_scale);

        // ���� ������Ʈ�� ����
        m_content.padding = m_padding;
        m_content.cellSize = m_cellsize;
        m_content.spacing = m_spacing;
        m_content.childAlignment = m_childAlignment;
        m_content.constraint = m_constraint;
        m_content.constraintCount = m_cellCount.x;
    }

    void Scaling(Vector3 scale)
    {
        foreach (Transform child in m_content.transform)
        {
            child.GetComponent<RectTransform>().localScale = Vector3.one;
        }

        //float scale_x = scale.x;
        //float scale_y = scale.y;

        //m_padding = new RectOffset((int)(m_padding.left * scale_x),
        //                            (int)(m_padding.right * scale_x),
        //                            (int)(m_padding.top * scale_y),
        //                            (int)(m_padding.bottom * scale_y));
        //m_cellsize = new Vector2(m_cellsize.x * scale_x, m_cellsize.y * scale_y);
        //m_spacing = new Vector2(m_spacing.x * scale_x, m_spacing.y * scale_y);
    }
}
