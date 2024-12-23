using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.MsEmployee.Interfaces.Brokers.Publishers;
using DreamTeamOptimizer.MsEmployee.Interfaces.Services;
using MassTransit;

namespace DreamTeamOptimizer.MsEmployee.Brokers.Consumers;

public class VotingStartedConsumer(
    ILogger<VotingStartedConsumer> logger,
    IConfiguration config,
    IWishListPublisher wishListPublisher,
    IWishListService wishListService)
    : IConsumer<Batch<VotingStartedEvent>>
{
    public Task Consume(ConsumeContext<Batch<VotingStartedEvent>> context)
    {
        logger.LogInformation("consume voting started events");
        
        var employeeId = config.GetValue<int>("Application:EmployeeID");

        foreach (var ctx in context.Message)
        {
            List<int> desiredEmployees;
            if (ctx.Message.Juniors.FindAll(j => j.Id == employeeId).Count != 0)
            {
                desiredEmployees = ctx.Message.TeamLeads.Select(t => t.Id).ToList();
            } else if (ctx.Message.TeamLeads.FindAll(t => t.Id == employeeId).Count != 0)
            {
                desiredEmployees = ctx.Message.Juniors.Select(j => j.Id).ToList();
            }
            else
            {
                logger.LogWarning("");
                return Task.CompletedTask;
            }

            logger.LogDebug("get wish list");
            var wishList = wishListService.GetWishlist(desiredEmployees);
            
            logger.LogDebug("publish wish list");
            wishListPublisher.SendWishList(wishList, ctx.Message.HackathonId);
        }
        
        return Task.CompletedTask;
    }
}