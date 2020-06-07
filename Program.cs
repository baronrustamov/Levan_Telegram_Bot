using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using ApiAiSDK;

namespace TelegramBot
{
    class Program
    {
        static TelegramBotClient Bot;
        static ApiAi apiAi;
        static void Main(string[] args)
        {

            Bot = new TelegramBotClient("TELEGRAM API HERE");   // telegram botis api
            AIConfiguration config = new AIConfiguration("DIALOGFLOW API HERE", SupportedLanguage.Russian); // dialogwlod talk is api da ena
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
            Console.WriteLine($"{name} Нажал кнопку: {buttonText}");

            if (buttonText == "Комната Мечты")
            {
                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "https://www.gamercamp.ca/wp-content/uploads/2019/03/5-ways-to-pimp-out-your-living-room-for-ultimate-gaming.jpg");
            }
            else if (buttonText == "Faradenza")
            {
                await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "https://www.youtube.com/watch?v=1t_sMynan_k");
            }
                await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы нажали кнопку {buttonText}");  // uvedomlenie rom daachire konkretul gilaks      
            }

        private static async void BotOnMessageRecieved(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;


            if (message == null || message.Type != MessageType.Text)
                return;

            string name = $"{message.From.FirstName} {message.From.LastName}"; 

            Console.WriteLine($"{name} Отправил Сообщение: '{message.Text}'");  // pokazivaet kto napisal soobshenie i shto napisal

            switch (message.Text)
            {
                case "/start":
                    string text =
@"Привет , " +  message.From.FirstName + @"👼
    Я понимаю только Русский язык и это временно 🧗.
    Ты можешь писать мне напрямую, либо использовать 
Список команд 🧏:

    /start     🏃 - Запуск бота 
    /callback  📞 - Контакты Левана
    /chill     ⛱️ - Музыка и Видео
    /info      ℹ️ - Твои: Геолокация и контакты
    /keyboard  ⌨️ - Заготовоки быстрых ответов";


                    await Bot.SendTextMessageAsync(message.From.Id, text);
                    break;

                case "/info":
                    var replyKeyboardTwo = new ReplyKeyboardMarkup(new[]
                    {
                        new []
                        {
                            new KeyboardButton("Контакт"){ RequestContact = true},
                            new KeyboardButton("Геолокация"){ RequestLocation = true},
                        }
                    });
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Сообщение", replyMarkup: replyKeyboardTwo);
                    break;

                case "/keyboard":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                         new KeyboardButton("Привет"),                   
                         new KeyboardButton("Как дела?"),
                         new KeyboardButton("Сколько тебе лет?"),
                         new KeyboardButton("Вишня"),
                         new KeyboardButton("Спасибо")
                        }
                    });
                    await Bot.SendTextMessageAsync(message.Chat.Id, "Вывод заготовок:", replyMarkup: replyKeyboard);
                    break;

                case "/chill":
                    var inlineKeyboardTwo = new InlineKeyboardMarkup(new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("Комната Мечты"),
                            InlineKeyboardButton.WithCallbackData("Faradenza")
                        }
                    });
                    await Bot.SendTextMessageAsync(message.From.Id, "Выберите Пункт Меню",
                        replyMarkup: inlineKeyboardTwo);

                    break;

                case "/callback":
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithUrl("YouTube Sub", "https://www.youtube.com/channel/UCGESJVF-rSoGV5N5ayatTCw?sub_confirmation=1"),
                            InlineKeyboardButton.WithUrl("Telegram", "https://t.me/Genue69"),
                            InlineKeyboardButton.WithUrl("Facebook", "https://www.facebook.com/levan.amashukeli.3"),
                            InlineKeyboardButton.WithUrl("Instagram", "https://www.instagram.com/levanamashukeli/") 
                        }
                    });
                    await Bot.SendTextMessageAsync(message.From.Id, "Выберите Пункт Меню", 
                        replyMarkup: inlineKeyboard);
                    
                    break;

                default:
                    var response = apiAi.TextRequest(message.Text);
                    string answer = response.Result.Fulfillment.Speech;
                    if (answer == "")
                        answer = message.Text + "? "+ "Прости, но я тебя не понял. 🙄";
                    await Bot.SendTextMessageAsync(message.From.Id, answer);
                    break;
            }
        }
    }
}
