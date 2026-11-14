using BepInEx;
using DressmakerMods;
using Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Dressmaker_AutoPattern
{
    [BepInPlugin("ambrosia.dressmaker.autopattern", "Auto Pattern", "0.1.0")]
    [BepInDependency("yuu.dressmaker.mods")]
    public class Plugin : BaseUnityPlugin
    {
        private const string PanelInventoryName = "PanelInventory";
        private ModContext mod;

        public string Folder;

        private string movePatternsId = "ambrosia.movePatterns";
        private string alignPatternsId = "ambrosia.alignPatterns";

        public Dress DressPanelInventory;
        public GameObject SideBarPanelInventory;

        public readonly int[] rotateAngle = [0, 90, 180, 270];
        public int rotateIndex = 0;

        private void Awake()
        {
            mod = ModLoaderApi.Register(this, new ModInfo { Author = "AmbrosiaFae", Name = "Auto Pattern" });
            Folder = Path.GetDirectoryName(Info.Location);

            mod.Track(ModKeybindApi.Register(mod.Id, movePatternsId, "Move Patterns to table", KeyCode.Alpha7));
            mod.Track(ModKeybindApi.Register(mod.Id, alignPatternsId, "Align patterns", KeyCode.Alpha8));

            ModLifecycleApi.Subscribe(mod, ModLifecycleEvent.SaveLoaded, x => SetupAutoPattern());

        }

        private void Update()
        {
            if (ModKeybindApi.WasPressed(mod.Id + "." + movePatternsId))
            {
                PlacePatternsOnTable();
            }
            if (ModKeybindApi.WasPressed(mod.Id + "." + alignPatternsId))
            {
                RotatePatternsStraight();
            }
        }

        public void PlacePatternsOnTable()
        {
            var panels = FindUncutPatternsInInventory();

            foreach (var item in panels)
            {
                SideBarPanelInventory.GetChildObjects().FirstOrDefault(x => x.name.Contains(item.name)).SetActive(false);
                var world = item.gameObject.GetChildObjects().First(x => x.name == "worldPattern");
                world.transform.position = new Vector3(1f, 0f, -0.075f);
                world.SetActive(true);
            }

        }

        public void RotatePatternsStraight()
        {
            var activePanels = FindPatternsOnTable();

            foreach (var item in activePanels)
            {
                Logger.LogInfo($"setting {item.name}'s angle to {rotateAngle[rotateIndex]}");

                //item.transform.rotation = Quaternion.an

                item.transform.eulerAngles = new Vector3(0, 0 , rotateAngle[rotateIndex]); //.z = 90; // SetLookRotation(new Vector3(0, 0, 90));
            }
            rotateIndex++;
            if (rotateIndex >= rotateAngle.Length) rotateIndex = 0;
        }

        public List<PatternPanel> FindUncutPatternsInInventory()
        {
            var children = SideBarPanelInventory.GetChildObjects().Where(x => x.activeSelf);
            var activePanelNames = children.Select(x => x.name.Replace(" inventory button", "")).ToList();
            var panels = DressPanelInventory.gameObject.GetChildObjects().Where(x => activePanelNames.Contains(x.name)).Select(x => x.GetComponent<PatternPanel>()).ToList();
            foreach (var item in panels)
            {
                Logger.LogInfo($"{item.name} {item.HasBeenCut}");
            }
            return panels.Where(x => !x?.state?.hasBeenCut ?? false).ToList();
        }

        public List<GameObject> FindPatternsOnTable()
        {
            var allPanels = DressPanelInventory.gameObject.GetChildObjects().ToList();

            Logger.LogInfo("allPanels found: " + allPanels.Count);
            Logger.LogInfo("allpanels " + allPanels.Where(x => x.name == "worldPattern").Count());
            List<GameObject> worldObjects = new List<GameObject>();
            foreach (var item in allPanels.Where(x => x.name == "worldPattern"))
            {
                if (item.activeSelf) worldObjects.Add(item);
            }

            Logger.LogInfo("Patterns found: " + worldObjects.Count);
            return worldObjects;
        }

        public void SetupAutoPattern()
        {
            DressPanelInventory = Resources.FindObjectsOfTypeAll<Dress>().FirstOrDefault();
            SideBarPanelInventory = Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.name == PanelInventoryName).FirstOrDefault();
            if (DressPanelInventory == null || SideBarPanelInventory == null)
            {
                Logger.LogInfo("Auto pattern set up failed.");
            }
            else
            {
                Logger.LogInfo("Auto Pattern Loaded.");
            }

        }
    }
}
