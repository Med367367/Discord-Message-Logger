using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DSharpPlus;
using System.Reflection;
using System.Diagnostics;

namespace Discord_Message_Logger.Startup
{
    class Startup : IDisposable
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        static bool consoleShow = true;

        private DiscordClient _discordClient;
        readonly bool disposed = false;

        public static string token = Properties.Settings.Default.Token;
        public static bool Running = false;
        public static TokenType TokenTyp;

        public static NotifyIcon notificationIcon;
        public static ContextMenu contextMenu1;
        public static MenuItem menuItemGithub;
        public static MenuItem menuItemHideAndShow;
        public static MenuItem menuItemSaveLog;

        static ConsoleEventDelegate handler;
        private delegate bool ConsoleEventDelegate(int eventType);


        public static void PreventSleep()
        {
            SetThreadExecutionState(ExecutionState.EsContinuous | ExecutionState.EsSystemRequired);
        }

        public static void AllowSleep()
        {
            SetThreadExecutionState(ExecutionState.EsContinuous);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        [FlagsAttribute]
        private enum ExecutionState : uint
        {
            EsAwaymodeRequired = 0x00000040,
            EsContinuous = 0x80000000,
            EsDisplayRequired = 0x00000002,
            EsSystemRequired = 0x00000001
        }

        #region Running Client Information
        public static string ClientName;
        public static string ClientTag;
        public static int ClientGuild;
        #endregion

        static void Main()
        {
            
            /* Run Config Checks */
            Config.ConfigChecks();

            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            /* Setup NotifIcon Menu */
            contextMenu1 = new ContextMenu();
            menuItemHideAndShow = new MenuItem();
            menuItemSaveLog = new MenuItem();
            menuItemGithub = new MenuItem();
            menuItemHideAndShow.Text = "Hide Console";
            menuItemSaveLog.Text = "Save logs";
            menuItemGithub.Text = "Github";
            contextMenu1.MenuItems.AddRange( new MenuItem[] { menuItemGithub, menuItemHideAndShow, menuItemSaveLog });
            menuItemHideAndShow.Click += new EventHandler(onHideAndShowClick);
            menuItemSaveLog.Click += new EventHandler(onSaveLogClick);
            menuItemGithub.Click += new EventHandler(onGithubClick);

            /*  NotifIcon Setup */
            Thread trayThread = new Thread(
            delegate ()
            {
                Icon appIcon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
                notificationIcon = new NotifyIcon()
                {
                    Icon = appIcon,
                    Text = "Discord Message Logger",
                    ContextMenu = contextMenu1
                };

                notificationIcon.Visible = true;
                Application.Run();
            }
            );


            trayThread.Start();

            PreventSleep();
            Menu();
        }

        private static void onGithubClick(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Med367367/Discord-Message-Logger");
        }
        private static void onHideAndShowClick(object sender, EventArgs e)
        {

            var handle = GetConsoleWindow();
            if (consoleShow)
            {
                consoleShow = false;
                ShowWindow(handle, SW_HIDE);
                notificationIcon.ShowBalloonTip(100, "Console Hidden", "The console has been hidden in the background", ToolTipIcon.Info);
                menuItemHideAndShow.Text = "Show Console";
            }
            else
            {
                consoleShow = true;
                ShowWindow(handle, SW_SHOW);
                notificationIcon.ShowBalloonTip(100, "Console Displayed", "The console is now visible", ToolTipIcon.Info);
                menuItemHideAndShow.Text = "Hide Console";
            }
        }

        private static void onSaveLogClick(object sender, EventArgs e)
        {
            if (Running)
            {
                DialogResult diaResult = MessageBox.Show("Do you really want save the logs?" + Environment.NewLine + "All the logs will be saved in a file named customSave", "Saving ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (diaResult == DialogResult.Yes)
                {
                    Config.ConfigSaveSetup(); /* Run Config Save Checks */
                    string ClientFullName = ClientName + "#" + ClientTag;
                    Logger.SaveLog(Config.SaveRoot + "/" + TokenTyp + "/" + ClientFullName + "/" + Convert.ToString(DateTime.Now.Year + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute) + ".txt");
                    MessageBox.Show("Logs saved !", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Before trying to save, please start the program", "Nothing to save", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2) // Console window closing
            {
                notificationIcon.Icon = null;
                notificationIcon.Visible = false;
                notificationIcon.Dispose();
                Application.Exit();
                if (Properties.Settings.Default.SaveOnClosing)
                {
                    Config.ConfigSaveSetup(); /* Run Config Save Checks */
                    string ClientFullName = ClientName + "#" + ClientTag;
                    Logger.SaveLog(Config.SaveRoot + "/" + TokenTyp + "/"+ ClientFullName + "/" + Convert.ToString(DateTime.Now.Year + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute) + ".txt");
                }
                AllowSleep();
            }
            return false;
        }

        static void EditSave(string settings)
        {
            Properties.Settings.Default[settings] = !Convert.ToBoolean(Properties.Settings.Default[settings]);
            Properties.Settings.Default.Save();
        }

        static string getSettingsValue(string settings)
        {
            return Convert.ToString(Properties.Settings.Default[settings]);
        }
        static void Menu()
        {
            Console.Clear();
            Console.Title = "Discord Message Logger : Menu";
            /* CREATE REFERENCE TO STARTUP */
            Startup startupClass = new Startup();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Github > github.com/Med367367/Discord-Message-Logger");
            Console.ForegroundColor = ConsoleColor.Gray;
            bool StartChoose = true;
            while (StartChoose)
            {
                Console.WriteLine("Please Choose Option (Write 0 at any time to return to this menu)");
                Console.WriteLine("1. Start the Logger with bot token");
                Console.WriteLine("2. Start the Logger with user token");
                Console.WriteLine("3. Set the token");
                Console.WriteLine("4. Display the current token");
                Console.WriteLine("5. Settings");
                string UserChoose = Console.ReadLine();
                switch (UserChoose)
                {
                    case "1":
                        StartChoose = false;
                        Console.Clear();
                        /* START ASYNC CODE */
                        startupClass.Run(TokenType.Bot).ConfigureAwait(false).GetAwaiter().GetResult();

                        break;
                    case "2":
                        StartChoose = false;
                        Console.Clear();
                        /* START ASYNC CODE */
                        startupClass.Run(TokenType.User).ConfigureAwait(false).GetAwaiter().GetResult();
                        break;
                    case "3":
                        
                        StartChoose = false;
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Please enter your new token");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        string NewToken = Console.ReadLine();
                        if (Properties.Settings.Default.Encrypt) // If encrypt settings enabled
                        {
                            Properties.Settings.Default.Token = Crypter.Encrypt(NewToken, Config.GetUniqueMachineId());
                        }
                        else // encrypt settings disabled
                        {
                            Properties.Settings.Default.Token = NewToken;
                        }
                        Properties.Settings.Default.Save();
                        Application.Restart();
                        break;
                    case "4":
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("The current token is : " + token);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case "5":
                        StartChoose = false;
                        Settings();
                        break;
                    default:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(UserChoose + " is not valid");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                }


            }

        }

        static void Settings()
        {
            Console.Clear();
            Console.Title = "Discord Message Logger : Settings";
            bool StartChoose = true;
            while (StartChoose)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("0. Back to the menu");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("--- APP CONFIG ---");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("[Save]. Save on closing");
                Console.WriteLine("[Encrypt]. Encrypt the saved token");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("--- LOGS ---");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("[1]. HeartBeatLogs");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("--- TYPES ---");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("[2]. DMs");
                Console.WriteLine("[3]. Groups");
                Console.WriteLine("[4]. Servers");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("--- CALL ---");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("[5]. Message Recieved");
                Console.WriteLine("[6]. Message Removed");
                Console.WriteLine("[7]. Message Updated");
                Console.WriteLine("[8]. Presence Updated");
                string UserChoose = Console.ReadLine();
                switch (UserChoose.ToLower())
                {
                    case "0":
                        StartChoose = false;
                        Console.Clear();
                        Menu();
                        break;
                    case "save":
                        Console.Clear();
                        EditSave("SaveOnClosing");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Save On Closing set to " + getSettingsValue("SaveOnClosing"));
                        break;
                    case "encrypt":
                        Console.Clear();
                        EditSave("Encrypt");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Encrypt token set to " + getSettingsValue("Encrypt"));
                        if (Properties.Settings.Default.Encrypt)
                        {
                            Properties.Settings.Default.Token = Crypter.Encrypt(Properties.Settings.Default.Token, Config.GetUniqueMachineId());
                        }
                        else
                        {
                            Properties.Settings.Default.Token = Crypter.Decrypt(Properties.Settings.Default.Token, Config.GetUniqueMachineId());
                        }
                        Properties.Settings.Default.Save();
                        break;
                    case "1": // HeartBeatLogs
                        Console.Clear();
                        EditSave("HeartBeatLogs");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("HeartBeatLogs set to " + getSettingsValue("HeartBeatLogs"));
                        break;
                    case "2":
                        Console.Clear();
                        EditSave("DMs");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("DMs set to " + getSettingsValue("DMs"));
                        break;
                    case "3":
                        Console.Clear();
                        EditSave("Groups");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Groups set to " + getSettingsValue("Groups"));
                        break;
                    case "4":
                        Console.Clear();
                        EditSave("Servers");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Servers set to " + getSettingsValue("Servers"));
                        break;
                    case "5":
                        Console.Clear();
                        EditSave("MessageRecievedBool");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Message Recieved set to " + getSettingsValue("MessageRecievedBool"));
                        break;
                    case "6":
                        Console.Clear();
                        EditSave("MessageRemovedBool");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Message Removed set to " + getSettingsValue("MessageRemovedBool"));
                        break;
                    case "7":
                        Console.Clear();
                        EditSave("MessageUpdatedBool");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Message Updated set to " + getSettingsValue("MessageUpdatedBool"));
                        break;
                    case "8":
                        Console.Clear();
                        EditSave("PresenceUpdatedBool");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Presence Updated set to " + getSettingsValue("PresenceUpdatedBool"));
                        break;
                    default:
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(UserChoose + " is not valid");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                }


            }

        }
        async Task Run(TokenType tkType)
        {
            Console.Title = "Discord Message Logger : Running ["+tkType+"]";
            Console.Clear();
            notificationIcon.ShowBalloonTip(100, "Starting", "The program will start with the [" + tkType + "] mod", ToolTipIcon.Info);
            Running = true;
            TokenTyp = tkType;
            try
            {
                var clientConfig = new DiscordConfiguration
                {
                    Token = token,
                    TokenType = tkType,
                };


                /* INIT DISCORDCLIENT */
                _discordClient = new DiscordClient(clientConfig);

                /* HOOK EVENTS */
                _discordClient.Ready += Logging.ClientReady;
                _discordClient.Heartbeated += Logging.HeartBeatRecieved;


                _discordClient.MessageCreated += Logging.MessageRecieved;
                _discordClient.MessageDeleted += Logging.MessageDeleted;
                _discordClient.MessageUpdated += Logging.MessageUpdated;
                _discordClient.PresenceUpdated += Logging.PresenceUpdated;

                _discordClient.ClientErrored += Logging.OnClientError;

                await _discordClient.ConnectAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Running = false;
                Console.Title = "Discord Message Logger : Error [" + tkType + "]";
                notificationIcon.ShowBalloonTip(3000, "Error", "The program has reported an error, look at the console for more information", ToolTipIcon.Error);
                Console.ReadLine();
                Environment.Exit(0);
            }
            await Task.Delay(-1);
        }

        /* DISPOSE OF MANAGED RESOURCES */
        public void Dispose()
        {
            
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                Run(TokenType.Bot).Dispose();
            }

        }
    }
}


