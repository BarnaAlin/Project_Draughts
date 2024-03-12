using System;

namespace DraughtsWip
{
    internal class Checker
    {
        private int number;
        private string type = "normal"; //normal/rege
        private string colour;          //alb/negru
        private int iBoard, jBoard;
        private bool captured = false;
        public Checker(int number, string type, string colour, int i, int j)
        {
            this.number = number;
            this.type = type;
            this.colour = colour;
            this.iBoard = i;
            this.jBoard = j;
        }
        public void SetCheckerType(string type)
        {
            this.type = type;
        }

        public void SetCheckerAsCaptured()
        {
            this.captured = true;
        }

        public bool GetCheckerCaptured()
        {
            return this.captured;
        }

        public string GetCheckerType()
        {
            return this.type;
        }

        public string GetCheckerColour()
        {
            return this.colour;
        }

        public int GetCheckerNumber()
        {
            return this.number;
        }

        public void SetCheckerPosition(int i, int j)
        {
            this.iBoard = i;
            this.jBoard = j;
        }
        public int GetCheckerI()
        {
            return this.iBoard;
        }
        public int GetCheckerJ()
        {
            return this.jBoard;
        }
    }
}