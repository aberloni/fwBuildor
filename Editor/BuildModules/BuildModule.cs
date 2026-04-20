using UnityEngine;

abstract public class BuildModule : ScriptableObject
{
    /// <summary>
    /// this module is applied when profil is applied
    /// </summary>
    virtual public bool isProfilModule() => true;

    abstract public void Apply();

    virtual public string strOneLine()
    {
        return "module:" + name;
    }
}
