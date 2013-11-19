using System;
using System.Windows;
using System.Windows.Input;

    public class CardSwipeHandler
    {
        //***********************************************************************************************************************
        //Private Attributes
        #region Private Attributes

        private const int _lengthOfEagleID = 8;
        private const string _firstCharInSwipedCard = ";";

        #endregion


        //***********************************************************************************************************************
        //Constructor
        #region Constructor

        public CardSwipeHandler()
        {
        }

        #endregion


        //***********************************************************************************************************************
        //Public Methods
        #region Public Methods

        public string HandleInput(string input)
        {
            if (input.Length >= _lengthOfEagleID)
            {
                if (input.StartsWith(_firstCharInSwipedCard))
                {
                    return CardSwipeInput(input);
                }
                else if (input.Length == _lengthOfEagleID)
                {
                    MessageBox.Show("Entered: " + input);
                    return input;
                }
            }
            return string.Empty;
        }

        #endregion


        //***********************************************************************************************************************
        //Private Methods
        #region Private Methods

        private string CardSwipeInput(string input)
        {
            string[] parsedArray = input.Split(';');
            string finalMemberID = parsedArray[1].Remove(0, 1);
            if (finalMemberID.Length == _lengthOfEagleID)
            {
                MessageBox.Show("SWIPED INPUT: " + input + "\nCONVERTED TO: " + finalMemberID);
                return finalMemberID;
            }
            return string.Empty;
        }

        #endregion
        
    }


