using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EzySystem_Class;
//using BusStop_Class;
using UserInQueue_Class;

/****************************************
 * NOTES
 * 1) UserUI will contain only the raw text info
 * 2) The only object instance contained in the UI is EzySystem
 * 3) Other objects must be placed in EzySystem class
 * 4) TextUI, AdminUI, UserUI and SimulationUI inherit from UI_Class (to be implemented later)
 * 
 * STATE TABLE
 * 1) Enqueue a trip for a passenger
 * 2) Go to next available event
 ****************************************/

namespace SimulationUI_Class
{
    class SimulationUI
    {
        // ATTRIBUTES
        private int _state;

        // CONSTRUCTOR
        public SimulationUI() {
            _state=-1;
        }
        ~SimulationUI() { }

        // METHODS
        // initializing page
        public void Initialize()
        {
            // Load data here
        }

        public void Exit()
        {
            // Save data here
        }

        // returns the current state of the software
        public int GetState()
        {
            return _state;
        }

        // lets a user choose an option from the given set of options
        public void ChooseOption(EzySystem ezySystem)
        {
            Console.Clear();
            Console.WriteLine("SIMULATION MENU- Local time is now: " + ezySystem.GetTime() );
            Console.WriteLine("\n0) Return to Previous Menu");
            Console.WriteLine("1) Start a passenger trip");
            Console.WriteLine("2) Go to next available event");

            Console.Write("\nPlease enter option number: ");
            _state = Int32.Parse(Console.ReadLine());
        }

        // executes the option according to chosen state
        public void ExecuteOption(EzySystem ezySystem)
        {
            if (_state == 0)        // go to previous menu
            {
                return;
            }
            else if (_state == 1)   // add a trip
            {
                Console.WriteLine("\nPlease enter the following details.");
                Console.Write("1) Passenger mobile number: ");
                string mobileNum = Console.ReadLine();
                Console.Write("2) Bus Stop to queue into: ");
                string stop = Console.ReadLine();
                Console.Write("2) Bus Service to hop into: ");
                string bus = Console.ReadLine();
                ezySystem.EnqueueUserToBusStop(mobileNum, stop, bus);
            }
            else if (_state == 2)   // go to next available event (when bus arrives to a bus stop)
            {
                // increase time first
                // continue increasing time until the next event is reached (i.e. bus arrives a bus stop)

                while (!ezySystem.GetHasEvent())
                {
                    ezySystem.IncreaseTime();
                }
                Console.Clear();
                Console.WriteLine("SIMULATION MENU- Local time is now: " + ezySystem.GetTime());
                Console.WriteLine("\n0) Return to Previous Menu");
                Console.WriteLine("1) Start a passenger trip");
                Console.WriteLine("2) Go to next available event");

                Console.Write("\nPlease enter option number: 2\n");
                
                // after increasing the time, check the bus stop list for buses which has arrived and display all of them
                DisplayAllBusArrivals(ezySystem);
                // Ask 'God' if any user wants to hop off

                UserAutoHopOff(ezySystem);

                UserHopOffMenu(ezySystem);
                // Process hop on logic inside the ezySystem
                // Display successful user hop ons
                UserHopOn(ezySystem);

                //while (ezySystem.GetBusArrivalList().GetCount()>0)
                //{   
                //    // displaying that bus X arrived at bus stop Y
                //    Console.WriteLine(ezySystem.GetBusArrivalList().GetFirstBusArrival().GetBusName() + " has reached bus stop " + ezySystem.GetBusArrivalList().GetFirstBusArrival().GetBusStopName());
                //    // display passengers inside the bus
                //    Console.WriteLine("Bus " + ezySystem.GetBusArrivalList().GetFirstBusArrival().GetBusName() + " has the following passengers: "); // edit later
                //    // letting simulation 'God' decide which users to hop off the bus
                    
                //    // transferring users from the bus stop queue to the bus passenger list
                //    // displaying the successful hopping on of passengers
                //    // repeat this block of code (i.e. processing of bus arrival list) until all bus arrivals are processed
                //}
                Console.Write("Press any key to continue "); Console.ReadKey();
                ezySystem.ResetHasEvent();
            }
        }

       

        // displays the bus arrival event
        private void DisplayBusArrival(string busStopName, string busName)
        {
            Console.WriteLine("Bus " + busName + " has reached " + busStopName);
        }

        // displays all of the bus arrivals
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
                        DisplayBusArrival(ezySystem.GetBusStopList().At(i).GetName(), ezySystem.GetBusStopList().At(i).GetBusQueue().ElementAt(j));
                    }
                }
            }
        }


        private void UserAutoHopOff(EzySystem ezySystem)
        {
            List<string> temp = ezySystem.GetWhoHasAutoHopOff();
            if (temp.Count == 0) return;
            Console.Write("The following passengers have been hopped off because they reach the last bus stop \n");
            
            for (int i = 0; i < temp.Count; i++)
            {
                Console.WriteLine(temp.ElementAt(i));
            }
            ezySystem.ClearAutoHopOffList();
        }


        // menu which asks for user input for hopping off the bus
        private void UserHopOffMenu(EzySystem ezySystem)
        {
            if (ezySystem.ThereIsPassengerOnBus())
            {
                Console.Write("Do you want to hop off passengers? (Y/N) ");
                string input = Console.ReadLine();

                while (String.Equals(input, "Y") || String.Equals(input, "y"))
                {
                    Console.WriteLine("Please select a bus stop from the list above.");
                    string busStopName = Console.ReadLine();
                    Console.WriteLine("The following are the buses which arrived at the chosen bus stop. Please select one.\n");
                    // display all buses in the bus stop
                    for (int i = 0; i < ezySystem.GetBusStopList().FindBusStopWithBusStopName(busStopName).GetBusQueueCount(); i++)
                    {
                        Console.WriteLine((i + 1) + ")" + ezySystem.GetBusStopList().FindBusStopWithBusStopName(busStopName).GetBusQueue()[i]);
                    }

                    string busName = Console.ReadLine();
                    Console.WriteLine("The following are the passengers inside this particular bus. Please select who will hop off.");
                    // display all passengers inside the bus // where to get the passenger
                    for (int i = 0; i < ezySystem.GetBusList().FindBusWithBusName(busName).GetPassengerList().Count; i++)
                    {
                        Console.WriteLine((i + 1) + ")" + ezySystem.FindUserWithMobileNumber(ezySystem.GetBusList().FindBusWithBusName(busName).GetPassengerList().ElementAt(i).GetMobileNum()));
                        //Console.WriteLine((i + 1) + ")" + ezySystem.GetUserList().FindUserWithMobileNumber((ezySystem.GetBusStopList().FindBusStopWithBusStopName(busStopName).GetPassengerList()[i].GetMobileNum())).GetName());
                    }

                    string userName = Console.ReadLine();
                    // remove passenger from the bus after this line
                    ezySystem.DequeueUserFromBus(userName, busName, busStopName);

                    // ask again if another passenger wants to hop off
                    Console.Write("\nDo you want to hop off passengers? (Y/N) ");
                    input = Console.ReadLine();
                }
            }
        }

        // display all passengers which were able to hop on successfully, send SMS, issue ticket and deduct from account
        private void UserHopOn(EzySystem ezySystem)
        {
            // check expiry ticket
            List<string> temp = ezySystem.GetWhoHasHopOn();
            if (temp.Count == 0) return;
            
            Console.WriteLine("The following users have sucessfully hopped on: ");
            for (int i = 0; i < temp.Count; i++)
            {
                Console.WriteLine(temp.ElementAt(i));
            }
            

        }
    }
}