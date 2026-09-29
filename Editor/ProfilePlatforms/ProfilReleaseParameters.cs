namespace fwp.buildor.editor
{
    /// <summary>
    /// release level only (debug level : ProfilDebugParameters)
    /// </summary>
    [System.Serializable]
    public class ProfilReleaseParameters : ProfilParameter
    {
        public override string GetUid() => "release";
    }
}
