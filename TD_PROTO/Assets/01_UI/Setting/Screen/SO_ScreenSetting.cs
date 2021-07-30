using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Resolution
{
    _1280x720, // HD
    _1920x1080, // FHD
    _2560x1440, // QHD
    _3840x2160, // 4K
    _5120x2880, // 5K
    _7680x4320, // 8K
};

[System.Serializable]
public struct ScreenSettingData
{
    // ��üȭ��
    public bool m_Windowed;
    // �ػ�
    public E_Resolution m_Resolution;
    // ���
    [Range(0.0f, 1.0f)]
    public float m_Brightness;
}


[System.Serializable, CreateAssetMenu(fileName = "New Screen", menuName = "Scriptable Object/Screen")]
public class SO_ScreenSetting : ScriptableObject
{
    public ScreenSettingData m_Data;
}
