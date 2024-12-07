namespace DreamTeamOptimizer.ConsoleApp.Exceptions;

public class NoTeamsException : Exception
{
    public NoTeamsException() : base("No teams")
    {
    }
}