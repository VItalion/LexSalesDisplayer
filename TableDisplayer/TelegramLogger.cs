using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TableDisplayer
{
    public class TelegramLogger
    {
    }
    public static class TLGRMBot {
        private const string botID = "1166200468:AAF-09z72GErOcsZwnWXMwadLv6sIuNMXK4";
        private const string chatId = "-425932343";

        public static void SendMessage(string message) {
            try {
                string url = $"https://api.telegram.org/bot{botID}/sendMessage?disable_web_page_preview=true&chat_id={chatId}&text={WebUtility.UrlEncode(message)}";
                using (WebClient wc = new WebClient()) {
                    wc.DownloadString(url);
                }
            } catch { }
        }
    }
}
