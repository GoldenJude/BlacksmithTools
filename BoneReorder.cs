using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlacksmithTools
{
    [HarmonyPatch]
    public static class BoneReorder
    {
        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.AttachItem))]
        [HarmonyPostfix]
        static void AttachItemPatch(VisEquipment __instance, GameObject __result, int itemHash)
        {
            if (!Main.reorderEnabled.Value) return;

            if (__result == null) return;
            if (__result.name.StartsWith("attach_skin") && ObjectDB.instance.GetItemPrefab(itemHash) != null)
            {
                BoneReorder.SetSMRBones(__instance, __result, itemHash);
            }
        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.AttachArmor))]
        [HarmonyPostfix]
        static void AttachArmorPatch(VisEquipment __instance, List<GameObject> __result, int itemHash)
        {
            if (!Main.reorderEnabled.Value || __result == null) return;

            foreach (GameObject result in __result)
            {
                if (!result.name.StartsWith("attach_skin")) continue;
                BoneReorder.SetSMRBones(__instance, result, itemHash);
            }
        }

        public static void SetSMRBones(VisEquipment ve, GameObject instance, int hash)
        {
            Util.LogMessage("Reordering bones");

            try
            {
                SkinnedMeshRenderer origsmr = instance.GetComponentInChildren<SkinnedMeshRenderer>();
                SkinnedMeshRenderer[] smrs = instance.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                foreach (SkinnedMeshRenderer smr in smrs)
                {
                    SetBones(smr, GetBoneNames(origsmr), ve.m_bodyModel.rootBone);
                }
            }
            catch(Exception e)
            {
                Util.LogMessage(e.Message, BepInEx.Logging.LogLevel.Error);
            }
        }

        public static string[] GetBoneNames(SkinnedMeshRenderer smr)
        {
            List<string> boneNames = new List<string>();

            foreach (Transform bone in smr.bones)
            {
                boneNames.Add(bone.name);
            }

            return boneNames.ToArray();
        }

        public static void SetBones(SkinnedMeshRenderer smr, string[] boneNames, Transform skeletonRoot)
        {
            Transform[] bones = new Transform[smr.bones.Length];
            for (int j = 0; j < bones.Length; j++)
            {
                bones[j] = Util.FindInChildren(skeletonRoot, boneNames[j]);
            }

            smr.bones = bones;
            smr.rootBone = skeletonRoot;
        }
    }
}
