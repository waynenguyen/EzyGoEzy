﻿/* Author/s: Paul Averilla and Nguyen Hoang Duy
 * BUS STOP CLASS
 *
 * Dependencies:
 *      UserInQueue
 *      
 * Change log:
 *      ADD attribute _statCounter
 *      EDIT constructor _statCounter
 *      ADD function GetStatCounter
 *      ADD function IncrementStatCounter
 *      ADD function - SetStatCounter
 *      EDIT function EnqueueUser
 *      EDIT functino GetAllDataAsString
 *      
 *      ADD attribute _coordinates
 *      EDIT constructor _coordinates
 *      ADD function GetCoordinates
 *      ADD function SetCoordinates
 *      EDIT function GetAlLDataAsString
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ADDITIONAL CLASSES
using UserInQueue_Class;

namespace BusStop_Class
{
    public class BusStop
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private string _name;                   // name of bus stop
        private Boolean _status;                // holds the status of the bus stop ie. true = in operation; false = not in operation
        private int _statCounter;               // number of times the bus stop is enqueued in
        private double[] _coordinates = new double[2];            // map coordinates of the bus stop

        private List<UserInQueue> _userQueue;   // queue of users in the bus stop waiting for the bus to arrive
        private List<string> _busQueue;         // LIST of buses that are at this bus stop at CURRENT time. Named busqueue for synchronization purpose
        private List<string> _subscribeList;    // LIST of users subscribed to the SMS service for this bus stop

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public BusStop(string name)
        {
            _name = name;
            _status = true;
            _statCounter = 0;
            _coordinates[0] = -1;
            _coordinates[0] = -1;

            _userQueue = new List<UserInQueue>();
            _busQueue = new List<string>();
            _subscribeList = new List<string>();
        }
        ~BusStop() { }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/
		public void Clear()
        {
            _userQueue.Clear();
            _busQueue.Clear();
            _subscribeList.Clear();
            _status = true;
            _statCounter = 0;
        }

        // Concatenates all data under the bus stop class and returns it as one long string
        // Each data entry is separated by a delimiter, %
        public string GetAllDataAsString(string delimiter = "%")
        {
            return (_name + delimiter + _status + delimiter + _statCounter.ToString() + delimiter + _coordinates[0].ToString() + delimiter + _coordinates[1].ToString() + "\r\n" + GetUserInQueueDataAsString() + "\r\n" + GetBusQueueDataAsString() + "\r\n" + GetSubscribeListDataAsString());
        }

        // Concatenates all data under the user in queue list and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetUserInQueueDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$$$START USERINQUEUE$$$" + delimiter;

            for (int i = 0; i < _userQueue.Count(); i++)
            {
                tempReturn = tempReturn + _userQueue[i].GetAllDataAsString() + delimiter;
            }

            tempReturn = tempReturn + "$$$END USERINQUEUE$$$";

            return tempReturn;
        }

        // Concatenates all data under the bus queue list and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetBusQueueDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$$$START BUSQUEUE$$$" + delimiter;

            for (int i = 0; i < _busQueue.Count(); i++)
            {
                tempReturn = tempReturn + _busQueue[i] + delimiter;
            }

            tempReturn = tempReturn + "$$$END BUSQUEUE$$$";

            return tempReturn;
        }

        // Concatenates all data under the subscribe list and returns it as one long string
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

        // return the name of the bus stop
        public string GetName()
        {
            return _name;
        }

        // return status of the bus stop: either active or inactive (not in service)
        public Boolean GetStatus()
        {
            return _status;
        }

        public int GetStatCounter()
        {
            return _statCounter;
        }

        public double[] GetCoordinates()
        {
            return _coordinates;
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS (Passenger)
        /****************************************************************************************************/

        // returns the number of passengers in queue
        public int GetPassengerCount()
        {
            return _userQueue.Count();
        }

        // returns the passenger list
        public List<UserInQueue> GetPassengerList()
        {
            return _userQueue;
        }

        // returns the user object with mobileNum and busName
        private UserInQueue GetEnqueuedUserWithMobileNum(string mobileNum)
        {
            int i = -1;
            for (i = 0; i < _userQueue.Count(); i++)
            {
                if (_userQueue[i].GetMobileNum() == mobileNum)
                    break;
            }
            return _userQueue[i];
        }

        // returns an object with busName (multiple instances may happen)
        private UserInQueue GetEnqueuedUserWithBusName(string busName)
        {
            int i = -1;
            for (i = 0; i < _userQueue.Count(); i++)
            {
                if (_userQueue[i].GetBusName() == busName)
                    break;
            }
            return _userQueue[i];
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS (Bus)
        /****************************************************************************************************/

        // returns the number of buses in queue
        public int GetBusQueueCount()
        {
            return _busQueue.Count();
        }

        // returns the bus queue object
        public List<string> GetBusQueue()
        {
            return _busQueue;
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS (Subscription)
        /****************************************************************************************************/

        // get list of subsriber
        public List<string> GetUserSubscribeList()
        {
            return _subscribeList;
        }

        public bool ExistSubscriber(string mobilenumber)
        {
            if (_subscribeList.IndexOf(mobilenumber) != -1) return true;
            return false;
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS
        /****************************************************************************************************/

        // sets the status of a bus stop
        public void SetStatus(Boolean newStatus)
        {
            _status = newStatus;
        }

        private void IncrementStatCounter()
        {
            _statCounter++;
        }

        public void SetStatCounter(int n)
        {
            _statCounter = n;
        }

        public void SetCoordinates(double x, double y)
        {
            _coordinates[0] = x;
            _coordinates[1] = y;
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS (Passenger)
        /****************************************************************************************************/

        // adds a passenger ID to the bus stop queue
        public void EnqueuePassenger(UserInQueue newUser)
        {
            _userQueue.Add(newUser);
            IncrementStatCounter();
        }

        // removes a passenger with the mobile number from queue
        public void DequeuePassengerWithMobileNum(string mobileNum)
        {
            _userQueue.Remove(GetEnqueuedUserWithMobileNum(mobileNum));
        }

        // removes a passenger with the bus name from queue
        public void DequeuePassengerWithBusName(string busName)
        {
            _userQueue.Remove(GetEnqueuedUserWithBusName(busName));
        }

        // dequeues all users in the bus stop queue
        public void ClearUserQueue()
        {
            _userQueue.Clear();
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS (Bus)
        /****************************************************************************************************/

        // adds a bus to the bus stop queue
        public void EnqueueBus(string busName)
        {
            _busQueue.Add(busName);
        }

        // removes all bus in the list
        public void ClearBusQueue()
        {
            _busQueue.Clear();
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS (Subscription)
        /****************************************************************************************************/

        // add new subscriber
        public void AddUserToSubscribeList(string mobilenumber)
        {
            _subscribeList.Add(mobilenumber);
        }

        // remove subscriber
        public void RemoveUserFromSubscribeList(string mobilenumber)
        {
            _subscribeList.Remove(mobilenumber);
        }
    }
}