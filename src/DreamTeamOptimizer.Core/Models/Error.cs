namespace DreamTeamOptimizer.Core.Models;

public record Error(int Status, string Message, DateTime Timestamp, string Path);