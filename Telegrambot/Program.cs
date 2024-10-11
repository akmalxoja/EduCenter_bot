using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static readonly TelegramBotClient Bot = new TelegramBotClient("6892121245:AAGgsNjFZAo8M-4l3sCgwPAyrGWYlkdnzOw");

    public static async Task Main()
    {
        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // receive all update types
        };
        Bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await Bot.GetMeAsync();
        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

        cts.Cancel();
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message && update.Type != UpdateType.CallbackQuery)
            return;

        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;
            if (message.Type != MessageType.Text)
                return;

            if (message.Text == "/start")
            {
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🏫 Markaz 1", "center1"),
                        InlineKeyboardButton.WithCallbackData("🏫 Markaz 2", "center2")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🏫 Markaz 3", "center3"),
                        InlineKeyboardButton.WithCallbackData("🏫 Markaz 4", "center4")
                    }
                });

                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Salom! O'quv markazini tanlang:",
                    replyMarkup: inlineKeyboard
                );
            }
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            var callbackQuery = update.CallbackQuery;

            // Markaz 1 uchun menyu
            if (callbackQuery.Data == "center1")
            {
                await ShowCenterMenu(botClient, callbackQuery.Message.Chat.Id, "Markaz 1");
            }
            // Markaz 2 uchun menyu
            else if (callbackQuery.Data == "center2")
            {
                await ShowCenterMenu(botClient, callbackQuery.Message.Chat.Id, "Markaz 2");
            }
            // Markaz 3 uchun menyu
            else if (callbackQuery.Data == "center3")
            {
                await ShowCenterMenu(botClient, callbackQuery.Message.Chat.Id, "Markaz 3");
            }
            // Markaz 4 uchun menyu
            else if (callbackQuery.Data == "center4")
            {
                await ShowCenterMenu(botClient, callbackQuery.Message.Chat.Id, "Markaz 4");
            }
            // Kurslar uchun
            else if (callbackQuery.Data.StartsWith("courses_"))
            {
                var centerName = callbackQuery.Data.Split('_')[1];
                await SendCourses(botClient, callbackQuery.Message.Chat.Id, centerName);
            }
            // Bog'lanish uchun
            else if (callbackQuery.Data.StartsWith("contact_"))
            {
                var centerName = callbackQuery.Data.Split('_')[1];
                await SendContact(botClient, callbackQuery.Message.Chat.Id, centerName);
            }
            // Lokatsiya uchun
            else if (callbackQuery.Data.StartsWith("location_"))
            {
                var centerName = callbackQuery.Data.Split('_')[1];
                await SendLocation(botClient, callbackQuery.Message.Chat.Id, centerName);
            }
            // Orqaga qaytish
            else if (callbackQuery.Data == "back")
            {
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🏫 Markaz 1", "center1"),
                        InlineKeyboardButton.WithCallbackData("🏫 Markaz 2", "center2")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🏫 Markaz 3", "center3"),
                        InlineKeyboardButton.WithCallbackData("🏫 Markaz 4", "center4")
                    }
                });

                await botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: "O'quv markazini tanlang:",
                    replyMarkup: inlineKeyboard
                );
            }

            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
        }
    }

    private static async Task ShowCenterMenu(ITelegramBotClient botClient, long chatId, string centerName)
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("📚 Bizning kurslar", $"courses_{centerName}"),
                InlineKeyboardButton.WithCallbackData("📞 Biz bilan bog'lanish", $"contact_{centerName}")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("📍 Lokatsiya", $"location_{centerName}")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⬅️ Orqaga", "back")
            }
        });

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: $"{centerName} uchun kerakli bo'limni tanlang:",
            replyMarkup: inlineKeyboard
        );
    }

    private static async Task SendCourses(ITelegramBotClient botClient, long chatId, string centerName)
    {
        string coursesText = $"{centerName}dagi kurslar:\n1. C# dasturlash\n2. Web dasturlash\n3. Mobil dasturlash\n4. Grafik dizayn";

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: coursesText,
            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("⬅️ Orqaga", "back"))
        );
    }

    private static async Task SendContact(ITelegramBotClient botClient, long chatId, string centerName)
    {
        string contactText = $"{centerName} bilan bog'lanish uchun:\nTelefon: +998903311027\nEmail: info@{centerName.ToLower().Replace(" ", "")}.uz";

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: contactText,
            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("⬅️ Orqaga", "back"))
        );
    }

    private static async Task SendLocation(ITelegramBotClient botClient, long chatId, string centerName)
    {
        float latitude = 0f;
        float longitude = 0f;

        // Har bir markaz uchun lokatsiyalarni belgilaymiz
        switch (centerName)
        {
            case "Markaz 1":
                latitude = 41.3111f;
                longitude = 69.2797f;
                break;
            case "Markaz 2":
                latitude = 41.3275f;
                longitude = 69.2817f;
                break;
            case "Markaz 3":
                latitude = 41.2995f;
                longitude = 69.2401f;
                break;
            case "Markaz 4":
                latitude = 41.3122f;
                longitude = 69.2785f;
                break;
        }

        await botClient.SendLocationAsync(
            chatId: chatId,
            latitude: latitude,
            longitude: longitude,
            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("⬅️ Orqaga", "back"))
        );
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}
