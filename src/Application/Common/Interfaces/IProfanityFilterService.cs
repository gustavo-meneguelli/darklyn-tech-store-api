namespace Application.Common.Interfaces;

public interface IProfanityFilterService
{
    /// <summary>
    /// Checks if the text contains any profanity or inappropriate content.
    /// </summary>
    /// <param name="text">The text to evaluate.</param>
    /// <returns>True if profanity is detected; otherwise, false.</returns>
    bool ContainsProfanity(string text);
}
