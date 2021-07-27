using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableDisplayer.Controllers {
    public class TableHub : Hub {
        public async Task Update(string json) {
            await this.Clients.All.SendAsync("Update", json);
        }
    }
}
