namespace BetterAmongUs.Commands.Arguments;

/// <summary>
/// Represents a string command argument.
/// </summary>
/// <param name="command">The command this argument belongs to.</param>
/// <param name="argInfo">Information about the argument (default: "{String}").</param>
internal sealed class StringArgument(BaseCommand command, string argInfo = "{String}") : BaseArgument(command, argInfo)
{
    protected override string[] GetArgSuggestions()
    {
        return ArgSuggestions.Invoke();
    }

    /// <summary>
    /// Gets or sets the function that provides argument suggestions.
    /// </summary>
    internal Func<string[]> ArgSuggestions { get; set; } = () => { return []; };

    /// <summary>
    /// Gets the string argument.
    /// </summary>
    /// <returns></returns>
    internal string GetString()
    {
        return Arg;
    }
}