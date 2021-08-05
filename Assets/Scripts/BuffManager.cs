using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{
    #region ���� ������Ƽ
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    protected BuffData BuffData
    {
        get
        {
            return M_Resources.GetScriptableObject<BuffData>("Synergy", "BuffData");
        }
    }
    #endregion

    public S_BuffData_Excel GetData(int code)
    {
        return BuffData.GetData(code);
    }
}
