using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableDisplayer.Controllers {
    public class TableHub : Hub {
        public static Dictionary<string, string> ActiveUsers = new Dictionary<string, string>();
       
        public override Task OnConnectedAsync() {
            ActiveUsers.Add(Context.User?.Identity?.Name, Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            ActiveUsers.Remove(Context.User.Identity.Name);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
