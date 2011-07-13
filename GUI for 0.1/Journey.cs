/* Author/s: Paul Averilla and Nguyen Hoang Duy
 * JOURNEY CLASS
 *
 * Dependencies:
 *      NONE
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Journey_Class
{
    public class Journey
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private string _from;       // initial bus stop
        private string _to;         // terminal bus stop
        private string _busName;    // bus service name
        private int _departtime;
        private int _arrivetime;

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public Journey(string from, string to, string busName, int departtime, int arrivetime)
        {
            _from = from;
            _to = to;
            _busName = busName;
            _departtime = departtime;
            _arrivetime = arrivetime;
        }
        ~Journey() { }


        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/

        // Concatenates all data under the hourney class and returns it as one long string
        // Each data entry is separated by a delimiter, %
        public string GetAllDataAsString(string delimiter = "%")
        {
            return (_from + delimiter + _to + delimiter + _busName + delimiter + _departtime.ToString() + delimiter + _arrivetime.ToString());
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS
        /****************************************************************************************************/

        // returns the initial stop of the journey
        public string GetInitialStop()
        {
            return _from;
        }

        // returns the terminal stop of the journey
        public string GetTerminalStop()
        {
            return _to;
        }

        // returns the bus name taken
        public string GetBusName()
        {
            return _busName;
        }

        // returns the departing time
        public int GetDepartTime()
        {
            return _departtime;
        }

        // returns the arrival time
        public int GetArriveTime()
        {
            return _arrivetime;
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS (ROUTE)
        /****************************************************************************************************/

        // update the intitial bus stop
        public void UpdateFrom(string from)
        {
            _from = from;
        }

        // update the terminal bus stop
        public void UpdateTo(string to)
        {
            _to = to;
        }

        // upadte the bus stop taken
        public void UpdateBus(string bus)
        {
            _busName = bus;
        }

        // update the arrival time
        public void UpdateArrivalTime(int newtime)
        {
            _arrivetime = newtime;
        }

        // update the departure time
        public void UpdateDepartTime(int newtime)
        {
            _departtime = newtime;
        }
    }
}
