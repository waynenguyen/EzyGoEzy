/* Author/s: Paul Averilla
 * ROUTE CLASS
 *
 * Dependencies:
 *      NONE
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Route_Class
{
    public class Route
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private string _busStop;      // initial bus stop
        private int _travelTime;   // "distance" to the next bus stop in terms of time (i.e. minutes)

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public Route(string busStop, int travelTime)
        {
            _busStop = busStop;
            _travelTime = travelTime;
        }
        ~Route() { }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/

        // Concatenates all data under the route class and returns it as one long string
        // Each data entry is separated by a delimiter, %
        public string GetAllDataAsString(string delimiter = "%")
        {
            return (_busStop + delimiter + _travelTime.ToString());
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS
        /****************************************************************************************************/

        // returns the name of the bus stop
        public string GetBusStopName()
        {
            return _busStop;
        }

        // returns the travel time from the current bus stop to the next
        public int GetTravelTime()
        {
            return _travelTime;
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS (ROUTE)
        /****************************************************************************************************/

        // updates the traveling time from the current bus stop to the next
        public void UpdateTravelTime(int newTravelTime)
        {
            _travelTime = newTravelTime;
        }
    }
}
