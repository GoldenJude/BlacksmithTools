using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlacksmithTools
{
    public static class Util
    {
        public static Transform FindInChildrenWc(Transform trans, string name)
        {
            if (trans.name.ToLower().Contains(name.ToLower()))
                return trans;
            else
            {
                Transform found;

                for (int i = 0; i < trans.childCount; i++)
                {
                    found = FindInChildrenWc(trans.GetChild(i), name);
                    if (found != null)
                        return found;
                }

                return null;
            }
        }

        public static Transform FindInChildren(Transform trans, string name)
        {
            if (trans.name == name)
                return trans;
            else
            {
                Transform found;

                for (int i = 0; i < trans.childCount; i++)
                {
                    found = FindInChildren(trans.GetChild(i), name);
                    if (found != null)
                        return found;
                }

                return null;
            }
        }

        public static List<BodypartSystem.bodyPart> StringToParts(string partstring)
        {
            List<BodypartSystem.bodyPart> parts = new List<BodypartSystem.bodyPart>();
            //Main.log.LogWarning("BSMITH STRINGTOPARTS " + partstring);
            foreach (string part in partstring.Split(';'))
            {
                BodypartSystem.bodyPart bPart;
                if (Enum.TryParse<BodypartSystem.bodyPart>(part, out bPart))
                {
                    //Main.log.LogWarning("BSMITH STRINGTOPARTS " + bPart.ToString());
                    if (!parts.Contains(bPart)) parts.Add(bPart);
                }
            }
            return parts;
        }

        public static string CorrectRRRArmorPrefabName(string name)
        {
            //RRRN_FriendlyMelee_Male_0@conqchest@@@
            string[] split = name.Split('@');
            if(split.Length > 1)
            {
                return split[1];
            }
            return name;
        }

        public static string[] GetEquippedItemNames(VisEquipment ve, ObjectDB db)
        {
            if(ve == null || db == null)
            {
                return new string[] { };
            }

            List<string> equippedNames = new List<string>();

            /*if (ve.m_currentLeftItemHash != 0) equippedNames.Add(db.GetItemPrefab(ve.m_currentLeftItemHash)?.name);
            if (ve.m_currentRightItemHash != 0) equippedNames.Add(db.GetItemPrefab(ve.m_currentRightItemHash)?.name);
            if (ve.m_currentChestItemHash != 0) equippedNames.Add(db.GetItemPrefab(ve.m_currentChestItemHash)?.name);
            if (ve.m_currentLegItemHash != 0) equippedNames.Add(db.GetItemPrefab(ve.m_currentLegItemHash)?.name);
            if (ve.m_currentHelmetItemHash != 0) equippedNames.Add(db.GetItemPrefab(ve.m_currentHelmetItemHash)?.name);
            if (ve.m_currentShoulderItemHash != 0) equippedNames.Add(db.GetItemPrefab(ve.m_currentShoulderItemHash)?.name);
            if (ve.m_currentBeardItemHash != 0) equippedNames.Add(db.GetItemPrefab(ve.m_currentBeardItemHash)?.name);
            if (ve.m_currentHairItemHash != 0) equippedNames.Add(db.GetItemPrefab(ve.m_currentHairItemHash)?.name);
            if (ve.m_currentUtilityItemHash != 0) equippedNames.Add(db.GetItemPrefab(ve.m_currentUtilityItemHash)?.name);
            if (ve.m_currentLeftBackItemHash != 0) equippedNames.Add(db.GetItemPrefab(ve.m_currentLeftBackItemHash)?.name);
            if (ve.m_currentRightBackItemHash != 0) equippedNames.Add(db.GetItemPrefab(ve.m_currentRightBackItemHash)?.name);*/

            foreach (int hash in GetEquippedHashes(ve))
            {
                GameObject prefab = db.GetItemPrefab(hash);
                if (prefab == null) continue;
                equippedNames.Add(prefab.name);
            }

            return equippedNames.ToArray();
        }

        //could use reflection to make this a tiny bit less disgusting
        public static int[] GetEquippedHashes(VisEquipment ve)
        {
            if (ve == null)
            {
                return new int[] { };
            }

            List<int> equippedHashes = new List<int>();

            if (ve.m_currentLeftItemHash != 0) equippedHashes.Add(ve.m_currentLeftItemHash);
            if (ve.m_currentRightItemHash != 0) equippedHashes.Add(ve.m_currentRightItemHash);
            if (ve.m_currentChestItemHash != 0) equippedHashes.Add(ve.m_currentChestItemHash);
            if (ve.m_currentLegItemHash != 0) equippedHashes.Add(ve.m_currentLegItemHash);
            if (ve.m_currentHelmetItemHash != 0) equippedHashes.Add(ve.m_currentHelmetItemHash);
            if (ve.m_currentShoulderItemHash != 0) equippedHashes.Add(ve.m_currentShoulderItemHash);
            if (ve.m_currentBeardItemHash != 0) equippedHashes.Add(ve.m_currentBeardItemHash);
            if (ve.m_currentHairItemHash != 0) equippedHashes.Add(ve.m_currentHairItemHash);
            if (ve.m_currentUtilityItemHash != 0) equippedHashes.Add(ve.m_currentUtilityItemHash);
            if (ve.m_currentLeftBackItemHash != 0) equippedHashes.Add(ve.m_currentLeftBackItemHash);
            if (ve.m_currentRightBackItemHash != 0) equippedHashes.Add(ve.m_currentRightBackItemHash);

            return equippedHashes.ToArray();
        }

        public static void LogMessage(string message, BepInEx.Logging.LogLevel level = BepInEx.Logging.LogLevel.Message)
        {
            if (!Main.loggingEnabled.Value) return;

            if (level == BepInEx.Logging.LogLevel.Message)
            {
                Main.log.LogMessage(message);
            }
            if (level == BepInEx.Logging.LogLevel.Warning)
            {
                Main.log.LogWarning(message);
            }
            if (level == BepInEx.Logging.LogLevel.Error)
            {
                Main.log.LogError(message);
            }
        }

        public static int[] BodyPartToBoneIndexes(string part)
        {
            BodypartSystem.bodyPart bodyPart;
            if(Enum.TryParse(part, out bodyPart))
            {
                return BodyPartToBoneIndexes(bodyPart);
            }
            else
            {
                return new int[] { -100 };
            }
        }

        public static int[] BodyPartToBoneIndexes(BodypartSystem.bodyPart[] part)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < part.Length; i++)
            {
                result.AddRange(BodyPartToBoneIndexes(part[i]));
            }
            return result.ToArray();
        }

        public static int[] BodyPartToBoneIndexes(BodypartSystem.bodyPart part)
        {
            switch(part)
            {
                case BodypartSystem.bodyPart.All: return new int[] { -1 };

                case BodypartSystem.bodyPart.Head: return new int[] { 4, 5, 6 };

                case BodypartSystem.bodyPart.Torso: return new int[] { 0, 1, 2, 3, 7, 26 };

                case BodypartSystem.bodyPart.ArmUpperLeft: return new int[] { 8 };

                case BodypartSystem.bodyPart.ArmLowerLeft: return new int[] { 9 };

                case BodypartSystem.bodyPart.HandLeft: return new int[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 };

                case BodypartSystem.bodyPart.ArmUpperRight: return new int[] { 27 };

                case BodypartSystem.bodyPart.ArmLowerRight: return new int[] { 28 };

                case BodypartSystem.bodyPart.HandRight: return new int[] { 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44 };

                case BodypartSystem.bodyPart.LegUpperLeft: return new int[] { 45 };

                case BodypartSystem.bodyPart.LegLowerLeft: return new int[] { 46 };

                case BodypartSystem.bodyPart.FootLeft: return new int[] { 47, 48 };

                case BodypartSystem.bodyPart.LegUpperRight: return new[] { 49 };

                case BodypartSystem.bodyPart.LegLowerRight: return new[] { 50 };

                case BodypartSystem.bodyPart.FootRight: return new int[] { 51, 52 };

                default: return new int[] { -100 };
            }
        }
    }
}
