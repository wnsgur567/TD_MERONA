using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ScreenSetting
{
    public ScreenSettingData m_Data;

    public ScreenSetting(ScreenSettingData data)
    {
        m_Data = data;
    }
}
[System.Serializable]
public struct AudioSettings
{
    public List<AudioSettingData> m_Data;

    public AudioSettings(List<AudioSettingData> data)
    {
        m_Data = data;
    }
}
[System.Serializable]
public struct KeySettings
{
    public List<KeySettingData> m_Data;

    public KeySettings(List<KeySettingData> data)
    {
        m_Data = data;
    }
}

[System.Serializable]
public struct SettingArgs
{
    public ScreenSetting screen_data;
    public AudioSettings audio_data;
    public KeySettings key_data;
}

public class SettingManager : Singleton<SettingManager>
{
    // ���� ���� �����쿡�� Ȯ�� ��ư�� ���� ��쿡 ���ð��� �����
    // ���� ��(m file setting)�� ������ ���� ��� ȣ��˴ϴ�
    // current setting �� args �� ��ü�� �ѱ�
    public delegate void OnSettingChangeHandler(SettingArgs args);
    public event OnSettingChangeHandler OnSettingChnagedEvent;

    [Space(10)]
    [SerializeField] string m_screen_directoryPath;      // ���� ������ ����� directory ���
    [SerializeField] string m_audio_directoryPath;       // ���� ������ ����� directory ���
    [SerializeField] string m_key_directoryPath;         // ���� ������ ����� directory ���

    [Space(10)]
    [SerializeField] string m_screen_filename;      // ex ) ScreenSetting.json
    [SerializeField] string m_audio_filename;       // ex ) ScreenSetting.json
    [SerializeField] string m_key_filename;         // ex ) ScreenSetting.json

    [Space(10)]
    [SerializeField] SettingArgs m_Initial_Setting;  // �ʱ� ����
    [SerializeField] SettingArgs m_File_Setting;     // ���� ����
    [SerializeField] SettingArgs m_Current_Setting;  // ���� ����

    Canvas m_Canvas = null;
    CanvasScaler m_CanvasScaler = null;

    public SettingArgs GetCurrentSetting()
    {
        return m_Current_Setting;
    }

    public void Awake()
    {
        Initialize();
    }

    public void OnApplicationQuit()
    {
        SaveFileSetting();
    }
    public void Initialize()
    {
        // new
        AllocData();

        // ������ �о� ���� ����
        LoadInitSetting();
        LoadFileSetting();
        // ���� ���� ���� �����մϴ�
        SetCurrentSetting();

        InitCanvas();
    }

    // ��������� �Ҵ�
    private void AllocData()
    {
        m_Initial_Setting = new SettingArgs();
        m_File_Setting = new SettingArgs();
        m_Current_Setting = new SettingArgs();       
    }

    private void InitCanvas()
    {
        if (m_Canvas == null)
            m_Canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
        if (m_CanvasScaler == null)
            m_CanvasScaler = m_Canvas.GetComponent<CanvasScaler>();
    }

    // scriptable object ���� ������ ��� �о�
    // �� setting class �� ������ ȣ�� �� init setting �� �ʱ�ȭ   
    private void LoadInitSetting()
    {
        // Load ScriptableObject( Screen_Setting )
        SO_ScreenSetting Temp_Screen = Resources.Load<SO_ScreenSetting>("Screen");
        ScreenSettingData Initial_Screen = Temp_Screen.m_Data;

        // Load ScriptableObject( Audio_Setting )        
        SO_AudioSetting[] Temp_Audio = Resources.LoadAll<SO_AudioSetting>("");
        List<AudioSettingData> Initial_Audio = new List<AudioSettingData>();
        for (int i = 0; i < Temp_Audio.Length; ++i)
        {
            Initial_Audio.Add(Temp_Audio[i].m_Data);
        }
        Initial_Audio.Sort((AudioSettingData x, AudioSettingData y) => { return x.CompareTo(y); });

        // Load ScriptableObject( Key_Setting )
        SO_KeySetting[] Temp_Key = Resources.LoadAll<SO_KeySetting>("");
        List<KeySettingData> Initial_Key = new List<KeySettingData>();
        for (int i = 0; i < Temp_Key.Length; ++i)
        {
            Initial_Key.Add(Temp_Key[i].m_Data);
        }
        Initial_Key.Sort((KeySettingData x, KeySettingData y) => { return x.CompareTo(y); });

        // load �� ������ ��������� ����
        m_Initial_Setting.screen_data = new ScreenSetting(Initial_Screen);
        m_Initial_Setting.audio_data = new AudioSettings(Initial_Audio);
        m_Initial_Setting.key_data = new KeySettings(Initial_Key);
    }

    // JSON ������ ��� �о�
    // ������ ���ſ� ������ ������ ����
    private void LoadFileSetting()
    {
        // load from json
        ScreenSetting? loaded_screensetting_data = JsonUtilityEx.LoadFromJson<ScreenSetting>(m_screen_directoryPath, m_screen_filename);
        AudioSettings? loaded_audiosetting_data = JsonUtilityEx.LoadFromJson<AudioSettings>(m_audio_directoryPath, m_audio_filename);
        KeySettings? loaded_keysetting_data = JsonUtilityEx.LoadFromJson<KeySettings>(m_key_directoryPath, m_key_filename);

        // �׻� �����ؾ� �Ѵٰ� �����ϰ� �ۼ���
        if (null == loaded_screensetting_data || null == loaded_audiosetting_data || null == loaded_keysetting_data)
        {
            Debug.LogError("ScreenSetting load file ����");
        }

        // �ҷ��� �� ��� ������ ����
        m_File_Setting.screen_data = loaded_screensetting_data.Value;
        m_File_Setting.audio_data = loaded_audiosetting_data.Value;
        m_File_Setting.key_data = loaded_keysetting_data.Value;
    }

    private void SetCurrentSetting()
    {
        // file setting �� �׻� �ҷ��� ���ٰ� �����ϰ� �ۼ���
        // file setting �״�� �����ϱ�
        m_Current_Setting = m_File_Setting;
    }

    // ���� �� ���� �Ź� ������Ʈ ���ٰ�
    private void SetFileSetting()
    {
        // current setting �״�� �����ϱ�
        m_File_Setting = m_Current_Setting;
    }

    // file setting ���� �����Ѵ�
    private void SaveFileSetting()
    {
        // �����͸� �ֽ�ȭ �ϰ�
        SetFileSetting();

        // Json ������ ����
        JsonUtilityEx.SaveToJson<ScreenSetting>(m_File_Setting.screen_data, m_screen_directoryPath, m_screen_filename);
        JsonUtilityEx.SaveToJson<AudioSettings>(m_File_Setting.audio_data, m_audio_directoryPath, m_audio_filename);
        JsonUtilityEx.SaveToJson<KeySettings>(m_File_Setting.key_data, m_key_directoryPath, m_key_filename);
    }

#if UNITY_EDITOR
    private void Update()
    {        
        OnSettingChnagedEvent?.Invoke(m_Current_Setting);
    }
#endif
}
