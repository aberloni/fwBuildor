using fwp.buildor;
using UnityEngine;

abstract public class ProfilParameter
{
    abstract public string GetUid();

    public BuildModule[] modules = new BuildModule[0];

    readonly GUIContent gui_btn_browse = new GUIContent("browse");

    virtual public void applyProfil()
    { }

    public void ApplyModules()
    {
        if (modules == null) return;

        foreach (var m in modules)
        {
            m?.Apply();
        }
    }
}
