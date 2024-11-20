// SPDX-License-Identifier: MIT
// Copyright 2024 hirmiura (https://github.com/hirmiura)
using System.Collections.Generic;
using System.Linq;
using ReflexCLI.Attributes;

namespace NotRandomQuest;

[ConsoleCommandClassCustomizer("")]
public static class Console
{
    public static bool HasReset = true;

    // id, group
    private static Dictionary<string, string> s_backup = [];

    [ConsoleCommand("")]
    public static string NRQReset()
    {
        if (HasReset)
        {
            Plugin.Logger.LogDebug("NRQReset: Did nothing.");
            return "Did nothing because of don't need recovery.";
        }
        else
        {
            var current = EClass.sources.quests.map;
            s_backup.ToList().ForEach(pair => current[pair.Key].group = pair.Value);
            Plugin.ConfigId.Value = "";
            HasReset = true;
            Plugin.Logger.LogDebug($"NRQReset: Resore default.");
            return "Restore default.";
        }
    }

    [ConsoleCommand("")]
    public static string NRQSet(string id)
    {
        var current = EClass.sources.quests.map;
        if (!current.ContainsKey(id))
        {
            Plugin.Logger.LogDebug($"NRQSet: {id} not Found.");
            return $"{id} not Found.";
        }
        if (HasReset)
        {
            Backup();
        }
        else
        {
            NRQReset();
        }
        HasReset = false;
        current.Where(pair => pair.Key != id && pair.Value.group == "random")
            .Select(pair => pair.Key).ToList()
            .ForEach(key => current[key].group = "dummy");
        Plugin.ConfigId.Value = id;
        Plugin.Logger.LogDebug($"NRQSet: {id} set.");
        // var dummies = string.Join(", ", current.Where(pair => pair.Value.group == "dummy").Select(pair => pair.Key));
        // Plugin.Logger.LogDebug($"dummy: {dummies}");
        return $"Only quest: {id}";
    }

    public static void Backup()
    {
        var current = EClass.sources.quests.map;
        s_backup = current
            .Where(pair => pair.Value.group == "random")
            .ToDictionary(pair => pair.Key, pair => pair.Value.group)
            .DeepCopy();
        // var backuped = string.Join(", ", s_backup.Select(pair => pair.Key));
        // Plugin.Logger.LogDebug($"backuped: {backuped}");
        Plugin.Logger.LogDebug($"Backup: Save values.");
    }
}
