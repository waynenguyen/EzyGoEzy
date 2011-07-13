/* Author/s: Paul Averilla and Nguyen Hoang Duy
 * BUS STOP LIST CLASS
 *
 * Dependencies:
 *      Bus Stop
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ADDITIONAL CLASSES
using BusStop_Class;

namespace BusStopList_Class
{
    public class BusStopList
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private List<BusStop> _busStopList; // list of bus stops

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public BusStopList()
        {
            _busStopList = new List<BusStop>();
        }
        ~BusStopList() { }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/

        // Concatenates all data under the bus stop class and returns it as one long string
        // Each data entry is separated by a delimiter, \n
        public string GetAllDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$START BUSSTOPLIST$" + delimiter + delimiter;

            for (int i = 0; i < _busStopList.Count(); i++)
            {
                tempReturn = tempReturn + "$$START BUSSTOP$$" + delimiter + _busStopList[i].GetAllDataAsString() + delimiter + "$$END BUSSTOP$$" + delimiter + delimiter;
            }

            tempReturn = tempReturn + "$END BUSSTOPLIST$";
            return tempReturn;
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS
        /****************************************************************************************************/

        // gets the number of bus stops in the list
        public int GetCount()
        {
            return _busStopList.Count();
        }

        // get the Bus stop at a particular index
        public BusStop At(int i)
        {
            return _busStopList.ElementAt(i);
        }

        // returns the bus stop with the indicated name
        public BusStop FindBusStopWithBusStopName(string name)
        {
            return _busStopList[FindIndexOfBusStopName(name)];
        }

        // gets the index of the bus stop with name
        private int FindIndexOfBusStopName(string name)
        {
            int i;
            bool found = false;

            for (i = 0; i < _busStopList.Count(); i++)
            {
                if (String.Equals(_busStopList[i].GetName(), name))
                {
                    found = true;
                    break;
                }
            }

            if (found)
                return i;
            else
                return -1;
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS
        /****************************************************************************************************/

        // adds a new bus stop with name
        public void Add(BusStop newBusStop)
        {
            //string temp = newBusStop.GetName();
            //BusStop newBusStop2 = new BusStop(temp);
            _busStopList.Add(newBusStop);
        }

        // clear the bus queue of every bus stop
        public void Refresh()
        {
            for (int i = 0; i < _busStopList.Count; i++)
                _busStopList.ElementAt(i).ClearBusQueue();
        }
    }
}
