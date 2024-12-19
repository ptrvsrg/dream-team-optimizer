namespace DreamTeamOptimizer.Core.Models.Events;

public record VotingStartedEvent(int HackathonId, List<Employee> TeamLeads, List<Employee> Juniors);