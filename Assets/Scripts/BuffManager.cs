using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{

    #region ���� ������Ƽ
    protected ResourcesManager M_Resources
    {
        get
        {
            return ResourcesManager.Instance;
        }
    }
    #endregion

}
