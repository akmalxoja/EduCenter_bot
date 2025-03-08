using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using static System.Net.WebRequestMethods;

namespace Telegrambot
{
    internal class TelegramBotHandler
    {
        public long userid;
        public string Token { get; set; }
        public object currenttime = DateTime.Now.ToString("HH:mm");

        public TelegramBotHandler(string token)
        {
            this.Token = token;
        }

        public async Task BotHandle()
        {
            var botClient = new TelegramBotClient($"{this.Token}");

            using CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
                );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();



            cts.Cancel();

        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {


            var Handlar = update.Type switch
            {
                UpdateType.Message => HandleMessageAsync(botClient, update, cancellationToken),
                UpdateType.EditedMessage => HandleEditMessageAsync(botClient, update, cancellationToken),
                UpdateType.CallbackQuery => HandleCallbackQueryAsync(botClient, update, cancellationToken),
                _ => HandleMessageAsync(botClient, update, cancellationToken),

            };


            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;



            var chatId = message.Chat.Id;

            string filepath = @"C:\Users\VICTUS\Desktop\.Net\RamozonTaqvimBot\users.txt";
            var user_message = $"Received a '{messageText}' message in chat {chatId}. UserName =>  {message.Chat.Username} at {currenttime}\n";
            System.IO.File.AppendAllText(filepath, user_message);
            await Console.Out.WriteLineAsync(user_message);

            await Console.Out.WriteLineAsync(message.Chat.Username);
        }


        private async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {




        }



        private object HandleEditMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task HandleMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            string urll = " a";
            userid = update.Message.Chat.Id;

            if (update.Message.Text == "/start")
            {
                // using Telegram.Bot.Types.ReplyMarkups;

                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                    new KeyboardButton[] { "Akmalxoja", "Saman" },
                    new KeyboardButton[] { "Callme", "Behruz" },
                })
                {
                    ResizeKeyboard = true
                };

                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Choose a response",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);

            }

        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {

            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()

            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
