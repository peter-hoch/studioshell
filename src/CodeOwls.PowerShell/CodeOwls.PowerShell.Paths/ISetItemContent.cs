using System.Management.Automation.Provider;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;

namespace CodeOwls.PowerShell.Provider.PathNodes
{
    public interface ISetItemContent
    {
        IContentWriter GetContentWriter(IContext context);
        object GetContentWriterDynamicParameters(IContext context);
    }
}