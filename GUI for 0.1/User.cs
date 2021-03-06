﻿/* Author/s: Paul Averilla and Nguyen Hoang Duy
 * USER CLASS
 *
 * Dependencies:
 *      Journey
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ADDITIONAL CLASSES
using Journey_Class;
using GsmComm.GsmCommunication;
using GsmComm.PduConverter;
// ENUMERATIONS
public enum PAYMENT_MODE {account, mobile, credit};
public enum USER_STATUS { inactive, active, waiting };

namespace User_Class
{
    public class User
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private string _name;               // name of user
        private string _mobileNum;          // mobile number of user
        private float _account;             // account of user (pre-paid credits)
        private string _creditCardNum;      // credit card number of user
        private PAYMENT_MODE _paymentMode1; // primary payment mode
        private PAYMENT_MODE _paymentMode2; // secondary payment mode
        private int _ticketExpiry;          // time of expiry of ticket i.e. if current (time > ticketExpiry) then ticket is expired
        private Journey _journeyPlan;       // stores the interested bus stop to be hopped on from and hopped off to and the corresponding bus name that will be taken (future)
        private USER_STATUS _userStatus;    // stores the status of the user ie. active (inside bus), waiting (enqueued at bus stop), inactive (neither at bus or bus stop)
        private string _location;           // stores the location of the user (if inactive, set to null)
        private string _password;

        private List<string> _smsLog;       // stores the sms messages received by the user
        private List<Journey> _journeyLog;  // stores the bus stops hopped on from and hopped off to and the corresponding bus name taken (past)

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public User(string name, string mobileNum, float account, string creditCardNum, PAYMENT_MODE paymentMode1, PAYMENT_MODE paymentMode2, string password)
        {
            _name = name;
            _mobileNum = mobileNum;
            _account = account;
            _creditCardNum = creditCardNum;
            _paymentMode1 = paymentMode1;
            _paymentMode2 = paymentMode2;
            _ticketExpiry = 0;
            _journeyPlan = new Journey(null, null, null, -1, -1);
            _userStatus = USER_STATUS.inactive;
            _location = null;
            _password = password;

            _smsLog = new List<string>();
            _journeyLog = new List<Journey>();
        }
        ~User() { }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/

        // Concatenates all data under the user class and returns it as one long string
        // Each data entry is separated by a delimiter, %

        public void Clear()
        {
            _ticketExpiry = 0;
            _smsLog.Clear();
            _journeyLog.Clear();
            _userStatus = USER_STATUS.inactive;
            _location = null;
            _journeyPlan = new Journey(null, null, null, -1, -1);
            _account = 0;

        }
        public string GetAllDataAsString(string delimiter = "%")
        {
            return (_name + delimiter + _mobileNum + delimiter + _account.ToString() + delimiter + _creditCardNum + delimiter + _paymentMode1.ToString() + delimiter + _paymentMode2.ToString() + delimiter + _ticketExpiry.ToString() + delimiter + _userStatus.ToString() + delimiter + _location + delimiter + _password + delimiter + "\r\n" + GetSMSLogDataAsString() + "\r\n" + GetJourneyLogDataAsString() + "\r\n" + GetJourneyPlanDataAsString());
        }

        // Concatenates all data under the sms log and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetSMSLogDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$$$START SMSLOG$$$" + delimiter;

            for (int i = 0; i < _smsLog.Count(); i++)
            {
                tempReturn = tempReturn + _smsLog[i] + delimiter;
            }

            tempReturn = tempReturn + "$$$END SMSLOG$$$";

            return tempReturn;
        }

        // Concatenates all data under the journey log and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetJourneyLogDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$$$START JOURNEYLOG$$$" + delimiter;

            for (int i = 0; i < _journeyLog.Count(); i++)
            {
                tempReturn = tempReturn + _journeyLog[i].GetAllDataAsString() + delimiter;
            }

            tempReturn = tempReturn + "$$$END JOURNEYLOG$$$";

            return tempReturn;
        }

        // Concatenates all data under the journey plan and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetJourneyPlanDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$$$START JOURNEYPLAN$$$" + delimiter;

            tempReturn = tempReturn + _journeyPlan.GetAllDataAsString() + delimiter;
            tempReturn = tempReturn + "$$$END JOURNEYPLAN$$$";

            return tempReturn;
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS
        /****************************************************************************************************/

        // returns the name of the user
        public string GetName()
        {
            return _name;
        }

        // returns the mobile number of the user
        public string GetMobileNum()
        {
            return _mobileNum;
        }

        // returns the account of the user
        public float GetAccount()
        {
            return _account;
        }

        // returns the credit card number of the user
        public string GetCreditCardNum()
        {
            return _creditCardNum;
        }

        // returns the primary payment mode of the user
        public PAYMENT_MODE GetPaymentMode1()
        {
            return _paymentMode1;
        }

        // returns the secondary payment mode of the user
        public PAYMENT_MODE GetPaymentMode2()
        {
            return _paymentMode2;
        }

        // returns the ticket expiry of the user
        public int GetTicketExpiry()
        {
            return _ticketExpiry;
        }

        // returns the sms log of the user
        public List<string> GetSMSLog()
        {
            return _smsLog;
        }

        // returns the journey log of the user
        public List<Journey> GetJourneyLog()
        {
            return _journeyLog;
        }

        // returns the journey plan of the user
        public Journey GetJourneyPlan()
        {
            return _journeyPlan;
        }

        // returns whether the user is on a bus or not
        public Boolean IsOnBus()
        {
            if (GetUserStatus() == USER_STATUS.active)
                return true;
            return false;
        }

        // returns the status of a user
        public USER_STATUS GetUserStatus()
        {
            return _userStatus;
        }

        // returns the location of a user
        public string GetUserLocation()
        {
            return _location;
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS
        /****************************************************************************************************/

        // updates the name of a user
        public void UpdateName(string name)
        {
            _name = name;
        }

        // updates the mobile number of  a user
        public void UpdateMobileNum(string mobileNum)
        {
            _mobileNum = mobileNum;
        }

        // replaces the account value to the input value
        public void ReloadAccount(float add)
        {
            _account = add;
        }

        // updates the credit card number of the user
        public void UpdateCreditCardNum(string creditCardNum)
        {
            _creditCardNum = creditCardNum;
        }

        // updates the primary mode of payment
        public void UpdatePaymentMode1(PAYMENT_MODE paymentMode)
        {
            _paymentMode1 = paymentMode;
        }

        // updates the secondary mode of payment
        public void UpdatePaymentMode2(PAYMENT_MODE paymentMode)
        {
            _paymentMode2 = paymentMode;
        }

        // updates the ticket expiry
        public void UpdateTicketExpiry(int ticketExpiry)
        {
            _ticketExpiry = ticketExpiry;
        }

        // updates the user status
        public void UpdateUserStatus(USER_STATUS newUserStatus)
        {
            _userStatus = newUserStatus;
        }

        // updates the user location
        public void UpdateUserLocation(string newLocation)
        {
            _location = newLocation;
        }

        // checks the password validity of a user
        public bool CheckPassword(string inputpassword)
        {
            if (inputpassword == _password)
                return true;
            return false;
        }

        // updates the journey plan of the user
        public void UpdateJourneyPlan(string from, string busName, int departtime)
        {
            _journeyPlan.UpdateFrom(from);
            _journeyPlan.UpdateBus(busName);
            _journeyPlan.UpdateDepartTime(departtime);
        }

        // resets the journey plan of a user
        public void ResetJourneyPlan()
        {
            _journeyPlan = new Journey(null, null, null, -1, -1);
        }

        public void AddSMSWhenLoadData(string SMS)
        {
            _smsLog.Add(SMS);

        }
        // adds an sms to the log
        public void AddSMS(string SMS)
        {
            _smsLog.Add(SMS);

            try
            {
                GsmCommMain comm = new GsmCommMain("COM4", 19200, 300);
                // Send an SMS message
                SmsSubmitPdu pdu;

                // The straightforward version
                pdu = new SmsSubmitPdu(SMS, "+65" + _mobileNum, "+6596197777");
                comm.Open();
                comm.SendMessage(pdu);
                comm.Close();
            }
            catch (Exception e)
            {
                return;
            }
        }

        // adds a journey to the log
        public void AddJourney(Journey journey)
        {
            _journeyLog.Add(journey);
        }

        // updates the number of times travelling with this bus
        public int NumberOfTimesTravelThisBus(string busName)
        {
            int count = 0;

            for (int i = 0; i < _journeyLog.Count; i++)
            {
                if (_journeyLog.ElementAt(i).GetBusName() == busName) count++;                
            }

            return count;
        }
    }
}
