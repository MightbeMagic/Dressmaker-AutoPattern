using System;
using System.IO;

namespace Dressmaker_AutoPattern
{
    [BepInPlugin("ambrosia.dressmaker.autopattern", "Auto Pattern", "1.0.0")]
    [BepInDependency("yuu.dressmaker.mods")]
    public class Plugin
    {

        private ModContext mod;
        private void Awake()
        {
            mod = ModLoaderApi.Register(this, new ModInfo { Author = "AmbrosiaFae", Name = "Auto Pattern" });
            Folder = Path.GetDirectoryName(Info.Location);

            Logger.LogInfo("Auto Pattern Loaded.");
        }

    }
}
