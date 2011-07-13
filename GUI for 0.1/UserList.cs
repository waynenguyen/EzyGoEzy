/* Author/s: Paul Averilla
 * BUS CLASS
 *
 * Dependencies:
 *      USER
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ADDITIONAL CLASSES
using User_Class;

namespace UserList_class
{
    public class UserList
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private List<User> _userList;   // list of users

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public UserList()
        {
            _userList = new List<User>();
        }
        ~UserList() { }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/

        // Concatenates all data under the user list class and returns it as one long string
        // Each data entry is separated by a delimiter, new line
        public string GetAllDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$START USERLIST$" + delimiter + delimiter;

            for (int i = 0; i < _userList.Count(); i++)
            {
                tempReturn = tempReturn + "$$START USER$$" + delimiter + _userList[i].GetAllDataAsString() + delimiter + "$$END USER$$" + delimiter + delimiter;
            }

            tempReturn = tempReturn + "$END USERLIST$";
            return tempReturn;
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS
        /****************************************************************************************************/

        // gets the number of users in the list
        public int GetCount()
        {
            return _userList.Count();
        }

        // gets the user object with name
        public User FindUserWithName(string name)
        {
            return _userList[FindIndexOfUserName(name)];
        }

        // gets the user object with mobile number
        public User FindUserWithMobileNumber(string number)
        {

            return _userList[FindIndexOfUserMobileNumber(number)];
        }

        // gets the user object at the specified index
        public User FindUserWithIndex(int index)
        {
            return _userList.ElementAt(index);
        }

        // gets the index number of the user with name
        private int FindIndexOfUserName(string name)
        {
            int i;
            bool found = false;

            for (i = 0; i < _userList.Count(); i++)
            {
                if (String.Equals(_userList[i].GetName(), name))
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

        // gets the index of the user with mobile number
        private int FindIndexOfUserMobileNumber(string name)
        {
            int i;
            bool found = false;

            for (i = 0; i < _userList.Count(); i++)
            {
                if (String.Equals(_userList[i].GetMobileNum(), name))
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

        // add a user to the list
        public void Add(User newUser)
        {
            _userList.Add(newUser);
        }
    }
}
