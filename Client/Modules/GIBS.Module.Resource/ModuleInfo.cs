using Oqtane.Models;
using Oqtane.Modules;

namespace GIBS.Module.Resource
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Resource",
            Description = "Resource Manager",
            Version = "1.0.3",
            ServerManagerType = "GIBS.Module.Resource.Manager.ResourceManager, GIBS.Module.Resource.Server.Oqtane",
            ReleaseVersions = "1.0.0,1.0.1,1.0.2,1.0.3",
            Dependencies = "GIBS.Module.Resource.Shared.Oqtane",
            PackageName = "GIBS.Module.Resource" 
        };
    }
}
