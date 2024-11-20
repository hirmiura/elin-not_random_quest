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
    public const string PLUGIN_VERSION = "0.1.1";
}


[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]

public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    public static ConfigEntry<string> ConfigId;

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
        var id = Plugin.ConfigId.Value;
        if (id.IsNullOrWhiteSpace())
        {
            Plugin.Logger.LogDebug($"GamePatch.Postfix: id not set.");
            return;
        }
        Console.HasReset = true;  // ロード後はリセットフラグを立てる
        Console.NRQSet(id);  // 再設定
    }
}
