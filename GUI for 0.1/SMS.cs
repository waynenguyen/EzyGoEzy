/* Author/s: Nguyen Hoang Duy
 * SMS CLASS
 *
 * Dependencies:
 *      NONE
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS_Class
{
    class SMS
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private int _time;
        private string _smscontent;

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public SMS(int time, string smscontent)
        {
            _time = time;
            _smscontent = smscontent;
        }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/

        // Concatenates all data under the sms class and returns it as one long string
        // Each data entry is separated by a delimiter, %
        public string GetAllDataAsString(string delimiter = "%")
        {
            return (_time.ToString() + "%" + _smscontent);
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS
        /****************************************************************************************************/

        // returns the time the sms was created
        public int GetTime()
        {
            return _time;
        }

        // returns the contents of the sms
        public string GetSMSContent()
        {
            return _smscontent;
        }
    }
}
