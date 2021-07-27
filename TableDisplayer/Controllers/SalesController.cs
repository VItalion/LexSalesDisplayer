using Microsoft.AspNetCore.Authorization;
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

        public SalesController(ApplicationDbContext dbContext, IHubContext<TableHub> hubContext) {
            this.dbContext = dbContext;
            this.hubContext = hubContext;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetLex() {
            var sales = dbContext.LexSales.ToArray();
            var vms = Convert(sales.OrderBy(x => x.Date));

            return View("SalesView", vms);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetCredit() {
            var sales = dbContext.CreditSales.ToArray();
            var vms = Convert(sales.OrderBy(x => x.Date));

            return View("SalesView", vms);
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
            var sales = dbContext.LexSales.OrderBy(x=>x.Date).Take(10).ToArray();
            var vms = Convert(sales);
            var json = JsonConvert.SerializeObject(vms);
            await hubContext.Clients.All.SendAsync("UpdateLex", json);

            return Ok();
        }
    }
}
