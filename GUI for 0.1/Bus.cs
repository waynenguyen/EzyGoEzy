/* Author/s: Paul Averilla and Nguyen Hoang Duy
 * BUS CLASS
 *
 * Dependencies:
 *      Route
 *      UserInQueue
 *      
 * Change log:
 *      ADD attribute - statCounter
 *      EDIT constructor - _statCounter
 *      ADD function - GetStatCounter
 *      ADD function - IncrementStatCounter
 *      ADD function - SetStatCounter
 *      EDIT function - EnqueueUser
 *      EDIT function - GetAllDataAsString
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ADDITIONAL CLASSES
using Route_Class;
using UserInQueue_Class;

namespace Bus_Class
{
    public class Bus
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private string _name;                       // NAME of the bus
        private string _currentStop;                // Current BUS STOP location of the bus
        private int _frequency;                     // number of MINUTES in between bus instances
        private int _statCounter;                   // number of times the bus is hopped on

        private List<Route> _routeList;             // list of all BUS STOPS that the bus passes through
        private List<UserInQueue> _passengerList;   // list of USERS that are inside the bus (aka passengers)
        private List<string> _subscribeList;        // list of USERS that are subscribed to this bus service

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public Bus(string name, int frequency)
        {
            _name = name;
            _currentStop = null;
            _frequency = frequency;
            _statCounter = 0;

            _routeList = new List<Route>();
            _passengerList = new List<UserInQueue>();
            _subscribeList = new List<string>();
        }
        ~Bus() { }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/
		public void Clear()
        {
            _passengerList.Clear();
            _subscribeList.Clear();
            _statCounter = 0;
        }

        // Concatenates all data under the bus class and returns it as one long string
        // Each data entry is separated by a delimiter, %
        public string GetAllDataAsString(string delimiter = "%")
        {
            return (_name + delimiter + _currentStop + delimiter + _frequency.ToString() + delimiter + _statCounter.ToString() + "\r\n" + GetRouteListDataAsString() + "\r\n" + GetPassengerListDataAsString() + "\r\n" + GetSubscribeListDataAsString());
        }

        // Concatenates all data under the route list and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetRouteListDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$$$START ROUTELIST$$$" + delimiter;

            for (int i = 0; i < _routeList.Count(); i++)
            {
                tempReturn = tempReturn + _routeList[i].GetAllDataAsString() + delimiter;
            }

            tempReturn = tempReturn + "$$$END ROUTELIST$$$";

            return tempReturn;
        }

        // Concatenates all data under the passenger list and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetPassengerListDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$$$START PASSENGERLIST$$$" + delimiter;

            for (int i = 0; i < _passengerList.Count(); i++)
            {
                tempReturn = tempReturn + _passengerList[i].GetAllDataAsString() + delimiter;
            }

            tempReturn = tempReturn + "$$$END PASSENGERLIST$$$";

            return tempReturn;
        }

        // Concatenates all data under the subscription list and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetSubscribeListDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$$$START SUBSCRIBELIST$$$" + delimiter;

            for (int i = 0; i < _subscribeList.Count(); i++)
            {
                tempReturn = tempReturn + _subscribeList[i] + delimiter;
            }

            tempReturn = tempReturn + "$$$END SUBSCRIBELIST$$$";

            return tempReturn;
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS
        /****************************************************************************************************/

        // Returns the Bus Name
        public string GetName()
        {
            return _name;
        }

        // Returns the Route List
        public List<Route> GetRouteList()
        {
            return _routeList;
        }

        // Returns the frequency of the bus
        public int GetFrequency()
        {
            return _frequency;
        }

        public int GetStatCounter()
        {
            return _statCounter;
        }

        // Returns the passenger list of the bus
        public List<UserInQueue> GetPassengerList()
        {
            return _passengerList;
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS (BUS STOPS)
        /****************************************************************************************************/

        // Returns the current bus stop location of the bus
        public string GetCurrentStop()
        {
            return _currentStop;
        }

        // Returns the Next Bus Stop that the bus will go to
        public string GetNextStop(string currentStop)
        {
            // Traverse through the route list
            for (int i = 0; i < _routeList.Count(); i++)
            {
                // Checks if the current stop is found
                if (i == _routeList.Count()-1) return null;
                if (_routeList[i].GetBusStopName() == currentStop)
                {
                    // Checks if the end of the route list is reached
                    if (i == _routeList.Count())
                    {
                        return null;
                    }

                    return _routeList[i + 1].GetBusStopName();
                }
            }
            return null;
        }

        // Returns the last bus stop of the bus according to its route
        public string GetLastStop()
        {
            return _routeList.ElementAt(_routeList.Count - 1).GetBusStopName();
        }

        // Check if a given bus stop is exists in the bus route
        public bool CheckBusStopExistedInRoute(string busStopName)
        {
            // Traverse through the bus route
            for (int i = 0; i < _routeList.Count; i++)
            {
                // Check if bus stop is found
                if (_routeList.ElementAt(i).GetBusStopName() == busStopName)
                {
                    return true;
                }
            }

            return false;
        }

        // Returns the next stop of a given stop
        // Returns null if the given stop is the last stop
        public string RetrieveNextStop(string currentBusStopName)
        {
            // Traverse through the route list
            for (int i = 0; i < _routeList.Count - 1; i++)
            {
                if (_routeList.ElementAt(i).GetBusStopName() == currentBusStopName)
                {
                    string temp = _routeList.ElementAt(i + 1).GetBusStopName();
                    return temp;
                }
            }

            // Exception next stop does not exist or current stop does not exist
            return null;
        }

        // This return the previous stop of a given stop
        // Returns null if the given stop is the 1st stop
        public string RetrievePreviousStop(string currentBusStopName)
        {
            // Traverse through the route list
            for (int i = 1; i < _routeList.Count; i++)
            {
                // Checks if the current bus stop is found
                if (_routeList.ElementAt(i).GetBusStopName() == currentBusStopName)
                {
                    string temp = _routeList.ElementAt(i - 1).GetBusStopName();
                    return temp;
                }
            }

            // Exception previous stop does not exist or current stop does not exist
            return null;
        }

        // Returns the index of the bus stop with the indicated name
        private int FindIndexOfBusStopWithName(string busStopName)
        {
            int i;
            bool found = false;

            // Traverse through the route list
            for (i = 0; i < _routeList.Count(); i++)
            {
                // Check if the bus stop is found
                if (String.Equals(_routeList[i].GetBusStopName(), busStopName))
                {
                    found = true;
                    break;
                }
            }

            // Double check if the bus stop is found
            if (found)
            {
                return i;
            }
            else
            {
                // Exception: bus stop does not exist
                return -1;
            }
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS
        /****************************************************************************************************/

        // Increase stat counter
        private void IncerementStatCounter()
        {
            _statCounter++;
        }

        public void SetStatCounter(int n)
        {
            _statCounter = n;
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS (ROUTE)
        /****************************************************************************************************/

        // Adds a bus stop to the end of the route
        public void AddRoute(string initialStop, int travelTime)
        {
            Route newRoute = new Route(initialStop, travelTime);
            _routeList.Add(newRoute);
        }

        // Adds a bus stop after a target bus stop
        // Adds a route with (newBusStop, 0) into RouteList, and after targetBusStop
        // and modify current travel time of targetBusStop to traveltime
        // Exceptions - if inserted after the last stop (update later)
        public void InsertRouteAfter(string targetBusStop, string newBusStop, int travelTime)
        {
            Route newRoute = new Route(newBusStop, 0);
            int targetBusStopIndex = FindIndexOfBusStopWithName(targetBusStop);

            _routeList.Insert(targetBusStopIndex + 1, newRoute);
            _routeList.ElementAt(targetBusStopIndex).UpdateTravelTime(travelTime);
        }

        // Adds a bus stop before a target bus stop
        // Adds a route with (newBusStop, 0) into RouteList, and before targetBusStop
        // and modify current travel time of newBusStop to traveltime
        // Exceptions - if inserted before the first stop (update later)
        public void InsertRouteBefore(string targetBusStop, string newBusStop, int travelTime)
        {
            Route newRoute = new Route(newBusStop, 0);
            int targetBusStopIndex = FindIndexOfBusStopWithName(targetBusStop);

            _routeList.Insert(targetBusStopIndex, newRoute);
            _routeList.ElementAt(targetBusStopIndex).UpdateTravelTime(travelTime);
        }

        // Edit the route traveling time from one bus stop to another bus stop
        public void EditRouteTravelTime(string from, string to, int traveltime)
        {
            // Traverse through the route list
            for (int i = 0; i < _routeList.Count; i++)
            {
                // Check if the bus stop from is found
                if (_routeList.ElementAt(i).GetBusStopName() == from)
                {
                    // Check if the end of the route list is reached
                    if (i == _routeList.Count)
                    {
                        // NULL Exception: Bus stop from not found
                        return;
                    }

                    // Check if the bus stop to is valid
                    if (_routeList.ElementAt(i + 1).GetBusStopName() != to)
                    {
                        // Exception: Bus stop to is invalid
                        return;
                    }

                    _routeList.ElementAt(i).UpdateTravelTime(traveltime);
                    return;
                }
            }
        }

        // Enqueue user to the passenger list ie. passenger enters the bus
        public void EnqueueUser(UserInQueue newuser)
        {
            _passengerList.Add(newuser);
            IncerementStatCounter();
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS (PASSENGER)
        /****************************************************************************************************/

        // Add a new subscriber
        public void AddUserToSubscribeList(string mobileNumber)
        {
            _subscribeList.Add(mobileNumber);
        }

        // Get list of subscribers
        public List<string> GetUserSubscribeList()
        {
            return _subscribeList;
        }

        // Remove subscriber
        public void RemoveUserFromSubscribeList(string mobileNumber)
        {
            _subscribeList.Remove(mobileNumber);
        }

        // Checks if the subscriber exists
        public bool ExistSubscriber(string mobilenumber)
        {
            if (_subscribeList.IndexOf(mobilenumber) != -1)
            {
                return true;
            }
            return false;
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS (BUS STOP)
        /****************************************************************************************************/

        // Updates the current stop to a new bus stop
        public void UpdateCurrentStop(string newCurrentStop)
        {
            _currentStop = newCurrentStop;
        }
    }
}