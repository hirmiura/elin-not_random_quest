// SPDX-License-Identifier: MIT
// Copyright 2024 hirmiura (https://github.com/hirmiura)
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ReflexCLI.Attributes;

namespace NotRandomQuest;

[ConsoleCommandClassCustomizer("")]
public static class Console
{
    public static readonly string GrpRandom = "random";
    public static readonly string GrpDummy = "dummy";

    [ConsoleCommand("")]
    public static string NRQSet(string id)
    {
        var quest_source = _GetRowsByGroup([GrpRandom, GrpDummy]);
        var idlist = quest_source.Select(row => row.id);
        if (!idlist.Contains(id))
        {
            var msg = $"{id} not Found.";
            Plugin.Logger.LogDebug($"NRQSet: {msg}");
            return msg;
        }
        SetGroup([id]);
        Plugin.ConfigId.Value = id;  // 設定を保存
        Plugin.Logger.LogDebug($"NRQSet: \"{id}\" set.");
        return $"Only quest: {id}";
    }

    [ConsoleCommand("")]
    public static string NRQReset()
    {
        RestoreGroup();
        Plugin.ConfigId.Value = "";  // 設定を更新
        var msg = "Restore default.";
        Plugin.Logger.LogDebug($"NRQReset: {msg}");
        return msg;
    }

    public static void SetGroup(IEnumerable<string> target_ids)
    {
        var quest_source = _GetRowsByGroup([GrpRandom, GrpDummy]);
        foreach (var row in quest_source)
        {
            var grp = target_ids.Contains(row.id) ? GrpRandom : GrpDummy;
            _SetGroupToRow(row, grp);
        }
    }

    public static void RestoreGroup()
    {
        var quest_source = _GetRowsByGroup([GrpDummy]);
        foreach (var row in quest_source)
        {
            _SetGroupToRow(row, GrpRandom);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IEnumerable<SourceQuest.Row> _GetRowsByGroup(IEnumerable<string> groups) =>
        EClass.sources.quests.rows.Where(r => groups.Contains(r.group));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void _SetGroupToRow(SourceQuest.Row row, string group)
    {
        row.group = group;
        Plugin.Logger.LogDebug($"\"{row.id}\".group = {GrpRandom}");
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
