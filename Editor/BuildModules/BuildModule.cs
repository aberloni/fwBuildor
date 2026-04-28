using UnityEngine;

abstract public class BuildModule : ScriptableObject
{
    abstract public void Apply();

    virtual public string strOneLine()
    {
        return "module:" + name;
    }
}
