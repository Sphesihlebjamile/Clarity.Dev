namespace Clarity.Dev.NET.Core.Models;

/// <summary>
/// Provides static helper methods for determining whether a specified argument matches predefined command-line options
/// related to <see cref="SolutionCommandConstants"/>.
/// </summary>
public static class SolutionCommandConstantsHelper
{
    /// <summary>
    /// Determines whether the specified argument matches the predefined path option, ignoring case.
    /// </summary>
    /// <param name="arg">
    /// This string is compared to the constant value defined in <see cref="SolutionCommandConstants.PathOption"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the argument matches the path option; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsPathOption(string arg) => arg.Equals(SolutionCommandConstants.PathOption, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Determines whether the specified argument matches the predefined output option, ignoring case.
    /// </summary>
    /// <param name="arg">
    /// This string is compared to the constant value defined in <see cref="SolutionCommandConstants.OutputOption"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the argument matches the output option; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsOutputOption(string arg) => arg.Equals(SolutionCommandConstants.OutputOption, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Determines whether the specified argument matches the predefined output path option, ignoring case.
    /// </summary>
    /// <param name="arg">
    /// This string is compared to the constant value defined in <see cref="SolutionCommandConstants.OutputPathOption"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the argument matches the output path option; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsOutputPathOption(string arg) => arg.Equals(SolutionCommandConstants.OutputPathOption, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Determines whether the specified argument matches the predefined help option, ignoring case.
    /// </summary>
    /// <param name="arg">
    /// This string is compared to the constant value defined in <see cref="SolutionCommandConstants.HelpOption"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the argument matches the help option; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsHelpOption(string arg) => arg.Equals(SolutionCommandConstants.HelpOption, StringComparison.OrdinalIgnoreCase);
}
