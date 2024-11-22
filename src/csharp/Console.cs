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

    public static readonly string GrpRandom = "random";
    public static readonly string GrpDummy = "dummy";

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
        var quest_source = EClass.sources.quests.map
            .Where(pair => pair.Value.group == GrpRandom || pair.Value.group == GrpDummy)
            .ToDictionary(p => p.Key, p => p.Value);
        if (!quest_source.ContainsKey(id))
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
        quest_source.Where(pair => pair.Key != id && pair.Value.group == GrpRandom)
            .Select(pair => pair.Key).ToList()
            .ForEach(key => quest_source[key].group = GrpDummy);
        Plugin.ConfigId.Value = id;
        Plugin.Logger.LogDebug($"NRQSet: {id} set.");
        // var dummies = string.Join(", ", quest_source.Where(pair => pair.Value.group == GrpDummy).Select(pair => pair.Key));
        // Plugin.Logger.LogDebug($"dummy: {dummies}");
        return $"Only quest: {id}";
    }

    public static void Backup()
    {
        var current = EClass.sources.quests.map;
        s_backup = current
            .Where(pair => pair.Value.group == GrpRandom)
            .ToDictionary(pair => pair.Key, pair => pair.Value.group)
            .DeepCopy();
        // var backuped = string.Join(", ", s_backup.Select(pair => pair.Key));
        // Plugin.Logger.LogDebug($"backuped: {backuped}");
        Plugin.Logger.LogDebug($"Backup: Save values.");
    }

    [ConsoleCommand("")]
    public static string NRQDest(string destination)
    {
        var zones = EClass.game.spatials.Zones;
        var dstZone = zones.Where(z => z.Name == destination).FirstOrDefault();
        if (dstZone is null)
        {
            var msg = $"{destination} not found.";
            Plugin.Logger.LogDebug($"NRQDest: {msg}");
            return msg;
        }
        QuestDestZonePatch.PatchingZone = dstZone;  // パッチャーにゾーンを教える
        Plugin.ConfigDestZone.Value = destination;  // 設定を保存

        var message = $"Destination lock only for escort quests: \"{destination}\", uid:{dstZone.uid}";
        Plugin.Logger.LogDebug($"NRQDest: {message}");
        return message;
    }

    [ConsoleCommand("")]
    public static string NRQDestReset()
    {
        QuestDestZonePatch.PatchingZone = null;  // パッチャーにnullセット
        Plugin.ConfigDestZone.Value = "";  // 設定を保存
        var message = "Clear destination lock.";
        Plugin.Logger.LogDebug($"NRQDestReset: {message}");
        return message;
    }
}
