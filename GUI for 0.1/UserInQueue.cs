/* Author/s: Paul Averilla
 * USER IN QUEUE CLASS
 *
 * Dependencies:
 *      NONE
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserInQueue_Class
{
    public class UserInQueue
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private string _mobileNum;
        private string _busName;
        private string _nextStop;

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public UserInQueue(string mobileNum, string busName)
        {
            _mobileNum = mobileNum;
            _busName = busName;
            //_nextStop = nextStop;
        }
        ~UserInQueue() { }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/

        // Concatenates all data under the user in queue class and returns it as one long string
        // Each data entry is separated by a delimiter, %
        public string GetAllDataAsString(string delimiter = "%")
        {
            return (_mobileNum + delimiter + _busName + delimiter + _nextStop);
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS
        /****************************************************************************************************/

        // returns the mobile number of the user in queue
        public string GetMobileNum()
        {
            return _mobileNum;
        }

        // returns the bus name that the user is queueing for
        public string GetBusName()
        {
            return _busName;
        }

        // returns the next stop that the user is going to
        public string GetNextStop()
        {
            return _nextStop;
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS
        /****************************************************************************************************/

        // updates the next stop that the user is going to
        public void UpdateNextStop(string newNextStop)
        {
            _nextStop = newNextStop;
        }   
    }
}