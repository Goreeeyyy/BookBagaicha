using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

public class NotificationWebSocketHandler
{
    private readonly ILogger<NotificationWebSocketHandler> _logger;

    // In-memory dictionary to store connected WebSockets per user
    private static ConcurrentDictionary<long, WebSocket> _connections = new ConcurrentDictionary<long, WebSocket>();

    public NotificationWebSocketHandler(ILogger<NotificationWebSocketHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
    {
        var userId = GetUserIdFromContext(context);

        if (userId.HasValue)
        {
            bool added = _connections.TryAdd(userId.Value, webSocket);
            if (!added)
            {
                _logger.LogWarning($"Failed to add WebSocket for user {userId.Value}");
            }
            await SendConnectionConfirmation(webSocket, userId.Value);
            await Receive(webSocket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received message from user {userId}: {message}");
                    // Handle client messages if needed (e.g., subscribing to specific topics)
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    bool removed = _connections.TryRemove(userId.Value, out var removedWebSocket);
                    if (removed)
                    {
                        Console.WriteLine($"WebSocket connection closed by user: {userId}");
                    }
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                }
            });
        }
        else
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Unauthorized", CancellationToken.None);
        }
    }

    private async Task SendConnectionConfirmation(WebSocket webSocket, long userId)
    {
        var confirmationMessage = JsonSerializer.Serialize(new { type = "connection_success", userId });
        await SendMessageAsync(webSocket, confirmationMessage);
    }

    public async Task SendNotificationToUserAsync(long userId, object notification)
    {
        if (_connections.TryGetValue(userId, out var webSocket) && webSocket.State == WebSocketState.Open)
        {
            var message = JsonSerializer.Serialize(notification);
            await SendMessageAsync(webSocket, message);
        }
    }

    // Helper method to send a message over the WebSocket
    private async Task SendMessageAsync(WebSocket webSocket, string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    // Helper method to continuously receive messages from the WebSocket
    private async Task Receive(WebSocket webSocket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            try
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
                if (result.MessageType == WebSocketMessageType.Close)
                    break;
            }
            catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely ||
                                                ex.WebSocketErrorCode == WebSocketError.Faulted ||
                                                ex.WebSocketErrorCode == WebSocketError.InvalidMessageType
)
            {
                // Handle various closure reasons
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"WebSocket receive error for user: {GetUserIdForWebSocket(webSocket)}");
                break;
            }
        }
        // Clean up connection on disconnect
        var disconnectedUserId = GetUserIdForWebSocket(webSocket);
        if (disconnectedUserId.HasValue)
        {
            bool removed = _connections.TryRemove(disconnectedUserId.Value, out var removedWebSocket);
            if (removed)
            {
                Console.WriteLine($"WebSocket connection removed for user: {disconnectedUserId}");
            }
        }
    }
    // Helper method to get UserId from WebSocket (if needed for logging)
    private long? GetUserIdForWebSocket(WebSocket webSocket)
    {
        return _connections.FirstOrDefault(kvp => kvp.Value == webSocket).Key;
    }

    // Implement this based on how you identify users from the HttpContext
    private long? GetUserIdFromContext(HttpContext context)
    {
        _logger.LogInformation("[WS] User Claims: {Claims}", context.User.Claims.Select(c => $"{c.Type}: {c.Value}"));
        var userIdClaim = context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                          ?? context.User.FindFirst("sub")
                          ?? context.User.FindFirst("id"); // Fallback to 'sub' or 'id'
        if (userIdClaim != null)
        {
            _logger.LogInformation($"[WS] Found claim: '{userIdClaim.Type}' with value: '{userIdClaim.Value}'");
            if (long.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogInformation($"[WS] Parsed User ID: {userId}");
                return userId;
            }
            else
            {
                _logger.LogWarning($"[WS] Failed to parse User ID from claim: '{userIdClaim.Value}'");
                return null;
            }
        }
        else
        {
            _logger.LogWarning("[WS] No suitable user ID claim found.");
            return null;
        }
    }
}