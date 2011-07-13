/* Author/s: Paul Averilla
 * BUS LIST CLASS
 *
 * Dependencies:
 *      Bus
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ADDITIONAL CLASSES
using Bus_Class;

namespace BusList_Class
{
    public class BusList
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private List<Bus> _busList; // list of bus services

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public BusList()
        {
            _busList = new List<Bus>();
        }
        ~BusList() { }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/

        // Concatenates all data under the bus list class and returns it as one long string
        // Each data entry is separated by a delimiter, \n
        public string GetAllDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$START BUSLIST$" + delimiter + delimiter;

            for (int i = 0; i < _busList.Count(); i++)
            {
                tempReturn = tempReturn + "$$START BUS$$" + delimiter + _busList[i].GetAllDataAsString() + delimiter + "$$END BUS$$" + delimiter + delimiter;
            }

            tempReturn = tempReturn + "$END BUSLIST$";
            return tempReturn;
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS
        /****************************************************************************************************/

        // gets the number of buses in the list
        public int GetCount()
        {
            return _busList.Count();
        }

        // returns the bus object with the bus name
        public Bus FindBusWithBusName(string name)
        {
            return _busList[FindIndexOfBusName(name)];
        }

        // returns the index of the bus with bus name
        private int FindIndexOfBusName(string name)
        {
            int i;
            bool found = false;

            for (i = 0; i < _busList.Count(); i++)
            {
                if (String.Equals(_busList[i].GetName(), name))
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

        // returns the bus object at index i
        public Bus GetBusAtIndex(int i)
        {
            return _busList.ElementAt(i);
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS
        /****************************************************************************************************/

        // adds a new bus to the list
        public void Add(Bus newBus)
        {
            _busList.Add(newBus);
        }
    }
}
