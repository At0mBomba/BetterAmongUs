using BetterAmongUs.Interfaces;
using BetterAmongUs.Modules;
using HarmonyLib;
using UnityEngine;

namespace BetterAmongUs.Patches.Unity;

[HarmonyPatch]
internal static class MonoExtensionPatch
{
    private static bool _patched = false;

    [HarmonyPatch(typeof(MonoBehaviour), MethodType.Constructor, [typeof(IntPtr)])]
    [HarmonyPostfix]
    private static void MonoBehaviour_Constructor_Postfix(MonoBehaviour __instance)
    {
        IMonoExtension.TryAddAutoExtension(__instance);
    }

    internal static void Patch(Harmony harmony)
    {
        if (_patched) return;
        _patched = true;

        try
        {
            // Find all types that implement IMonoExtension
            var extensionTypes = FindAllExtensionTypes();

            foreach (var type in extensionTypes)
            {
                // Patch Update method if it exists
                var updateMethod = AccessTools.Method(type, nameof(IMonoExtension.Update));
                if (updateMethod != null)
                {
                    harmony.Patch(updateMethod, prefix: new HarmonyMethod(typeof(MonoExtensionPatch), nameof(IMonoExtension_Update_Prefix)));
                }

                // Patch OnDestroy method if it exists
                var destroyMethod = AccessTools.Method(type, nameof(IMonoExtension.OnDestroy));
                if (destroyMethod != null)
                {
                    harmony.Patch(destroyMethod, postfix: new HarmonyMethod(typeof(MonoExtensionPatch), nameof(IMonoExtension_OnDestroy_Postfix)));
                }
            }
        }
        catch (Exception ex)
        {
            _patched = false;
            Logger_.Error($"Failed to patch MonoExtension methods: {ex.Message}");
            throw;
        }
    }

    internal static void Unpatch(Harmony harmony)
    {
        if (!_patched) return;

        try
        {
            // Find all types that implement IMonoExtension
            var extensionTypes = FindAllExtensionTypes();

            foreach (var type in extensionTypes)
            {
                // Unpatch Update method
                var updateMethod = AccessTools.Method(type, nameof(IMonoExtension.Update));
                if (updateMethod != null)
                {
                    harmony.Unpatch(updateMethod, HarmonyPatchType.Prefix, harmony.Id);
                }

                // Unpatch OnDestroy method
                var destroyMethod = AccessTools.Method(type, nameof(IMonoExtension.OnDestroy));
                if (destroyMethod != null)
                {
                    harmony.Unpatch(destroyMethod, HarmonyPatchType.Postfix, harmony.Id);
                }
            }
        }
        catch (Exception ex)
        {
            Logger_.Error($"Failed to unpatch MonoExtension methods: {ex.Message}");
            throw;
        }
    }

    private static List<Type> FindAllExtensionTypes()
    {
        var types = new List<Type>();
        var assembly = ModInfo.Assembly;

        foreach (var type in assembly.GetTypes())
        {
            if (type.IsInterface || type.IsAbstract)
                continue;

            if (!typeof(IMonoExtension).IsAssignableFrom(type))
                continue;

            if (type.IsGenericTypeDefinition)
                continue;

            types.Add(type);
        }

        return types;
    }

    private static bool IMonoExtension_Update_Prefix(IMonoExtension __instance)
    {
        if (__instance.BaseMono == null)
        {
            IMonoExtension.RemoveExtension(__instance);
            return false;
        }

        return true;
    }

    private static void IMonoExtension_OnDestroy_Postfix(IMonoExtension __instance)
    {
        IMonoExtension.RemoveExtension(__instance);
    }
}