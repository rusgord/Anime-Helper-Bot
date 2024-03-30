using System.Net.Mime;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Xml;
using System.Net;
using System.Text;


namespace TGBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TelegramBotClient("PASTE YOUR BOT TOKEN HERE");            
            client.StartReceiving(Update, Error);
            while (true)
            {
                string end = Console.ReadLine();
                if (end == "end")
                    return;
            }
        }

        private static async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private static async Task Update(ITelegramBotClient botclient, Update update, CancellationToken token)
        {
            var message = update.Message;
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[]{"Random anime"},
                new KeyboardButton[]{"About"}
            })
            { ResizeKeyboard = true };
            if (message != null)
                Console.WriteLine($"{message.Date}, {message.Chat.Username}\n\t {message.Text}");
                switch (message.Text)
                {
                    case "/start":
                    await Actions.Start(botclient, message.Chat.Id, replyKeyboardMarkup, token);
                    break;
                    case "Random anime":
                    await Actions.RandAnimes(botclient, message.Chat.Id, token);
                    break;
                    case "About":
                    await Actions.About(botclient, message.Chat.Id, replyKeyboardMarkup, token);                    
                    break;
                }
        }
    }
}
