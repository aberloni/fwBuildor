using fwp.buildor;
using UnityEngine;

abstract public class ProfilParameter
{
    abstract public string GetUid();

    public BuildModule[] modules = new BuildModule[0];


    virtual public void applyProfil()
    { }

    /// <summary>
    /// apply all modules matching context phase
    /// </summary>
    public void ApplyModules(BuildContext ctx)
    {
        if (modules == null) return;

        foreach (var m in modules)
        {
            if (m == null || m.Phase != ctx.phase) continue;
            m.Apply(ctx);
        }
    }
}
