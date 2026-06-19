using BetterAmongUs.Modules;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;

namespace BetterAmongUs.Attributes;

/// <summary>
/// Attribute to register a class in the Il2Cpp runtime, optionally specifying interfaces to implement.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
internal class RegisterInIl2Cpp(params Type[] interfaces) : Attribute
{
    /// <summary>
    /// Gets the array of interface types that the registered class should implement in Il2Cpp.
    /// </summary>
    /// <value>An array of interface types, or an empty array if none were specified.</value>
    public Type[] Interfaces { get; } = interfaces ?? [];

    /// <summary>
    /// Registers all classes marked with <see cref="RegisterInIl2Cpp"/> from the executing assembly.
    /// </summary>
    internal static void RegisterAll()
    {
        var assembly = ModInfo.Assembly;

        var types = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<RegisterInIl2Cpp>() != null)
            .ToList();

        foreach (var type in types)
        {
            try
            {
                var attr = type.GetCustomAttribute<RegisterInIl2Cpp>();

                if (attr.Interfaces.Any())
                    ClassInjector.RegisterTypeInIl2Cpp(type, new RegisterTypeOptions { Interfaces = attr.Interfaces });
                else
                    ClassInjector.RegisterTypeInIl2Cpp(type);
            }
            catch (Exception ex)
            {
                Logger_.Error($"Failed to register {type.Name}: {ex.Message}");
            }
        }
    }
}