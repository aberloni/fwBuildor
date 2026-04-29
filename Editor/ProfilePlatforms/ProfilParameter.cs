using UnityEngine;

public class ProfilParameter
{
    public BuildModule[] modules = new BuildModule[0];

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
