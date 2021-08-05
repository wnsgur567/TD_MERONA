using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{
    #region 내부 프로퍼티
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
