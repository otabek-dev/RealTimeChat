using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealTimeMail.Model;

namespace RealTimeMail
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private static readonly Dictionary<string, List<string>> _connections = new Dictionary<string, List<string>>();

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task GetAllUsers()
        {
            var users = _context.Users.Select(x => x.UserName).ToList();
            await Clients.All.SendAsync("GetAllUsers", users);
        }

        public async Task GetAllMsg(string userName, string connectionId)
        {
            var messages = _context.ChatMessages
                .Where(x => x.ReceiverName == userName)
                .OrderBy(x => x.Timestamp)
                .ToList();

            await Clients.Client(connectionId).SendAsync("GetAllMsg", messages);
        }

        public async Task SendPrivateMessage(string user, string receiver, string subject, string message)
        {
            var msg = new Message
            {
                Id = Guid.NewGuid(),
                SenderName = user,
                ReceiverName = receiver,
                Subject = subject,
                Text = message,
                Timestamp = DateTime.Now
            };
            _context.ChatMessages.Add(msg);
            await _context.SaveChangesAsync();
            var currentUser = _context.Users.FirstOrDefault(x => x.UserName == receiver && x.ConnectionId != string.Empty);
            if (currentUser != null)
                await Clients.Client(currentUser.ConnectionId).SendAsync("ReceiveMessage", new List<object> { msg });
        }

        public async Task Login(string userName)
        {
            var findUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            if (findUser == null)
            {
                var userr = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = userName,
                    ConnectionId = Context.ConnectionId
                };
                _context.Users.Add(userr);
            }
            else
                findUser.ConnectionId = Context.ConnectionId;

            await _context.SaveChangesAsync();
            await GetAllMsg(userName, Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                user.ConnectionId = string.Empty;
                await _context.SaveChangesAsync();
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
