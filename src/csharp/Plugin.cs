// SPDX-License-Identifier: MIT
// Copyright 2024 hirmiura (https://github.com/hirmiura)
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ReflexCLI;

namespace NotRandomQuest;
public static class MyPluginInfo
{
    public const string PLUGIN_GUID = "yararezon.not_random_quest";
    public const string PLUGIN_NAME = "Not Random Quest";
    public const string PLUGIN_VERSION = "0.1.3";
}


[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]

public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    public static ConfigEntry<string> ConfigId;
    public static ConfigEntry<string> ConfigDestZone;

    private void Awake()
    {
        Logger = base.Logger;

        var executingAssembly = Assembly.GetExecutingAssembly();
        // 設定
        ConfigId = Config.Bind(
            "General",  // Section
            "id",  // Key
            "", // Default value
            "The target quest id, blank means not set."); // Description of the option to show in the config file
        ConfigDestZone = Config.Bind(
            "General",  // Section
            "destZone",  // Key
            "", // Default value
            "The destination zone name, blank means not set."); // Description of the option to show in the config file
        // ハーモニーパッチ
        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
        // コマンドの登録
        CommandRegistry.assemblies.Add(executingAssembly);
    }
}
[HarmonyPatch(typeof(Game), nameof(Game.OnLoad))]
public static class GamePatch
{
    public static void Postfix()
    {
        // id の設定
        var id = Plugin.ConfigId.Value;
        if (id.IsNullOrWhiteSpace())
        {
            Plugin.Logger.LogDebug($"GamePatch.Postfix: id not set.");
        }
        else
        {
            Console.HasReset = true;  // ロード後はリセットフラグを立てる
            Console.NRQSet(id);  // 再設定
        }

        // destZone の設定
        var destination = Plugin.ConfigDestZone.Value;
        if (id.IsNullOrWhiteSpace())
        {
            Plugin.Logger.LogDebug($"GamePatch.Postfix: destZone not set.");
        }
        else
        {
            Console.NRQDest(destination);
        }
    }
}
[HarmonyPatch(typeof(QuestDestZone), nameof(QuestDestZone.SetDest))]
public static class QuestDestZonePatch
{
    public static Zone PatchingZone = null;
#pragma warning disable IDE1006 // 命名スタイル
    public static void Prefix(QuestDestZone __instance, ref Zone z)
#pragma warning restore IDE1006 // 命名スタイル
    {
        if (PatchingZone is not null
            && typeof(QuestEscort).IsInstanceOfType(__instance))
        {
            z = PatchingZone;
            Plugin.Logger.LogDebug($"QuestDestZone.SetDest.Prefix: Zone \"{z.Name}\"");
        }
    }
}
