using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UserUI_Class;
using AdminUI_Class;
using SimulationUI_Class;
using EzySystem_Class;

/****************************************
 * NOTES
 * 1) TextUI will contain only the raw text info
 * 2) The only object instance contained in the UI is EzySystem
 * 3) Other objects must be placed in EzySystem class
 * 4) TextUI is the root UI of AdminUI, UserUI and SimulationUI
 * 5) TextUI, AdminUI, UserUI and SimulationUI inherit from UI_Class (to be implemented later)
 * 
 * STATE TABLE
 * 1) Login as User
 * 2) Login as Admin
 * 3) Run Simulation
 ****************************************/

namespace TextUI_Class
{
    public class TextUI
    {
        // ATTRIBUTES
        private int _state;

        // CONSTRUCTOR
        public TextUI() {
            _state=-1;
        }
        ~TextUI() { }

        // METHODS
        // initializing page
        public void Initialize()
        {
            // Load data here
            Console.WriteLine("Welcome to EzyGoEzy!");
            Console.WriteLine("Initializing Software...");
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
        }

        public void Exit()
        {
            Console.Clear();
            Console.WriteLine("Thank you for using EzyGoEzy!");
            Console.WriteLine("Press any key to continue.");
            Console.ReadLine();
            Console.Clear();
        }

        // returns the current state of the software
        public void printheaeder()
        { 
        
        
        }
        public int GetState()
        {
            return _state;
        }

        // lets a user choose an option from the given set of options
        public void ChooseOption()
        {
            Console.Clear();
            Console.WriteLine("MAIN MENU\n");
            Console.WriteLine("0) Exit");
            Console.WriteLine("1) Login as User");
            Console.WriteLine("2) Login as Admin");
            Console.WriteLine("3) Run Simulation");

            Console.Write("\nPlease enter option number: ");
            _state = Int32.Parse(Console.ReadLine());
        }

        // executes the option according to chosen state
        public void ExecuteOption(EzySystem ezySystem)
        {
            if (_state == 0)                         // go back to previous menu
                return;

            else if (_state == 1)                    // enter user UI
            {
                UserUI newUserUI = new UserUI();
                newUserUI.Initialize(ezySystem);

                // Run the UI until exit
                while (true)
                {
                    if (newUserUI.GetState() == 0)   // mark of program exit
                        break;
                    else                             // if not exit
                    {
                        newUserUI.ChooseOption();
                        newUserUI.ExecuteOption(ezySystem);
                    }
                }

                newUserUI.Exit(ezySystem);
            }

            else if (_state == 2)                    // enter admin UI
            {
                AdminUI newAdminUI = new AdminUI();
                newAdminUI.Initialize();

                // Run the UI until exit
                while (true)
                {
                    if (newAdminUI.GetState() == 0) break;// mark of program exit
                    else                            // if not exit
                    {
                        newAdminUI.ChooseOption();
                        newAdminUI.ExecuteOption(ezySystem);
                        Console.Write("Press any key to continue.");
                        Console.ReadKey();
                    }
                }

                newAdminUI.Exit();
            }
            else if (_state == 3)                   // enter simulation UI
            {
                SimulationUI newSimulationUI = new SimulationUI();
                newSimulationUI.Initialize();

                // Run the UI until exit
                while (true)
                {
                    if (newSimulationUI.GetState() == 0) // mark of program exit
                        break;
                    else                            // if not exit
                    {
                        newSimulationUI.ChooseOption(ezySystem);
                        newSimulationUI.ExecuteOption(ezySystem);
                    }
                }

                newSimulationUI.Exit();
            }
        }
    }
}