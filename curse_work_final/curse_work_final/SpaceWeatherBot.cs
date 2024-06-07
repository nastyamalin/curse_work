using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace curse_work_final
{
    public class SpaceWeatherBot
    {
        private TelegramBotClient botClient = new TelegramBotClient("7342698686:AAFUCVVmJGRlQxO-KFM29a3svEpOoG3FyEo");
        private CancellationToken cancellationToken = new CancellationToken();
        private ReceiverOptions receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }
        };
        private Clients clients = new Clients();
        private string currentQueryType;
        private DateTime startDate;
        private DateTime endDate;
        private CurrentMenu currentMenu = CurrentMenu.MainMenu;
        private Controller controller;
        private readonly HttpClient httpClient;
        private RatingService ratingService = new RatingService();
        private List<(DateTime Date, int Rating)> ratings = new List<(DateTime Date, int Rating)>();

        private enum CurrentMenu
        {
            MainMenu,
            RateDaysMenu,
            WeatherStatisticsMenu
        }

        public async Task Start()
        {
            botClient.StartReceiving(HandlerUpdateAsync, HandlerError, receiverOptions, cancellationToken);
            var botMe = await botClient.GetMeAsync();
            Console.WriteLine($"Bot is working: {botMe.Username}");
            Console.ReadKey();
        }

        private Task HandlerError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"API Error:\n {apiRequestException.ErrorCode}\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private async Task HandlerUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandlerMessageAsync(botClient, update.Message);
            }
        }

        private async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message)
        {
            switch (message.Text)
            {
                case "/start":
                case "/menu":
                    var mainMenuKeyboard = GetMainMenuKeyboard();
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Привіт! Оберіть, що хочете дізнатися:",
                        replyMarkup: mainMenuKeyboard,
                        cancellationToken: cancellationToken
                    );
                    break;
                case "GST":
                case "FLR":
                case "SEP":
                    currentQueryType = message.Text;
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Введіть дату початку у форматі YYYY-MM-DD:",
                        cancellationToken: cancellationToken
                    );
                    break;
                case string date when DateTime.TryParse(date, out DateTime parsedDate):
                    if (startDate == default)
                    {
                        startDate = parsedDate;
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Введіть дату закінчення у форматі YYYY-MM-DD:",
                            cancellationToken: cancellationToken
                        );
                    }
                    else
                    {
                        endDate = parsedDate;
                        await ProcessQuery(botClient, message.Chat.Id);
                    }
                    break;
                case "Оцінка метеозалежності":
                    await ShowRatingMenu(botClient, message.Chat.Id);
                    break;
                case "Оцінити дні":
                    await StartRatingProcess(botClient, message.Chat.Id);
                    break;
                case string date when DateTime.TryParse(date, out DateTime parsedDate):
                    if (startDate == default)
                    {
                        startDate = parsedDate;
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Введіть дату закінчення у форматі YYYY-MM-DD:",
                            cancellationToken: cancellationToken
                        );
                    }
                    else
                    {
                        endDate = parsedDate;
                        await ProcessRatingCompletion(botClient, message.Chat.Id);
                    }
                    break;
                default:
                    if (message.Text.StartsWith("2024-") && int.TryParse(message.Text.Split()[1], out int rating))
                    {
                        var date = DateTime.Parse(message.Text.Split()[0]);
                        ratings.Add((date, rating));
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Оцінка додана. Введіть ще одну або 'завершити' для обробки.",
                            cancellationToken: cancellationToken
                        );
                    }
                    else if (message.Text.ToLower() == "завершити")
                    {
                        await ProcessRatingCompletion(botClient, message.Chat.Id);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Невірна команда. Спробуйте ще раз.",
                            cancellationToken: cancellationToken
                        );
                    }
                    break;
            }
        }

        private async Task ShowMainMenu(ITelegramBotClient botClient, long chatId)
        {
            currentMenu = CurrentMenu.MainMenu;
            var mainMenuKeyboard = GetMainMenuKeyboard();
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Головне меню",
                replyMarkup: mainMenuKeyboard,
                cancellationToken: cancellationToken
            );
        }

        private async Task ShowRatingMenu(ITelegramBotClient botClient, long chatId)
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Виберіть опцію:",
                replyMarkup: new ReplyKeyboardMarkup(new[]
                {
            new KeyboardButton[] { "Оцінити дні" },
            new KeyboardButton[] { "Оновити оцінки", "Видалити оцінку" },
            new KeyboardButton[] { "Повернутися до головного меню" }
                }),
                cancellationToken: cancellationToken
            );
        }

        private async Task StartRatingProcess(ITelegramBotClient botClient, long chatId)
        {
            ratings.Clear(); 
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Введіть дату і оцінку (наприклад: 2024-06-01 3). Введіть 'завершити' для обробки введених оцінок.",
                cancellationToken: cancellationToken
            );
        }

        private async Task ProcessRatingCompletion(ITelegramBotClient botClient, long chatId)
        {
            if (ratings.Count > 0)
            {
                var minDate = ratings.Min(r => r.Date);
                var maxDate = ratings.Max(r => r.Date);
                var avgRating = ratings.Average(r => r.Rating);

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Статистика введених оцінок:\n" +
                          $"Мін. дата: {minDate.ToShortDateString()}\n" +
                          $"Макс. дата: {maxDate.ToShortDateString()}\n" +
                          $"Середнє оцінювання: {avgRating:F2}",
                    cancellationToken: cancellationToken
                );
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Не введено жодної оцінки.",
                    cancellationToken: cancellationToken
                );
            }
        }
        private async Task PostRatingAsync(long chatId, DateTime date, int rating)
        {
            await ratingService.PostRatingAsync(chatId.ToString(), date, rating);
            await botClient.SendTextMessageAsync(chatId, "Оцінка успішно додана.", cancellationToken: cancellationToken);
        }

        private async Task UpdateRatingAsync(long chatId, DateTime date, int newRating)
        {
            await ratingService.UpdateRatingAsync(chatId.ToString(), date, newRating);
            await botClient.SendTextMessageAsync(chatId, "Оцінка успішно оновлена.", cancellationToken: cancellationToken);
        }

        private async Task DeleteRatingAsync(long chatId, DateTime date)
        {
            await ratingService.DeleteRatingAsync(chatId.ToString(), date);
            await botClient.SendTextMessageAsync(chatId, "Оцінка успішно видалена.", cancellationToken: cancellationToken);
        }
        private ReplyKeyboardMarkup GetMainMenuKeyboard()
        {
            var keyboardButtons = new KeyboardButton[][]
            {
        new KeyboardButton[] { "GST", "FLR", "SEP" },
        new KeyboardButton[] { "Статистика метеозалежності", "Оцінка метеозалежності" }
            };

            var replyMarkup = new ReplyKeyboardMarkup(keyboardButtons);
            replyMarkup.ResizeKeyboard = true;
            return replyMarkup;
        }




        private async Task GetStatisticsAsync(long chatId)
        {
            if (ratings.Count == 0)
            {
                await botClient.SendTextMessageAsync(chatId, "Немає  доступних оцінок", cancellationToken: cancellationToken);
                return;
            }

            var minDate = ratings.Min(r => r.Date);
            var maxDate = ratings.Max(r => r.Date);
            var avgRating = ratings.Average(r => r.Rating);

            await botClient.SendTextMessageAsync(chatId, $"Статистика введених оцінок:\n" +
                                                          $"Мін. дата: {minDate.ToShortDateString()}\n" +
                                                          $"Макс. дата: {maxDate.ToShortDateString()}\n" +
                                                          $"Середнє оцінювання: {avgRating:F2}",
                cancellationToken: cancellationToken);
        }

        private async Task ProcessQuery(ITelegramBotClient botClient, long chatId)
        {
            try
            {
                if (currentQueryType == "GST")
                {
                    var gstData = await clients.GetGSTAsync(startDate, endDate);
                    foreach (var gst in gstData)
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"Geomagnetic Storm: Start Time: {gst.StartTime}, Link: {gst.Link}",
                            cancellationToken: cancellationToken
                        );
                    }
                }
                else if (currentQueryType == "FLR")
                {
                    var flrData = await clients.GetFLRAsync(startDate, endDate);
                    foreach (var flr in flrData)
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"Solar Flare: Begin Time: {flr.BeginTime}, Peak Time: {flr.PeakTime}, End Time: {flr.EndTime}, Class Type: {flr.ClassType}",
                            cancellationToken: cancellationToken
                        );
                    }
                }
                else if (currentQueryType == "SEP")
                {
                    var sepData = await clients.GetSEPAsync(startDate, endDate);
                    foreach (var sep in sepData)
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"Solar Energetic Particle: Event Time: {sep.EventTime}",
                            cancellationToken: cancellationToken
                        );
                        foreach (var instrument in sep.Instruments)
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: $"Instrument: {instrument.DisplayName}",
                                cancellationToken: cancellationToken
                            );
                        }
                    }
                }
                startDate = default;
                endDate = default;
                currentQueryType = null;
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Error fetching data: {ex.Message}",
                    cancellationToken: cancellationToken
                );
            }
        }

    }
}