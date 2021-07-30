using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Grid layout 전용
[ExecuteInEditMode]
[RequireComponent(typeof(GridLayoutGroup))]
[RequireComponent(typeof(Image))]
public class CellSizeFitter : MonoBehaviour
{
    GridLayoutGroup m_layoutGroup = null;

    [SerializeField] RectOffset m_padding;
    [SerializeField] Vector2Int m_cellcount; 
    [SerializeField] Vector2 m_cellsize;    // space 와 padding 으로 자동 조절됨
    [SerializeField] Vector2 m_spacing;

    private void Awake()
    {
        m_layoutGroup = this.GetComponent<GridLayoutGroup>();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (m_cellcount.x < 1) 
            m_cellcount.x = 1;
        if (m_cellcount.y < 1)
            m_cellcount.y = 1;
        m_layoutGroup = this.GetComponent<GridLayoutGroup>();
        SetCellSize();
        UpdateGridLayoutGroup();
#endif
    }

    private void SetCellSize()
    {
        RectTransform panel_rt = this.GetComponent<RectTransform>();
        Vector2 panel_size = panel_rt.rect.size;
        Debug.Log(panel_size);

        float total_width_space = m_padding.left + m_padding.right + m_spacing.x * (m_cellcount.x - 1);
        float total_height_space = m_padding.top + m_padding.bottom + m_spacing.y * (m_cellcount.y - 1);

        m_cellsize.x = (panel_size.x - total_width_space) / m_cellcount.x;
        m_cellsize.y = (panel_size.y - total_height_space) / m_cellcount.y;
    }

    private void UpdateGridLayoutGroup()
    {
        m_layoutGroup.cellSize = m_cellsize;
        m_layoutGroup.spacing = m_spacing;
        m_layoutGroup.padding = m_padding;
    }
}
