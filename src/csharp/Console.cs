// SPDX-License-Identifier: MIT
// Copyright 2024 hirmiura (https://github.com/hirmiura)
using System.Collections.Generic;
using System.Linq;
using ReflexCLI.Attributes;

namespace NotRandomQuest;

[ConsoleCommandClassCustomizer("")]
public static class Console
{
    private static bool s_hasReset = true;

    // id, group
    private static Dictionary<string, string> s_backup = [];

    [ConsoleCommand("")]
    public static string NRQReset()
    {
        if (s_hasReset)
        {
            return "Did nothing because of don't need recovery.";
        }
        else
        {
            var current = EClass.sources.quests.map;
            s_backup.ToList().ForEach(pair => current[pair.Key].group = pair.Value);
            s_hasReset = true;
            return "Restore default.";
        }
    }

    [ConsoleCommand("")]
    public static string NRQSet(string id)
    {
        var current = EClass.sources.quests.map;
        if (!current.ContainsKey(id)) return $"{id} not Found.";

        if (s_hasReset)
        {
            Backup();
            s_hasReset = false;
        }
        current.Where(pair => pair.Key != id && pair.Value.group == "random")
            .Select(pair => pair.Key).ToList()
            .ForEach(key => current[key].group = "dummy");
        return $"Only quest: {id}";
    }

    public static void Backup()
    {
        var current = EClass.sources.quests.map;
        s_backup = current
            .Where(pair => pair.Value.group == "random")
            .ToDictionary(pair => pair.Key, pair => pair.Value.group)
            .DeepCopy();
    }
}
