using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine.SceneManagement;

namespace BlacksmithTools
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        #region[Declarations]

        public const string
            MODNAME = "BlacksmithTools",
            AUTHOR = "GoldenJude",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "2.0.1";

        public static ManualLogSource log;
        public static Harmony harmony;
        public static Assembly assembly;
        public static string modFolder;

        public static ConfigFile configFile;

        public static ConfigEntry<bool> reorderEnabled;
        public static ConfigEntry<bool> bodyHidingEnabled;
        public static ConfigEntry<bool> loggingEnabled;

        #endregion

        public Main()
        {
            log = Logger;
            harmony = new Harmony(GUID);
            assembly = Assembly.GetExecutingAssembly();
            modFolder = Path.GetDirectoryName(assembly.Location);
        }

        public void Start()
        {
            harmony.PatchAll(assembly);
        }

        public void Awake()
        {
            configFile = Config;

            reorderEnabled = configFile.Bind("bone reorder", "enabled", true, new ConfigDescription("", null));
            bodyHidingEnabled = configFile.Bind("bodypart hiding", "enabled", true, new ConfigDescription("", null));

            loggingEnabled = configFile.Bind("logging", "enabled", false, new ConfigDescription("", null));

            if (bodyHidingEnabled.Value)
            {
                BodypartSystem.BindConfigs();
            }
        }
    }
}
