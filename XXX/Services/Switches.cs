namespace Services
{
    public static class Switches
    {
        public static bool SwitchZeroUSersFromDB { get; set; } = false;

        public static event Func<Task> Render;
        public static bool SwitchLoad { get; set; } = true;
        public static bool SwitchGlobalXXX { get; set; } = false;
        public static bool SwitchXXX { get; set; } = true;
        public static bool SwitchXXXChat { get; set; } = false;
        public static bool SwitchChatOnContacts { get; set; } = false;
        public static bool SwitchAboutUser { get; set; } = false;
        public static bool SwitchContacts { get; set; } = false;
        public static bool SwitchSearchUser { get; set; } = false;
        public static bool SwitchAccount { get; set; } = false;
        public static bool switchChatComponent { get; set; } = false;
        public static bool permissionToSwitch { get; set; } = true;            
        public static bool DoubleTach { get; set; } = false;       
        public static void SelectMethodForSwitch(byte number)
        {
            switch (number)
            {
                case 1:
                    OnSwitchXXX();
                    break;
                case 2:
                    SwitchForContact();
                    break;
                case 3:
                    SwitchForAboutUser();
                    break;
                case 4:
                    SwitchForSearchUser();
                    break;
                case 5:
                    SwitchForAccount();
                    break;
            }
            Render.Invoke();
        }
        //Event: переключение главнйю страницу 
        private static void OnSwitchXXX()
        {
            if (SwitchXXXChat) switchChatComponent = true;
            else switchChatComponent = false;

            SwitchXXX = true;
            SwitchContacts = false;
            SwitchAboutUser = false;
            SwitchSearchUser = false;
            SwitchAccount = false;
            SwitchChatOnContacts = false;
        }

        //EVENT: Переключатель между Index & Contacts и contact
        private static void SwitchForContact()
        {
            //проверка с какого компонента зашол
            if (SwitchContacts && switchChatComponent) switchChatComponent = false;
            else if (SwitchContacts) SwitchAboutUser = false;
            else
            {
                SwitchContacts = true;
                SwitchXXX = false;
                SwitchAboutUser = false;
                SwitchSearchUser = false;
                SwitchAccount = false;
                switchChatComponent = false;
            }
        }

        //EVENT: Переключатель между Index & About User
        private static void SwitchForAboutUser()
        {
            if (!SwitchAboutUser)
            {                
                //Переключаюсь на компонент
                SwitchAboutUser = true;
                SwitchXXX = false;
                SwitchContacts = false;
                SwitchSearchUser = false;
                SwitchAccount = false;
            }
        }
        //EVENT: Переключатель между Index & Search User
        private static void SwitchForSearchUser()
        {
            if (!SwitchSearchUser)
            {
                SwitchSearchUser = true;
                SwitchXXX = false;
                SwitchContacts = false;
                SwitchAboutUser = false;
                SwitchAccount = false;
            }
        }

        //EVENT: Переключатель между Index & Account
        private static void SwitchForAccount()
        {
            if (!SwitchAccount)
            {
                SwitchAccount = true;
                SwitchXXX = false;
                SwitchContacts = false;
                SwitchAboutUser = false;
                SwitchSearchUser = false;
            }
        }

        //Event: Double Tach
        public static void DoubleTaches()
        {
            if (DoubleTach)
            {
                SwitchAboutUser = true;
                DoubleTach = false;
                return;
            }

            DoubleTach = true;
            Task.Run(() =>
            {
                Thread.Sleep(500);
                DoubleTach = false;
            });
        }
        //Event: Double Tach Cancel
        public static void DoubleTachesCancel()
        {
            if (DoubleTach)
            {
                //проверка с контактов или с индекса был заход на страницу
                if (!SwitchContacts) SelectMethodForSwitch(1);
                else SelectMethodForSwitch(2);

                DoubleTach = false;
                return ;
            }

            DoubleTach = true;
            Task.Run(() =>
            {
                Thread.Sleep(500);
                DoubleTach = false;
            });
        }

        //Event: Colose or open chat
        public static void CloseOrOpenChat()
        {
            if (permissionToSwitch && !switchChatComponent)
            {
                switchChatComponent = true;
                SwitchXXXChat = true;
            }
            else if (switchChatComponent)
            {
                switchChatComponent = false;
                SwitchXXXChat = false;
            }
        }

        //Evennt Colse chat
        public static void ButtomCloseChat()
        {
            if (SwitchChatOnContacts) SwitchChatOnContacts = false;
            else
            {
                switchChatComponent = false;
                SwitchXXXChat = false;                
            }                
        }
    }
}