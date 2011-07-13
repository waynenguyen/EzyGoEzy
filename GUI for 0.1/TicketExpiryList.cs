/* Author/s: Paul Averilla
 * TICKET EXPIRY CLASS
 *
 * Dependencies:
 *      TICKET EXPIRY CLASS
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ADDITIONAL CLASSES
using TicketExpiry_Class;

namespace TicketExpiryList_Class
{
    public class TicketExpiryList
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private List<TicketExpiry> _ticketExpiryList;   // a list of ticket expiry times

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public TicketExpiryList(){
            _ticketExpiryList = new List<TicketExpiry>();
        }
        ~TicketExpiryList() { }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/

        // Concatenates all data under the ticket expiry class and returns it as one long string
        // Each data entry is separated by a delimiter, \n
        public string GetAllDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$START TICKETLIST$" + delimiter;

            for (int i = 0; i < _ticketExpiryList.Count(); i++)
            {
                tempReturn = tempReturn + _ticketExpiryList[i].GetAllDataAsString() + delimiter;
            }

            tempReturn = tempReturn + "$END TICKETLIST$";
            return tempReturn;
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS
        /****************************************************************************************************/

        // returns the number of tickets in the expiry list
        public int GetCount()
        {
            return _ticketExpiryList.Count();
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS
        /****************************************************************************************************/

        // insert mobile num at the end of the ticket list
        public void Add(TicketExpiry newTicket)
        {
            _ticketExpiryList.Add(newTicket);
        }

        // returns the expiry time of oldest ticket
        public int GetTopExpiry()
        {
            return _ticketExpiryList[0].GetExpiryTime();
        }

        // returns the mobile number of oldest ticket owner
        public string GetTopTicketUserMobile()
        {
            return _ticketExpiryList[0].GetMobileNum();
        }

        // deletes the oldest ticket
        public void DeleteTopTicket()
        {
            _ticketExpiryList.RemoveAt(0);
        }
		 // clear all
        public void Clear()
        {
            _ticketExpiryList.Clear();
        }
    }
}
