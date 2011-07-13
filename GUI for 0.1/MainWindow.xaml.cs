using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using EzySystem_Class;
using System.Windows.Media.Animation;
using System.Timers;


namespace GUI_for_0._1
{
       
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Object Instantiation
        EzySystem systemInstance = new EzySystem();
        private string mobilenumber;
        private System.Timers.Timer Visibilitytimer = new System.Timers.Timer(800);
        private System.Timers.Timer NotificationTimer = new System.Timers.Timer(2000);
        private System.Timers.Timer AnimationTimer = new System.Timers.Timer(500);
        private System.Timers.Timer HideUserUITimer = new System.Timers.Timer(800);
        private Point origin;
        private Point start;
        private double curzoom = 0.0;
        private double zoomFactorWidth;
        private double zoomFactorHeight;

        //animation flags
        private bool LegendGridup = false;
        private bool userStatusUp = false;
        private bool busInfoUp = false;

        public MainWindow()
        {
            InitializeComponent();
            
            //TextUI UI = new TextUI();

            // ASSUMPTION:  
            // 1. ALL BUSES START AT 0:00 TIME
            // 2. AT 0:00, EACH BUS IS AT THEIR FIRST BUS STOP
            systemInstance.Init();
            MainWindow1.Visibility = Visibility.Visible;
            UserUI.Visibility = Visibility.Hidden;
            AdminUI.Visibility = Visibility.Hidden;
            Register.Visibility = Visibility.Hidden;
            UserLoggedIn.Visibility = Visibility.Hidden;
            SimulationUI.Visibility = Visibility.Hidden;

            zoomFactorWidth = BusStopButtonLayer.Width * 0.2;
            zoomFactorHeight = BusStopButtonLayer.Height * 0.2;

            TransformGroup group = new TransformGroup();

            ScaleTransform xform = new ScaleTransform();
            group.Children.Add(xform);

            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);

            Map.RenderTransform = group;

            Map.MouseWheel += image_MouseWheel;
            Map.MouseLeftButtonDown += image_MouseLeftButtonDown;
            Map.MouseLeftButtonUp += image_MouseLeftButtonUp;
            Map.MouseMove += image_MouseMove;

            return;
        }

        private void HideAll()
        {
            MainWindow1.Visibility = Visibility.Hidden;
            UserUI.Visibility = Visibility.Hidden;
            AdminUI.Visibility = Visibility.Hidden;
            Register.Visibility = Visibility.Hidden;
            UserLoggedIn.Visibility = Visibility.Hidden;
            SimulationUI.Visibility = Visibility.Hidden;

        }

        private void ExitProgram(object sender, EventArgs e)
        {
            systemInstance.SaveAllData();
        }

//--------------------------------------------------------Main Menue--------------------------------------------------------\\

        private void UserUIButton_Click(object sender, RoutedEventArgs e)
        {

            UserUI.Visibility = Visibility.Visible;
            Register.Visibility = Visibility.Hidden;

            UserUIButton.CaptureMouse();

           //Animating Simulation Button
            animationTranslateY(SimulationButton, 0, 30, 1000);
            animationFade(SimulationButton, SimulationButton.Opacity, 0, 500);
            

           //Animating Admin Button
            animationTranslateX(AdminUIButton, 0, -50, 1000);
            animationFade(AdminUIButton, AdminUIButton.Opacity, 0, 300);

           //Animating User Button
            animationXandY(UserUIButton, -2, -16, 1000, -2, -26, 1000);
            animationFade(UserUIButton, UserUIButton.Opacity, 0, 300);

            //animating menuSelectGrid fade
            animationFadeGrid(MenuSelectGrid, 1, 0, 300);

            //animate userUI
            animationFadeGrid(UserUI, 0, 1, 1000);
            animationTranslateXText(LoginText, 50, 0, 1000);
            animationTranslateX(LoginButton1, 20, 0, 1000);
            animationTranslateX(RegisterButton1, -20, 0, 1000);

            //End of Animation

            //Timer to set visibility to 0
            this.Visibilitytimer.Elapsed += new System.Timers.ElapsedEventHandler(this.Visibilitytimer_Tick);
            Visibilitytimer.AutoReset = false;
            Visibilitytimer.Enabled = true;
            Visibilitytimer.Start();
            //end of timer code
        }

        private void UserUIButton_MouseEnter(object sender, MouseEventArgs e)
        {
            animationXandY(UserUIButton, 0, -2, 200, 0, -2, 200);

            animationFade(UserUIButton, (double)UserUIButton.Opacity, 1, 200);

            MenuSelectionBox.Content = "User";
        }

        private void UserUIButton_MouseLeave(object sender, MouseEventArgs e)
        {           
            animationXandY(UserUIButton, -2, 0, 200, -2, 0, 200);
            animationFade(UserUIButton, UserUIButton.Opacity, .75, 200);

            MenuSelectionBox.Content = null;
        }

        private void AdminUIButton_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow1.Visibility = Visibility.Hidden;
            UserUI.Visibility = Visibility.Hidden;
            AdminUI.Visibility = Visibility.Visible;
            T_VALTextbox.Text = systemInstance.GetTVAL().ToString();
            T_COSTTextbox.Text = systemInstance.GetTCOST().ToString();

            AdminUIButton.CaptureMouse();

            //Animating Simulation Button
            animationTranslateY(SimulationButton, 0, 30, 1000);
            animationFade(SimulationButton, SimulationButton.Opacity, 0, 500);

            //Animating Admin Button
            animationTranslateX(AdminUIButton, -2, -16, 1000);
            animationFade(AdminUIButton, AdminUIButton.Opacity, 0, 300);

            //Animating User Button
            animationXandY(UserUIButton, 0, -30, 1000, 0, -30, 1000);
            animationFade(UserUIButton, UserUIButton.Opacity, 0, 300);

            //animating menuSelectGrid fade
            animationFadeGrid(MenuSelectGrid, 1, 0, 300);

            //animating adminUI grid
            AdminUIGrid.Visibility = Visibility.Hidden;
            animationTranslateXGrid(AdminUI, 400, 0, 1000);
            animationTranslateXGrid(AdminLogInGrid, 0, 0, 0);
            animationFadeGrid(AdminUI, 0, 1, 1500);
            animationFadeGrid(AdminLogInGrid, 0, 1, 1500);
            animationTranslateXText(AdministratorTitle, 100, 0, 2200);

            //End of Animation

            //Timer to set visibility to 0
            this.Visibilitytimer.Elapsed += new System.Timers.ElapsedEventHandler(this.Visibilitytimer_Tick);
            Visibilitytimer.AutoReset = false;
            Visibilitytimer.Enabled = true;
            Visibilitytimer.Start();
            //end of timer code
            
            // Update bus stops in 2 Combo Box
            List<string> busstop = GetBusStopsList();
            FromComboBox.ItemsSource = busstop;
            ToComboBox.ItemsSource = busstop;

            List<string> bus = GetBusList();
            BusServiceComboBox2.ItemsSource = bus;
			BusStopCoOrdComboBox.ItemsSource = busstop;

            BusRouteDispComboBox.ItemsSource = bus;
			
			List<string> user = systemInstance.FindMobileNumberList();
			user.Sort();
			mobileJourneyTextBox.ItemsSource = user;
			
			
        }

        private void AdminUIButton_MouseEnter(object sender, MouseEventArgs e)
        {
            animationTranslateX(AdminUIButton, 0, -2, 200);
            animationFade(AdminUIButton, (double)AdminUIButton.Opacity, 1, 200);

            MenuSelectionBox.Content = "Administrator";
        }

        private void AdminUIButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation AdminUIButtonAnimationX = new DoubleAnimation();
            TranslateTransform AdminUITranslateX = new TranslateTransform();

            animationTranslateX(AdminUIButton, -2, 0, 200);
            animationFade(AdminUIButton, (double)AdminUIButton.Opacity, 0.75, 200);

            MenuSelectionBox.Content = null;
        }

        private void MainMenuSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow1.Visibility = Visibility.Hidden;
            UserUI.Visibility = Visibility.Hidden;
            AdminUI.Visibility = Visibility.Hidden;
            SimulationManagerGrid.Visibility = Visibility.Hidden;
            SimulationGrid.Visibility = Visibility.Visible;
            SimulationUI.Visibility = Visibility.Visible;

            SimulationButton.CaptureMouse();

            //remove then render busstops
            for (int i = 0; BusStopButtonLayer.Children.Count != 0; i++)
                BusStopButtonLayer.Children.RemoveAt(0);

            renderBusStops();
            
            List<string> busstop = GetBusStopsList();
            List<string> bus = GetBusList();
            StartingBusStop.ItemsSource = busstop;
            BusService2.ItemsSource = bus;
            NextBusInterruptComboBox.ItemsSource = busstop;
            BusInterruptComboBox.ItemsSource = bus;
			List<string> user = systemInstance.FindMobileNumberList();
			user.Sort();
			UserMobileNoTextbox.ItemsSource = user;

//Animating Simulation Button            
            animationTranslateY(SimulationButton, 2, 30, 1000);
            animationFade(SimulationButton, SimulationButton.Opacity, 0, 500);

//Animating Admin Button            
           animationTranslateX(AdminUIButton, 0, -30, 1000);
            animationFade(AdminUIButton, AdminUIButton.Opacity, 0, 300);

//Animating User Button            
            animationXandY(UserUIButton, 0, -30, 1000, 0, -30, 1000);
            animationFade(UserUIButton, UserUIButton.Opacity, 0, 300);

//animating menuSelectGrid fade
            animationFadeGrid(MenuSelectGrid, 1, 0, 300);

            //Animating left panle
            animationTranslateYGrid(LeftPanelGrid, -500, 0, 1200);
            animationFadeGrid(LeftPanelGrid, 0, 1, 1700);

            //Animating Bottom panel
            animationTranslateXGrid(BottomPanelGrid1, -300, 0, 1200);
            animationFadeGrid(BottomPanelGrid1, 0, 1, 1700);

            //animating MapGrid
            animationTranslateXGrid(MapGrid, 500, 0, 1200);
            animationFadeGrid(MapGrid, 0, 1, 1700);

            //animating logo
            DoubleAnimation LogoAnimationFade = new DoubleAnimation();
            LogoAnimationFade.From = 0;
            LogoAnimationFade.To = 1;
            LogoAnimationFade.Duration = new Duration(TimeSpan.FromMilliseconds(1200));
            //SimulationLogo.BeginAnimation(Image.OpacityProperty, LogoAnimationFade);

            //animating Simulation Grid
            animationTranslateXGrid(SimulationGrid, 700, 0, 1200);
            animationFadeGrid(SimulationGrid, 0, 1, 1200);
            //end of animation
            
            //Timer to set visibility to 0
            this.Visibilitytimer.Elapsed += new System.Timers.ElapsedEventHandler(this.Visibilitytimer_Tick);
            Visibilitytimer.AutoReset = false;
            Visibilitytimer.Enabled = true;

            Visibilitytimer.Start();
            //end of timer code            
        }

        private void Visibilitytimer_Tick(object sender, EventArgs e)
        {
            MainWindow1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
            delegate()
            {
                MainWindow1.Visibility = Visibility.Hidden;
            }));
        }
//--------------------------------------------------------Main Menue End--------------------------------------------------------\\


//--------------------------------------------------------Methods--------------------------------------------------------\\
        private List<string> GetBusStopsList()
        {
            List<string> busstop = new List<string>();
            for (int i = 0; i < systemInstance.GetBusStopListLength(); i++)
            {
                busstop.Add(systemInstance.GetBusStopStringAtIndex(i));
            }
            busstop.Sort();
            return busstop;
        }

        private List<string> GetBusList()
        {
            List<string> bus = new List<string>();
            for (int i = 0; i < systemInstance.GetBusListLength(); i++)
            {
                bus.Add(systemInstance.GetBusStringAtIndex(i));
            }
            bus.Sort();
            return bus;
        }

        //private List<string> GetpassengerList(string busName)
        //{
        //    List<string> passenger = new List<string>();
        //    for (int i = 0; i < systemInstance.GetBusList().FindBusWithBusName(busName).GetPassengerList().Count; i++)
        //    {
        //        passenger.Add(systemInstance.FindUserWithMobileNumber(systemInstance.GetBusList().FindBusWithBusName(busName).GetPassengerList().ElementAt(i).GetMobileNum()));
        //    }
        //    return passenger;
        //}

        private List<string> GetBusQList(string busStopName)
        {
            List<string> bus = new List<string>();
            for (int i = 0; i < systemInstance.GetBusStopList().FindBusStopWithBusStopName(busStopName).GetBusQueue().Count; i++)
            {
                bus.Add(systemInstance.GetBusStopList().FindBusStopWithBusStopName(busStopName).GetBusQueue()[i]);
            }
            return bus;
        }

        private List<string> GetPassengerAtBusStopQList(string busStopName)
        {
            List<string> passenger = new List<string>();
            for (int i = 0; i < systemInstance.GetBusStopList().FindBusStopWithBusStopName(busStopName).GetPassengerList().Count; i++)
            {
                passenger.Add(systemInstance.FindUserWithMobileNumber(systemInstance.GetBusStopList().FindBusStopWithBusStopName(busStopName).GetPassengerList()[i].GetMobileNum()));
            }
            return passenger;
        }

        private void simulationManagerNotification(String notification)
        {
            NotificationText.Text = notification;

            //animate notification
            animationTranslateXGrid(TripStartedGrid, 0, -140, 800);

            //timer
            this.NotificationTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.TripStartedHidetimer_Tick);
            NotificationTimer.AutoReset = false;
            NotificationTimer.Enabled = true;
            NotificationTimer.Start();
        }

        private void TripStartedHidetimer_Tick(object sender, EventArgs e)
        {
            MainWindow1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
            delegate()
            {
                animationTranslateXGrid(TripStartedGrid, -140, 0, 800);
            }));
        }

        private void UserCovertimer_Tick(object sender, EventArgs e)
        {
            MainWindow1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
            delegate()
            {
                userStatusUp = true;
            }));
        }

        private void HideUserUI_Tick(object sender, EventArgs e)
        {
            MainWindow1.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
            delegate()
            {
                UserUI.Visibility = Visibility.Hidden;
            }));
        }

//--------------------------------------------------------Methods End--------------------------------------------------------\\

        private void RegisterUI(object sender, RoutedEventArgs e)
        {
            //MainWindow1.Visibility = Visibility.Hidden;
            //UserUI.Visibility = Visibility.Hidden;
            //AdminUI.Visibility = Visibility.Hidden;
            Register.Visibility = Visibility.Visible;

            //animating uerUI 
            animationFadeGrid(UserUI, 1, 0, 1000);
            animationTranslateXText(LoginText, 0, 80, 1000);
            //animating registerUI
            animationTranslateXText(RegisterText, 80, 0, 1000);
            animationTranslateX(RegisterButton, 80, 0, 1500);
            animationFadeGrid(Register, 0, 1, 1500);

            this.HideUserUITimer.Elapsed += new System.Timers.ElapsedEventHandler(this.HideUserUI_Tick);
            HideUserUITimer.AutoReset = false;
            HideUserUITimer.Enabled = true;
            HideUserUITimer.Start();

        }
        private void RegisterUser(object sender, RoutedEventArgs e)
        {
            string name = namebox.Text;
            mobilenumber = MobileBox.Text ;
            string creditcardnumber = CCBox.Text ;
            string paymentmode1 = Payment1Box.Text;
            string paymentmode2 = Payment2Box.Text;
            string password = RegisterPassBox.Password;
            PAYMENT_MODE paymentmode_1, paymentmode_2;
			// Exception
			
            if (!(name != "" && mobilenumber != "" && creditcardnumber != "" && paymentmode1 != "" && paymentmode2 != "" && password != ""))
            {
                MessageBox.Show("Error! All fields have to be filled");
                return;
                ;
            }
            else if (!(CheckDigit(mobilenumber) == true && CheckDigit(creditcardnumber) == true))
            {
                MessageBox.Show("Error! Numerical values required for Mobile/Credit Card");
                return;
            }
            else if (paymentmode1 == paymentmode2)
            {
                MessageBox.Show("Error! 2 Payment Modes must be different");
                return;
            }
			else if (systemInstance.CheckDuplicateMobileNumber(mobilenumber)==true)
            {
                MessageBox.Show("Error! Mobile Number already existed");
                return;
            }
            else
				{
	
				if (paymentmode1 == "Credit") paymentmode_1 = PAYMENT_MODE.credit;
				else if (paymentmode1 == "Account") paymentmode_1 = PAYMENT_MODE.account;
				else paymentmode_1 = PAYMENT_MODE.mobile;
	
				if (paymentmode2 == "Credit") paymentmode_2 = PAYMENT_MODE.credit;
				else if (paymentmode2 == "Account") paymentmode_2 = PAYMENT_MODE.account;
				else paymentmode_2 = PAYMENT_MODE.mobile;
	
				systemInstance.CreateUser(name, mobilenumber, 0, creditcardnumber, paymentmode_1, paymentmode_2, password);
				MessageBox.Show("Thank you for registering.");
				}

        }

            private bool CheckDigit(string text)
            {
                foreach (char ch in text)
                 if (!(Char.IsDigit(ch)))
                     return false;
                return true;
            }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            
            mobilenumber = MobileNoBox.Text;
            string password = PasswordBox1.Password;
			MobileNoBox.Clear();
			PasswordBox1.Clear();
            if (mobilenumber == "" || password == "")
            {
                MessageBox.Show("Error! Please Enter User Name and Password");
                return;
            }

            if (systemInstance.CheckDuplicateMobileNumber(mobilenumber) == false)
            {
                MessageBox.Show("Error! User does not exist");
                return;
            }    

            string currentusername = systemInstance.FindUserWithMobileNumber(mobilenumber);
            bool login = systemInstance.CheckPassWordLogin(mobilenumber, password);
            
            if (login==true)
            {
				
			TopupCardNumTextBox.Text = "";
			PaymentMode2ComboBox.Text = null;
			PaymentMode1ComboBox.SelectedItem = null;
			SubBusSMSComboBox.SelectedItem=null;
			SubBusStopSMSComboBox.SelectedItem = null;
			UnsubBusSMSComboBox.SelectedItem = null;
			SubBusStopSMSComboBox.SelectedItem = null;
				
            // if LOGIN go through
            UserWelcomeText.Text = "Welcome back, "+ systemInstance.FindUserWithMobileNumber(mobilenumber).ToString();
            LogTextBox.Text ="Your current balance is "+ systemInstance.FindAccountWithMobileNumber(mobilenumber)+ " SGD\n";
			LogTextBox.Text +="Your Primary mode of payment is "+ systemInstance.FindPaymentMode1WithMobileNumber(mobilenumber) + " and your secondary mod of payment mode is "+ systemInstance.FindPaymentMode2WithMobileNumber(mobilenumber) + "\n";
            UserLoggedIn.Visibility = Visibility.Visible;
            DispCCNum.Text = systemInstance.FindCreditCardNumberWithMobileNumber(mobilenumber);
            List<string> busstop = GetBusStopsList();
            SubBusStopSMSComboBox.ItemsSource = busstop;
            UnsubBusstopSMSComboBox.ItemsSource = busstop;

            List<string> bus = GetBusList();
            SubBusSMSComboBox.ItemsSource = bus;
            UnsubBusSMSComboBox.ItemsSource = bus;

                //animating uerUI 
            animationFadeGrid(UserUI, 1, 0, 1000);
            animationTranslateXText(LoginText, 0, 80, 1000);
                //animating loggedinUI
            animationTranslateXGrid(UserLoggedIn, 800, 0, 1800);
            animationFadeGrid(UserLoggedIn, 0, 1, 1800);
            animationTranslateXText(UserWelcomeText, 250, 0, 2000);
            animationTranslateX(LogOutButton, 100, 0, 2500);

            this.HideUserUITimer.Elapsed += new System.Timers.ElapsedEventHandler(this.HideUserUI_Tick);
            HideUserUITimer.AutoReset = false;
            HideUserUITimer.Enabled = true;
            HideUserUITimer.Start();

            //string payment1 = systemInstance.FindPaymentMode1WithMobileNumber(mobilenumber);
            //string payment2 = systemInstance.FindPaymentMode2WithMobileNumber(mobilenumber);

            //PaymentMode1ComboBox.Text = payment1;
            //PaymentMode2ComboBox.Text = payment2;
            }
            else
            {
                MessageBox.Show("Username and password combination incorrect. Please try again");
            }
        }

        private void TopUpPrepaidCard(object sender, RoutedEventArgs e)
        {
            if (TopupCardNumTextBox.Text == "")
            {
                MessageBox.Show("Please enter Pre-paid Card Number");
                return;
            }
            int CardNumber = Int32.Parse(TopupCardNumTextBox.Text);
            int value = systemInstance.TopUp(mobilenumber, CardNumber);
            if (value == 0)
                MessageBox.Show("Invalid Pre-paid Card Number");
            else
            {
     
                LogTextBox.Text = "Your account has been successfully topup " + value + " SGD.\n"+"Your Current Balance is " + systemInstance.FindAccountWithMobileNumber(mobilenumber) + " SGD";
            }
        }

        private void UpdateCCNoClick(object sender, RoutedEventArgs e)
        {
            if (DispCCNum.Text=="")
			{
				MessageBox.Show("Please enter Credit Card Number");
				return;
			}
			
			string newcreditcardnumber = DispCCNum.Text;
            systemInstance.UpdateUserCreditCardNumber(mobilenumber, newcreditcardnumber);
            MessageBox.Show("Your Credit Card number has been updated");
        }

        private void UpdatePM1Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PaymentMode1ComboBox.Text == "")
            {
                MessageBox.Show("Please select a payment mode");
                return;
            }
            PAYMENT_MODE paymentmode_1;
            if (PaymentMode1ComboBox.Text == "Credit") paymentmode_1 = PAYMENT_MODE.credit;
            else if (PaymentMode1ComboBox.Text == "Account") paymentmode_1 = PAYMENT_MODE.account;
            else paymentmode_1 = PAYMENT_MODE.mobile;

            systemInstance.UpdateUserFirstPaymentMethod(mobilenumber, paymentmode_1);
        }

        private void UpdatePM2Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            if (PaymentMode2ComboBox.Text == "")
            {
                MessageBox.Show("Please select a payment mode");
                return;
            }
            PAYMENT_MODE paymentmode_2;

            if (PaymentMode2ComboBox.Text == "Credit") paymentmode_2 = PAYMENT_MODE.credit;
            else if (PaymentMode2ComboBox.Text == "Account") paymentmode_2 = PAYMENT_MODE.account;
            else paymentmode_2 = PAYMENT_MODE.mobile;
            systemInstance.UpdateUserSecondPaymentMethod(mobilenumber, paymentmode_2);
        }
		
		private void SubBusSMSClick(object sender, RoutedEventArgs e)
        {	if (SubBusSMSComboBox.Text == "")
            {
                MessageBox.Show("Please select a bus service");
                return;
            }
			
            systemInstance.AddBusServiceSubscriber(mobilenumber, SubBusSMSComboBox.Text);
            MessageBox.Show("Bus SMS subscription added");
        }

        private void SubBusStopSMSClick(object sender, RoutedEventArgs e)
        {	
			if (SubBusStopSMSComboBox.Text == "")
            {
                MessageBox.Show("Please select a bus stop");
                return;
            }
            systemInstance.AddBusStopSubscriber(mobilenumber, SubBusStopSMSComboBox.Text);
            MessageBox.Show("Bus Stop SMS aubscription added");
        }

        private void UnsubBusSMSClick(object sender, RoutedEventArgs e)
        {	
			if (UnsubBusSMSComboBox.Text == "")
            {
                MessageBox.Show("Please select a bus service");
                return;
            }
            systemInstance.RemoveBusServiceSubscriber(mobilenumber, UnsubBusSMSComboBox.Text);
            MessageBox.Show("Bus SMS subscription removed");
        }

        private void UnsubBusstopSMSClick(object sender, RoutedEventArgs e)
        {
			if (UnsubBusSMSComboBox.Text == "")
            {
                MessageBox.Show("Please select a bus stop");
                return;
            }
            systemInstance.RemoveBusStopSubscriber(mobilenumber, UnsubBusSMSComboBox.Text);
            MessageBox.Show("Bus Stop SMS subscription removed");
        }
		
		
        private void BacktoUserPageClick(object sender, RoutedEventArgs e)
        {
            //MainWindow1.Visibility = Visibility.Hidden;
            UserUI.Visibility = Visibility.Visible;
			
            //AdminUI.Visibility = Visibility.Hidden;
            //Register.Visibility = Visibility.Hidden;
            //UserLoggedIn.Visibility = Visibility.Hidden;
            //animate loggedinUI
            animationTranslateXGrid(UserLoggedIn, 0, 800, 1500);
            animationFadeGrid(UserLoggedIn, 1, 0, 1500);

            //animate userUI
            animationFadeGrid(UserUI, 0, 1, 1000);
            animationTranslateXText(LoginText, 50, 0, 1000);
            animationTranslateX(LoginButton1, 20, 0, 1000);
            animationTranslateX(RegisterButton1, -20, 0, 1000);
        }

        private void HomeClick(object sender, RoutedEventArgs e)
        {
            MainWindow1.Visibility = Visibility.Visible;
            //UserUI.Visibility = Visibility.Hidden;
            //AdminUI.Visibility = Visibility.Hidden;
            //Register.Visibility = Visibility.Hidden;
            //UserLoggedIn.Visibility = Visibility.Hidden;
            //SimulationUI.Visibility = Visibility.Hidden;
          
            Button home = (Button)sender;
            switch (home.Name)
            {
                case "HomeButton_Simulation":
                    animationTranslateXGrid(SimulationGrid, 0, 800, 1500);
                    break;
                case "HomeButton_SimulationManager":
                    animationTranslateXGrid(SimulationManagerGrid, 0, 800, 1500);
                    break;
                case "HomeButton_UserLoggedInUI":
                    animationTranslateXGrid(UserLoggedIn, 0, 800, 1500);
                    break;
                case "HomeButton_RegisterUI":
                    animationFadeGrid(Register, 1, 0, 1000);
                    animationTranslateXText(RegisterText, 0, 50, 1000);
                    break;
                case "HomeButton_AdminUI":
                    animationTranslateXGrid(AdminUI, 0, 800, 1500);
                    break;
                case "HomeButton_AdminLogin":
                    animationTranslateXGrid(AdminUI, 0, 800, 1500);
                    break;
                case "HomeButton_UserUI":
                    animationFadeGrid(UserUI, 1, 0, 1000);
                    animationTranslateXText(LoginText, 0, 50, 1000);
                    break;
            }

            //clear everything
            mobilenumber = null;

            //// UserUI
            MobileNoBox.Clear();
            PasswordBox1.Clear();

            //Animating Simulation Button
            animationTranslateY(SimulationButton, 30, 0, 1000);
            animationFade(SimulationButton, 0, 0.75, 2500);

            //Animating Admin Button
            animationTranslateX(AdminUIButton, -30, 0, 1000);
            animationFade(AdminUIButton, 0, 0.75, 2500);

            //Animating User Button
            animationXandY(UserUIButton, -30, 0, 1000, -30, 0, 1000);
            animationFade(UserUIButton, 0, 0.75, 2500);

            //animating menuSelectGrid appear
            animationFadeGrid(MenuSelectGrid, 0, 1, 300);

            //End of Animation
        }

        

        private void AddBusStopClick(object sender, RoutedEventArgs e)
        {
            string name = AddBusStopTextBox.Text;
			string xcoord = XCoordTextBox.Text;
            string ycoord = YCoordTextBox.Text;
			
			if (name==""||xcoord==""||ycoord=="") 
			{
				MessageBox.Show("Please ensure bus stop information is entered");
				return;
			}
            systemInstance.CreateBusStop(name,Convert.ToDouble(xcoord),Convert.ToDouble(ycoord));
            simulationManagerNotification("Bus stop added");
            List<string> busstop = GetBusStopsList();
            FromComboBox.ItemsSource = busstop;
            ToComboBox.ItemsSource = busstop; 
			
        }

        private void AddBusSvcClick(object sender, RoutedEventArgs e)
        {
            string name = AddBusSvcTextBox.Text;
            string frequency = FrequencyTextBox.Text;
			
			if (name==""||frequency=="") 
			{
				MessageBox.Show("Please ensure bus information is entered");
				return;
			}
            systemInstance.CreateBus(name, System.Convert.ToInt32(frequency));
            simulationManagerNotification("Bus service added");
            List<string> bus = GetBusList();
            BusService2.ItemsSource = bus;
            BusRouteDispComboBox.ItemsSource = bus;

        }

        private void AddRouteClick(object sender, RoutedEventArgs e)
        {
            string busName = BusServiceComboBox2.Text;
            string initialBusStop = FromComboBox.Text;
            string terminalBusStop = ToComboBox.Text;
            string trv = TravelTimeTextBox.Text;
			
			if (busName==""||initialBusStop==""||terminalBusStop==""||trv=="") 
			{
				MessageBox.Show("Please ensure route information is entered");
				return;
			}
            int traveltime = System.Convert.ToInt32(trv, 10);
            systemInstance.CreateRoute(busName, initialBusStop, terminalBusStop, traveltime); 
            simulationManagerNotification("Bus route added");

        }

        private void DisplayBusStopsClick(object sender, RoutedEventArgs e)
        {
            AdminUITextBlock.Text = systemInstance.GetBusStopStringList();
        }

        private void DisplayBusSerClick(object sender, RoutedEventArgs e)
        {
            AdminUITextBlock.Text = systemInstance.GetBusStringList();
        }

        private void DisplayBusRouteClick(object sender, RoutedEventArgs e)
        {
            
			string busName = BusRouteDispComboBox.Text;
			if(busName=="")
			{
				MessageBox.Show("Please select the bus service");
				return;
			}
            AdminUITextBlock.Text= systemInstance.GetRoute(busName);
        }

        private void StartTripClick(object sender, RoutedEventArgs e)
        {
			string mobileNum = MobileNoTextBox.Text;
            string stop = StartingBusStop.Text;
            string bus = BusService2.Text;
			if (mobileNum==""||stop==""||bus=="")
			{
				MessageBox.Show("Please enter all information to enqueue user");
				return;				
			}
            if (systemInstance.CheckDuplicateMobileNumber(mobileNum) == false)
            {
                MessageBox.Show("User Mobile no. does not exist");
                return;	
            }
			
			if(systemInstance.CanEnqueueUserToBusStop(mobileNum)==false)
			{
				MessageBox.Show("User is already in the system");
                return;	
			}
			if(systemInstance.BusStopIsInterrupted(stop)==true)
			{
				MessageBox.Show("Bus Stop is interrupted");
				return;
			}
			if(systemInstance.CanEnqueueUserToBusStopGivenDetail(mobileNum,bus,stop)==false)
			{
				MessageBox.Show("Can't enqueue user to last bus stop");
				return;
			}
            systemInstance.EnqueueUserToBusStop(mobileNum, stop, bus);

            simulationManagerNotification("Passenger Trip Started");            
        }

        private void StartSimulationSlick(object sender, RoutedEventArgs e)
        {         
            SimulationBox.Clear();
            while (!systemInstance.GetHasEvent())
            {
                systemInstance.IncreaseTime();
            }
            SimulationTime.Text = systemInstance.GetTime().ToString();
			
			
			
            DisplayAllBusArrivals(systemInstance); // ?
           					
            UserHopOn(systemInstance);

            systemInstance.ResetHasEvent();
         
            if (systemInstance.GetWhoCanHopOffNow().Count()>0)
                animateUserHopOffMenuShow();
            //if (systemInstance.GetWhoCanHopOffNow().Count() == 0)
            //    animateUserHopOffMenuHide();
            
            List<string> busStops = GetBusStopsList();
			// SMS LOG
			List<string> smslog = systemInstance.GetSystemCurrentSMSLog();
            
			SmsNoticeBoard.Clear();
            if (smslog==null) ;
			else
            for (int i=0;i<smslog.Count();i++)
            {
                SmsNoticeBoard.Text+= smslog.ElementAt(i);
                SmsNoticeBoard.Text += "\n";
            }
			SmsNoticeBoard.Text+="\n";
            List<string> interrupt = systemInstance.GetCurrentInterruptLog();
            //SmsNoticeBoard.Clear();
            if (interrupt==null) ;
			else
            for (int i=0;i<interrupt.Count();i++)
            {
                SmsNoticeBoard.Text+= interrupt.ElementAt(i);
                SmsNoticeBoard.Text += "\n";
            }
            SmsNoticeBoard.Text += "\n";
            //colour for bust stop status
            for (int i = 0; i < busStops.Count; i++)
            {
                //pink if bus stop is interrupted
                if (!systemInstance.GetBusStopList().FindBusStopWithBusStopName(GetBusStopsList()[i]).GetStatus())
                {
                    Button btn = (Button)BusStopButtonLayer.Children[i];
                    btn.Background = Brushes.Plum;
                }
                else //blue if user is queued at bus stop
                    if (GetPassengerAtBusStopQList(GetBusStopsList()[i]).Count > 0)
                {
                    Button btn = (Button)BusStopButtonLayer.Children[i];
                    btn.Background = Brushes.CornflowerBlue;
                }
                //green if bus is queued at busstop
                else if (GetBusQList(GetBusStopsList()[i]).Count > 0)
                {
                    Button btn = (Button)BusStopButtonLayer.Children[i];
                    btn.Background = Brushes.LawnGreen;
                }
                //white if no bus and users
                else
                {
                    Button btn = (Button)BusStopButtonLayer.Children[i];
                    btn.Background = Brushes.White;
                }

            }
            
            //animating run simulation button
            animationScaleAR((Button)sender, -0.01, 80);
        }

        private void DisplayAllBusArrivals(EzySystem ezySystem)
        {
            // for each bus stop, check whether the bus queue has a bus which indicates that a bus has arrived
            for (int i = 0; i < ezySystem.GetBusStopList().GetCount(); i++)
            {
                // if there is at least one bus, display all bus arrivals
                if (ezySystem.GetBusStopList().At(i).GetBusQueueCount() > 0)
                {
                    for (int j = 0; j < ezySystem.GetBusStopList().At(i).GetBusQueueCount(); j++)
                    {
                        string busname = ezySystem.GetBusStopList().At(i).GetBusQueue().ElementAt(j), busstopname = ezySystem.GetBusStopList().At(i).GetName();

                        if (!ezySystem.BusNameIsInBusInterruptList1(busname, busstopname) && !ezySystem.BusStopIsInterrupted(busstopname))
                            DisplayBusArrival(ezySystem.GetBusStopList().At(i).GetName(), ezySystem.GetBusStopList().At(i).GetBusQueue().ElementAt(j));
                    }
                }
            }
        }

        private void DisplayBusArrival(string busStopName, string busName)
        {
            
            SimulationBox.Text+= ("Bus " + busName + " has reached " + busStopName + "\n");
        }

        // display all passengers which were able to hop on successfully, send SMS, issue ticket and deduct from account
        private void UserHopOn(EzySystem ezySystem)
        {
            // check expiry ticket
            List<string> temp = ezySystem.GetWhoHasHopOn();
            if (temp.Count == 0) return;

            SimulationBox.Text += ("The following users have sucessfully hopped on: "+ "\n");
            for (int i = 0; i < temp.Count; i++)
            {
                SimulationBox.Text += temp.ElementAt(i);
            }


        }

        private void HopOnUser1_Click(object sender, RoutedEventArgs e)
        {
            string userName = HopOnUserBox1.Text;
            // remove passenger from the bus after this line
            //systemInstance.DequeueUserFromBus(userName, HopOnBusBox1.Text, HopOnBusStopBox1.Text);
            if (userName == "")
            {
                MessageBox.Show("Please select a user to hop off");
                return;
            }
            systemInstance.HopOffUser(userName);
            SimulationBox.Text = (userName + " has alighted" + "\n");

            DoubleAnimation AnimationY = new DoubleAnimation();
            TranslateTransform TranslateY = new TranslateTransform();

            AnimationY.From = -120;
            AnimationY.To = 0;

            AnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            HopOnBusUserText.RenderTransform = TranslateY;
            AlightPassengerBorder.RenderTransform = TranslateY;
            HopOnUserBox1.RenderTransform = TranslateY;
            HopOnUser1.RenderTransform = TranslateY;

            TranslateY.BeginAnimation(TranslateTransform.YProperty, AnimationY);

            UserHopOn(systemInstance);

            systemInstance.ResetHasEvent();

            if (systemInstance.ThereIsPassengerOnBusAtBusStop())
                animateUserHopOffMenuShow();
        }

        private void JourneyHistoryClick(object sender, RoutedEventArgs e)
        {
            List<string> journeyhistory = systemInstance.GetJourneyHistoryOfUser(mobilenumber);
            if (journeyhistory.Count() == 0)
            {
				LogTextBox.Text="Your Journey History is empty\n";
				return;
			}
            LogTextBox.Clear();
            for (int i = 0; i < journeyhistory.Count; i++)
            {
                //LogTextBox.Clear();
                LogTextBox.Text += (journeyhistory.ElementAt(i)) + "\n";
            }
        }

        private void SMSLogButton_Click(object sender, RoutedEventArgs e)
        {
			
            List<string> smslog = systemInstance.GetSMSLogOfUser(mobilenumber);
            if (smslog.Count() == 0)
            {
				LogTextBox.Text="Your SMS Log is empty\n";
				return;
			}
			LogTextBox.Clear();
            for (int i = 0; i < smslog.Count(); i++)
            {
                LogTextBox.Text += smslog.ElementAt(i)+"\n";

            }
        }

        private void UpdateTVALClick(object sender, RoutedEventArgs e)
        {
			if (T_VALTextbox.Text=="")
			{
				MessageBox.Show("Please enter T_VAL");
				return;
			}
            systemInstance.SetTVAL(System.Convert.ToInt32(T_VALTextbox.Text));
            MessageBox.Show("T_Val Updated");
        }

        private void UpdateTCOSTClick(object sender, RoutedEventArgs e)
        {
			if (T_COSTTextbox.Text=="")
			{
				MessageBox.Show("Please enter T_COST");
				return;
			}
			
            systemInstance.SetTVAL(System.Convert.ToInt32(T_COSTTextbox.Text));
            MessageBox.Show("T_Cost Updated");
        }

        private void SetInterruptClick(object sender, RoutedEventArgs e)
        {
            string StartBusInterrupt = BusInterruptComboBox.Text;
            string NextBusStopInterrupt = NextBusInterruptComboBox.Text;
			if (StartBusInterrupt==""||NextBusStopInterrupt=="")
			{
				MessageBox.Show("Please enter information to set interruption");
				return;
			}
			
            systemInstance.SetBusInterrupt(StartBusInterrupt, NextBusStopInterrupt);
            simulationManagerNotification("Bus Interruption added");
        }


        private void SetBSInterruptClick(object sender, RoutedEventArgs e)
        {			
			if (BSInterruptComboBox.Text=="")
			{
				MessageBox.Show("Please select a bus stop");
				return;
			}
            systemInstance.SetBusStopInterrupt(BSInterruptComboBox.Text);
            simulationManagerNotification("Interruption Added");
			
			//update remove bus stop interrupt click
			//List<string> BusStopInterruptList = systemInstance.GetBusStopInterruptList();  
			//RemoveInterruptCombo.ItemsSource = BusStopInterruptList;
        }

        private void RevBSInterruptClick(object sender, RoutedEventArgs e)
        {
			if (RemoveInterruptCombo.Text=="")
			{
				MessageBox.Show("Please select a bus stop");
				return;
			}
            systemInstance.RemoveBusStopInterrupt(RemoveInterruptCombo.Text);
			
			List<string> bus = systemInstance.GetBusStopNotInterruptList();
			bus.Sort();
			BSInterruptComboBox.ItemsSource = bus;
			
            simulationManagerNotification("Interruption Cleared");
        }

        public class ImageButton : Button
        {
            public ImageSource Source
            {
                get { return base.GetValue(SourceProperty) as ImageSource; }
                set { base.SetValue(SourceProperty, value); }
            }
            public static readonly DependencyProperty SourceProperty =
              DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageButton));
        }

        private void SimulationButton_MouseEnter(object sender, MouseEventArgs e)
        {
            animationTranslateY(SimulationButton, 0, 2, 200);
            animationFade(SimulationButton, AdminUIButton.Opacity, 1, 200);

            MenuSelectionBox.Content = "Simulator";
        }

        private void SimulationButton_MouseLeave(object sender, MouseEventArgs e)
        {            
            animationTranslateY(SimulationButton, 2, 0, 200);
            animationFade(SimulationButton, AdminUIButton.Opacity, 0.75, 200);

            MenuSelectionBox.Content = null;
        }

        private void GetUserStatusClick(object sender, RoutedEventArgs e)
        {
            string mobilenum = UserMobileNoTextbox.Text;

            if (systemInstance.CheckDuplicateMobileNumber(mobilenum) == false)
            {
                MessageBox.Show("Mobile number does not exist");
                return;
            }
			

			
            string location = systemInstance.GetUserLocation(mobilenum);
            USER_STATUS status = systemInstance.GetUserStatus(mobilenum);
            if (location == null || location == "" ) location = "unknown location";

            SimulationBox.Text = (systemInstance.FindUserWithMobileNumber(mobilenum) + " is at " + location + " and status is " + status.ToString() + "\n");

            //animationTranslateX(UserStatusCover, 120, 0, 500);
            //animationTranslateX(UserStatusCoverBase, 120, 0, 500);
            userStatusUp = false;
        }


        private void UpdateNextBusStopForBus(object sender, EventArgs e)
        {
            
            if (BusInterruptComboBox.SelectedItem==null) return;
            List<string> busstop2 = systemInstance.GetRouteAsList(BusInterruptComboBox.SelectedItem.ToString());
            NextBusInterruptComboBox.ItemsSource = busstop2;
        }

        private void UpdateBusServiceForBusStop(object sender, EventArgs e)
        {
            string a = StartingBusStop.Text;
            List<string> bus = systemInstance.GetBusServiceUseThisBusStop(a);
            BusService2.ItemsSource = bus;
        }

        private void BuyPrepaidCardClick(object sender, RoutedEventArgs e)
        {
			if (TopUpComboBox.Text=="")
			{
                MessageBox.Show("Please select a prepaid amount");
				return;
			}
            int value = Int32.Parse(TopUpComboBox.Text);
            int cardnum;

            cardnum = systemInstance.IssueNewCardNumber(value);
            MessageBox.Show("Your Prepaid Card Number is " + cardnum + " with a value of " + value);
        }

        
        
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {	
            //SimulationGrid.Visibility = Visibility.Hidden;
            SimulationManagerGrid.Visibility = Visibility.Visible;
			
			List<string> user = systemInstance.FindMobileNumberList();
			user.Sort();
			MobileNoTextBox.ItemsSource = user;
		
			List<string> bus = systemInstance.GetBusStopNotInterruptList();
			bus.Sort();
			BSInterruptComboBox.ItemsSource = bus;
			
            animationTranslateXGrid(SimulationGrid, 0, 800, 1500);
            animationFadeGrid(SimulationGrid, 1, 0, 1500);

            animationTranslateXGrid(SimulationManagerGrid, 800, 0, 1500);
            animationFadeGrid(SimulationManagerGrid, 0, 1, 1500);

            animationTranslateXText(QpassengersText, 100, 0, 2500);
            animationTranslateXText(SetBusInterruptsText, 100, 0, 2500);
            animationTranslateXText(SetBusStopIntertuptsText, 100, 0, 2500);
            animationTranslateXText(TopUpText, 100, 0, 2500);
            animationTranslateX(ManagerToSimulatoinButton, -100, 0, 2500);
            animationTranslateXText(SimulationManagerTItle, -100, 0, 2800);

            //Animating left panle
            animationTranslateYGrid(LeftPanelGrid, -0, -500, 1200);
            animationFadeGrid(LeftPanelGrid, 1, 0, 1700);

            //Animating Bottom panel
            animationTranslateXGrid(BottomPanelGrid1, 0, -300, 1200);
            animationFadeGrid(BottomPanelGrid1, 1, 0, 1700);

            //animating MapGrid
            animationTranslateXGrid(MapGrid, 0, 500, 1200);
            animationFadeGrid(MapGrid, 1, 0, 1700);

        }


//----------------------------------------------Animation Functions----------------------------------------------\\
        private void animationTranslateX(Button btn, double from, double to, int duration)
        {
            DoubleAnimation AnimationX = new DoubleAnimation();
            TranslateTransform TranslateX = new TranslateTransform();

            AnimationX.From = from;
            AnimationX.To = to;

            AnimationX.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            btn.RenderTransform = TranslateX;

            TranslateX.BeginAnimation(TranslateTransform.XProperty, AnimationX);
        }

        private void animationTranslateXCanvas(Canvas canvas, double from, double to, int duration)
        {
            DoubleAnimation AnimationX = new DoubleAnimation();
            TranslateTransform TranslateX = new TranslateTransform();

            AnimationX.From = from;
            AnimationX.To = to;

            AnimationX.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            canvas.RenderTransform = TranslateX;

            TranslateX.BeginAnimation(TranslateTransform.XProperty, AnimationX);
        }

        private void animationTranslateXGrid(Grid grid, double from, double to, int duration)
        {
            DoubleAnimation Animation = new DoubleAnimation();
            TranslateTransform Translate = new TranslateTransform();

            Animation.From = from;
            Animation.To = to;

            Animation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            grid.RenderTransform = Translate;

            Translate.BeginAnimation(TranslateTransform.XProperty, Animation);
        }

        private void animationTranslateXText(TextBlock text, double from, double to, int duration)
        {
            DoubleAnimation Animation = new DoubleAnimation();
            TranslateTransform Translate = new TranslateTransform();

            Animation.From = from;
            Animation.To = to;

            Animation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            text.RenderTransform = Translate;

            Translate.BeginAnimation(TranslateTransform.XProperty, Animation);
        }

        private void animationTranslateY(Button btn, double from, double to, int duration)
        {
            DoubleAnimation AnimationY = new DoubleAnimation();
            TranslateTransform TranslateY = new TranslateTransform();

            AnimationY.From = from;
            AnimationY.To = to;

            AnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            btn.RenderTransform = TranslateY;

            TranslateY.BeginAnimation(TranslateTransform.YProperty, AnimationY);
        }

        private void animationTranslateYGrid(Grid grid, double from, double to, int duration)
        {
            DoubleAnimation AnimationY = new DoubleAnimation();
            TranslateTransform TranslateY = new TranslateTransform();

            AnimationY.From = from;
            AnimationY.To = to;

            AnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            grid.RenderTransform = TranslateY;

            TranslateY.BeginAnimation(TranslateTransform.YProperty, AnimationY);
        }

        private void animationTranslateYText(TextBlock text, double from, double to, int duration)
        {
            DoubleAnimation AnimationY = new DoubleAnimation();
            TranslateTransform TranslateY = new TranslateTransform();

            AnimationY.From = from;
            AnimationY.To = to;

            AnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            text.RenderTransform = TranslateY;

            TranslateY.BeginAnimation(TranslateTransform.YProperty, AnimationY);
        }

        private void animationXandY(Button btn, double fromX, double toX, int durationX, double fromY, double toY, int durationY)
        {
            DoubleAnimation AnimationX = new DoubleAnimation();
            DoubleAnimation AnimationY = new DoubleAnimation();
            TranslateTransform TranslateX = new TranslateTransform();
            TranslateTransform TranslateY = new TranslateTransform();
            TransformGroup animationGroup = new TransformGroup();

            animationGroup.Children.Add(TranslateX);
            animationGroup.Children.Add(TranslateY);
            AnimationX.From = fromX;
            AnimationX.To = toX;
            AnimationY.From = fromY;
            AnimationY.To = toY;
            AnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(durationY));
            AnimationX.Duration = new Duration(TimeSpan.FromMilliseconds(durationX));
            btn.RenderTransform = animationGroup;
            TranslateY.BeginAnimation(TranslateTransform.YProperty, AnimationX);
            TranslateX.BeginAnimation(TranslateTransform.XProperty, AnimationY);
        }

        private void animationFade(Button btn, double from, double to, int duration)
        {
            DoubleAnimation animationFade = new DoubleAnimation();
            animationFade.From = from;
            animationFade.To = to;
            animationFade.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            btn.BeginAnimation(Button.OpacityProperty, animationFade);
        }

        private void animationFadeText(TextBlock text, double from, double to, int duration)
        {
            DoubleAnimation animationFade = new DoubleAnimation();
            animationFade.From = from;
            animationFade.To = to;
            animationFade.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            text.BeginAnimation(Button.OpacityProperty, animationFade);
        }

        private void animationFadeGrid(Grid grid, double from, double to, int duration)
        {
            DoubleAnimation animationFade = new DoubleAnimation();
            animationFade.From = from;
            animationFade.To = to;
            animationFade.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            grid.BeginAnimation(Grid.OpacityProperty, animationFade);
        }

        private void animationScale(Button btn, double scale, int duration)
        {
            DoubleAnimation animationResizeX = new DoubleAnimation();
            DoubleAnimation animationResizeY = new DoubleAnimation();
            ScaleTransform transform = new ScaleTransform();

            btn.RenderTransform = transform;

            animationResizeX.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            animationResizeX.From = transform.ScaleX;
            animationResizeX.To = transform.ScaleX + scale;

            animationResizeY.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            animationResizeY.From = transform.ScaleY;
            animationResizeY.To = transform.ScaleY + scale;

            transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationResizeX);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationResizeY);
        }

        private void animationScaleAR(Button btn, double scale, int duration)
        {
            DoubleAnimation animationResizeX = new DoubleAnimation();
            DoubleAnimation animationResizeY = new DoubleAnimation();
            ScaleTransform transform = new ScaleTransform();

            btn.RenderTransform = transform;

            animationResizeX.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            animationResizeX.From = transform.ScaleX;
            animationResizeX.To = transform.ScaleX + scale;

            animationResizeY.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            animationResizeY.From = transform.ScaleY;
            animationResizeY.To = transform.ScaleY + scale;
            
            animationResizeX.AutoReverse = true;
            animationResizeY.AutoReverse = true;

            transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationResizeX);
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationResizeY);           
        }

//----------------------------------------------Animation Functions End----------------------------------------------\\        

        private void RunSimulationButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            animationScale(btn, 0.06, 200);
        }

        private void RunSimulationButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            animationScale(btn, -0.06, 200);
        }

      

        private void SMSboardNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SMSboardPrev_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserStatusCover_MouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = new Button();
            btn = (Button)sender;
            animationFade(btn, btn.Opacity, 0.9, 300);
        }

        private void UserStatusCover_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = new Button();
            btn = (Button)sender;
            animationFade(btn, btn.Opacity, 0.75, 300);
        }

        //private void UserStatusCover_Click(object sender, RoutedEventArgs e)
        //{
        //    Button btn = new Button();
        //    btn = (Button)sender;
        //    if (!userStatusUp)
        //    {
        //        animationTranslateX(btn, 0, 120, 500);
        //        animationTranslateX(UserStatusCoverBase, 0, 120, 500);

        //        this.Visibilitytimer.Elapsed += new System.Timers.ElapsedEventHandler(this.UserCovertimer_Tick);
        //        Visibilitytimer.AutoReset = false;
        //        Visibilitytimer.Enabled = true;
        //        Visibilitytimer.Start();
        //    }
        //}

        //private void UserStatusCanvas_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    if (!userStatusUp)
        //    animationTranslateX(UserStatusCover, 0, -120, 300);
        //}

        //private void UserStatusArea_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    if (userStatusUp)
        //    {
        //        animationTranslateX(UserStatusCover, 120, 0, 500);
        //        animationTranslateX(UserStatusCoverBase, 120, 0, 500);
        //        userStatusUp = false;
        //    }
        //}


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SimulationGrid.Visibility = Visibility.Visible;
            //SimulationManagerGrid.Visibility = Visibility.Hidden;

            //animeate simulation manager
            animationTranslateXGrid(SimulationManagerGrid, 0, 800, 1500);
            animationFadeGrid(SimulationManagerGrid, 1, 0, 1500);

            //Animating left panle
            animationTranslateYGrid(LeftPanelGrid, -500, 0, 1200);
            animationFadeGrid(LeftPanelGrid, 0, 1, 1700);

            //Animating Bottom panel
            animationTranslateXGrid(BottomPanelGrid1, -300, 0, 1200);
            animationFadeGrid(BottomPanelGrid1, 0, 1, 1700);

            //animating MapGrid
            animationTranslateXGrid(MapGrid, 500, 0, 1200);
            animationFadeGrid(MapGrid, 0, 1, 1700);

            //animating logo
            DoubleAnimation LogoAnimationFade = new DoubleAnimation();
            LogoAnimationFade.From = 0;
            LogoAnimationFade.To = 1;
            LogoAnimationFade.Duration = new Duration(TimeSpan.FromMilliseconds(1200));
           //SimulationLogo.BeginAnimation(Image.OpacityProperty, LogoAnimationFade);

            //animating Simulation Grid
            animationTranslateXGrid(SimulationGrid, 700, 0, 1200);
            animationFadeGrid(SimulationGrid, 0, 1, 1200);
            //end of animation
			
						// SMS LOG
			List<string> smslog = systemInstance.GetSystemCurrentSMSLog();


           //string smslog = systemInstance.GetSystemSMSLogDataAsString();
          	SmsNoticeBoard.Clear();
            if (smslog==null) ;
			else for (int i=0;i<smslog.Count();i++)
            {
                SmsNoticeBoard.Text+= smslog.ElementAt(i);
				SmsNoticeBoard.Text+="\n";
            }
			
			List<string> interrupt = systemInstance.GetCurrentInterruptLog();
            //SmsNoticeBoard.Clear();
            if (interrupt==null) ;
			else
            for (int i=0;i<interrupt.Count();i++)
            {
                SmsNoticeBoard.Text+= interrupt.ElementAt(i);
                SmsNoticeBoard.Text += "\n";
            }

        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            animationFade(btn, 0.6, 1, 200);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            animationFade(btn, 1, 0.6, 200);
        }

        private void Button_MouseEnter_1(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            animationScale(btn, 0.04, 200);
        }

        private void Button_MouseLeave_1(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            animationScale(btn, -0.04, 200);
        }

        private void AlightPassengerYes_Click(object sender, RoutedEventArgs e)
        {
            List<string> userhopoff = systemInstance.GetWhoCanHopOffNow();
            //userhopoff.Sort();
            HopOnUserBox1.ItemsSource = null;
            HopOnUserBox1.ItemsSource = userhopoff;

            //animate alight passenger
            animationTranslateYText(AlightPassengerText, -120, 0, 1500);
            animationTranslateY(AlightPassengerYes, -120, 0, 1500);
            animationTranslateY(AlightPassengerNo, -120, 0, 1500);

            animationFade(AlightPassengerYes, 1, 0, 200);
            animationFade(AlightPassengerNo, 1, 0, 200);
            animationFadeText(AlightPassengerText, 1, 0, 200);

            //animate user menu
            DoubleAnimation AnimationYUser = new DoubleAnimation();
            DoubleAnimation AnimationFadeUser = new DoubleAnimation();
            TranslateTransform TranslateYUser = new TranslateTransform();

            AnimationYUser.From = 0;
            AnimationYUser.To = -120;
            AnimationYUser.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            AnimationFadeUser.From = 0;
            AnimationFadeUser.To = 1;
            AnimationFadeUser.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            HopOnUserBox1.RenderTransform = TranslateYUser;

            HopOnUserBox1.BeginAnimation(Button.OpacityProperty, AnimationFadeUser);

            TranslateYUser.BeginAnimation(TranslateTransform.YProperty, AnimationYUser);

            animationTranslateYText(HopOnBusUserText, 0, -120, 500);
            animationTranslateY(HopOnUser1, 0, -120, 500);


            animationFade(HopOnUser1, 0, 1, 200);
            animationFadeText(HopOnBusUserText, 0, 1, 200);


            List<string> busstop = new List<string>();
           
        }

        private void animateUserHopOffMenuShow()
        {

            DoubleAnimation AnimationY = new DoubleAnimation();
            TranslateTransform TranslateY = new TranslateTransform();

            AnimationY.From = 0;
            AnimationY.To = -120;
            AnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            
            AlightPassengerBorder.RenderTransform = TranslateY;

            TranslateY.BeginAnimation(TranslateTransform.YProperty, AnimationY);

            animationTranslateYText(AlightPassengerText, 0, -120, 500);
            animationTranslateY(AlightPassengerYes, 0, -120, 500);
            animationTranslateY(AlightPassengerNo, 0, -120, 500);

            animationFade(AlightPassengerYes, 0, 1, 500);
            animationFade(AlightPassengerNo, 0, 1, 500);
            animationFadeText(AlightPassengerText, 0, 1, 500);
        }

        private void animateUserHopOffMenuHide()
        {
            DoubleAnimation AnimationY = new DoubleAnimation();
            TranslateTransform TranslateY = new TranslateTransform();

            AnimationY.From = -120;
            AnimationY.To = 0;

            AnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            AlightPassengerText.RenderTransform = TranslateY;
            AlightPassengerBorder.RenderTransform = TranslateY;
            AlightPassengerYes.RenderTransform = TranslateY;
            AlightPassengerNo.RenderTransform = TranslateY;

            TranslateY.BeginAnimation(TranslateTransform.YProperty, AnimationY);
        }

        private void AlightPassengerNo_Click(object sender, RoutedEventArgs e)
        {
            animateUserHopOffMenuHide();
        }

        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Map.ReleaseMouseCapture();
        }

        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Map.IsMouseCaptured) return;

            var tt = (TranslateTransform)((TransformGroup)Map.RenderTransform).Children.First(tr => tr is TranslateTransform);
            Vector v = start - e.GetPosition(MapBoundary);

            var imageBounds = Map.RenderTransform.TransformBounds(VisualTreeHelper.GetDrawing(Map).Bounds);

            //x-axis translation
            if (imageBounds.Left < (imageBounds.Width - MapBoundary.ActualWidth) / 2  && imageBounds.Right > (imageBounds.Width + MapBoundary.ActualWidth) / 2)
            {
                tt.X = origin.X - v.X;
                Canvas.SetLeft(BusStopButtonLayer, tt.X);
            }
            //allow movement away from border
            else if (imageBounds.Left >= (imageBounds.Width - MapBoundary.ActualWidth) / 2 && v.X > 0 || imageBounds.Right <= (imageBounds.Width + MapBoundary.ActualWidth) / 2 && v.X < 0)
            {
                tt.X = origin.X - v.X;
                Canvas.SetLeft(BusStopButtonLayer, tt.X);
            }

            //y-axis translation
            if (imageBounds.Bottom > (imageBounds.Height + MapBoundary.ActualHeight) / 2 && imageBounds.Top < (imageBounds.Height - MapBoundary.ActualHeight) / 2)
            {
                tt.Y = origin.Y - v.Y;
                Canvas.SetTop(BusStopButtonLayer, tt.Y);
            }
            //allow movement away from border
            else if (imageBounds.Bottom <= (imageBounds.Height + MapBoundary.ActualHeight) / 2 && v.Y < 0 || imageBounds.Top >= (imageBounds.Height - MapBoundary.ActualHeight) / 2 && v.Y > 0)
            {
                tt.Y = origin.Y - v.Y;
                Canvas.SetTop(BusStopButtonLayer, tt.Y);
            }
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Map.CaptureMouse();
            var tt = (TranslateTransform)((TransformGroup)Map.RenderTransform).Children.First(tr => tr is TranslateTransform);
            start = e.GetPosition(MapBoundary);
            origin = new Point(tt.X, tt.Y);
        }

        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)Map.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            BusStopButtonLayer.RenderTransform = transform;

            double zoom = e.Delta > 0 ? .2 : -.2;

            //zoom image
            if ((curzoom >= 1.8 && zoom > 0) || (curzoom <= .01 && zoom < 0))
                return;
            else
            {
                transform.ScaleX += zoom;
                transform.ScaleY += zoom;
            }

            //reset map location when zooming out
            if (zoom < 0)
            {
                var imageBounds = Map.RenderTransform.TransformBounds(VisualTreeHelper.GetDrawing(Map).Bounds);
                var tt = (TranslateTransform)((TransformGroup)Map.RenderTransform).Children.First(tr => tr is TranslateTransform);

                tt.X = 0;
                tt.Y = 0;
                Canvas.SetLeft(BusStopButtonLayer, imageBounds.Left);
                Canvas.SetTop(BusStopButtonLayer, imageBounds.Top);
            }
            
            //update zoom factor
            curzoom += zoom;
        }

        private void renderBusStops()
        {
            List<string> busStops = GetBusStopsList();

            for (int i = 0; i < busStops.Count; i++)
            {
                //create new button
                Button btnTest = new Button();
                Thickness btnThickness = new Thickness();

                btnTest.Name = "B" + i.ToString();

                btnThickness.Left = 0;
                btnThickness.Top = 0;
                btnTest.Margin = btnThickness;
                btnTest.Width = 14;
                btnTest.Height = 14;
                btnTest.Click += new RoutedEventHandler(busStop_Click);
                btnTest.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                btnTest.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                
                //set bus image
                Image img = new Image();
                img.VerticalAlignment = VerticalAlignment.Center;
                img.Source = CreateImage("symbol_bus2.gif");
                btnTest.Content = img;

                //add to canvas
                BusStopButtonLayer.Children.Add(btnTest);
                //set location of button
                Canvas.SetLeft(btnTest, systemInstance.GetBusStopList().FindBusStopWithBusStopName(busStops[i]).GetCoordinates()[0] / 100 * BusStopButtonLayer.Width);
                Canvas.SetTop(btnTest, systemInstance.GetBusStopList().FindBusStopWithBusStopName(busStops[i]).GetCoordinates()[1] / 100 * BusStopButtonLayer.Height);
            }
        }

        private void busStop_Click(Object sender, System.EventArgs e)
        {
            Button btn = (Button)sender;

            //animate info tab
            animationTranslateXCanvas(BusInfoCanvas, 0, -160, 500);

            List<string> bus123 = GetBusStopsList();

            //char endofstring;
            string buttonIndexNum = null;
            for (int i = 1; i < btn.Name.Length; i++)
                buttonIndexNum += btn.Name[i];

            int nameToIndex = Convert.ToInt32(buttonIndexNum);
            string busStopName = bus123[nameToIndex];


            BusStopNameText.Text = busStopName;
            List<string> bus = GetBusQList(busStopName);
            BusAtStopText.Text = null;
            for (int i = 0; i < bus.Count; i++)
                BusAtStopText.Text += bus[i] + ", ";

            List<string> passengerAtStop = GetPassengerAtBusStopQList(busStopName);
            PassengersAtStopText.Text = null;
            for (int i = 0; i < passengerAtStop.Count; i++)
                PassengersAtStopText.Text += passengerAtStop[i] + "\n";

            BusAtBusStopComboBox.ItemsSource = bus;

            busInfoUp = true;

        }

        public static System.Windows.Media.Imaging.BitmapImage CreateImage(string path)
        {

            System.Windows.Media.Imaging.BitmapImage myBitmapImage = new System.Windows.Media.Imaging.BitmapImage();
            myBitmapImage.BeginInit();

            myBitmapImage.UriSource = new Uri(path, UriKind.Relative); // bitmapImage.UriSource = new Uri("myIcon.ico", UriKind.Relative);

            myBitmapImage.EndInit();

            return myBitmapImage;
        }

        private void BusInfoCanvas_MouseLeave(Object sender, System.EventArgs e)
        {
            if (busInfoUp)
            {
                animationTranslateXCanvas(BusInfoCanvas, -160, 0, 500);
                PassengersOnBusText.Text = null;
                busInfoUp = false;
            }
        }


        private void PassengerOnBusAtBusSTopButton_MouseDown(object sender, RoutedEventArgs e)
        {
            if (BusAtBusStopComboBox.Text == "")
            {
                MessageBox.Show("Please select a bus service");
                return;
            }
            List<string> passenger = systemInstance.GetUserListOfABusInstance(BusAtBusStopComboBox.Text, BusStopNameText.Text);

             PassengersOnBusText.Text = null;

             for (int i = 0; i < passenger.Count; i++ )
                 PassengersOnBusText.Text += passenger[i] + "\n";
        }

        private void ResetButtonClick(object sender, RoutedEventArgs e)
        {
            systemInstance.Reset();
			simulationManagerNotification("System has been reset");     
        }

        private void UpdateCoOrdClick(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
			string busstopname = BusStopCoOrdComboBox.Text;
			string xcoord = ChangeXCoOrdTextBox.Text;
            string ycoord = ChangeYCoOrdTextBox.Text;
			if (busstopname==""||xcoord==""||ycoord=="") 
			{
				MessageBox.Show("Please endutr all bus stop information is entered");
				return;
			}
			systemInstance.UpdateBusStopCoordinates(busstopname,Convert.ToDouble(xcoord),Convert.ToDouble(ycoord));
        }

        private void ViewJourney_Admin(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            if (mobileJourneyTextBox.Text == "")
            {
                MessageBox.Show("Please enter mobile number");
                return;
            }

            if (systemInstance.CheckDuplicateMobileNumber(mobileJourneyTextBox.Text) == false)
            {
                MessageBox.Show("Mobile Number does not exist");
                return;
            }

            List<string> journeyhistory = systemInstance.GetJourneyHistoryOfUser(mobileJourneyTextBox.Text);
            if (journeyhistory.Count() == 0)
            {
                AdminUITextBlock.Text = "Journey History of User is empty";
                return;
            }
            AdminUITextBlock.Clear();
            for (int i = 0; i < journeyhistory.Count; i++)
            {
                //LogTextBox.Clear();
                AdminUITextBlock.Text += (journeyhistory.ElementAt(i)) + "\n";
            }
        }


        private void LegendGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            animationTranslateYGrid(LegendGrid, 0, -100, 500);
            LegendGridup = true;
        }

        private void LegendGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            animationFadeGrid(LegendGrid, LegendGrid.Opacity, 1, 200);
        }

        private void LegendGrid_MouseLeave(object sender, MouseEventArgs e)
        {            
            animationFadeGrid(LegendGrid, LegendGrid.Opacity, 0.70, 200);
            if (LegendGridup)
            animationTranslateYGrid(LegendGrid, -100, 0, 500);
            LegendGridup = false;
        }

        private void UpdateBusStopInterruptCombo(object sender, System.EventArgs e)
        {
        	// TODO: Add event handler implementation here.
						List<string> BusStopInterruptList = systemInstance.GetBusStopInterruptList();  
			RemoveInterruptCombo.ItemsSource = BusStopInterruptList;
        }

        

        private void SmsNoticeBoard_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void AdminLogIn_Click(object sender, RoutedEventArgs e)
        {
            if (AdminUserName == null && AdminPassword == null)
                MessageBox.Show("Please Enter your Username and Password");
            else if
                (AdminUserName.Text == "admin" && AdminPassword.Password == "123")
            {
				AdminUserName.Clear();
				AdminPassword.Clear();
                AdminUIGrid.Visibility = Visibility.Visible;

                animationTranslateXGrid(AdminUIGrid, 800, 0, 1500);
                animationFadeGrid(AdminUIGrid, 0, 1, 1500);
                animationTranslateXText(AdministratorTitle, 100, 0, 2200);

                animationTranslateXGrid(AdminLogInGrid, 0, 400, 1500);
                animationFadeGrid(AdminLogInGrid, 1, 0, 1500);
                animationTranslateXText(AdminLoginText, 0, 100, 2200);
            }
            else
                MessageBox.Show("Incorrect Username/Password");
        }



    }
}

