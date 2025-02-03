using System.Collections.Immutable;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using TestApi.Messages;
using TestApi.Model;

namespace TestApi.Controllers;

[ApiController]
[Route("api")]
public class DDHelperController : ControllerBase
{
    private readonly ILogger<DDHelperController> _logger;
    private readonly Random _random;
    private readonly RoomMessagesContext _context;
    private readonly Meter _meter;
    private readonly Histogram<int> _histogram;

    public DDHelperController(ILogger<DDHelperController> logger, RoomMessagesContext context)
    {
        _logger = logger;
        _random = new Random();
        _context = context;
        _meter = new Meter("My.GT.Meter");
        _histogram = _meter.CreateHistogram<int>("rolls");
    }

    [HttpGet("last-events")]
    public async Task<IEnumerable<RoomMessage>> GetLastMessages([FromQuery] string roomId, [FromQuery] int? afterId)
    {
        return await _context.RoomMessages
            .Where(r => r.RoomId == roomId && (!afterId.HasValue || r.Id > afterId))
            .ToListAsync();
    }

    [HttpPost("roll")]
    public async Task Roll([FromBody] RollRequest request)
    {
        var message = new RoomMessage
        {
            Date = DateTime.UtcNow,
            RoomId = request.RoomId,
            Message = GenerateRollMessage(request),
        };

        _context.RoomMessages.Add(message);
        await _context.SaveChangesAsync();
    }

    private string GenerateRollMessage(RollRequest request)
    {
        var message = $"{request.PlayerId} rolled dice:\n";

        foreach (var batchOfDie in request.Dice)
        {
            message += $"{batchOfDie.NumberOfDice}D{batchOfDie.NumberOfSides}: ";
            for (var d = 0; d < batchOfDie.NumberOfDice; d++)
            {
                var roll = _random.Next(1, batchOfDie.NumberOfSides+1);
                _histogram.Record(roll);
                message += roll + ";";
            }
            message += "\n";
        }

        return message;
    }
}
