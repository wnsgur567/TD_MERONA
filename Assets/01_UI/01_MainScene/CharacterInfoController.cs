using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfoController : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_nameTextpro;
    [SerializeField] TMPro.TextMeshProUGUI m_infoTextpro;

    public void Set(string name, string info)
    {
        m_nameTextpro.text = name;
        m_infoTextpro.text = info;
    }

}
