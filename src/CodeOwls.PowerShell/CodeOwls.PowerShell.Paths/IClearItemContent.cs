using CodeOwls.PowerShell.Provider.PathNodeProcessors;

namespace CodeOwls.PowerShell.Provider.PathNodes
{
    public interface IClearItemContent
    {
        void ClearContent (IContext context);
        object GetClearContentDynamicParameters(IContext context);
    }
}