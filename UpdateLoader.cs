using MelonLoader;
using Mono.Cecil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace SticksUItilityMod
{
    class UpdateLoader
    {
        public string version = "1.0.0";
        private string assemblyVersion;

        public void updater()
        {
            /*
             * Most of the updater code IS NOT MINE, mostly based off of Slaynash/VRCModUpdater on github.
             * Thanks Slaynash :)
             */

            string targetDir = Path.Combine(MelonHandler.ModsDirectory, "..", "UserData");
            string modfile = Path.Combine(MelonHandler.ModsDirectory, "..", "UserData", "SticksUItilityMod.dll");

            if (File.Exists(modfile))
            {
                System.Reflection.Assembly.LoadFile(modfile);
                return;
            }
            else
            {
                MelonLogger.Warning("Could not load the .dll for updating, this may be a bug!");
            }
            string latestVersion = GetLatestVersion();
            if (latestVersion == null)
            {
                MelonLogger.Error("Failed to fetch latest SticksUItilityMod version.");
                return;
            }
            string GetLatestVersion()
            {
                string gitRes;
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.github.com/repos/Thatcooldevguy/SticksUItilityMod/releases/latest");
                    request.Method = "GET";
                    request.KeepAlive = true;
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.UserAgent = $"SticksUItilityMod/{version}";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (StreamReader requestReader = new StreamReader(response.GetResponseStream()))
                    {
                        gitRes = requestReader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    MelonLogger.Error("Failed to fetch latest plugin version info from github... Unable to update. Stack: \n" + e);
                    return null;
                }

                JObject obj = JsonConvert.DeserializeObject(gitRes) as JObject;

                return obj.GetValue("tag_name")?.ToString();
            }
            void Update(string version)
            {
                MelonLogger.Msg("(Update Found!) Downloading SticksUItilityMod at version tag" + GetLatestVersion());

                byte[] data;
                using (WebClient wc = new WebClient())
                {
                    data = wc.DownloadData($"https://github.com/Thatcooldevguy/SticksUItilityMod/releases/download/{version}/VRCModUpdater.Core.dll");
                }

                File.WriteAllBytes(modfile, data);
                MelonLogger.Msg("Update Complete, please restart your VRC for updates to apply.");
            }

            void invoke()
            {
                MelonLogger.Msg("Latest SticksUItilityMod version: " + latestVersion);
                MelonLogger.Msg("Checking for updates...");
                if (File.Exists(modfile))
                {
                    try
                    {
                        using (AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(modfile, new ReaderParameters { ReadWrite = true }))
                        {
                            CustomAttribute melonInfoAttribute = assembly.CustomAttributes.First(a => a.AttributeType.Name == "AssemblyFileVersionAttribute");
                            assemblyVersion = melonInfoAttribute.ConstructorArguments[0].Value as string;
                        }
                        MelonLogger.Msg("Installed SticksUItilityMod version: " + assemblyVersion);
                    }
                    catch (Exception e)
                    {
                        MelonLogger.Error("Failed to load SticksUItilityMod. Redownloading.\n" + e);
                    }
                }

                if (assemblyVersion != latestVersion)
                    Update(latestVersion);
            }

        }
    }
}




