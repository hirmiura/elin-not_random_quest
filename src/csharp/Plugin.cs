// SPDX-License-Identifier: MIT
// Copyright 2024 hirmiura (https://github.com/hirmiura)
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using ReflexCLI;

namespace NotRandomQuest;
public static class MyPluginInfo
{
    public const string PLUGIN_GUID = "yararezon.not_random_quest";
    public const string PLUGIN_NAME = "Not Random Quest";
    public const string PLUGIN_VERSION = "0.1.0";
}


[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]

public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;

        var executingAssembly = Assembly.GetExecutingAssembly();
        // コマンドの登録
        CommandRegistry.assemblies.Add(executingAssembly);
    }
}
