using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableDisplayer.Data;
using TableDisplayer.Models;

namespace TableDisplayer.Controllers {
    public class SalesController : Controller {
        readonly ApplicationDbContext dbContext;
        readonly IHubContext<TableHub> hubContext;
        readonly UserManager<User> userManager;

        public SalesController(ApplicationDbContext dbContext, IHubContext<TableHub> hubContext, UserManager<User> userManager) {
            this.dbContext = dbContext;
            this.hubContext = hubContext;
            this.userManager = userManager;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLex() {
            var name = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(name)) { return RedirectToAction("AccessDenied", "Account"); }

            var user = await userManager.FindByNameAsync(name);
            if (user == null || user.IsSuspended) { return RedirectToAction("AccessDenied", "Account"); }

            var sales = dbContext.LexSales.ToArray();
            var vms = Convert(sales.OrderBy(x => x.Date));

            return View("LexView", vms);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCredit() {
            var name = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(name)) { return RedirectToAction("AccessDenied", "Account"); }

            var user = await userManager.FindByNameAsync(name);
            if (user == null || user.IsSuspended) { return RedirectToAction("AccessDenied", "Account"); }

            var sales = dbContext.CreditSales.ToArray();
            var vms = Convert(sales.OrderBy(x => x.Date));

            return View("CreditView", vms);
        }

        IEnumerable<SaleViewModel> Convert(IEnumerable<Sale> models) {
            var vms = new List<SaleViewModel>(models.Count());
            var i = 1;
            foreach (var sale in models) {
                vms.Add(new SaleViewModel {
                    Agent = sale.Agent,
                    Comment = sale.Comment,
                    Date = sale.Date.ToString("MM/dd/yyyy-HH:mm:ss"),
                    Email = sale.Email,
                    First_Name = sale.First_Name,
                    IsSale = sale.IsSale,
                    Last_Name = sale.Last_Name,
                    Phone = sale.Phone,
                    Status = sale.Status,
                    TID = sale.TID,
                    Vendor_ID = sale.Vendor_ID,
                    SortId = i++
                });
            }

            return vms;
        }

        [HttpPost]
        [Authorize(Roles = RoleInitializer.UPLOADER_ROLE)]
        public async Task<IActionResult> Upload([FromBody] SalesDto dto) {
            //if (string.IsNullOrWhiteSpace(json)) { return StatusCode(400); }

            //var dto = Newtonsoft.Json.JsonConvert.DeserializeObject<SalesDto>(json);
            if (dto.Name == "Lex") {
                var lex = ConvertToLex(dto);
                dbContext.LexSales.RemoveRange(dbContext.LexSales.ToArray());
                dbContext.LexSales.AddRange(lex);
            } else if (dto.Name == "Credit") {
                var credit = ConvertToCredit(dto);
                dbContext.CreditSales.RemoveRange(dbContext.CreditSales.ToArray());
                dbContext.CreditSales.AddRange(credit);
            }

            await dbContext.SaveChangesAsync();

            var suspendedUsers = dbContext.Users.Where(x => x.IsSuspended);
            await DisconnectUsersAsync(suspendedUsers);

            var json = JsonConvert.SerializeObject(dto);
            await hubContext.Clients.All.SendAsync($"Update{dto.Name}", json);

            return Ok();
        }

        IEnumerable<LexSale> ConvertToLex(SalesDto dto) {
            if (dto.Name == "Lex") {
                var lex = new List<LexSale>();
                foreach (var sale in dto.Rows) {
                    try {
                        var model = new LexSale {
                            Agent = sale.Agent,
                            Comment = sale.Comment,
                            Date = DateTime.Parse(sale.Date),
                            Email = sale.Email,
                            First_Name = sale.First_Name,
                            IsSale = sale.IsSale,
                            Last_Name = sale.Last_Name,
                            Phone = sale.Phone,
                            Status = sale.Status,
                            TID = sale.TID,
                            Vendor_ID = sale.Vendor_ID
                        };

                        lex.Add(model);
                    } catch { }
                }

                return lex;
            }

            return Array.Empty<LexSale>();
        }

        IEnumerable<CreditSale> ConvertToCredit(SalesDto dto) {
            if (dto.Name == "Credit") {
                var credit = new List<CreditSale>();
                foreach (var sale in dto.Rows) {
                    try {
                        var model = new CreditSale {
                            Agent = sale.Agent,
                            Comment = sale.Comment,
                            Date = DateTime.Parse(sale.Date),
                            Email = sale.Email,
                            First_Name = sale.First_Name,
                            IsSale = sale.IsSale,
                            Last_Name = sale.Last_Name,
                            Phone = sale.Phone,
                            Status = sale.Status,
                            TID = sale.TID,
                            Vendor_ID = sale.Vendor_ID
                        };

                        credit.Add(model);
                    } catch { }
                }

                return credit;
            }

            return Array.Empty<CreditSale>();
        }

        [HttpGet]
        public async Task<IActionResult> Test() {
            var sales = dbContext.LexSales.OrderBy(x => x.Date).Take(10).ToArray();
            var vms = Convert(sales);
            var json = JsonConvert.SerializeObject(vms);
            var suspendedUsers = dbContext.Users.Where(x => x.IsSuspended);
            await DisconnectUsersAsync(suspendedUsers);
            await hubContext.Clients.All.SendAsync("UpdateLex", json);

            return Ok();
        }

        private async Task DisconnectUsersAsync(IEnumerable<User> users) {
            if (users == null || !users.Any()) { return; }

            foreach (var user in users) {
                if (!TableHub.ActiveUsers.TryGetValue(user.UserName, out var connectionId)) { continue; }

                var client = hubContext.Clients.Client(connectionId);
                if (client != null) {
                    await client.SendAsync("Suspend");
                }
            }
        }
    }
}
