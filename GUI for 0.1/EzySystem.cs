/* Author/s: Paul Averilla and Nguyen Hoang Duy
 EZYSYSTEM CLASS
 *      The system is where all objects are created, handled, and managed
 * 
 * Dependencies:
 *      Bus Stop
 *      Bus
 *      User
 *      Bus Stop
 *      Bus List
 *      User List
 *      Route
 *      User in Queue
 *      Journey
 *      Ticket Expiry
 *      Ticket Expiry List
 *      SMS
 *      Bus Interrupt
 *      Top Up Retail Shop
 *      
 * Change log:
 *      ADD function GetBusStatCounter
 *      ADD function GetBusStopStatCounter
 *      EDIT function LoadBusList
 *      EDIT function LoadBusStopList
 *      ADD function GetBusStopCoordinates
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

// ADDITIONAL CLASSES
using BusStop_Class;
using Bus_Class;
using User_Class;
using BusStopList_Class;
using BusList_Class;
using UserList_class;
using Route_Class;
using UserInQueue_Class;
using Journey_Class;
using TicketExpiry_Class;
using TicketExpiryList_Class;
using SMS_Class;
using BusInterruptClass;
using TopUpRetailShop_Class;

// ADDITIONAL SYSTEM LIBRARIES
using GsmComm.GsmCommunication;
using GsmComm.PduConverter;

namespace EzySystem_Class
{
    public class EzySystem
    {
        /****************************************************************************************************/
        // ATTRIBUTES (Basic)
        /****************************************************************************************************/
        int T_VAL;
        int T_COST;
        int BUS_THRESHOLD;
        int BUSSTOP_THRESHOLD;
        private int _time;                  // records the system time
        private Boolean _hasEvent;          // indicates whether there is a current event


        /****************************************************************************************************/
        // ATTRIBUTES (Lists)
        /****************************************************************************************************/
        private List<SMS> _systemsmslog;
        private List<SMS> _interruptlog;
        private List<string> _autohopofflist;
        private List<string> _canhopoffnowlist;

        /****************************************************************************************************/
        // ATTRIBUTES (Lists which require a separate data file for storage)
        /****************************************************************************************************/
        private UserList _userList;         // contains a list of all users in the system
        private BusList _busList;           // contains a list of all bus services in the system
        private BusStopList _busStopList;   // contains a list of all bus stops in the system
        private TicketExpiryList _ticketList;   // contains a list of all tickets in the system (use to track ticket validity)
        private List<BusInterrupt> _busInterruptList;
        private TopUpRetailShop _retailShop;    // provides service for purchasing top up services e.g. buy top up card and top up using card

        /****************************************************************************************************/
        // ATTRIBUTES (Persistence)
        /****************************************************************************************************/
        private StreamWriter SW;            // this is for writing to files user information
        private string _userDataFile;       // name of the file that store user info
        private string _busListDataFile;    // name of the file that store bus list info
        private string _busStopListDataFile;    // name of the file that stores the bus stop list info
        private string _ticketListDataFile;
        private string _busInterruptListDataFile;
        private string _systemDataFile;
        private string _usedCardsListFile;    // name of the file that stores all used card numbers

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public EzySystem()
        {
            T_VAL = 5;
            T_COST = 2;
            BUS_THRESHOLD = 2;
            BUSSTOP_THRESHOLD = 2;
            _time = 0;
            _hasEvent = false;

            _systemsmslog = new List<SMS>();
            _interruptlog = new List<SMS>();
            _autohopofflist = new List<string>();
            _canhopoffnowlist = new List<string>();

            _userList = new UserList();
            _busList = new BusList();
            _busStopList = new BusStopList();
            _ticketList = new TicketExpiryList();
            _busInterruptList = new List<BusInterrupt>();
            _retailShop = new TopUpRetailShop();

            _userDataFile = "Data_User.txt";
            _busListDataFile = "Data_Bus.txt";
            _busStopListDataFile = "Data_BusStop.txt";
            _ticketListDataFile = "Data_Ticket.txt";
            _busInterruptListDataFile = "Data_BusInterrupt.txt";
            _systemDataFile = "Data_System.txt";
            _usedCardsListFile = "Data_CardList.txt";
        }
        ~EzySystem() {  }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE (String Data Manipulation)
        /****************************************************************************************************/
        public void Reset()
        {
           
            for (int i = 0; i < _busList.GetCount(); i++)
            {
                _busList.GetBusAtIndex(i).Clear();
            }

            for (int i = 0; i < _busStopList.GetCount(); i++)
            {
                _busStopList.At(i).Clear();
            }

            _time = 0;
            for (int i = 0; i < _userList.GetCount(); i++)
            {
                _userList.FindUserWithIndex(i).Clear();
            }

            T_VAL = 60;
            T_COST = 2;
            BUS_THRESHOLD = 10;
            BUSSTOP_THRESHOLD = 10;
            
            _hasEvent = false;

            _systemsmslog = new List<SMS>();
            _interruptlog = new List<SMS>();
            _autohopofflist = new List<string>();
            _canhopoffnowlist = new List<string>();

            _ticketList = new TicketExpiryList();
            _busInterruptList = new List<BusInterrupt>();
            _retailShop = new TopUpRetailShop();

        





        }
        // Concatenates all data under the EzySystem class and returns it as one long string
        // Each data entry is separated by a delimiter, %
        public string GetSystemDataAsString(string delimiter = "%")
        {
            return ("$START SYSTEMDATA$" + "\r\n" + T_VAL.ToString() + delimiter + T_COST.ToString() + delimiter + _time.ToString() + delimiter + _hasEvent.ToString() + "\r\n" + GetSystemSMSLogDataAsString() + "\r\n" + GetInterruptLogDataAsString() + "\r\n" + GetAutoHopOffListDataAsString() + "\r\n" + GetCanHopOffNowListDataAsString() + "\r\n" + "$END SYSTEMDATA$" + "\r\n");
        }

        // Concatenates all data under the SMS Log and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetSystemSMSLogDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$$START SYSTEMSMSLOG$$" + delimiter;

            for (int i = 0; i < _systemsmslog.Count(); i++)
            {
                tempReturn = tempReturn + _systemsmslog[i].GetAllDataAsString() + delimiter;
            }

            tempReturn = tempReturn + "$$END SYSTEMSMSLOG$$";

            return tempReturn;
        }

        // Concatenates all data under the Interrupt Log and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetInterruptLogDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$$START INTERRUPTLOG$$" + delimiter;

            for (int i = 0; i < _interruptlog.Count(); i++)
            {
                tempReturn = tempReturn + _interruptlog[i].GetAllDataAsString() + delimiter;
            }

            tempReturn = tempReturn + "$$END INTERRUPTLOG$$";

            return tempReturn;
        }

        // Concatenates all data under the Auto Hop Off list and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetAutoHopOffListDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$$START AUTOHOPOFFLIST$$" + delimiter;

            for (int i = 0; i < _autohopofflist.Count(); i++)
            {
                tempReturn = tempReturn + _autohopofflist[i] + delimiter;
            }

            tempReturn = tempReturn + "$$END AUTOHOPOFFLIST$$";

            return tempReturn;
        }

        // Concatenates all data under the Interrupt Log and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetCanHopOffNowListDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$$START CANHOPOFFNOWLIST$$" + delimiter;

            for (int i = 0; i < _canhopoffnowlist.Count(); i++)
            {
                tempReturn = tempReturn + _canhopoffnowlist[i] + delimiter;
            }

            tempReturn = tempReturn + "$$END CANHOPOFFNOWLIST$$";

            return tempReturn;
        }

        // Concatenates all data under the Bus Interrupt List and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetBusInterruptListAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$START BUSINTERRUPTLIST$" + delimiter;

            for (int i = 0; i < _busInterruptList.Count(); i++)
            {
                tempReturn = tempReturn + _busInterruptList[i].GetAllDataAsString() + delimiter;
            }

            tempReturn = tempReturn + "$END BUSINTERRUPTLIST$";

            return tempReturn;
        }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE (Save Data File Management)
        /****************************************************************************************************/

        // Saves the System Data into a data file
        public void SaveSystemData()
        {
            SW = new StreamWriter(_systemDataFile);
            SW.Close();
            File.Delete(_systemDataFile);

            SW = File.AppendText(_systemDataFile);
            
            SW.WriteLine(GetSystemDataAsString());
            SW.Close();
        }

        // Saves the Bus Data into a data file
        public void SaveBusList()
        {
            SW = new StreamWriter(_busListDataFile);
            SW.Close();
            File.Delete(_busListDataFile);

            SW = File.AppendText(_busListDataFile);

            SW.WriteLine(_busList.GetAllDataAsString());
            SW.Close();
        }

        // Saves the Bus Stop Data into a data file
        public void SaveBusStopList()
        {
            SW = new StreamWriter(_busStopListDataFile);
            SW.Close();
            File.Delete(_busStopListDataFile);

            SW = File.AppendText(_busStopListDataFile);

            SW.WriteLine(_busStopList.GetAllDataAsString());
            SW.Close();
        }

        // Saves the User Data into a data file
        public void SaveUserList()
        {
            SW = new StreamWriter(_userDataFile);
            SW.Close();
            File.Delete(_userDataFile);

            SW = File.AppendText(_userDataFile);

            SW.WriteLine(_userList.GetAllDataAsString());
            SW.Close();
        }

        // Saves the Ticket List into a data file
        public void SaveTicketList()
        {
            SW = new StreamWriter(_ticketListDataFile);
            SW.Close();
            File.Delete(_ticketListDataFile);

            SW = File.AppendText(_ticketListDataFile);

            SW.WriteLine(_ticketList.GetAllDataAsString());
            SW.Close();
        }

        // Saves the Bus Interrupt Data into a data file
        public void SaveBusInterruptList()
        {
            SW = new StreamWriter(_busInterruptListDataFile);
            SW.Close();
            File.Delete(_busInterruptListDataFile);


            SW = File.AppendText(_busInterruptListDataFile);

            SW.WriteLine(GetBusInterruptListAsString());
            SW.Close();
        }

        // Saves the Card List into a data file
        public void SaveCardList()
        {
            SW = new StreamWriter(_usedCardsListFile);
            SW.Close();
            File.Delete(_usedCardsListFile);

            SW = File.AppendText(_usedCardsListFile);

            //SW.WriteLine("Hello");
            SW.WriteLine(_retailShop.GetAllDataAsString());
            SW.Close();
        }

        // Saves all Data into a data file
        public void SaveAllData()
        {
            SaveSystemData();
            SaveBusList();
            SaveBusStopList();
            SaveBusInterruptList();
            SaveTicketList();
            SaveUserList();
            SaveCardList();
        }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE (Load Data File Management)
        /****************************************************************************************************/

        // Loads the System Data from a data file
        public void LoadSystemData()
        {
            if (!File.Exists(_systemDataFile))
            {
                // throw "file does not exist error"
                return;
            }

            StreamReader fin = new StreamReader(_systemDataFile);

            while (!fin.EndOfStream)
            {
                string tempBuffer = fin.ReadLine();

                if (tempBuffer.Equals("$START SYSTEMDATA$"))
                {
                    // Read in single data sets split with '%'
                    // FORMAT: T_VAL%T_COST%_time%_hasEvent
                    string tempData = fin.ReadLine();
                    string[] sysToken = tempData.Split('%');

                    T_VAL = System.Convert.ToInt32(sysToken[0]);
                    T_COST = System.Convert.ToInt32(sysToken[1]);
                    _time = System.Convert.ToInt32(sysToken[2]);
                    _hasEvent = System.Convert.ToBoolean(sysToken[3]);

                    // Read in Data
                    tempData = fin.ReadLine();

                    while (!tempData.Equals("$END SYSTEMDATA$"))
                    {
                        // read system sms log
                        if (tempData.Equals("$$START SYSTEMSMSLOG$$"))
                        {
                            tempData = fin.ReadLine();
                            while (!tempData.Equals("$$END SYSTEMSMSLOG$$"))
                            {
                                // Read sms log
                                // FORMAT: time%smsContent
                                string[] token = tempData.Split('%');

                                SMS tempSMS = new SMS(System.Convert.ToInt32(token[0]), token[1]);
                                _systemsmslog.Add(tempSMS);

                                tempData = fin.ReadLine();
                            }
                        }
                        else
                        {
                            // throw "format error" exception
                        }

                        // read interrupt log
                        tempData = fin.ReadLine();

                        if (tempData.Equals("$$START INTERRUPTLOG$$"))
                        {
                            tempData = fin.ReadLine();
                            while (!tempData.Equals("$$END INTERRUPTLOG$$"))
                            {
                                // Read interrupt log
                                // FORMAT: time%smsContent
                                string[] token = tempData.Split('%');

                                SMS tempInterrupt = new SMS(System.Convert.ToInt32(token[0]), token[1]);
                                _interruptlog.Add(tempInterrupt);

                                tempData = fin.ReadLine();
                            }
                        }
                        else
                        {
                            // throw "format error" exception
                        }

                        // read auto hopoff list
                        tempData = fin.ReadLine();

                        if (tempData.Equals("$$START AUTOHOPOFFLIST$$"))
                        {
                            tempData = fin.ReadLine();
                            while (!tempData.Equals("$$END AUTOHOPOFFLIST$$"))
                            {
                                // Read autohopoff list
                                // FORMAT: passenger
                                _autohopofflist.Add(tempData);

                                tempData = fin.ReadLine();
                            }
                        }
                        else
                        {
                            // throw "format error" exception
                        }

                        // read can hopoff now list
                        tempData = fin.ReadLine();

                        if (tempData.Equals("$$START CANHOPOFFNOWLIST$$"))
                        {
                            tempData = fin.ReadLine();
                            while (!tempData.Equals("$$END CANHOPOFFNOWLIST$$"))
                            {
                                // Read can hopoff now list
                                // FORMAT: passenger
                                _canhopoffnowlist.Add(tempData);

                                tempData = fin.ReadLine();
                            }
                        }
                        else
                        {
                            // throw "format error" exception
                        }

                        tempData = fin.ReadLine();
                    }

                    // end of file
                }
                else
                {
                    // throw "file format is wrong" error
                }
            }
            fin.Close();
        }

        // Loads the Bus Data from a data file
        public void LoadBusList()
        {
            if (!File.Exists(_busListDataFile))
            {
                // throw "file does not exist error"
                return;
            }

            StreamReader fin = new StreamReader(_busListDataFile);

            while (!fin.EndOfStream)
            {
                string tempBuffer = fin.ReadLine();

                if (tempBuffer.Equals("$START BUSLIST$"))
                {
                    tempBuffer = fin.ReadLine();

                    while (!tempBuffer.Equals("$END BUSLIST$"))
                    {
                        // Read data
                        tempBuffer = fin.ReadLine();
                        
                        // If end of bus list, break
                        if (tempBuffer.Equals("$END BUSLIST$"))
                            break;

                        if (tempBuffer.Equals("$$START BUS$$"))
                        {
                            tempBuffer = fin.ReadLine();
                            while (!tempBuffer.Equals("$$END BUS$$"))
                            {
                                // Read bus data
                                //tempBuffer = fin.ReadLine();
                                string[] busToken = tempBuffer.Split('%');

                                string tempBusName = busToken[0];
                                string tempCurrentStop = busToken[1];
                                int tempFrequency = System.Convert.ToInt32(busToken[2]);
                                int tempStatCounter = System.Convert.ToInt32(busToken[3]);

                                // create bus
                                Bus tempBus = new Bus(tempBusName, tempFrequency);
                                tempBus.UpdateCurrentStop(tempCurrentStop);
                                tempBus.SetStatCounter(tempStatCounter);

                                // Read RouteList
                                tempBuffer = fin.ReadLine();
                                if (tempBuffer.Equals("$$$START ROUTELIST$$$"))
                                {
                                    tempBuffer = fin.ReadLine();

                                    while (!tempBuffer.Equals("$$$END ROUTELIST$$$"))
                                    {
                                        string[] token = tempBuffer.Split('%');

                                        string tempBusStop = token[0];
                                        int tempTravelTime = System.Convert.ToInt32(token[1]);

                                        // add route
                                        tempBus.AddRoute(tempBusStop, tempTravelTime);

                                        tempBuffer = fin.ReadLine();
                                    }
                                }
                                else
                                {
                                    // throw "wrong format" exception
                                }

                                // Read Passenger List
                                tempBuffer = fin.ReadLine();
                                if (tempBuffer.Equals("$$$START PASSENGERLIST$$$"))
                                {
                                    tempBuffer = fin.ReadLine();

                                    while (!tempBuffer.Equals("$$$END PASSENGERLIST$$$"))
                                    {
                                        string[] token = tempBuffer.Split('%');

                                        string tempMobileNum = token[0];
                                        string tempBusName2 = token[1];
                                        string tempNextStop = token[2];

                                        // enqueue user to bus
                                        UserInQueue tempUser = new UserInQueue(tempMobileNum, tempBusName2);
                                        tempUser.UpdateNextStop(tempNextStop);
                                        tempBus.EnqueueUser(tempUser);

                                        tempBuffer = fin.ReadLine();
                                    }
                                }
                                else
                                {
                                    // throw "wrong format" exception
                                }

                                // Read subscribe list
                                tempBuffer = fin.ReadLine();
                                if (tempBuffer.Equals("$$$START SUBSCRIBELIST$$$"))
                                {
                                    tempBuffer = fin.ReadLine();
                                    while (!tempBuffer.Equals("$$$END SUBSCRIBELIST$$$"))
                                    {
                                        tempBus.AddUserToSubscribeList(tempBuffer);

                                        tempBuffer = fin.ReadLine();
                                    }
                                }
                                else
                                {
                                    // throw "wrong format" exception
                                }

                                // add bus to current databases
                                _busList.Add(tempBus);
                                // read next bus in the list
                                tempBuffer = fin.ReadLine();
                            }
                        }
                        else
                        {
                            // throw "format error" exception
                        }

                        // read empty line
                        tempBuffer = fin.ReadLine();
                    }
                    // end of file
                }
                else
                {
                    // throw "file format is wrong" error
                }
                tempBuffer = fin.ReadLine();
            }
            fin.Close();
        }

        // Loads the Bus Stop Data from a data file
        public void LoadBusStopList()
        {
            if (!File.Exists(_busStopListDataFile))
            {
                // throw "file does not exist error"
                return;
            }

            StreamReader fin = new StreamReader(_busStopListDataFile);

            while (!fin.EndOfStream)
            {
                string tempBuffer = fin.ReadLine();

                if (tempBuffer.Equals("$START BUSSTOPLIST$"))
                {
                    tempBuffer = fin.ReadLine();

                    while (!tempBuffer.Equals("$END BUSSTOPLIST$"))
                    {
                        // If end of bus stop list, break
                        //if (tempBuffer.Equals("$END BUSSTOPLIST$"))
                        //    break;

                        // Read data
                        tempBuffer = fin.ReadLine();

                        // Break if end of the bus stop list
                        if (tempBuffer.Equals("$END BUSSTOPLIST$"))
                        {
                            break;
                        }

                        if (tempBuffer.Equals("$$START BUSSTOP$$"))
                        {
                            tempBuffer = fin.ReadLine();
                            while (!tempBuffer.Equals("$$END BUSSTOP$$"))
                            {
                                // Read bus stop data
                                //tempBuffer = fin.ReadLine();

                                string[] busStopToken = tempBuffer.Split('%');

                                string tempBusStopName = busStopToken[0];
                                Boolean tempStatus = System.Convert.ToBoolean(busStopToken[1]);
                                int tempStatCounter = System.Convert.ToInt32(busStopToken[2]);
                                double tempCoordinateX = System.Convert.ToDouble(busStopToken[3]);
                                double tempCoordinateY = System.Convert.ToDouble(busStopToken[4]);

                                // create bus stop
                                BusStop tempBusStop = new BusStop(tempBusStopName);
                                tempBusStop.SetStatus(tempStatus);
                                tempBusStop.SetStatCounter(tempStatCounter);
                                tempBusStop.SetCoordinates(tempCoordinateX, tempCoordinateY);

                                // Read user queue
                                tempBuffer = fin.ReadLine();
                                if (tempBuffer.Equals("$$$START USERINQUEUE$$$"))
                                {
                                    tempBuffer = fin.ReadLine();
                                    while (!tempBuffer.Equals("$$$END USERINQUEUE$$$"))
                                    {
                                        string[] token = tempBuffer.Split('%');

                                        string tempMobileNum = token[0];
                                        string tempBusName = token[1];
                                        string tempNextStop = token[2];

                                        // add user
                                        UserInQueue tempUser = new UserInQueue(tempMobileNum, tempBusName);
                                        tempUser.UpdateNextStop(tempNextStop);

                                        tempBusStop.EnqueuePassenger(tempUser);

                                        // read next passenger
                                        tempBuffer = fin.ReadLine();
                                    }
                                }
                                else
                                {
                                    // throw "wrong format" exception
                                }

                                // Read Bus queue
                                tempBuffer = fin.ReadLine();
                                if (tempBuffer.Equals("$$$START BUSQUEUE$$$"))
                                {
                                    tempBuffer = fin.ReadLine();
                                    while (!tempBuffer.Equals("$$$END BUSQUEUE$$$"))
                                    {
                                        tempBusStop.EnqueueBus(tempBuffer);
                                        tempBuffer = fin.ReadLine();
                                    }
                                }
                                else
                                {
                                    // throw "wrong format" exception
                                }

                                // Read subscribe list
                                tempBuffer = fin.ReadLine();

                                if (tempBuffer.Equals("$$$START SUBSCRIBELIST$$$"))
                                {
                                    tempBuffer = fin.ReadLine();
                                    while (!tempBuffer.Equals("$$$END SUBSCRIBELIST$$$"))
                                    {
                                        tempBusStop.AddUserToSubscribeList(tempBuffer);
                                        tempBuffer = fin.ReadLine();
                                    }
                                }
                                else
                                {
                                    // throw "wrong format" exception
                                }

                                // add current bus stop to list
                                _busStopList.Add(tempBusStop);

                                // read next bus stop
                                tempBuffer = fin.ReadLine();
                            }
                        }
                        // read empty line
                        tempBuffer = fin.ReadLine();
                    }
                    // end of file
                }
                else
                {
                    // throw "file format is wrong" error
                }
            }
            fin.Close();
        }

        // Loads the Bus Interrupt Data from a data file
        public void LoadBusInterruptList()
        {
            if (!File.Exists(_busInterruptListDataFile))
            {
                // throw "file does not exist error"
                return;
            }

            StreamReader fin = new StreamReader(_busInterruptListDataFile);

            while (!fin.EndOfStream)
            {
                string tempBuffer = fin.ReadLine();

                if (tempBuffer.Equals("$START BUSINTERRUPTLIST$"))
                {
                    tempBuffer = fin.ReadLine();

                    while (!tempBuffer.Equals("$END BUSINTERRUPTLIST$"))
                    {
                        string[] token = tempBuffer.Split('%');

                        int tempTime = System.Convert.ToInt32(token[0]);
                        string tempBusName = token[1];
                        string tempNextStop = token[2];
                        string tempInterruptStop = token[3];

                        // create bus stop
                        BusInterrupt tempBusInterrupt = new BusInterrupt(tempTime, tempBusName, tempNextStop, tempInterruptStop);
                        _busInterruptList.Add(tempBusInterrupt);

                        // read next interrupt
                        tempBuffer = fin.ReadLine();
                    }
                }
            }
            fin.Close();
        }

        // Loads the User Data from a data file
        public void LoadUserList()
        {
            if (!File.Exists(_userDataFile))
            {
                // throw "file does not exist error"
                return;
            }

            StreamReader fin = new StreamReader(_userDataFile);

            while (!fin.EndOfStream)
            {
                string tempBuffer = fin.ReadLine();

                if (tempBuffer.Equals("$START USERLIST$"))
                {
                    tempBuffer = fin.ReadLine();

                    while (!tempBuffer.Equals("$END USERLIST$"))
                    {
                        // Read data
                        tempBuffer = fin.ReadLine();

                        // check if end of user list
                        if (tempBuffer.Equals("$END USERLIST$"))
                        {
                            break;
                        }

                        if (tempBuffer.Equals("$$START USER$$"))
                        {
                            tempBuffer = fin.ReadLine();
                            while (!tempBuffer.Equals("$$END USER$$"))
                            {
                                // Read user data
                                string[] userToken = tempBuffer.Split('%');

                                string tempuserName = userToken[0];
                                string tempMobileNum = userToken[1];
                                int tempAccount = System.Convert.ToInt32(userToken[2]);
                                string tempCreditCardNum = userToken[3];
                                PAYMENT_MODE tempPaymentMode1 = (PAYMENT_MODE)Enum.Parse(typeof(PAYMENT_MODE), userToken[4]);
                                PAYMENT_MODE tempPaymentMode2 = (PAYMENT_MODE)Enum.Parse(typeof(PAYMENT_MODE), userToken[5]);
                                int tempTicketExpiry = System.Convert.ToInt32(userToken[6]);
                                USER_STATUS tempUserStatus = (USER_STATUS)Enum.Parse(typeof(USER_STATUS), userToken[7]);
                                string tempLocation = userToken[8];
                                string tempPassword = userToken[9];

                                // create user
                                User tempUser = new User(tempuserName, tempMobileNum, tempAccount, tempCreditCardNum, tempPaymentMode1, tempPaymentMode2, tempPassword);
                                tempUser.UpdateTicketExpiry(tempTicketExpiry);
                                tempUser.UpdateUserStatus(tempUserStatus);
                                tempUser.UpdateUserLocation(tempLocation);

                                // Read sms log
                                tempBuffer = fin.ReadLine();
                                if (tempBuffer.Equals("$$$START SMSLOG$$$"))
                                {
                                    tempBuffer = fin.ReadLine();

                                    while (!tempBuffer.Equals("$$$END SMSLOG$$$"))
                                    {
                                        // add sms
                                        tempUser.AddSMSWhenLoadData(tempBuffer);
                                        tempBuffer = fin.ReadLine();
                                    }
                                }
                                else
                                {
                                    // throw "wrong format" exception
                                }

                                // Read Journey Log
                                tempBuffer = fin.ReadLine();
                                if (tempBuffer.Equals("$$$START JOURNEYLOG$$$"))
                                {
                                    tempBuffer = fin.ReadLine();

                                    while (!tempBuffer.Equals("$$$END JOURNEYLOG$$$"))
                                    {
                                        string[] token = tempBuffer.Split('%');

                                        string tempFrom = token[0];
                                        string tempTo = token[1];
                                        string tempBusName = token[2];
                                        int tempDepartTime = System.Convert.ToInt32(token[3]);
                                        int tempArriveTime = System.Convert.ToInt32(token[4]);

                                        // add journey log to user
                                        Journey tempJourney = new Journey(tempFrom, tempTo, tempBusName, tempDepartTime, tempArriveTime);
                                        tempUser.AddJourney(tempJourney);

                                        tempBuffer = fin.ReadLine();
                                    }
                                }
                                else
                                {
                                    // throw "wrong format" exception
                                }

                                // Read subscribe list
                                tempBuffer = fin.ReadLine();
                                if (tempBuffer.Equals("$$$START JOURNEYPLAN$$$"))
                                {
                                    tempBuffer = fin.ReadLine();
                                    while (!tempBuffer.Equals("$$$END JOURNEYPLAN$$$"))
                                    {
                                        string[] token = tempBuffer.Split('%');

                                        string tempFrom = token[0];
                                        string tempTo = token[1];
                                        string tempBusName = token[2];
                                        int tempDepartTime = System.Convert.ToInt32(token[3]);
                                        int tempArriveTime = System.Convert.ToInt32(token[4]);

                                        // add journey log to user
                                        // Journey tempJourneyPlan = new Journey(tempFrom, tempTo, tempBusName, tempDepartTime, tempArriveTime);
                                        tempUser.UpdateJourneyPlan(tempFrom, tempBusName, tempDepartTime);

                                        tempBuffer = fin.ReadLine();
                                    }
                                }
                                else
                                {
                                    // throw "wrong format" exception
                                }

                                // add current user to the list
                                _userList.Add(tempUser);

                                // read next user in the list
                                tempBuffer = fin.ReadLine();
                            }
                        }
                        else
                        {
                            // throw "format error" exception
                        }

                        // read empty line
                        tempBuffer = fin.ReadLine();
                    }
                    // end of file
                }
                else
                {
                    // throw "file format is wrong" error
                }
            }
            fin.Close();
        }

        // Loads the Ticket List from a data file
        public void LoadTicketList()
        {
            if (!File.Exists(_ticketListDataFile))
            {
                // throw "file does not exist error"
                return;
            }

            StreamReader fin = new StreamReader(_ticketListDataFile);

            while (!fin.EndOfStream)
            {
                string tempBuffer = fin.ReadLine();

                if (tempBuffer.Equals("$START TICKETLIST$"))
                {
                    tempBuffer = fin.ReadLine();

                    while (!tempBuffer.Equals("$END TICKETLIST$"))
                    {
                        string[] token = tempBuffer.Split('%');

                        string tempMobileNum = token[0];
                        int tempExpiryTime = System.Convert.ToInt32(token[1]);

                        TicketExpiry tempTicket = new TicketExpiry(tempMobileNum, tempExpiryTime);
                        _ticketList.Add(tempTicket);

                        // read next interrupt
                        tempBuffer = fin.ReadLine();
                    }
                }
            }
            fin.Close();
        }

        // Loads the Card List from a data file
        public void LoadCardList()
        {
            if (!File.Exists(_usedCardsListFile))
            {
                // throw "file does not exist error"
                return;
            }

            StreamReader fin = new StreamReader(_usedCardsListFile);

            while (!fin.EndOfStream)
            {
                string tempBuffer = fin.ReadLine();

                if (tempBuffer.Equals("$START CARDLIST$"))
                {
                    tempBuffer = fin.ReadLine();

                    while (!tempBuffer.Equals("$END CARDLIST$"))
                    {
                        _retailShop.AddUsedCardNumberToList(System.Convert.ToInt32(tempBuffer));

                        // read next used card
                        tempBuffer = fin.ReadLine();
                    }
                }
            }
            fin.Close();
        }

        // Loads all Data from a data file
        public void LoadAllData()
        {
            LoadSystemData();
            LoadTicketList();
            LoadBusList();
            LoadBusStopList();
            LoadBusInterruptList();
            LoadUserList();
            LoadCardList();
        }

        /****************************************************************************************************/
        // METHODS
        /****************************************************************************************************/

        // Initialize the system
        public void Init()
        {
            LoadAllData();
        }

        /****************************************************************************************************/
        // METHODS - MISCELLANEOUS
        /****************************************************************************************************/

        // Return the current system time
        public int GetTime()
        {
            return _time;
        }

        // returns the has event boolean value
        public Boolean GetHasEvent()
        {
            return _hasEvent;
        }

        //  resets has event back to false
        public void ResetHasEvent()
        {
            _hasEvent = false;
        }

        public void sendSMS(string text, string MobileNumber)
        {
            GsmCommMain comm = new GsmCommMain("COM4", 19200, 300);
            // Send an SMS message
            SmsSubmitPdu pdu;

            // The straightforward version
            pdu = new SmsSubmitPdu(text, "+65" + MobileNumber, "+6596197777");
            comm.Open();
            comm.SendMessage(pdu);
            comm.Close();
        }

        /****************************************************************************************************/
        // METHODS - T_VAL AND T_COST
        /****************************************************************************************************/

        // returns the t_val value
        public int GetTVAL()
        {
            return T_VAL;
        }

        // returns the t_cost value
        public int GetTCOST()
        {
            return T_COST;
        }

        // configures TVAL
        public void SetTVAL(int tval)
        {
            T_VAL = tval;
        }

        // configures TCOST
        public void SetTCost(int tcost)
        {
            T_COST = tcost;
        }

        /****************************************************************************************************/
        // METHODS - USER
        /****************************************************************************************************/

        // returns the user list
        public UserList GetUserList()
        {
            return _userList;
        }

        public List<string> GetUserListOfABusInstance(string busName, string currentStop)
        {
            List<string> temp = new List<string>();
            
            List<UserInQueue> userlist = _busList.FindBusWithBusName(busName).GetPassengerList();
            for (int i = 0; i < userlist.Count; i++)
            {
                string nextstop = userlist.ElementAt(i).GetNextStop();
                string prevstop = _busList.FindBusWithBusName(busName).RetrievePreviousStop(nextstop);
                if (currentStop== prevstop)
                    temp.Add(_userList.FindUserWithMobileNumber(userlist.ElementAt(i).GetMobileNum()).GetName());

            }

            



            return temp;


        }


        // gets the journey history of user
        public List<string> GetJourneyHistoryOfUser(string mobilenumber)
        {
            List<string> temp = new List<string>();
            List<Journey> journeyhistory = _userList.FindUserWithMobileNumber(mobilenumber).GetJourneyLog();

            for (int i = 0; i < journeyhistory.Count; i++)
            {
                string from = journeyhistory.ElementAt(i).GetInitialStop();
                string to = journeyhistory.ElementAt(i).GetTerminalStop();
                string busname = journeyhistory.ElementAt(i).GetBusName();
                string departtime = journeyhistory.ElementAt(i).GetDepartTime().ToString();
                string arrivetime = journeyhistory.ElementAt(i).GetArriveTime().ToString();
                string everythingcombined = "At time " + departtime + ", Depart from: " + from + ", Arrive to: " + to + " at " + arrivetime;
                temp.Add(everythingcombined);
            }
            return temp;
        }

        // gets the sms log of a user
        public List<string> GetSMSLogOfUser(string mobilenumber)
        {
            List<string> smslog = _userList.FindUserWithMobileNumber(mobilenumber).GetSMSLog();
            return smslog;
        }

        // gets a user location
        public string GetUserLocation(string mobileNum)
        {
            return _userList.FindUserWithMobileNumber(mobileNum).GetUserLocation();
        }

        // gets a user's status
        public USER_STATUS GetUserStatus(string mobileNum)
        {
            return _userList.FindUserWithMobileNumber(mobileNum).GetUserStatus();
        }

        // update a User account balance
        public void ReloadUserAccount(string mobilenumber, float amount)
        {
            _userList.FindUserWithMobileNumber(mobilenumber).ReloadAccount(amount);
        }

        // return name of a user, given phone number
        public string FindUserWithMobileNumber(string mobilenumber)
        {
            return _userList.FindUserWithMobileNumber(mobilenumber).GetName();
        }

        // return a list of mobile number
        public List<string> FindMobileNumberList()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < _userList.GetCount(); i++)
            {
                temp.Add(_userList.FindUserWithIndex(i).GetMobileNum());
            }
            return temp;
        }
        // return account balance of a given user
        public float FindAccountWithMobileNumber(string mobilenumber)
        {
            return _userList.FindUserWithMobileNumber(mobilenumber).GetAccount();
        }

        // return credit card number of a given user
        public string FindCreditCardNumberWithMobileNumber(string mobilenumber)
        {
            return _userList.FindUserWithMobileNumber(mobilenumber).GetCreditCardNum();
        }

        // return payment mode 1 of a given user
        public string FindPaymentMode1WithMobileNumber(string mobilenumber)
        {
            return _userList.FindUserWithMobileNumber(mobilenumber).GetPaymentMode1().ToString();
        }

        // return payment mode 2 of a given user
        public string FindPaymentMode2WithMobileNumber(string mobilenumber)
        {
            return _userList.FindUserWithMobileNumber(mobilenumber).GetPaymentMode2().ToString();
        }

        // update new credit card number for a user
        public void UpdateUserCreditCardNumber(string mobilenumber, string newnum)
        {
            _userList.FindUserWithMobileNumber(mobilenumber).UpdateCreditCardNum(newnum);
        }

        // update new 1st payment method for user
        public void UpdateUserFirstPaymentMethod(string mobilenumber, PAYMENT_MODE paymentmethod)
        {
            _userList.FindUserWithMobileNumber(mobilenumber).UpdatePaymentMode1(paymentmethod);
        }

        // update new 2nd payment method for user
        public void UpdateUserSecondPaymentMethod(string mobilenumber, PAYMENT_MODE paymentmethod)
        {
            _userList.FindUserWithMobileNumber(mobilenumber).UpdatePaymentMode1(paymentmethod);
        }

        // check if mobile number is duplicated
        public bool CheckDuplicateMobileNumber(string mobilenumber)
        {
            for (int i = 0; i < _userList.GetCount(); i++)
            {
                if (_userList.FindUserWithIndex(i).GetMobileNum() == mobilenumber) return true;

            }

            return false;
        }

        // check if the User's nextstop is the same as the given nextbusstop 
        public bool CheckIfPassengerIsOnBus(string mobilenumber, string busname, string nextbusstop)
        {
            for (int i = 0; i < _busList.FindBusWithBusName(busname).GetPassengerList().Count; i++)
            {
                if (_busList.FindBusWithBusName(busname).GetPassengerList().ElementAt(i).GetMobileNum() == mobilenumber)
                {
                    if (_busList.FindBusWithBusName(busname).GetPassengerList().ElementAt(i).GetNextStop() == nextbusstop)
                        return true;
                }

            }
            return false;
        }

        // creates a user object and adds it to the database
        public void CreateUser(string name, string mobileNum, float account, string creditCardNum, PAYMENT_MODE paymentMode1, PAYMENT_MODE paymentMode2, string password)
        {
            User newUser = new User(name, mobileNum, account, creditCardNum, paymentMode1, paymentMode2, password);
            _userList.Add(newUser);
        }

        /****************************************************************************************************/
        // METHODS - BUS
        /****************************************************************************************************/

        //returns the bus list
        public BusList GetBusList()
        {
            return _busList;
        }

        // returns the number of buses
        public int GetBusListLength()
        {
            return _busList.GetCount();
        }

        // returns the bus name at index i
        public string GetBusStringAtIndex(int i)
        {
            return _busList.GetBusAtIndex(i).GetName();
        }

        // get bus services use this bus stop
        public List<string> GetBusServiceUseThisBusStop(string busStopName)
        {
            List<string> temp = new List<string>();

            for (int i = 0; i < _busList.GetCount(); i++)
            {
                Bus bus = _busList.GetBusAtIndex(i);

                for (int j = 0; j < bus.GetRouteList().Count; j++)
                {
                    if (bus.GetRouteList().ElementAt(j).GetBusStopName() == busStopName)
                    {
                        temp.Add(bus.GetName());
                        break;
                    }
                }
            }
            return temp;
        }

        // returns bus services as a concatenated string
        public string GetBusStringList()
        {
            string buslist = "";

            for (int i = 0; i < _busList.GetCount(); i++)
            {
                Bus temp = _busList.GetBusAtIndex(i);

                buslist += temp.GetName();
                if (i < _busStopList.GetCount() - 1)
                {
                    buslist += "\n ";
                }
            }

            return buslist;
        }

        // creates a bus object and adds it to the database
        public void CreateBus(string name, int frequency)
        {
            Bus newBus = new Bus(name, frequency);
            _busList.Add(newBus);
        }

        /****************************************************************************************************/
        // METHODS - BUS STOP
        /****************************************************************************************************/

        // returns the bus stop list
        public BusStopList GetBusStopList()
        {
            return _busStopList;
        }

        // returns the number of bus stops
        public int GetBusStopListLength()
        {
            return _busStopList.GetCount();
        }

        // Return the bus stop name at index i
        public string GetBusStopStringAtIndex(int i)
        {
            return _busStopList.At(i).GetName();
        }

        // finds the next bus stop given a bus and a current bus stop
        public string FindNextStop(string busName, string busStopName)
        {
            return _busList.FindBusWithBusName(busName).GetNextStop(busStopName);
        }

        // returns bus stops as a concatenated string
        public string GetBusStopStringList()
        {
            string busstoplist = "";

            for (int i = 0; i < _busStopList.GetCount(); i++)
            {
                BusStop temp = _busStopList.At(i);

                busstoplist += temp.GetName();

                if (i < _busStopList.GetCount() - 1)
                {
                    busstoplist += "\n";
                }
            }

            return busstoplist;
        }

        // returns bus stops with buses
        public List<string> GetBusStopWithBusList()
        {
            List<string> busStopList = new List<string>();

            for (int i = 0; i < _busStopList.GetCount(); i++)
            {
                if (_busStopList.At(i).GetBusQueue().Count != 0)
                {
                    busStopList.Add(_busStopList.At(i).GetName());
                }
            }

            return busStopList;
        }

        // creates a bus stop object and adds it to the database
        public void CreateBusStop(string name, double coordinateX, double coordinateY)
        {
            BusStop newBusStop = new BusStop(name);
            newBusStop.SetCoordinates(coordinateX, coordinateY);
            _busStopList.Add(newBusStop);
        }

        // updates the bus stop object with the new coordinates
        public void UpdateBusStopCoordinates(string name, double coordinateX, double coordinateY)
        {
            _busStopList.FindBusStopWithBusStopName(name).SetCoordinates(coordinateX, coordinateY);
        }

        /****************************************************************************************************/
        // METHODS - ROUTE
        /****************************************************************************************************/

        // returns bus service route as a concatenated string
        public string GetRoute(string busName)
        {
            string routelist = "";
            Bus tempbus = _busList.FindBusWithBusName(busName);

            for (int i = 0; i < tempbus.GetRouteList().Count(); i++)
            {
                routelist += tempbus.GetRouteList().ElementAt(i).GetBusStopName() + "\n";
            }

            return routelist;
        }

        // return bus service route as List<string>
        public List<string> GetRouteAsList(string busName)
        {
            List<string> temp = new List<string>();

            if (busName == "")
            {
                return temp;
            }

            for (int i = 0; i < _busList.FindBusWithBusName(busName).GetRouteList().Count; i++)
            {
                temp.Add(_busList.FindBusWithBusName(busName).GetRouteList().ElementAt(i).GetBusStopName());
            }

            return temp;
        }

        // creates a route object, modifies the appropriate bus object and updates the database
        public void CreateRoute(string busName, string initialBusStop, string terminalBusStop, int traveltime)
        {
            if (_busList.FindBusWithBusName(busName).CheckBusStopExistedInRoute(initialBusStop))
            {
                if (_busList.FindBusWithBusName(busName).CheckBusStopExistedInRoute(terminalBusStop))
                {
                    _busList.FindBusWithBusName(busName).EditRouteTravelTime(initialBusStop, terminalBusStop, traveltime);
                    return;
                }
            }

            if (_busList.FindBusWithBusName(busName).CheckBusStopExistedInRoute(initialBusStop))
            {
                _busList.FindBusWithBusName(busName).InsertRouteAfter(initialBusStop, terminalBusStop, traveltime);
            }
            else
            {
                if (_busList.FindBusWithBusName(busName).CheckBusStopExistedInRoute(terminalBusStop))
                {
                    _busList.FindBusWithBusName(busName).InsertRouteBefore(terminalBusStop, initialBusStop, traveltime);
                }
                else
                {
                    _busList.FindBusWithBusName(busName).AddRoute(initialBusStop, traveltime);
                    _busList.FindBusWithBusName(busName).InsertRouteAfter(initialBusStop, terminalBusStop, traveltime);
                }
            }
        }

        /****************************************************************************************************/
        // METHODS - SYSTEM UPDATE (INCREMENTING TIME)
        /****************************************************************************************************/

        // increases the time by 1 unit
        public void IncreaseTime()
        {
            _time++;

            _busStopList.Refresh();
            _canhopoffnowlist.Clear();

            for (int i = 0; i < _busList.GetCount(); i++)
            {
                LocateBus(_busList.GetBusAtIndex(i).GetName()); 
  
                List<UserInQueue> temp = _busList.GetBusAtIndex(i).GetPassengerList();
                int count = temp.Count;

                for (int j = 0; j < count; j++)
                {
                    LocatePassenger(_busList.GetBusAtIndex(i).GetName(), temp.ElementAt(j));

                    if (count != temp.Count)
                        j--;

                    count = temp.Count;
                }
            }

            LocateBusInterrupt();

            // check if topmost ticket is valid
            if (_ticketList.GetCount() > 0)
            {
                while (_ticketList.GetTopExpiry() <= _time)
                {
                    // if not valid, check if user is on bus
                    if (_userList.FindUserWithMobileNumber(_ticketList.GetTopTicketUserMobile()).IsOnBus())
                    {
                        IssueNewTicket(_ticketList.GetTopTicketUserMobile());
                        DeductFromModeOfPayment(_ticketList.GetTopTicketUserMobile());
                    }

                    // whether user is on bus or not, delete overdue ticket from the ticket expiry queue
                    _ticketList.DeleteTopTicket();

                    if (!(_ticketList.GetCount() > 0))
                        break;
                }
            }
           
        }

        // locate which bus is at which bus stop
        public void LocateBus(string busName)
        {
            Bus tempBus = _busList.FindBusWithBusName(busName);

            if (tempBus.GetRouteList().Count == 0)
                return;

            BusInterrupt ibus = null;

            for (int i = 0; i < _busInterruptList.Count; i++)
            {
                if (_busInterruptList.ElementAt(i).GetBusName() == busName)
                {
                    ibus = _busInterruptList.ElementAt(i);
                    break;
                }
            }

            int time = _time;
            int numberofbus = time / tempBus.GetFrequency() + 1;

            for (int i=0; i< numberofbus ; i++)
            {
                time = _time - i*tempBus.GetFrequency();

                if (time == 0)
                    _busStopList.FindBusStopWithBusStopName(tempBus.GetRouteList().ElementAt(0).GetBusStopName()).EnqueueBus(busName);
                else
                {
                    int j;

                    for (j=0; j < tempBus.GetRouteList().Count; j++)
                    {
                        time -= tempBus.GetRouteList().ElementAt(j).GetTravelTime();

                        if (time <= 0)
                            break;
                    }

                    if (time == 0)
                    {
                        if (!BusStopIsInterrupted(_busStopList.FindBusStopWithBusStopName(tempBus.GetRouteList().ElementAt(j+1).GetBusStopName()).GetName()))
                            _busStopList.FindBusStopWithBusStopName(tempBus.GetRouteList().ElementAt(j+1).GetBusStopName()).EnqueueBus(busName);
                        string busstopname = tempBus.GetRouteList().ElementAt(j+1).GetBusStopName();
                        List<UserInQueue> userlist = _busList.FindBusWithBusName(busName).GetPassengerList();

                        for (int k = 0; k < userlist.Count; k++)
                        {
                            string username = _userList.FindUserWithMobileNumber(userlist.ElementAt(k).GetMobileNum()).GetName();
                            if ((userlist.ElementAt(k).GetNextStop() == busstopname) && (!BusStopIsInterrupted(busstopname)))
                            {
                                _userList.FindUserWithName(username).UpdateUserLocation(busstopname);
                                userlist.ElementAt(k).UpdateNextStop(_busList.FindBusWithBusName(tempBus.GetName()).RetrieveNextStop(busstopname));
                                _canhopoffnowlist.Add(username);

                            }


                        }
                        _hasEvent = true;




                    }
                }
            }
        }

        // locates a passenger
        public void LocatePassenger(string busName, UserInQueue user)
        {
            string currentstop = user.GetNextStop();
            string currentNextStop = _busList.FindBusWithBusName(busName).RetrieveNextStop(user.GetNextStop());

            // update passenger's location
            User tempUser = _userList.FindUserWithMobileNumber(user.GetMobileNum());
            //tempUser.UpdateUserLocation(currentstop);

            if (BusNameIsInBusInterruptList(busName, currentNextStop))
                return;

            //user.UpdateNextStop(currentNextStop);

            if (currentNextStop == null)
            {
                //List<string> temp = (_busStopList.FindBusStopWithBusStopName(_busList.FindBusWithBusName(busName).GetLastStop()).GetBusQueue());
                //if (temp.IndexOf(busName) != -1)
                if (currentstop == null)
                {
                    SMS newsms = new SMS(_time, "At time " + _time + " user " + tempUser.GetName() + " has been auto hopped off.");
                    _systemsmslog.Add(newsms);
                    _autohopofflist.Add(_userList.FindUserWithMobileNumber(user.GetMobileNum()).GetName());
                    _userList.FindUserWithMobileNumber(user.GetMobileNum()).AddSMS("At time " + _time + " you have been auto hopped off.");
                    DequeueUserFromBus(_userList.FindUserWithMobileNumber(user.GetMobileNum()).GetName(), busName, _busList.FindBusWithBusName(busName).GetLastStop());
                }
            }
        }

        /****************************************************************************************************/
        // METHODS - SYSTEM CHECK
        /****************************************************************************************************/
        
        // check if is there any passenger on any bus running on the road
        public bool ThereIsPassengerOnBus()
        {
            for (int i = 0; i < _busList.GetCount(); i++)
            {
                if (_busList.GetBusAtIndex(i).GetPassengerList().Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        // check if is there any passenger on any bus at bus stop
        public bool ThereIsPassengerOnBusAtBusStop()
        {
            for (int i = 0; i < _busList.GetCount(); i++)
            {
                if (_busList.GetBusAtIndex(i).GetPassengerList().Count > 0 && _busList.GetBusAtIndex(i).GetCurrentStop() != null)
                {
                    return true;
                }
            }
            return false;
        }

        // gets the system sms log
        public List<string> GetSystemCurrentSMSLog()
        {
            List<string> allsmslog = new List<string>();

            if (_systemsmslog.Count == 0)
            {
                return null;
            }

            for (int i = _systemsmslog.Count - 1; i >= 0; i--)
            {
                if (_systemsmslog.ElementAt(i).GetTime() < _time)
                {
                    break;
                }

                allsmslog.Add(_systemsmslog.ElementAt(i).GetSMSContent());
            }

            return allsmslog;

        }

        /****************************************************************************************************/
        // METHODS - HOPON HOPOFF
        /****************************************************************************************************/

        // takes care of auto hop off
        public List<string> GetWhoHasAutoHopOff()
        {
            List<string> temp = _autohopofflist;
            return temp;
        }

        // clears the auto hop off list
        public void ClearAutoHopOffList()
        {
            _autohopofflist.Clear();
        }

        // takes care of hopping on
        public List<string> GetWhoHasHopOn()
        {
            List<string> whohashopon = new List<string>();

            for (int i = 0; i < _busStopList.GetCount(); i++)
            {
                // we are now at bus stop i
                // we check for current passenger queue at bus stop i
                // then check if the desirable bus is at bus stop i
                List<UserInQueue> passengerqueue = _busStopList.At(i).GetPassengerList();
                List<string> busqueue = _busStopList.At(i).GetBusQueue();
                int j = 0;

                while (_busStopList.At(i).GetPassengerList().Count >0)
                {
                    if (j == _busStopList.At(i).GetPassengerList().Count)
                    {
                        break;
                    }

                    UserInQueue tempuserinqueue = passengerqueue.ElementAt(j);
                    string temp = tempuserinqueue.GetBusName();

                    if (busqueue.IndexOf(temp) != -1)
                    {
                        string mobilenum = tempuserinqueue.GetMobileNum();
                        string name = _userList.FindUserWithMobileNumber(mobilenum).GetName();

                        whohashopon.Add(name);
                        _userList.FindUserWithMobileNumber(_busStopList.At(i).GetPassengerList().ElementAt(j).GetMobileNum()).UpdateJourneyPlan(_busStopList.At(i).GetName(), temp, _time);

                        // update user location and status
                        _userList.FindUserWithMobileNumber(mobilenum).UpdateUserStatus(USER_STATUS.active);
                        //_userList.FindUserWithMobileNumber(mobilenum).UpdateUserLocation(temp);

                        _busList.FindBusWithBusName(temp).EnqueueUser(tempuserinqueue);
                        _busStopList.At(i).DequeuePassengerWithMobileNum(mobilenum);
                        
                        // We check for the expiry ticket here
                        if (_userList.FindUserWithMobileNumber(mobilenum).GetTicketExpiry() < _time)
                        {
                            // invalid ticket
                            // now we update the ticket
                            IssueNewTicket(mobilenum);
                            // then deduct money from the user
                            DeductFromModeOfPayment(mobilenum);
                        }
                        else
                        {
                            _userList.FindUserWithMobileNumber(mobilenum).AddSMS("At time " + _time + " you have hopped on bus " + _busList.FindBusWithBusName(temp).GetName() + " and your TVAL is still valid");
                            string smscontent = "At time " + _time + " " + name +  " have hopped on bus " + _busList.FindBusWithBusName(temp).GetName() + " and his/her TVAL is still valid";
                            SMS smsforuser = new SMS(_time, smscontent);
                            _systemsmslog.Add(smsforuser);
                        }


                        // We send notification if there's a bus stop in the waiting bus's route that has been interrupted
                        Bus bus = _busList.FindBusWithBusName(temp);
                        List<Route> busroute = bus.GetRouteList();
                        string tempsms1 = "At time " + _time + " you wait for bus " + bus.GetName() + ". We want to notify you that these following bus stops" + " has/have been interrupted";
                        List<string> busstopname = new List<string>();
                        for (int k = 0; k < busroute.Count; k++)
                        {         
                           
                            // check if busroute[k] is in the bus stop interrupt

                            if (_busStopList.FindBusStopWithBusStopName(busroute.ElementAt(k).GetBusStopName()).GetStatus() == false)
                            {
                                // send msg to user
                                busstopname.Add(busroute.ElementAt(k).GetBusStopName());
                            }
                        }
                        for (int k = 0; k < busstopname.Count; k++)
                        {
                            tempsms1 = tempsms1 + " " + busstopname.ElementAt(k);
                        }
                        //if (busstopname.Count>0)
                        //_userList.FindUserWithMobileNumber(mobilenum).AddSMS(tempsms1);

                        j--;
                    }
                    j++;
                }
            }
            return whohashopon;
        }

        // issues a new ticket
        public void IssueNewTicket(string mobileNum)
        {
            TicketExpiry newTicket = new TicketExpiry(mobileNum, _time + T_VAL);
            _ticketList.Add(newTicket);
            _userList.FindUserWithMobileNumber(mobileNum).UpdateTicketExpiry(_time + T_VAL);
            //sendSMS("hello "+ FindUserWithMobileNumber(mobileNum), mobileNum);
        }

        // deducts money from the indicated mode of payment
        private void DeductFromModeOfPayment(string mobileNum)
        {
            User currentuser = _userList.FindUserWithMobileNumber(mobileNum);

            if (currentuser.GetPaymentMode1() == PAYMENT_MODE.account)
            {
                if (currentuser.GetAccount() >= T_COST)
                {
                    currentuser.ReloadAccount(currentuser.GetAccount() - T_COST);

                    string temp = "At time " + _time + " " + currentuser.GetName() + " account balance has been deducted by " + T_COST;
                    currentuser.AddSMS("At time " + _time + " your account balance has been deducted by " + T_COST);

                    SMS tempSMS = new SMS(_time, temp);
                    _systemsmslog.Add(tempSMS);
                   

                    if (currentuser.GetAccount() < T_COST)
                    {
                        currentuser.AddSMS("At time " + _time + " your account balance is below the minimum fare cost which is " + T_COST + ". Please top-up before boarding the next bus.");
                        temp = "At time " + _time + " " + currentuser.GetName() + " account balance is below the minimum fare cost which is " + T_COST + ". Please top-up before boarding the next bus.";
                        SMS tempSMS1 = new SMS(_time, temp);
                        _systemsmslog.Add(tempSMS1);
                    }
                }
                else
                {
                    if (currentuser.GetPaymentMode2() == PAYMENT_MODE.credit)
                    {
                        currentuser.AddSMS("At time " + _time + " Your credit card has been deducted by " + T_COST);
                        string temp = "At time " + _time + " " + currentuser.GetName() + " credit card has been deducted by " + T_COST;
                        SMS tempSMS = new SMS(_time, temp);
                        _systemsmslog.Add(tempSMS);
                    }
                    else
                    {
                        currentuser.AddSMS("At time " + _time + " Your mobile account has been deducted by " + T_COST);
                        string temp = "At time " + _time + " " + currentuser.GetName() + " mobile account has been deducted by " + T_COST;
                        SMS tempSMS = new SMS(_time, temp);
                        _systemsmslog.Add(tempSMS);
                    }
                }
            }
            else if (currentuser.GetPaymentMode1() == PAYMENT_MODE.credit)
            {
                currentuser.AddSMS("At time " + _time + " Your credit card has been deducted by " + T_COST);                
                string temp = "At time " + _time + " " + currentuser.GetName() + " credit card has been deducted by " + T_COST;
                SMS tempSMS = new SMS(_time, temp);
                _systemsmslog.Add(tempSMS);
            }
            else
            {
                currentuser.AddSMS("At time " + _time + " Your mobile account has been deducted by " + T_COST);
                string temp = "At time " + _time + " " + currentuser.GetName() + " mobile account has been deducted by " + T_COST;
                SMS tempSMS = new SMS(_time, temp);
                _systemsmslog.Add(tempSMS);
            }
        }
        public bool CanEnqueueUserToBusStopGivenDetail(string mobileNum, string busName, string busStopName)
        {
            Bus tempbus = _busList.FindBusWithBusName(busName);
            if (tempbus.GetLastStop() == busStopName) return false;
               
            

            return true;
        }
        public bool CanEnqueueUserToBusStop(string mobileNum)
        {
            for (int i = 0; i < _busList.GetCount(); i++)
            {
                List<UserInQueue> templist = _busList.GetBusAtIndex(i).GetPassengerList();
                if (templist == null) continue;
                for (int j = 0; j < templist.Count; j++)
                {
                    if (templist.ElementAt(j).GetMobileNum() == mobileNum) return false;
                }

            }
            for (int i = 0; i < _busStopList.GetCount(); i++)
            {
                List<UserInQueue> templist1 = _busStopList.At(i).GetPassengerList();
                for (int j = 0; j < templist1.Count; j++)
                {
                    if (templist1.ElementAt(j).GetMobileNum() == mobileNum) return false;
                }

            }
            return true;
        }

        // enqueues a user to a bus stop
        public void EnqueueUserToBusStop(string mobileNum, string stop, string bus)
        {
            // check if bus stop is available
            // if available, enqueue user
            // otherwise, throw error exception "Bus Stop is currently not in operation."

            // get bus stop
            BusStop currentBusStop = _busStopList.FindBusStopWithBusStopName(stop);

            if (currentBusStop.GetStatus())
            {
                UserInQueue newUser = new UserInQueue(mobileNum, bus);
                newUser.UpdateNextStop(FindNextStop(bus, stop));
                currentBusStop.EnqueuePassenger(newUser);

                // update user object
                User tempUser = _userList.FindUserWithMobileNumber(mobileNum);
                tempUser.UpdateUserLocation(stop);
                tempUser.UpdateUserStatus(USER_STATUS.waiting);

                Bus thebus = _busList.FindBusWithBusName(bus);
                List<Route> busroute = thebus.GetRouteList();
                string tempsms1 = "At time " + _time + " you wait for bus " + thebus.GetName() + ". We want to notify you that these following bus stops" + " has/have been interrupted: ";
                List<string> busstopname = new List<string>();
                for (int k = 0; k < busroute.Count; k++)
                {

                    // check if busroute[k] is in the bus stop interrupt

                    if (_busStopList.FindBusStopWithBusStopName(busroute.ElementAt(k).GetBusStopName()).GetStatus() == false)
                    {
                        // send msg to user
                        busstopname.Add(busroute.ElementAt(k).GetBusStopName());
                    }
                }
                for (int k = 0; k < busstopname.Count; k++)
                {
                    tempsms1 = tempsms1 + " " + busstopname.ElementAt(k);
                }
                if (busstopname.Count > 0)
                {
                    _userList.FindUserWithMobileNumber(mobileNum).AddSMS(tempsms1);
                    SMS sms = new SMS(_time, tempsms1);
                    _systemsmslog.Add(sms);
                }
            }
            else
            {
                // throw "Bus Stop is currently not in operation."
            }
        }

        // dequeques a user from a bus
        public void DequeueUserFromBus(string userName, string busName, string terminalBusStop)
        {
            for (int i = 0; i < _busList.FindBusWithBusName(busName).GetPassengerList().Count(); i++)
            {
                if (_busList.FindBusWithBusName(busName).GetPassengerList()[i].GetMobileNum() == _userList.FindUserWithName(userName).GetMobileNum())
                {
                    _busList.FindBusWithBusName(busName).GetPassengerList().RemoveAt(i);

                    // update user location and user status
                    User tempUser = _userList.FindUserWithName(userName);
                    Journey lastJourney = new Journey(tempUser.GetJourneyPlan().GetInitialStop(), terminalBusStop, busName, _userList.FindUserWithName(userName).GetJourneyPlan().GetDepartTime(), _time);

                    tempUser.AddJourney(lastJourney);
                    tempUser.UpdateUserStatus(USER_STATUS.inactive);
                    tempUser.UpdateUserLocation(null);
                    
                    //_userList.FindUserWithName(userName).AddJourney(lastJourney);
                    User user = _userList.FindUserWithName(userName);

                    if (!_busList.FindBusWithBusName(busName).ExistSubscriber(user.GetMobileNum()))
                    {
                        if (user.NumberOfTimesTravelThisBus(busName) >= BUS_THRESHOLD)
                            AddBusServiceSubscriber(user.GetMobileNum(), busName);
                    }

                    List<Route> busroute = _busList.FindBusWithBusName(busName).GetRouteList();

                    for (int j = 0; j < busroute.Count; j++)
                    {
                        if (busroute.ElementAt(j).GetBusStopName() == lastJourney.GetInitialStop())
                        {
                            if (!_busStopList.FindBusStopWithBusStopName(busroute.ElementAt(j).GetBusStopName()).ExistSubscriber(user.GetMobileNum()))
                                if (NumberOfTimesUserGoThroughThisBusStop(user.GetMobileNum(), busroute.ElementAt(j).GetBusStopName()) >= BUSSTOP_THRESHOLD)
                                    AddBusStopSubscriber(user.GetMobileNum(), busroute.ElementAt(j).GetBusStopName());
                        }
                    }

                    break;
                }
            }
        }

        // gets the list of users who can hop off at the current time
        public List<string> GetWhoCanHopOffNow()
        {
            return _canhopoffnowlist;
        }

        // hops off a user from its bus
        public void HopOffUser(string username)
        {
            User user = _userList.FindUserWithName(username);
            string mobilenumber = user.GetMobileNum();
            DequeueUserFromBus(username, user.GetJourneyPlan().GetBusName(), user.GetUserLocation());
            _canhopoffnowlist.Remove(username);
        }

        /****************************************************************************************************/
        // METHODS - BUS INTERRUPTS
        /****************************************************************************************************/

        // sets a bus interrupt
        public void SetBusInterrupt(string busname, string nextstop)
        {
            Bus bus = _busList.FindBusWithBusName(busname);
            // send notification to user on the bus
            for (int i = 0; i < bus.GetPassengerList().Count; i++)
            {
                UserInQueue user = bus.GetPassengerList().ElementAt(i);
                User realuser = _userList.FindUserWithMobileNumber(user.GetMobileNum());
                realuser.AddSMS("At time " + _time + " this bus has been interrupted, and will be terminated at the next stop. Please alight at the next bus stop");
                string smsnews = "At time " + _time + " notification sent to user " + realuser.GetName() + " about bus " + bus.GetName() + " interruption";
                SMS sms = new SMS(_time, smsnews);
                _systemsmslog.Add(sms);
            }

            string interruptstop = bus.RetrievePreviousStop(nextstop);
            BusInterrupt ibus = new BusInterrupt(_time, busname, nextstop, interruptstop);

            _busInterruptList.Add(ibus);

            if (_busList.FindBusWithBusName(busname).GetPassengerList().Count > 0)
            {
                string currentstop = _busList.FindBusWithBusName(busname).RetrievePreviousStop(nextstop);

                for (int i = 0; i < _busList.FindBusWithBusName(busname).GetPassengerList().Count; i++)
                {
                    UserInQueue user = _busList.FindBusWithBusName(busname).GetPassengerList().ElementAt(i);
                    User user1 = _userList.FindUserWithMobileNumber(user.GetMobileNum());

                    DequeueUserFromBus(user1.GetName(), busname, currentstop);
                    EnqueueUserToBusStop(user1.GetMobileNum(), currentstop, busname);
                }
            }

            // notify user
            string message = "At " + _time + " bus " + busname + " has been interrupted.";
            NotifyBusSubscriber(busname, message);
            SMS systemmessage = new SMS(_time, message);
            _interruptlog.Add(systemmessage);
        }

        public void RemoveBusInterrupt(string busname, string nextstop)
        {
            for (int i = 0; i < _busInterruptList.Count; i++)
            {
                if (_busInterruptList.ElementAt(i).GetBusName() == busname)
                {
                    if (_busInterruptList.ElementAt(i).GetNextStop() == nextstop)
                    {
                        _busInterruptList.RemoveAt(i);
                        return;
                    }
                }
            }
        }

       
        // check if a bus is in the interrupt list, by insert the busname and its next stop
        public bool BusNameIsInBusInterruptList(string busName, string nextstop)
        {
            for (int i = 0; i < _busInterruptList.Count; i++)
            {
                if ((_busInterruptList.ElementAt(i).GetBusName() == busName) && (_busInterruptList.ElementAt(i).GetNextStop() == nextstop))
                {
                    return true;
                }
            }
            return false;
        }

        // check if a bus is in the interrupt list, by insert the busname and its current stop
        public bool BusNameIsInBusInterruptList1(string busName, string currentstop)
        {
            string nextstop = _busList.FindBusWithBusName(busName).RetrieveNextStop(currentstop);

            for (int i = 0; i < _busInterruptList.Count; i++)
            {
                if ((_busInterruptList.ElementAt(i).GetBusName() == busName) && (_busInterruptList.ElementAt(i).GetNextStop() == nextstop))
                {
                    return true;
                }
            }
            return false;
        }

        // locates the interrupted buses
        public void LocateBusInterrupt()
        {
            for (int i = 0; i < _busInterruptList.Count; i++)
            {
                BusInterrupt ibus = _busInterruptList.ElementAt(i);
                int timedifference = _time - ibus.GetInterruptTime();
                Bus tempbus = _busList.FindBusWithBusName(ibus.GetBusName());
                List<Route> tempbusroute = tempbus.GetRouteList();
                int j = 0;

                for (j = 0; j < tempbusroute.Count; j++)
                {
                    if (tempbusroute.ElementAt(j).GetBusStopName() == ibus.GetInterruptStop())
                    {
                        break;
                    }
                }

                if (j==tempbusroute.Count)
                {
                    _busInterruptList.RemoveAt(i);
                    i--;
                }
                j++;

                while (true)
                {
                    timedifference -= tempbusroute.ElementAt(j).GetTravelTime();
                    j++;

                    if (j == tempbusroute.Count)
                    {
                        break;
                    }
                    if (timedifference <= 0)
                    {
                        break;
                    }
                }

                if (j == tempbusroute.Count)
                {
                    _busInterruptList.RemoveAt(i);
                    i--;
                }
                else
                {
                    ibus.SetNextStop(tempbusroute.ElementAt(j).GetBusStopName());
                }
            }
        }

        /****************************************************************************************************/
        // METHODS - BUS STOP INTERRUPTS
        /****************************************************************************************************/

        // Sets a bus stop's status to unavailable
        public List<string> SetBusStopInterrupt(string busStopName)
        {
            // get bus stop with bus stop name
            BusStop currentBusStop = _busStopList.FindBusStopWithBusStopName(busStopName);
            // set status of bus stop to unavailable
            currentBusStop.SetStatus(false);

            // update the status and location of all passengers
            for (int i = 0; i < currentBusStop.GetPassengerCount(); i++)
            {
                User tempUser = _userList.FindUserWithMobileNumber(currentBusStop.GetPassengerList()[i].GetMobileNum());
                tempUser.UpdateUserLocation(null);
                tempUser.UpdateUserStatus(USER_STATUS.inactive);
            }

            string message = "At " + _time + " bus stop " + busStopName + " has been under construction";
            NotifyBusStopSubscriber(busStopName, message);
            SMS systemmessage = new SMS(_time, message);
            _interruptlog.Add(systemmessage);

            // get current passengers in the bus stop
            List<UserInQueue> currentPassengers = new List<UserInQueue>();
            currentPassengers = currentBusStop.GetPassengerList();
            List<string> passengerList;
            passengerList = new List<string>();

            for (int i = 0; i < currentPassengers.Count(); i++)
            {
                passengerList.Add(currentPassengers[i].GetMobileNum());
            }

            // dequeue passengers from the bus stop
            currentBusStop.ClearUserQueue();

            return passengerList;
        }

        // sets a bus stop to available again
        public void RemoveBusStopInterrupt(string busStopName)
        {
            // get bus stop with bus stop name
            BusStop currentBusStop = _busStopList.FindBusStopWithBusStopName(busStopName);
            // set the bus stop status to available
            currentBusStop.SetStatus(true);
            string message = "At " + _time + " bus stop" + busStopName + " is working again";
            NotifyBusStopSubscriber(busStopName, message);
            SMS systemmessage = new SMS(_time, message);
            _interruptlog.Add(systemmessage);
        }

        // returns the status of a bus stop
        public Boolean BusStopIsInterrupted(string busStopName)
        {
            // get bus stop with bus stop name
            BusStop currentBusStop = _busStopList.FindBusStopWithBusStopName(busStopName);
            // return the inverse of the status of bus
            return (!currentBusStop.GetStatus());
        }

        /****************************************************************************************************/
        // METHODS - INTERRUPT NOTIFICATIONS/SUBSCRIPTION
        /****************************************************************************************************/

        // adds a subscriber to a bus service
        public void AddBusServiceSubscriber(string mobilenumber, string busname)
        {
            _busList.FindBusWithBusName(busname).AddUserToSubscribeList(mobilenumber);
        }

        // adds a subscriber to a bus stop
        public void AddBusStopSubscriber(string mobilenumber, string busstopname)
        {
            _busStopList.FindBusStopWithBusStopName(busstopname).AddUserToSubscribeList(mobilenumber);
        }

        // removes a subscriber from a bus
        public void RemoveBusServiceSubscriber(string mobilenumber, string busname)
        {
            _busList.FindBusWithBusName(busname).RemoveUserFromSubscribeList(mobilenumber);

        }

        // removes a subscriber from a bus stop
        public void RemoveBusStopSubscriber(string mobilenumber, string busstopname)
        {
            _busStopList.FindBusStopWithBusStopName(busstopname).RemoveUserFromSubscribeList(mobilenumber);
        }

        // notify a bus subscriber
        public void NotifyBusSubscriber(string busname, string message)
        {
            List<string> subscriber = _busList.FindBusWithBusName(busname).GetUserSubscribeList();

            for (int i = 0; i < subscriber.Count; i++)
            {
                _userList.FindUserWithMobileNumber(subscriber.ElementAt(i)).AddSMS(message);
                string smssystem = "At " + _time + " notification has been sent to user " + _userList.FindUserWithMobileNumber(subscriber.ElementAt(i)).GetName() + " because of bus " + busname + " interruption";
                SMS systemsms = new SMS(_time, smssystem);
                _systemsmslog.Add(systemsms);
            } 
        }

        // notify a bus stop subscriber
        public void NotifyBusStopSubscriber(string busstopname, string message)
        {
            List<string> subscriber = _busStopList.FindBusStopWithBusStopName(busstopname).GetUserSubscribeList();

            for (int i = 0; i < subscriber.Count; i++)
            {
                _userList.FindUserWithMobileNumber(subscriber.ElementAt(i)).AddSMS(message);
                string smssystem = "At " + _time + "notification has been sent to user " + _userList.FindUserWithMobileNumber(subscriber.ElementAt(i)).GetName() + " because of bus stop " + busstopname + " interruption";
                SMS systemsms = new SMS(_time, smssystem);
                _systemsmslog.Add(systemsms);
            } 
        }

        // returns the current interrupt log
        public List<string> GetCurrentInterruptLog()
        {
            List<string> interruptlog = new List<string>();

            if (_interruptlog == null)
                return null;

            for (int i = 0; i < _interruptlog.Count; i++)
            {
                if (_interruptlog.ElementAt(i).GetTime() == _time)
                {
                    interruptlog.Add(_interruptlog.ElementAt(i).GetSMSContent());
                }
            }

            return interruptlog;
        }

        public int NumberOfTimesUserGoThroughThisBusStop(string mobilenumber, string busStopName)
        {
            User user = _userList.FindUserWithMobileNumber(mobilenumber);
            List<Journey> journeyLog = user.GetJourneyLog();

            int count = 0;

            for (int i = 0; i < journeyLog.Count; i++)
            {
                Bus bus = _busList.FindBusWithBusName(journeyLog.ElementAt(i).GetBusName());
                List<Route> busroute = bus.GetRouteList();
                int u =0, v=0;

                for (int j = 0; j < busroute.Count - 1; j++)
                {
                    if (busroute.ElementAt(j).GetBusStopName() == busStopName)
                    {
                        if (u != 0)
                        {
                            if ((u <= j) && (v == 0))
                            {
                                count++;
                                break;
                            }
                        }
                    }
                    if (journeyLog.ElementAt(i).GetInitialStop() == busroute.ElementAt(j).GetBusStopName())
                    {
                        u = j;
                    }
                    if (journeyLog.ElementAt(i).GetTerminalStop() == busroute.ElementAt(j).GetBusStopName())
                    {
                        v = j;
                    }
                }

                if (i == journeyLog.Count - 1)
                {
                    if (journeyLog.ElementAt(i).GetTerminalStop() == busStopName)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public bool CheckPassWordLogin(string mobilenumber, string password)
        {
            if (_userList.FindUserWithMobileNumber(mobilenumber).CheckPassword(password))
            {
                return true;
            }

            return false;
        }

        public List<string> GetBusStopInterruptList()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < _busStopList.GetCount(); i++)
            {
                if (_busStopList.At(i).GetStatus() == false) temp.Add(_busStopList.At(i).GetName());
            }
            return temp;

        }

        public List<string> GetBusStopNotInterruptList()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < _busStopList.GetCount(); i++)
            {
                if (_busStopList.At(i).GetStatus() == true) temp.Add(_busStopList.At(i).GetName());
            }
            return temp;

        }

        /****************************************************************************************************/
        // METHODS - INTERRUPT NOTIFICATIONS/SUBSCRIPTION
        /****************************************************************************************************/

        // returns a new card number
        public int IssueNewCardNumber(int topUpValue)
        {
            return _retailShop.GenerateCardNumber(topUpValue);
        }

        // tops up a user's account and records the card number as used
        public int TopUp(string mobileNumber, int cardNumber)
        {
            int topUpValue = _retailShop.GetTopUpValueOfCard(cardNumber);
            int bonusValue = _retailShop.GetBonusValueOfCard(cardNumber);

            if (!_retailShop.CheckIfUsed(cardNumber) && _retailShop.CheckIfValid(cardNumber))
            {
                _retailShop.AddUsedCardNumberToList(cardNumber);
                _userList.FindUserWithMobileNumber(mobileNumber).ReloadAccount(_userList.FindUserWithMobileNumber(mobileNumber).GetAccount() + topUpValue + bonusValue);

                return (topUpValue + bonusValue);
            }
            else
            {
                // return an error i.e. invalid card is used
                // currently nothing will be added
                return 0;
            }
        }

        /****************************************************************************************************/
        // METHODS - STATISTICS COUNTERS
        /****************************************************************************************************/

        // gets the stat count for the indicated bus name
        public int GetBusStatCounter(string busName)
        {
            return _busList.FindBusWithBusName(busName).GetStatCounter();
        }

        // gets the stat count for the indicated bus stop name
        public int GetBusStopStatCounter(string busStopName)
        {
            return _busStopList.FindBusStopWithBusStopName(busStopName).GetStatCounter();
        }

        // START OF BUS STOP COORDINATES
        public double[] GetBusStopCoordinates(string busStopName)
        {
            return _busStopList.FindBusStopWithBusStopName(busStopName).GetCoordinates();
        }
    }
}
