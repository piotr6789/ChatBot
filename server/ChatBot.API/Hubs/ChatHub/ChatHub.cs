using Microsoft.AspNetCore.SignalR;
using ChatBot.Infrastructure.Services;
using System.Collections.Concurrent;
using System.Text;
using ChatBot.Persistence.Entities;

namespace ChatBot.API.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, (CancellationTokenSource, StringBuilder)> _activeTasks = new();
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SendMessage(Guid clientSessionId, string message)
        {
            var connectionId = Context.ConnectionId;
            var cts = new CancellationTokenSource();
            var responseBuilder = new StringBuilder();
            _activeTasks[connectionId] = (cts, responseBuilder);

            var session = await _chatService.GetOrCreateSessionAsync(clientSessionId);
            var userMessage = await _chatService.SaveUserMessageAsync(session, message);

            await Clients.Caller.SendAsync("MessageSentConfirmation", userMessage.Id, userMessage.Content);

            var response = await _chatService.GetRandomResponseAsync();
            await StreamBotResponseAsync(connectionId, userMessage.Id, response, cts.Token, responseBuilder, session);
        }

        private async Task StreamBotResponseAsync(
            string connectionId,
            int userMessageId,
            string response,
            CancellationToken cancellationToken,
            StringBuilder responseBuilder,
            Session session)
        {
            Message? botMessage = null;

            try
            {
                for (int i = 0; i < response.Length; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    responseBuilder.Append(response[i]);

                    await Clients.Client(connectionId).SendAsync(
                        "ReceiveMessageChunk", "ChatBot", responseBuilder.ToString(), userMessageId);

                    await Task.Delay(5, cancellationToken);
                }

                botMessage = await _chatService.SaveBotMessageAsync(session, responseBuilder.ToString());

                await Clients.Client(connectionId).SendAsync(
                    "ReceiveMessage", "ChatBot", responseBuilder.ToString(), botMessage.Id);

                await Clients.Client(connectionId).SendAsync("UpdateMessageId", userMessageId, botMessage.Id);
            }
            finally
            {
            }
        }
    }
}
