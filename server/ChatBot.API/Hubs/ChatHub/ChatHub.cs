using Microsoft.AspNetCore.SignalR;
using ChatBot.Persistence.Repositories;
using ChatBot.Persistence.Entities;
using ChatBot.Domain.Enums;
using System.Collections.Concurrent;
using System.Text;

namespace ChatBot.API.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, (CancellationTokenSource, StringBuilder)> _activeTasks = new();
        private readonly IMessageRepository _messageRepository;
        private readonly ISessionRepository _sessionRepository;

        private static readonly string[] _responses =
        {
            "Jasne, mogę Ci pomóc!",
            "To ciekawe pytanie. Muszę się nad tym zastanowić.",
            "Nie jestem pewien, ale spróbujmy znaleźć rozwiązanie.",
            "Możesz mi powiedzieć coś więcej na ten temat?",
            "To świetne pytanie! Odpowiedź może być skomplikowana, ale spróbuję wyjaśnić.",
            "Myślę, że istnieje kilka sposobów na rozwiązanie tego problemu.",
            "To zależy od kontekstu. Spróbujmy spojrzeć na to z różnych stron."
        };

        public ChatHub(IMessageRepository messageRepository, ISessionRepository sessionRepository)
        {
            _messageRepository = messageRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task SendMessage(string cookieId, string message)
        {
            var connectionId = Context.ConnectionId;
            var cts = new CancellationTokenSource();
            var responseBuilder = new StringBuilder();

            _activeTasks[connectionId] = (cts, responseBuilder);

            var session = await _sessionRepository.GetSessionByCookieIdAsync(cookieId);
            if (session == null)
            {
                session = new Session
                {
                    CookieId = cookieId,
                    CreatedAt = DateTime.UtcNow,
                    LastActivityAt = DateTime.UtcNow
                };
                await _sessionRepository.CreateSessionAsync(session);
                await _sessionRepository.SaveChangesAsync();
            }

            var userMessage = new Message
            {
                Session = session,
                Sender = SenderType.User,
                Content = message,
                CreatedAt = DateTime.UtcNow
            };
            await _messageRepository.AddMessageAsync(userMessage);
            await _messageRepository.SaveChangesAsync();

            await Clients.Caller.SendAsync("ReceiveMessage", "User", message, userMessage.Id);

            var random = new Random();
            var response = _responses[random.Next(_responses.Length)];

            for (int i = 0; i < response.Length; i++)
            {
                if (cts.Token.IsCancellationRequested)
                {
                    var partialBotMessageId = await SaveBotResponse(session, responseBuilder.ToString());
                    await Clients.Caller.SendAsync("ReceiveMessage", "ChatBot", responseBuilder.ToString(), partialBotMessageId);
                    _activeTasks.TryRemove(connectionId, out _);
                    return;
                }

                responseBuilder.Append(response[i]);
                await Clients.Caller.SendAsync("ReceiveMessageChunk", "ChatBot", responseBuilder.ToString());
                await Task.Delay(50, cts.Token);
            }

            var botMessageId = await SaveBotResponse(session, response);
            await Clients.Caller.SendAsync("ReceiveMessage", "ChatBot", response, botMessageId);

            _activeTasks.TryRemove(connectionId, out _);
        }

        public async Task CancelMessage()
        {
            var connectionId = Context.ConnectionId;
            if (_activeTasks.TryGetValue(connectionId, out var task))
            {
                task.Item1.Cancel();
                await Clients.Caller.SendAsync("CancelMessage");
            }
        }

        private async Task<int> SaveBotResponse(Session session, string responseContent)
        {
            if (string.IsNullOrWhiteSpace(responseContent))
                return 0;

            var botMessage = new Message
            {
                Session = session,
                Sender = SenderType.Bot,
                Content = responseContent,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepository.AddMessageAsync(botMessage);
            await _messageRepository.SaveChangesAsync();

            return botMessage.Id;
        }

    }
}
