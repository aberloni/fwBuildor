using UnityEngine;

abstract public class BuildorScriptable : ScriptableObject
{
    void OnValidate()
    {
        onValidate();
    }

    virtual protected void onValidate()
    {

    }
}
