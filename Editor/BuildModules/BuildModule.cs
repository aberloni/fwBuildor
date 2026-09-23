using UnityEngine;
using UnityEditor.Build.Reporting;
using fwp.buildor;
using fwp.buildor.editor;

/// <summary>
/// when a bodule is executed during build process
/// </summary>
public enum BodulePhase
{
    pre,  // before BuildPipeline.BuildPlayer
    post, // after a successful build
}

/// <summary>
/// what a bodule knows about the build it's part of
/// </summary>
public class BuildContext
{
    public DataBuildSettingProfile profile;
    public BodulePhase phase;

    /// <summary>
    /// null in pre, filled in post
    /// </summary>
    public BuildSummary? summary;

    /// <summary>
    /// platform being built
    /// post : from build summary
    /// pre/manual : active build target (what BuildPreprocess will build)
    /// </summary>
    public UnityEditor.BuildTarget Target => summary.HasValue ? summary.Value.platform : UnityEditor.EditorUserBuildSettings.activeBuildTarget;

    public BuildContext(DataBuildSettingProfile profile, BodulePhase phase, BuildSummary? summary = null)
    {
        this.profile = profile;
        this.phase = phase;
        this.summary = summary;
    }
}

abstract public class BuildModule : ScriptableObject
{
    virtual public bool askBeforeApply() => false;

    /// <summary>
    /// when this bodule is executed during build
    /// </summary>
    virtual public BodulePhase Phase => BodulePhase.pre;

    /// <summary>
    /// manual apply (context menu, buildor window)
    /// </summary>
    [ContextMenu("apply")]
    public void Apply() => Apply(new BuildContext(BuildorVars.Profile, Phase));

    public void Apply(BuildContext ctx)
    {
        if(askBeforeApply() &&
        !UnityEditor.EditorUtility.DisplayDialog("module", "apply module: " + GetType() + "." + name + " ?", "yes", "no"))
        {
            return;
        }

        doApply(ctx);
    }

    abstract protected void doApply(BuildContext ctx);

    virtual public string strOneLine()
    {
        return "module:" + name + " (" + Phase + ")";
    }

    public override string ToString() => strOneLine();
}
