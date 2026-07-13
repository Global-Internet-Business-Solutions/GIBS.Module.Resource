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
            Version = "1.0.0",
            ServerManagerType = "GIBS.Module.Resource.Manager.ResourceManager, GIBS.Module.Resource.Server.Oqtane",
            ReleaseVersions = "1.0.0",
            Dependencies = "GIBS.Module.Resource.Shared.Oqtane",
            PackageName = "GIBS.Module.Resource" 
        };
    }
}
