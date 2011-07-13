using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EzySystem_Class;

/****************************************
 * NOTES
 * 1) AdminUI will contain only the raw text info
 * 2) The only object instance contained in the UI is EzySystem
 * 3) Other objects must be placed in EzySystem class
 * 4) TextUI, AdminUI, UserUI and SimulationUI inherit from UI_Class (to be implemented later)
 * 
 * STATE TABLE
 * 1) Add Bus Stop
 * 2) Add Bus Service
 * 3) Add Route
 * 4) Display Bus Stops
 * 5) Display Bus Services
 ****************************************/

namespace AdminUI_Class
{
    class AdminUI
    {
        // ATTRIBUTES
        private int _state;

        // CONSTRUCTOR
        public AdminUI() {
            _state=-1;
        }
        ~AdminUI() { }

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
        public void ChooseOption()
        {
            Console.Clear();
            Console.WriteLine("ADMIN MENU\n");
            Console.WriteLine("0) Return to Previous Menu");
            Console.WriteLine("1) Add Bus Stop");
            Console.WriteLine("2) Add Bus Service");
            Console.WriteLine("3) Add Route");
            Console.WriteLine("4) Display Bus Stops");
            Console.WriteLine("5) Display Bus Services");
            Console.WriteLine("6) Display Bus Service Route");
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
            else if (_state == 1)   // add bus stop (prompt for necessary details then execute action)
            {
                AddBusStop(ezySystem);
            }
            else if (_state == 2)   // add bus service
            {
                AddBus(ezySystem);
            }
            else if (_state == 3)   // add route
            {
                AddRoute(ezySystem);
            }
            else if (_state == 4)   // display bus stops
            {
                DisplayBusStops(ezySystem);
            }
            else if (_state == 5)   // display bus services
            {
                DisplayBusServices(ezySystem);
            }
            else if (_state == 6)    // display bus service route
            {
                DisplayBusServiceRoute(ezySystem);
            }
        }
        // Add new busstop to the system
        private void AddBusStop(EzySystem ezySystem)
        {
            Console.Write("Please enter the name of the bus stop: ");
            string name = Console.ReadLine();
            ezySystem.CreateBusStop(name);
        }
        // Add new bus to the system
        private void AddBus(EzySystem ezySystem)
        {
            Console.Write("Please enter the name of the bus: ");
            string name = Console.ReadLine();
            Console.Write("Please enter the frequency of the bus: ");
            string frequency = Console.ReadLine();
            ezySystem.CreateBus(name, System.Convert.ToInt32(frequency));
            
        }
        // Add new route to the system
        private void AddRoute(EzySystem ezySystem)
        {
            Console.Write("Please enter the name of the bus that you want to modify: ");
            string busName = Console.ReadLine();
            Console.Write("Please enter the initial bus stop: ");
            string initialBusStop = Console.ReadLine();
            Console.Write("Please enter the terminal bus stop: ");
            string terminalBusStop = Console.ReadLine();
            Console.Write("Please enter the travel time from initial bus stop to terminal bus stop : ");
            string trv = Console.ReadLine();
            int traveltime = System.Convert.ToInt32(trv, 10);
            ezySystem.CreateRoute(busName, initialBusStop, terminalBusStop, traveltime);
            
        }
        // Display bus stops
        private void DisplayBusStops(EzySystem ezySystem)
        {
            Console.WriteLine(ezySystem.GetBusStopStringList());
        }

        // Display bus services
        private void DisplayBusServices(EzySystem ezySystem)
        {
            Console.WriteLine(ezySystem.GetBusStringList());
        }

        // Display bus services route
        private void DisplayBusServiceRoute(EzySystem ezySystem)
        {
            Console.Write("Please enter the name of the bus service that you want to view: ");
            string busName = Console.ReadLine();
            Console.WriteLine(ezySystem.GetRoute(busName));
        }
    }
}
