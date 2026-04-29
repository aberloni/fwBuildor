using UnityEngine;

abstract public class BuildModule : ScriptableObject
{
    virtual public bool askBeforeApply() => false;


    [ContextMenu("apply")]
    public void Apply()
    {
        if(askBeforeApply() && 
        !UnityEditor.EditorUtility.DisplayDialog("module", "apply module: " + GetType() + "." + name + " ?", "yes", "no"))
        {
            return;
        }

        doApply();
    }

    virtual protected void doApply()
    {
        
    }

    virtual public string strOneLine()
    {
        return "module:" + name;
    }
}
