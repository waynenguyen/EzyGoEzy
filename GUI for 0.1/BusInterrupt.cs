/* Author/s: Nguyen Hoang Duy
 * BUS INTERRUPT CLASS
 *
 * Dependencies:
 *      NONE
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusInterruptClass
{
    class BusInterrupt
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private int _interruptTime;
        private string _busName;
        private string _nextStop;       // current NEXT STOP of the interrupted BUS
        private string _interruptStop;  // 

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public BusInterrupt(int interrupttime, string busname, string nextstop, string interruptstop)
        {
            _interruptTime = interrupttime;
            _busName = busname;
            _nextStop = nextstop;
            _interruptStop = interruptstop;
        }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/

        // Concatenates all data under the bus interrupt class and returns it as one long string
        // Each data entry is separated by a delimiter, %
        public string GetAllDataAsString(string delimiter = "%")
        {
            return (_interruptTime.ToString() + delimiter + _busName + delimiter + _nextStop + delimiter + _interruptStop);
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS
        /****************************************************************************************************/

        // return the interrupt time
        public int GetInterruptTime()
        {
            return _interruptTime;
        }

        // return the name of the bus
        public string GetBusName()
        {
            return _busName;
        }

        // return the current next stop of the interrupted bus
        public string GetNextStop()
        {
            return _nextStop;
        }

        // gets the interrupted stop
        public string GetInterruptStop()
        {
            return _interruptStop;
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS
        /****************************************************************************************************/

        // sets the interrupt time
        public void SetInterruptTime(int newtime)
        {
            _interruptTime = newtime;
        }

        // modifies the next stop
        public void SetNextStop(string newnextstop)
        {
            _nextStop = newnextstop;
        }
    }
}
