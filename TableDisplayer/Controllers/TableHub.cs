using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableDisplayer.Controllers {
    public class TableHub : Hub {
        public static Dictionary<string, string> ActiveUsers = new Dictionary<string, string>();

        public static IEnumerable<KeyValuePair<string, string>> GetConnections(string username) =>
            ActiveUsers.Where(x => x.Key.StartsWith(username)).ToArray();


        public override Task OnConnectedAsync() {
            try {
                ActiveUsers.Add($"{Context.User?.Identity?.Name}-{Context.ConnectionId}", Context.ConnectionId);
            } catch { }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            try {
                ActiveUsers.Remove($"{Context.User?.Identity?.Name}-{Context.ConnectionId}");
            } catch { }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
