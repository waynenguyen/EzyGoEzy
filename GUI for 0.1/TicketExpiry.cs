/* Author/s: Paul Averilla
 * TICKET EXPIRY CLASS
 *
 * Dependencies:
 *      NONE
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicketExpiry_Class
{
    public class TicketExpiry
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private string _mobileNum;  // user mobile number
        private int _expiryTime;    // time of ticket expiry

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public TicketExpiry(string mobileNum, int expiryTime)
        {
            _mobileNum = mobileNum;
            _expiryTime = expiryTime;
        }
        ~TicketExpiry() { }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/

        // Concatenates all data under the ticket expiry class and returns it as one long string
        // Each data entry is separated by a delimiter, %
        public string GetAllDataAsString(string delimiter = "%")
        {
            return (_mobileNum + delimiter + _expiryTime.ToString());
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS
        /****************************************************************************************************/

        // returns mobile number associated with the ticket
        public string GetMobileNum()
        {
            return _mobileNum;
        }

        // returns expiry time of the ticket
        public int GetExpiryTime()
        {
            return _expiryTime;
        }
    }
}
