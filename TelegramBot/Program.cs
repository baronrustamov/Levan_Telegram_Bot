using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using ApiAiSDK;
using ApiAiSDK.Model;

namespace TelegramBot
{
    class Program
    {
        static TelegramBotClient Bot;
        static ApiAi apiAi;
        static void Main(string[] args)
        {

            Bot = new TelegramBotClient("TELEGRAM_BOT_API"); 
            AIConfiguration config = new AIConfiguration("GOOGLE_AI_API", SupportedLanguage.English);
            apiAi = new ApiAi(config);


            Bot.OnMessage += BotOnMessageRecieved;

            Bot.OnCallbackQuery +=BotOnCallbackQueryReceived;

            var me = Bot.GetMeAsync().Result;

            Console.WriteLine(me.FirstName);

            Bot.StartReceiving();

            Console.ReadLine();
            Bot.StopReceiving();
        
        }

        private static async void BotOnCallbackQueryReceived(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} Entered button: {buttonText}");

            if (buttonText == "Dream Room")
            {
                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "https://www.gamercamp.ca/wp-content/uploads/2019/03/5-ways-to-pimp-out-your-living-room-for-ultimate-gaming.jpg");
            }
            else if (buttonText == "MUSIC")
            {
                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "https://www.youtube.com/watch?v=1t_sMynan_k");
            }
                await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"You entered button {buttonText}");  // Button entered norification
            }

        private static async void BotOnMessageRecieved(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;


            if (message == null || message.Type != MessageType.Text)
                return;

            string name = $"{message.From.FirstName} {message.From.LastName}"; 

            Console.WriteLine($"{name} Send message: '{message.Text}'");  // Who who send message and what

            switch (message.Text)
            {
                case "/start":
                    string text =
@"Hello , " +  message.From.FirstName + @"👼
    You can ask me anything
Commands 🧏:

    /start     🏃 - Start bot 
    /callback  📞 - Contacts
    /chill     ⛱️ - Music and video
    /info      ℹ️ - Location and Contact
    /keyboard  ⌨️ - Fast answers";


                    await Bot.SendTextMessageAsync(message.From.Id, text);
                    break;

                case "/info":
                    var replyKeyboardTwo = new ReplyKeyboardMarkup(new[]
                    {
                        new []
                        {
                            new KeyboardButton("Contact"){ RequestContact = true},
                            new KeyboardButton("Location"){ RequestLocation = true},
                        }
                    });
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Message", replyMarkup: replyKeyboardTwo);
                    break;

                case "/keyboard":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                         new KeyboardButton("Hello"),                   
                         new KeyboardButton("How are you?"),
                         new KeyboardButton("How old are you?"),
                         new KeyboardButton("Thank you")
                        }
                    });
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Title:", replyMarkup: replyKeyboard);
                    break;

                case "/chill":
                    var inlineKeyboardTwo = new InlineKeyboardMarkup(new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("Dream Room"),
                            InlineKeyboardButton.WithCallbackData("MUSIC")
                        }
                    });
                    await Bot.SendTextMessageAsync(message.From.Id, "Choose a menu",
                        replyMarkup: inlineKeyboardTwo);

                    break;

                case "/callback":
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithUrl("EXP YouTube", "YOUR_URL"),
                            InlineKeyboardButton.WithUrl("EXP Telegram", "YOUR_URL"),
                            InlineKeyboardButton.WithUrl("EXP Facebook", "YOUR_URL"),
                            InlineKeyboardButton.WithUrl("EXP Instagram", "YOUR_URL") 
                        }
                    });
                    await Bot.SendTextMessageAsync(message.From.Id, "Choose a menu", 
                        replyMarkup: inlineKeyboard);
                    
                    break;

                default:
                    var response = apiAi.TextRequest(message.Text);
                    string answer = response.Result.Fulfillment.Speech;
                    if (answer == "")
                        answer = message.Text + "? "+ "Sorry, but i don't understand you. 🙄";
                    await Bot.SendTextMessageAsync(message.From.Id, answer);
                    break;
            }
        }
    }
}
