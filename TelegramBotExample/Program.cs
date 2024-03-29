﻿using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using TelegramBotExample.Logics.BotCommands;
using TelegramBotExample.Logics.BotCommands.Commands;
using TelegramBotExample.Logics.BotCommands.Factory;
using TelegramBotExample.Logics.Repositories;

namespace TelegramBotExample
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly TelegramBotClient BotClient = new TelegramBotClient("840840550:AAFD_4REeQebjWbJhCms0Sfc1nZRUTxp4co");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var me = BotClient.GetMeAsync().Result;
            Console.WriteLine(me.Username);

            BotClient.OnMessage += BotOnMessageReceived;
            BotClient.OnCallbackQuery += BotOnCallbackQueryReceived;

            BotClient.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            var userRepository = new UserRepository();
            var user = userRepository.FindUser(e.Message.Chat.Id);

            switch (e.Message.Text)
            {
                case "/start":
                    var startCommand = new StartCommand();
                    startCommand.Execute(e.Message, user);
                    return;
                case "/menu":
                    var menuCommand = new MenuCommand();
                    menuCommand.Execute(e.Message,user);
                    break;
            }

            var command = CommandFactory.GetPublicCommand(e.Message.Text);
            if (command != null)
            {
                command.Execute(e.Message, user);
                return;
            }

            command = CommandFactory.GetCommand(user?.Command);
            command?.Execute(e.Message, user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            var userRepository = new UserRepository();
            var user = userRepository.FindUser(e.CallbackQuery.Message.Chat.Id);
            if (user != null)
            {
                user.CallBackCommand = e.CallbackQuery.Data;
                userRepository.UpdateUser(user);
            }

            var callBackCommand = CommandFactory.GetCallBackCommand(user?.CallBackCommand);
            callBackCommand?.CallBackExecute(e.CallbackQuery,user);
        }
    }
}
