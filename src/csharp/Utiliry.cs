// SPDX-License-Identifier: MIT
// Copyright 2024 hirmiura (https://github.com/hirmiura)

using Newtonsoft.Json;

namespace NotRandomQuest;

public static class Utiliry
{
    public static T DeepCopy<T>(this T src)
    {
        var serialized = JsonConvert.SerializeObject(src);
        var deserialized = JsonConvert.DeserializeObject<T>(serialized);
        return deserialized;
    }
}
