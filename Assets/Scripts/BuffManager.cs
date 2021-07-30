using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{

    #region 내부 프로퍼티
    protected ResourcesManager M_Resources
    {
        get
        {
            return ResourcesManager.Instance;
        }
    }
    #endregion

}
