using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlacksmithTools
{
    [HarmonyPatch]
    public static class BodypartSystem
    {
        public static Dictionary<string, List<bodyPart>> bodypartSettings = new Dictionary<string, List<bodyPart>>();
        public static Dictionary<string, List<int>> bodypartSettingsAsBones = new Dictionary<string, List<int>>();
        
        //to be implemented later
        //public static Dictionary<string, List<string>> bodyPartSettingsOverrride = new Dictionary<string, List<string>>();

        public enum bodyPart
        {
            Head,
            Torso,
            ArmUpperLeft,
            ArmLowerLeft,
            HandLeft,
            ArmUpperRight,
            ArmLowerRight,
            HandRight,
            LegUpperLeft,
            LegLowerLeft,
            FootLeft,
            LegUpperRight,
            LegLowerRight,
            FootRight,

            All,
            Beard,
            Hair
        }

        //attaching controller
        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.Awake))]
        [HarmonyPostfix]
        static void VisEqAwakePatch(VisEquipment __instance)
        {
            if (!Main.bodyHidingEnabled.Value) return;

            if (!__instance.m_isPlayer) return;

            if (__instance.m_bodyModel?.sharedMesh == null) return;

            if (!__instance.m_bodyModel.sharedMesh.isReadable) return;

            //attach controller
            BodyPartController ctrl = __instance.gameObject.AddComponent<BodyPartController>();
            ctrl.Setup(__instance);
        }

        public static void BindConfigs()
        {
            string[] files = Directory.GetFiles(Paths.ConfigPath);
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                if (fileName.StartsWith("bsmith."))
                {
                    //get item name from file name
                    string itemName = fileName.Remove(0, 7);
                    itemName = itemName.Remove(itemName.Length - 4, 4);

                    Util.LogMessage("Loaded configuration file for " + itemName, BepInEx.Logging.LogLevel.Message);

                    //create entry for dictionary if one doesnt already exist
                    if (bodypartSettingsAsBones.ContainsKey(itemName)) return;
                    bodypartSettingsAsBones.Add(itemName, new List<int>());

                    //load part name list
                    ConfigFile cfg = new ConfigFile(file, true);
                    ConfigEntry<string> partList = cfg.Bind("Body Parts", "List", "", "List of body parts to hide, delimited by a semilocor. List of valid values on mod page");

                    //convert bodypart list to bone index array
                    string[] splitPartNames = partList.Value.Split(';');
                    for (int i = 0; i < splitPartNames.Length; i++)
                    {
                        bodypartSettingsAsBones[itemName].AddRange(Util.BodyPartToBoneIndexes(splitPartNames[i]));
                    }

                    //parse bone index list to array
                    ConfigEntry<string> boneIndexList = cfg.Bind("Body Parts", "Bone List", "", "List of bone indexes, body model geometry weighted to these bones will be hidden, delimited by a semilocor. List of valid values on mod page");
                    string[] explodedBoneIndexCfg = boneIndexList.Value.Split(';');
                    int boneIndex;
                    for (int i = 0; i < explodedBoneIndexCfg.Length; i++)
                    {
                        if (int.TryParse(explodedBoneIndexCfg[i], out boneIndex)) bodypartSettingsAsBones[itemName].Add(boneIndex);
                    }

                    Util.LogMessage(bodypartSettingsAsBones[itemName].Count + " bones for " + itemName, BepInEx.Logging.LogLevel.Message);
                }
            }
            PartCfgToBoneindexes();
            CleanupCfgs();
        }

        //convert bodypartSettings dictionary to bone inedexes and add to bodypartSettingsAsBones 
        public static void PartCfgToBoneindexes()
        {
            foreach (string itemName in bodypartSettings.Keys)
            {
                if(!bodypartSettingsAsBones.ContainsKey(itemName))
                {
                    bodypartSettingsAsBones.Add(itemName, new List<int>());
                }
                bodypartSettingsAsBones[itemName].AddRange(Util.BodyPartToBoneIndexes(bodypartSettings[itemName].ToArray()));
            }
        }

        public static void CleanupCfgs()
        {
            foreach (KeyValuePair<string, List<bodyPart>> cfg in bodypartSettings)
            {
                bodypartSettingsAsBones[cfg.Key] = new List<int>(bodypartSettingsAsBones[cfg.Key].Distinct().ToArray());
            }
        }

        static void EquipmentChanged(VisEquipment viseq)
        {
            if (!Main.bodyHidingEnabled.Value) return;

            viseq.GetComponent<BodyPartController>()?.FullUpdate();
            Util.LogMessage("Equipment changed ", BepInEx.Logging.LogLevel.Message);
        }

        //uhhh body part updates
        #region vile but thats how valheim code is
        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetRightHandEquiped))]
        [HarmonyPostfix]
        static void SetRightHandPatch(VisEquipment __instance, bool __result)
        {
            if(__result) EquipmentChanged(__instance);
        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetLeftHandEquiped))]
        [HarmonyPostfix]
        static void SetLeftHandPatch(VisEquipment __instance, bool __result)
        {
            if (__result) EquipmentChanged(__instance);
        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetChestEquiped))]
        [HarmonyPostfix]
        static void SetChestPatch(VisEquipment __instance, bool __result, int hash)
        {
            if (__result) EquipmentChanged(__instance);
        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetLegEquiped))]
        [HarmonyPostfix]
        static void SetLegPatch(VisEquipment __instance, bool __result, int hash)
        {
            if (__result) EquipmentChanged(__instance);
        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetHelmetEquiped))]
        [HarmonyPostfix]
        static void SetHelmetPatch(VisEquipment __instance, bool __result)
        {
            if (__result) EquipmentChanged(__instance);
        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetShoulderEquiped))]
        [HarmonyPostfix]
        static void SetShoulderPtach(VisEquipment __instance, bool __result)
        {
            if (__result) EquipmentChanged(__instance);
        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetUtilityEquiped))]
        [HarmonyPostfix]
        static void SetUtilityPatch(VisEquipment __instance, bool __result)
        {
            if (__result) EquipmentChanged(__instance);
        }
        #endregion
    }
}