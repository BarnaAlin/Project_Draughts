namespace DraughtsWip
{
    class Board
    {
        private string colour;          //alb/rosu
        private Checker checker;

        public Board(string colour)
        {
            this.colour = colour;
        }

        public void SetCheckerOnBoard(Checker checker)
        {
            this.checker = checker;
        }

        public void RemoveCheckerOnBoard()
        {
            this.checker = null;
        }

        public string getCheckerPieceColour()
        {
            if (checker != null)
                return this.checker.GetCheckerColour();
            else return "empty";
        }

        public int getCheckerNumber()
        {
            return this.checker.GetCheckerNumber();
        }

        public string getCheckerType()
        {
            if (checker != null)
                return this.checker.GetCheckerType();
            else return "null";
        }

    }
}
