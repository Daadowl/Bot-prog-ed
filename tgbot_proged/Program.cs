using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using tgbot;
using static System.Net.Mime.MediaTypeNames;
using static System.Data.Entity.Infrastructure.Design.Executor;
using System.CodeDom.Compiler;
using static System.Runtime.InteropServices.JavaScript.JSType;


string TELEGRAM_TOKEN = "...";
var botClient = new TelegramBotClient(TELEGRAM_TOKEN);
using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }
};

botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken: cts.Token);

Console.ReadLine();


async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken token)
{
    if (update.Message != null && update.Message.Text != null)
    {
        
        var chatID = update.Message.Chat.Id;
        var messageText = update.Message.Text;
        var id_user = update.Message.From.Id;
        var name = update.Message.From.FirstName;
        var messageId = update.Message.MessageId;
        var userts = SQL.TSUser(id_user);
        string nickname;
        if (update.Message.From.Username == null)
        {
            nickname = update.Message.From.FirstName;
        }
        else
        {
            nickname = update.Message.From.Username;
        }
        
        var language = SQL.languageUser(id_user);

        if (language == 1)
        {
            if (messageText.StartsWith("@ProgEd_bot") && userts == 1)
            {
                
                string text = SQL.TechSup(messageText,id_user,language);
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Главное меню", "ButtonBackMenu")
                    }
                });
                SQL.ChangeTS(id_user, 0);
                await botClient.SendTextMessageAsync(chatID, text, replyMarkup: inlineKeyboard);
            }

            if (messageText == "/start")
            {
                SQL.AddUser(id_user, nickname);
                SQL.ChangeTS(id_user, 0);
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⭐ Начать", "Start")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🗃️ Личный кабинет", "LK")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("💻 ТехПоддержка", "TS")
                    }
                });

                await botClient.SendTextMessageAsync(chatID,"Главное меню", replyMarkup: inlineKeyboard);

            }
        }
        if (language == 2)
        {
            if (messageText.StartsWith("@ProgEd_bot") && userts == 1)
            {

                string text = SQL.TechSup(messageText, id_user, language);
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Главное меню", "ButtonBackMenu")
                    }
                });
                SQL.ChangeTS(id_user, 0);
                await botClient.SendTextMessageAsync(chatID, text, replyMarkup: inlineKeyboard);
            }

            if (messageText == "/start")
            {

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⭐ Start", "Start")

                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("🗃️ Account", "LK")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("💻 Technical support", "TS")
                    }
                });

                await botClient.SendTextMessageAsync(chatID, "Main menu", replyMarkup: inlineKeyboard);
            }

        }
        
    }
    if (update.Type == UpdateType.CallbackQuery)
    {
        await HandleCallbackQuery(botClient, update.CallbackQuery);
        return;
    }
}




   


async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    
{
    var chatID = callbackQuery.Message.Chat.Id;
    var name = callbackQuery.Message.Chat.FirstName;
    string nickname;
    if (callbackQuery.Message.Chat.Username == null)
    {
        nickname = callbackQuery.Message.Chat.FirstName;
    }
    else
    {
        nickname = callbackQuery.Message.Chat.Username;
    }
    var messageID = callbackQuery.Message.MessageId;
    var data = callbackQuery.Data;
    var language = SQL.languageUser(chatID);



    if (data == "ButtonLangEng")
    {
        //await botClient.DeleteMessageAsync(chatID, messageID);
        DeleteMessage(botClient, chatID, messageID);
        SQL.ChangeLanguage(chatID.ToString(), 2);
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Хорошо", "Settings")

                }
            });
        await botClient.SendTextMessageAsync(chatID,"Язык изменён", replyMarkup: inlineKeyboard);
    }

    if (data == "ButtonLangRus")
    {
        //await botClient.DeleteMessageAsync(chatID, messageID);
        DeleteMessage(botClient, chatID, messageID);
        SQL.ChangeLanguage(chatID.ToString(), 1);
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Ok", "Settings")

                }
            });
        await botClient.SendTextMessageAsync(chatID, "The language has been changed", replyMarkup: inlineKeyboard);
    }

    if (language == 1)
    {
        if (data == "LK")
        {
            DeleteMessage(botClient, chatID, messageID);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("📈 Прогрес", "Progress")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⚙️ Настройки", "Settings")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⬅️ Назад", "ButtonBackMenu")
                    }
            });

            await botClient.SendTextMessageAsync(chatID, $"Личный кабинет\nПользователь: {name}", replyMarkup: inlineKeyboard);

        }

        if (data == "Progress")
        {
            DeleteMessage(botClient, chatID, messageID);
            List<(string,int, int)> lst = SQL.StatusProcent(SQL.GetProgressStatus(chatID, language));
            int count = lst.Count;
            string str = $"Прогрес пользователя {name}:\n\n";
            for (int i = 0; i < count; i++)
            {
                if (lst[i].Item2 == 1)
                {
                    string addStr = "(Python) " + lst[i].Item1 + ": " + lst[i].Item3.ToString() + "%\n";
                    str += addStr;

                }
                if (lst[i].Item2 == 2)
                {
                    string addStr = "(C++) " + lst[i].Item1 + ": " + lst[i].Item3.ToString() + "%\n";
                    str += addStr;

                }

            }
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⬅️ Назад", "LK")
                    }
            });

            await botClient.SendTextMessageAsync(chatID,str , replyMarkup: inlineKeyboard);
        }
        if (data == "TS")
        {
            DeleteMessage(botClient, chatID, messageID);
            SQL.ChangeTS(chatID, 0);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("✉️ Отправить сообщение", "SendMes")

                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "ButtonBackMenu")
                }
            });

            await botClient.SendTextMessageAsync(chatID, "ТехПоддержка", replyMarkup: inlineKeyboard);


        }
        if (data == "SendMes")
        {
            DeleteMessage(botClient, chatID, messageID);
            SQL.ChangeTS(chatID, 1);
            await botClient.SendTextMessageAsync(chatID, "Чтобы отправить сообщение в ТехПоддержку, отправьте сообщение, которое начинается с @ProgEd_bot.\nЧтобы вернуться назад, напишите /start.");
        }

        if (data == "ButtonBackMenu")
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⭐ Начать", "Start")
                
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🗃️ Личный кабинет", "LK")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("💻 ТехПоддержка", "TS")
                }
            });

            await botClient.SendTextMessageAsync(chatID, "Главное меню", replyMarkup: inlineKeyboard);

        }

        if (data == "Start")
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Python", "Menu1"),
                    InlineKeyboardButton.WithCallbackData("C++", "Menu2")

                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "ButtonBackMenu")
                }
            });
            await botClient.SendTextMessageAsync(chatID, "Выбор языка программирования", replyMarkup: inlineKeyboard);

        }

        if (data == "Settings")
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🔄 Сменить язык", "ButtonLangEng")

                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "LK")
                }
            });

            await botClient.SendTextMessageAsync(chatID, "Выбран язык: Русский", replyMarkup: inlineKeyboard);

        }

        if (data == "Menu1")
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📖 Теория", "Theory1"),
                    InlineKeyboardButton.WithCallbackData("📝 Практика", "Practic1")

                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "Start")
                }
            });

            await botClient.SendTextMessageAsync(chatID, "Выбран язык программирования: Python", replyMarkup: inlineKeyboard);

        }

        if (data == "Menu2")
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📖 Теория", "Theory2"),
                    InlineKeyboardButton.WithCallbackData("📝 Практика", "Practic2")

                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", "Start")
                }
            });

            await botClient.SendTextMessageAsync(chatID, "Выбран язык программирования: С++", replyMarkup: inlineKeyboard);

        }

        if (data.StartsWith("Practic"))
        {
            DeleteMessage(botClient, chatID, messageID);
            int id_proglanguage = int.Parse(data.Replace("Practic", ""));

            // Получение данных из базы данных 
            List<(string, string, int)> courses = SQL.GetCourses(language, id_proglanguage);

            // Создание списка списков кнопок на основе полученных данных из базы
            var keyboardButtons = courses.Select(course =>
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(course.Item1, $"prac{course.Item3}/{id_proglanguage}") }
            ).ToList();

            // Создание дополнительной кнопки в новой строке
            var additionalButton = new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("⬅️ Назад", $"Menu{id_proglanguage}") };
            keyboardButtons.Add(additionalButton);

            // Создание клавиатуры с кнопками
            var inlineKeyboard = new InlineKeyboardMarkup(keyboardButtons);

            // Отправка сообщения с клавиатурой кнопок
            await botClient.SendTextMessageAsync(
                chatId: chatID,
                text: "Выберите тему задач",
                replyMarkup: inlineKeyboard
            );
        }


        if (data.StartsWith("prac"))
        {
            DeleteMessage(botClient, chatID, messageID); // Перед этим определите эту функцию для удаления сообщений

            string[] parts = data.Split('/');

            int id_proglanguage;
            int courseId;

            if (int.TryParse(parts[1], out id_proglanguage) && int.TryParse(parts[0].Replace("prac", ""), out courseId))
            {
                
                List<(string, string, int)> exercises = SQL.GetExercises(courseId);
                int count = exercises.Count;

                var keyboardButtons = new List<List<InlineKeyboardButton>>();

                // Создаем кнопки для каждого элемента в списке
                for (int i = 0; i < count; i++)
                {
                    string status = SQL.CheckStatus(chatID, exercises[i].Item3);
                    var button = InlineKeyboardButton.WithCallbackData($"Задание №{i + 1}" + $" ({status})", $"Task{exercises[i].Item3}/{courseId}/{id_proglanguage}");
                    keyboardButtons.Add(new List<InlineKeyboardButton> { button });
                }
                var backButton = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Назад", $"Practic{id_proglanguage}")
                };
                keyboardButtons.Add(backButton);

                // Создаем клавиатуру из сгенерированных кнопок
                var inlineKeyboard = new InlineKeyboardMarkup(keyboardButtons);

                await botClient.SendTextMessageAsync(
                chatId: chatID,
                text: "Список заданий",
                replyMarkup: inlineKeyboard
            );

            }
        }

        if (data.StartsWith("Task"))
        {
            DeleteMessage(botClient, chatID, messageID);
            string[] parts = data.Split('/');
            int id_proglanguage;
            int courseId;
            int id_task;

            if (int.TryParse(parts[1], out courseId) && int.TryParse(parts[2], out id_proglanguage) && int.TryParse(parts[0].Replace("Task", ""), out id_task))
            {
                List<(string, string, int)> task = SQL.ShowTask(id_task);
                

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
               {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("✅ Решено", $"Ready{id_task}"),
                        InlineKeyboardButton.WithCallbackData("❓ Ответ", $"Ans{id_task}/{id_proglanguage}/{courseId}")

                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⬅️ Назад", $"prac{courseId}/{id_proglanguage}")

                    }
                });

                await botClient.SendTextMessageAsync(chatID, task[0].Item1, replyMarkup: inlineKeyboard);
            }
        }
        if (data.StartsWith("Ready"))
        {
            int id_exercise = int.Parse(data.Replace("Ready", ""));
            SQL.ChangeStatus(chatID, id_exercise);
        }

        if (data.StartsWith("Ans"))
        {
            DeleteMessage(botClient, chatID, messageID);
            string[] parts = data.Split('/');
            int id_proglanguage;
            int courseId;
            int id_task;
            if (int.TryParse(parts[2], out courseId) && int.TryParse(parts[1], out id_proglanguage) && int.TryParse(parts[0].Replace("Ans", ""), out id_task))
                {
                    List<(string, string, int)> task = SQL.ShowTask(id_task);
                    

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⬅️ Назад", $"Task{id_task}/{courseId}/{id_proglanguage}")
                    }
                });

                await botClient.SendTextMessageAsync(chatID, task[0].Item2, replyMarkup: inlineKeyboard);

            }
        }



        if (data.StartsWith("Theory"))
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            int id_proglanguage = int.Parse(data.Replace("Theory", ""));

            // Получение данных из базы данных 
            List<(string, string, int)> courses = SQL.GetCourses(language, id_proglanguage); 

            // Создание списка списков кнопок на основе полученных данных из базы
            var keyboardButtons = courses.Select(course =>
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(course.Item1 + $" ({course.Item2})", $"Command{course.Item3}/{id_proglanguage}") }
            ).ToList();

            // Создание дополнительной кнопки в новой строке
            var additionalButton = new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("⬅️ Назад", $"Menu{id_proglanguage}") };
            keyboardButtons.Add(additionalButton);

            // Создание клавиатуры с кнопками
            var inlineKeyboard = new InlineKeyboardMarkup(keyboardButtons);

            // Отправка сообщения с клавиатурой кнопок
            await botClient.SendTextMessageAsync(
                chatId: chatID,
                text: "Выберите тему для изучения",
                replyMarkup: inlineKeyboard
            );
        }

        if (data.StartsWith("Command"))
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            string[] parts = data.Split('/');

            int id_proglanguage;
            int courseId;

            if (int.TryParse(parts[1], out id_proglanguage) && int.TryParse(parts[0].Replace("Command", ""), out courseId))
            {
                string theory = SQL.ShowTheory(courseId);
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⬅️ Назад", $"Theory{id_proglanguage}")

                    }
                });

                await botClient.SendTextMessageAsync(chatID, theory, replyMarkup: inlineKeyboard);
            }
        }

    }

    if (language == 2)
    {
        if (data == "LK")
        {
            DeleteMessage(botClient, chatID, messageID);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("📈 Progress", "Progress")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⚙️ Settings", "Settings")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⬅️ Back", "ButtonBackMenu")
                    }
            });

            await botClient.SendTextMessageAsync(chatID, $"Account\nUser: {name}", replyMarkup: inlineKeyboard);

        }
        if (data == "TS")
        {
            DeleteMessage(botClient, chatID, messageID);
            SQL.ChangeTS(chatID, 0);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("✉️ Send message", "SendMes")

                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Back", "ButtonBackMenu")
                }
            });

            await botClient.SendTextMessageAsync(chatID, "Technical support", replyMarkup: inlineKeyboard);


        }
        if (data == "SendMes")
        {
            DeleteMessage(botClient, chatID, messageID);
            SQL.ChangeTS(chatID, 1);
            await botClient.SendTextMessageAsync(chatID, "To send a message to Tech Support, send a message that starts with @ProgEd_bot.\nTo go back, write /start.");
        }

        if (data == "Progress")
        {
            DeleteMessage(botClient, chatID, messageID);
            List<(string, int, int)> lst = SQL.StatusProcent(SQL.GetProgressStatus(chatID, language));
            int count = lst.Count;
            string str = $"{name}'s progress:\n\n";
            for (int i = 0; i < count; i++)
            {
                if (lst[i].Item2 == 1)
                {
                    string addStr = "(Python) " + lst[i].Item1 + ": " + lst[i].Item3.ToString() + "%\n";
                    str += addStr;

                }
                if (lst[i].Item2 == 2)
                {
                    string addStr = "(C++) " + lst[i].Item1 + ": " + lst[i].Item3.ToString() + "%\n";
                    str += addStr;

                }

            }
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⬅️ Back", "LK")
                    }
            });

            await botClient.SendTextMessageAsync(chatID, str, replyMarkup: inlineKeyboard);
        }

        if (data == "ButtonBackMenu")
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⭐ Start", "Start")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🗃️ Account", "LK")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("💻 Technical support", "TS")
                }
            }) ;

            await botClient.SendTextMessageAsync(chatID, "Main menu", replyMarkup: inlineKeyboard);

        }

        if (data == "Start")
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Python", "Menu1"),
                    InlineKeyboardButton.WithCallbackData("C++", "Menu2")

                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Back", "ButtonBackMenu")
                }
            });
            await botClient.SendTextMessageAsync(chatID, "Choosing a programming language", replyMarkup: inlineKeyboard);

        }

        if (data == "Settings")
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("🔄 Change the language", "ButtonLangRus")

                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Back", "LK")
                }
            });

            await botClient.SendTextMessageAsync(chatID, "Selected language: English", replyMarkup: inlineKeyboard);

        }

        if (data == "Menu1")
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📖 Theory", "Theory1"),
                    InlineKeyboardButton.WithCallbackData("📝 Exercises", "Practic1")

                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Back", "Start")
                }
            });

            await botClient.SendTextMessageAsync(chatID, "Programming language selected: Python", replyMarkup: inlineKeyboard);

        }

        if (data == "Menu2")
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📖 Theory", "Theory2"),
                    InlineKeyboardButton.WithCallbackData("📝 Exercises", "Practic2")

                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Back", "Start")
                }
            });

            await botClient.SendTextMessageAsync(chatID, "Programming language selected: C++", replyMarkup: inlineKeyboard);

        }

        if (data.StartsWith("Practic"))
        {
            DeleteMessage(botClient, chatID, messageID);
            int id_proglanguage = int.Parse(data.Replace("Practic", ""));

            // Получение данных из базы данных 
            List<(string, string, int)> courses = SQL.GetCourses(language, id_proglanguage);

            // Создание списка списков кнопок на основе полученных данных из базы
            var keyboardButtons = courses.Select(course =>
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(course.Item1, $"prac{course.Item3}/{id_proglanguage}") }
            ).ToList();

            // Создание дополнительной кнопки в новой строке
            var additionalButton = new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("⬅️ Back", $"Menu{id_proglanguage}") };
            keyboardButtons.Add(additionalButton);

            // Создание клавиатуры с кнопками
            var inlineKeyboard = new InlineKeyboardMarkup(keyboardButtons);

            // Отправка сообщения с клавиатурой кнопок
            await botClient.SendTextMessageAsync(
                chatId: chatID,
                text: "Select a exercise topic:",
                replyMarkup: inlineKeyboard
            );
        }


        if (data.StartsWith("prac"))
        {
            DeleteMessage(botClient, chatID, messageID); // Перед этим определите эту функцию для удаления сообщений

            string[] parts = data.Split('/');

            int id_proglanguage;
            int courseId;

            if (int.TryParse(parts[1], out id_proglanguage) && int.TryParse(parts[0].Replace("prac", ""), out courseId))
            {

                List<(string, string, int)> exercises = SQL.GetExercises(courseId);
                int count = exercises.Count;

                var keyboardButtons = new List<List<InlineKeyboardButton>>();

                // Создаем кнопки для каждого элемента в списке
                for (int i = 0; i < count; i++)
                {
                    string status = SQL.CheckStatus(chatID, exercises[i].Item3);
                    var button = InlineKeyboardButton.WithCallbackData($"Exercise №{i + 1}" + $" ({status})", $"Task{exercises[i].Item3}/{courseId}/{id_proglanguage}");
                    keyboardButtons.Add(new List<InlineKeyboardButton> { button });
                }
                var backButton = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("⬅️ Back", $"Practic{id_proglanguage}")
                };
                keyboardButtons.Add(backButton);

                // Создаем клавиатуру из сгенерированных кнопок
                var inlineKeyboard = new InlineKeyboardMarkup(keyboardButtons);

                await botClient.SendTextMessageAsync(
                chatId: chatID,
                text: "List of exercises",
                replyMarkup: inlineKeyboard
            );

            }
        }

        if (data.StartsWith("Task"))
        {
            DeleteMessage(botClient, chatID, messageID);
            string[] parts = data.Split('/');
            int id_proglanguage;
            int courseId;
            int id_task;

            if (int.TryParse(parts[1], out courseId) && int.TryParse(parts[2], out id_proglanguage) && int.TryParse(parts[0].Replace("Task", ""), out id_task))
            {
                List<(string, string, int)> task = SQL.ShowTask(id_task);


                var inlineKeyboard = new InlineKeyboardMarkup(new[]
               {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("✅ Ready", $"Ready{id_task}"),
                        InlineKeyboardButton.WithCallbackData("❓ Answer", $"Ans{id_task}/{id_proglanguage}/{courseId}")

                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⬅️ Back", $"prac{courseId}/{id_proglanguage}")

                    }
                });

                await botClient.SendTextMessageAsync(chatID, task[0].Item1, replyMarkup: inlineKeyboard);
            }
        }
        if (data.StartsWith("Ready"))
        {
            int id_exercise = int.Parse(data.Replace("Ready", ""));
            SQL.ChangeStatus(chatID, id_exercise);
        }

        if (data.StartsWith("Ans"))
        {
            DeleteMessage(botClient, chatID, messageID);
            string[] parts = data.Split('/');
            int id_proglanguage;
            int courseId;
            int id_task;
            if (int.TryParse(parts[2], out courseId) && int.TryParse(parts[1], out id_proglanguage) && int.TryParse(parts[0].Replace("Ans", ""), out id_task))
            {
                List<(string, string, int)> task = SQL.ShowTask(id_task);


                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⬅️ Back", $"Task{id_task}/{courseId}/{id_proglanguage}")
                    }
                });

                await botClient.SendTextMessageAsync(chatID, task[0].Item2, replyMarkup: inlineKeyboard);

            }
        }



        if (data.StartsWith("Theory"))
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            int id_proglanguage = int.Parse(data.Replace("Theory", ""));

            // Получение данных из базы данных 
            List<(string, string, int)> courses = SQL.GetCourses(language, id_proglanguage);

            // Создание списка списков кнопок на основе полученных данных из базы
            var keyboardButtons = courses.Select(course =>
                new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(course.Item1 + $" ({course.Item2})", $"Command{course.Item3}/{id_proglanguage}") }
            ).ToList();

            // Создание дополнительной кнопки в новой строке
            var additionalButton = new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("⬅️ Back", $"Menu{id_proglanguage}") };
            keyboardButtons.Add(additionalButton);

            // Создание клавиатуры с кнопками
            var inlineKeyboard = new InlineKeyboardMarkup(keyboardButtons);

            // Отправка сообщения с клавиатурой кнопок
            await botClient.SendTextMessageAsync(
                chatId: chatID,
                text: "Choose a topic to study:",
                replyMarkup: inlineKeyboard
            );
        }

        if (data.StartsWith("Command"))
        {
            //await botClient.DeleteMessageAsync(chatID, messageID);
            DeleteMessage(botClient, chatID, messageID);
            string[] parts = data.Split('/');

            int id_proglanguage;
            int courseId;

            if (int.TryParse(parts[1], out id_proglanguage) && int.TryParse(parts[0].Replace("Command", ""), out courseId))
            {
                string theory = SQL.ShowTheory(courseId);
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("⬅️ Back", $"Theory{id_proglanguage}")

                    }
                });

                await botClient.SendTextMessageAsync(chatID, theory, replyMarkup: inlineKeyboard);
            }
        }

    }

}



async Task DeleteMessage(ITelegramBotClient botClient, long chatId, int messageId)
{
    try
    {
        await botClient.DeleteMessageAsync(chatId, messageId);
    }
    catch (ApiRequestException ex)
    {
        // Обработка ошибки удаления сообщения
        Console.WriteLine($"Ошибка удаления сообщения: {ex.Message}");
    }
}



Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var errorMessage = exception switch
    {
        ApiRequestException apiRequestException => $"Telegram API Error:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    Console.WriteLine(errorMessage);
    return Task.CompletedTask;
}
