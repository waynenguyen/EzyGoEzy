/* Author/s: Paul Averilla and Nguyen Hoang Duy
 * BUS CLASS
 *
 * Dependencies:
 *      NONE
 */

// SYSTEM LIBRARIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopUpRetailShop_Class
{
    public class TopUpRetailShop
    {
        /****************************************************************************************************/
        // ATTRIBUTES
        /****************************************************************************************************/
        private List<int> _usedCards;
        private const int _KEY = 2;

        /****************************************************************************************************/
        // CONSTRUCTOR
        /****************************************************************************************************/
        public TopUpRetailShop()
        {
            _usedCards = new List<int>();
        }
        ~TopUpRetailShop() { }

        /****************************************************************************************************/
        // METHODS - PERSISTENCE
        /****************************************************************************************************/

        // Concatenates all data under the top up retail class and returns it as one long string
        // Each data entry is separated by a delimiter, \n
        public string GetAllDataAsString(string delimiter = "\r\n")
        {
            string tempReturn = "$START CARDLIST$" + delimiter;

            for (int i = 0; i < _usedCards.Count(); i++)
            {
                tempReturn = tempReturn + _usedCards[i].ToString() + delimiter;
            }

            tempReturn = tempReturn + "$END CARDLIST$";
            return tempReturn;
        }

        /****************************************************************************************************/
        // METHODS - ACCESSORS
        /****************************************************************************************************/

        // returns the number of used cards in the list
        public int GetCountOfUsedCards()
        {
            return _usedCards.Count();
        }

        // returns the used card number at index
        public int GetUsedCardNumberAtIndex(int n)
        {
            return _usedCards.ElementAt(n);
        }

        // checks if card number is already used
        public Boolean CheckIfUsed(int cardNumber)
        {
            if (_usedCards.Contains(cardNumber))
                return true;
            else
                return false;
        }

        // gets the top up value of card with card number
        // ie. ends in 0 -> $50; ends in 2 -> $20; else -> $10
        public int GetTopUpValueOfCard(int cardNumber)
        {
            if (cardNumber % 10 == 0)
            {
                return 50;
            }
            else if (cardNumber % 10 == 2)
            {
                return 20;
            }
            else
            {
                return 10;
            }   
        }

        // returns the bonus value of the card associated with it
        public int GetBonusValueOfCard(int cardNumber)
        {
            if (cardNumber % 10 == 0)
            {
                return 10;
            }
            else if (cardNumber % 10 == 2)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }

        // checks if card number can be generated using the key
        public Boolean CheckIfValid(int cardNumber)
        {
            if ((cardNumber % _KEY == 0) && (cardNumber<1000000) && (100000<cardNumber))
                return true;
            else
                return false;
        }

        // checks if card number corresponds to correct topUpValue
        // ie. ends in 0 -> $50; ends in 2 -> $20; else -> $10
        public Boolean CheckIfValueIsValid(int cardNumber, int topUpValue)
        {
            if (GetTopUpValueOfCard(cardNumber) == topUpValue)
                return true;
            else
                return false;
        }

        // generates a card number based on a key
        public int GenerateCardNumber(int topUpValue)
        {
            System.Random RandNum = new System.Random();
            int cardNumber = RandNum.Next(100000, 999999);

            while (true)
            {
                if ((!CheckIfUsed(cardNumber) && CheckIfValid(cardNumber) && CheckIfValueIsValid(cardNumber, topUpValue)))
                    break;
                else
                    cardNumber = RandNum.Next(100000, 999999);
            }

            return cardNumber;
        }

        /****************************************************************************************************/
        // METHODS - MODIFIERS (ROUTE)
        /****************************************************************************************************/

        // adds a used card number to the list of used cards
        public void AddUsedCardNumberToList(int cardNumber)
        {
            _usedCards.Add(cardNumber);
        }
    }
}
