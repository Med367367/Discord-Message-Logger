using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Discord_Message_Logger.Properties;
using System.IO;
using System.Text;

namespace Discord_Message_Logger.Startup
{
    public class Logging
    {

        /* CALL */
        public static bool MessageRecievedBool = Settings.Default.MessageRecievedBool;
        public static bool MessageRemovedBool = Settings.Default.MessageRemovedBool;
        public static bool MessageUpdatedBool = Settings.Default.MessageUpdatedBool;
        public static bool PresenceUpdatedBool = Settings.Default.PresenceUpdatedBool;
        /* TYPES */
        public static bool DMs = Settings.Default.DMs;
        public static bool Groups = Settings.Default.Groups;
        public static bool Servers = Settings.Default.Servers;
        /* LOGS */
        public static bool HeartBeatLogs = Settings.Default.HeartBeatLogs;


        /* TASKs */
        public static Task MessageRecieved(MessageCreateEventArgs e)
        {

            if (MessageRecievedBool)
            {
                /* AUTHOR CHECK */
                if (!e.Author.IsBot && e.Author != e.Client.CurrentUser)
                {
                    /* MESSAGE TYPE CHECK */
                    switch (e.Message.MessageType)
                    {
                        case MessageType.Default:
                            switch (e.Message.Channel.Type) 
                            {
                                case DSharpPlus.ChannelType.Private:
                                    if (DMs)
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        Logger.Write($"[Recieved] ");
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                        Logger.Write($"[DMs] ");
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Logger.Write($"[{DateTime.Now}] ");
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Logger.Write($"{e.Message.Author.Username}#{e.Message.Author.Discriminator} ({e.Message.Author.Id}) | ");
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                        Logger.WriteLine($"{e.Message.Content}");
                                        Console.ResetColor();
                                    }
                                break;
                                case DSharpPlus.ChannelType.Group:
                                    if (Groups)
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        Logger.Write($"[Recieved] ");
                                        Console.ForegroundColor = ConsoleColor.Blue;
                                        Logger.Write($"[Group '{e.Message.Channel.Name}'] ");
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Logger.Write($"[{DateTime.Now}] ");
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Logger.Write($"{e.Message.Author.Username}#{e.Message.Author.Discriminator} ({e.Message.Author.Id}) | ");
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                        Logger.WriteLine($"{e.Message.Content}");
                                    }
                                break;
                                case DSharpPlus.ChannelType.Text:
                                    if (Servers)
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        Logger.Write($"[Recieved] ");
                                        Console.ForegroundColor = ConsoleColor.Blue;
                                        Logger.Write($"[Server '{e.Guild.Name}' '#{e.Message.Channel.Name}'] ");
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Logger.Write($"[{DateTime.Now}] ");
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Logger.Write($"{e.Message.Author.Username}#{e.Message.Author.Discriminator} ({e.Message.Author.Id}) | ");
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                        Logger.WriteLine($"{e.Message.Content}");
                                    }
                                    break;
                            }
                         break;
                    }
                }
            }
            return Task.CompletedTask;
        }

        public static Task MessageDeleted(MessageDeleteEventArgs e)
        {
            /* AUTHOR CHECK */
            if (!e.Message.Author.IsBot && e.Message.Author != e.Client.CurrentUser)
            {
                /* MESSAGE TYPE CHECK */
                if (e.Message.MessageType == MessageType.Default)
                {
                    switch (e.Message.Channel.Type)
                    {
                        case DSharpPlus.ChannelType.Private:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Logger.Write($"[Removed] ");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Logger.Write($"[DMs] ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Logger.Write($"[{DateTime.Now}] ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Logger.Write($"{e.Message.Author.Username}#{e.Message.Author.Discriminator} ({e.Message.Author.Id}) | ");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Logger.WriteLine($"{e.Message.Content}");
                            Console.ResetColor();
                            break;
                        case DSharpPlus.ChannelType.Group:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Logger.Write($"[Removed] ");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Logger.Write($"[Group ({e.Message.Channel.Name})] ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Logger.Write($"[{DateTime.Now}] ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Logger.Write($"{e.Message.Author.Username}#{e.Message.Author.Discriminator} ({e.Message.Author.Id}) | ");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Logger.WriteLine($"{e.Message.Content}");
                            break;
                    }
                }
            }
            return Task.CompletedTask;
        }

        public static Task MessageUpdated(MessageUpdateEventArgs e)
        {
                if (!e.Message.Author.IsBot && e.Message.Author != e.Client.CurrentUser)
                {
                    if (e.Message.MessageType == MessageType.Default)
                    {
                        switch (e.Message.Channel.Type)
                        {
                        case DSharpPlus.ChannelType.Private:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Logger.Write($"[Updated] ");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Logger.Write($"[DMs] ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Logger.Write($"[{DateTime.Now}] ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Logger.Write($"{e.Message.Author.Username}#{e.Message.Author.Discriminator} ({e.Message.Author.Id}) | ");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Logger.WriteLine($"{e.Message.Content}");
                            //Console.ForegroundColor = ConsoleColor.Yellow;
                            //Console.Write($" | #{e.Message.Channel.Name} {e.Guild.Name}" + Environment.NewLine);
                            //Console.ResetColor();
                            break;
                        case DSharpPlus.ChannelType.Group:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Logger.Write($"[Updated] ");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Logger.Write($"[Group ({e.Message.Channel.Name})] ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Logger.Write($"[{DateTime.Now}] ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Logger.Write($"{e.Message.Author.Username}#{e.Message.Author.Discriminator} ({e.Message.Author.Id}) | ");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Logger.WriteLine($"{e.Message.Content}");
                            break;
                        }
                    }
                }
            return Task.CompletedTask;
        }

        public static ulong PUlastUserId;
        public static UserStatus PUlastUserStatus;
        public static Task PresenceUpdated(PresenceUpdateEventArgs e)
        {
            if (PresenceUpdatedBool)
            {
                if (!e.Member.IsBot && e.Member != e.Client.CurrentUser)
                    {
                    if (PUlastUserId != e.Member.Id)
                    {
                        PUlastUserId = e.Member.Id;
                        PUlastUserStatus = e.Member.Presence.Status;
                    }
                    else if (PUlastUserStatus != e.Member.Presence.Status)
                    {
                        PUlastUserId = e.Member.Id;
                        PUlastUserStatus = e.Member.Presence.Status;
                    }
                    else
                    {
                        return Task.CompletedTask; // try to prevent multiple guild update
                    }
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Logger.Write($"[Updated] ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Logger.Write($"[Status] ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Logger.Write($"[{DateTime.Now}] ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Logger.Write($"{e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id}) | ");
                        switch (e.Member.Presence.Status)
                            {
                                case UserStatus.Online:
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Logger.WriteLine($"{e.Status}");
                                    break;
                                case UserStatus.DoNotDisturb:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Logger.WriteLine($"{e.Status}");
                                break;
                                case UserStatus.Idle:
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    Logger.WriteLine($"{e.Status}");
                                    break;
                                case UserStatus.Offline:
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Logger.WriteLine($"{e.Status}");
                                    break;
                        }
                }
            }
            return Task.CompletedTask;
        }

        public static Task OnClientError(ClientErrorEventArgs e)
        {
            
            Error($"Error with event '{e.EventName}' | {e.Exception.Message}");
            return Task.CompletedTask;
        }
        public static Task ClientReady(ReadyEventArgs e)
        {
            Console.Clear();
            Startup.ClientName = e.Client.CurrentUser.Username;
            Startup.ClientTag = e.Client.CurrentUser.Discriminator;
            Startup.ClientGuild = e.Client.Guilds.Count;
            Beautify($"Connected on {Startup.ClientName}#{Startup.ClientTag}  | Guilds : {Startup.ClientGuild.ToString()}");

            return Task.CompletedTask;
        }
        public static Task HeartBeatRecieved(HeartbeatEventArgs e)
        {
            if (HeartBeatLogs)
            {
                Beautify($"Heartbeat recieved: {e.Ping}ms");
            }
            return Task.CompletedTask;
        }

        public static Task Beautify(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.Write($"[{DateTime.Now}] ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Logger.WriteLine(message);
            return Task.CompletedTask;
        }
        public static Task Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.Write($"[{DateTime.Now}] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Logger.Write(message + Environment.NewLine);
            return Task.CompletedTask;
        }
    }
}

public static class Logger
{
    public static StringBuilder LogStr = new StringBuilder();
    public static void WriteLine(string str)
    {
        Console.WriteLine(str);
        LogStr.Append(str).Append(Environment.NewLine);
    }
    public static void Write(string str)
    {
        Console.Write(str);
        LogStr.Append(str);
    }
    public static void SaveLog(string Path = "./Log.txt")
    {
        if (LogStr != null && LogStr.Length > 0)
        {
           using (StreamWriter file = File.AppendText(Path))
           {
              file.Write(LogStr.ToString());
              file.Close();
              file.Dispose();
           }
        }
    }
}
