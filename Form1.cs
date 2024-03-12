using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DraughtsWip
{
    public partial class Form1 : Form
    {
        static int boardSize;
        Board[,] boardPiece;
        Checker[] checkerWhite, checkerBlack;
        PictureBox[,] pictureBoxBoard;
        PictureBox[] pictureBoxCheckersWhite, pictureBoxCheckersBlack;
        Checker selectedChecker;
        int selectedCheckerIndex, scoreWhite = 0, scoreBlack = 0, capturedPieceI, capturedPieceJ, capturedCheckerIndex;
        string playerTurn = "white";
        bool conditionAI;

        //ai variables
        List<PictureBox> possibleMovesPictures = new List<PictureBox>();
        List<PictureBox> possibleCapturesPictures = new List<PictureBox>();
        List<int> possibleCaptureI = new List<int>();
        List<int> possibleCaptureJ = new List<int>();
        List<int> possibleCaptureIndex = new List<int>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            boardSize = Int16.Parse(textBox1.Text);

            if ((boardSize % 2 == 0) && boardSize >= 6)
            {
                conditionAI = checkBox1.Checked;
                InitializeGame();
            }
            else
            {
                MessageBox.Show("Board size must be >=6 and even!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Text = "8";
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //initializare joc
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void InitializeGame()
        {
            button1.Enabled = false;
            textBox1.Enabled = false;
            checkBox1.Enabled = false;
            boardPiece = new Board[boardSize, boardSize];
            checkerWhite = new Checker[boardSize];
            checkerBlack = new Checker[boardSize];
            pictureBoxBoard = new PictureBox[boardSize, boardSize];
            pictureBoxCheckersWhite = new PictureBox[boardSize];
            pictureBoxCheckersBlack = new PictureBox[boardSize];

            InitializeBoard(boardPiece);
            InitializeCheckersWhite(checkerWhite);
            InitializeCheckersBlack(checkerBlack);

            InitializePicturesCheckersWhite();
            InitializePicturesCheckersBlack();
            InitializePicturesBoard();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //verificare mutari posibile, poate returna un tuplu din numarul de mutari si capturi posibile
        //////////////////////////////////////////////////////////////////////////////////////////////
        private (int possibleMoves, int possibleCaptures) PossibleMovesForChecker(Checker selectedChecker)
        {
            ResetPicturesEvents();
            possibleMovesPictures.Clear();
            possibleCapturesPictures.Clear();
            possibleCaptureI.Clear();
            possibleCaptureJ.Clear();
            possibleCaptureIndex.Clear();

            int possibleMovesNumber = 0, possibleCapturesNumber = 0;
            int i = selectedChecker.GetCheckerI();
            int j = selectedChecker.GetCheckerJ();

            bool canGoLeftUp = true, canGoLeftDown = true,
                canGoRightUp = true, canGoRightDown = true;
            //initializare coordonate initiale checker
            int initialI = i, initialJ = j;

            //////////////////////////////////////////////////////////////////////////////////////////////
            //stanga sus /////////////////////////////////////////////////////////////////////////////////
            while ((i > 0 && j > 0) && canGoLeftUp && (boardPiece[initialI, initialJ].getCheckerPieceColour() == "white" || boardPiece[initialI, initialJ].getCheckerType() == "king"))
            {
                if (boardPiece[i - 1, j - 1].getCheckerPieceColour() == "empty")
                {
                    pictureBoxBoard[i - 1, j - 1].Image = global::DraughtsWip.Properties.Resources.boardPossibleMove;
                    pictureBoxBoard[i - 1, j - 1].Click += new EventHandler(checkerMove_Click);
                    possibleMovesNumber++;
                    possibleMovesPictures.Add(pictureBoxBoard[i - 1, j - 1]);
                }
                //daca se poate captura in stanga sus
                else if ((boardPiece[i - 1, j - 1].getCheckerPieceColour() != boardPiece[initialI, initialJ].getCheckerPieceColour()) && (i > 1 && j > 1) && (boardPiece[i - 2, j - 2].getCheckerPieceColour() == "empty"))
                {
                    canGoLeftUp = false;
                    capturedPieceI = i - 1;
                    capturedPieceJ = j - 1;
                    capturedCheckerIndex = boardPiece[i - 1, j - 1].getCheckerNumber();
                    pictureBoxBoard[i - 2, j - 2].Image = global::DraughtsWip.Properties.Resources.boardPossibleCapture;
                    pictureBoxBoard[i - 2, j - 2].Click += new EventHandler(checkerCapture_Click);
                    possibleCapturesNumber++;
                    possibleCapturesPictures.Add(pictureBoxBoard[i - 2, j - 2]);
                    possibleCaptureI.Add(i - 1);
                    possibleCaptureJ.Add(j - 1);
                    possibleCaptureIndex.Add(boardPiece[i - 1, j - 1].getCheckerNumber());
                }
                //nu se poate muta peste un checker din echipa
                if (boardPiece[i - 1, j - 1].getCheckerPieceColour() == boardPiece[initialI, initialJ].getCheckerPieceColour())
                {
                    canGoLeftUp = false;
                }
                //nu se poate captura peste 2 checkers
                if ((i > 1 && j > 1) && (boardPiece[i - 1, j - 1].getCheckerPieceColour() == boardPiece[i - 2, j - 2].getCheckerPieceColour()) && boardPiece[i - 1, j - 1].getCheckerPieceColour() != "empty")
                {
                    canGoLeftUp = false;
                }
                //daca nu e rege poate muta doar un boardPiece
                if (boardPiece[initialI, initialJ].getCheckerType() != "king") { canGoLeftUp = false; }
                else
                {
                    i--; j--;
                }
            }

            //resetare i si j pentru verificare corecta
            i = initialI; j = initialJ;
            //////////////////////////////////////////////////////////////////////////////////////////////
            //stanga jos /////////////////////////////////////////////////////////////////////////////////
            while ((i < boardSize - 1 && j > 0) && canGoLeftDown && (boardPiece[initialI, initialJ].getCheckerPieceColour() == "black" || boardPiece[initialI, initialJ].getCheckerType() == "king"))
            {
                if (boardPiece[i + 1, j - 1].getCheckerPieceColour() == "empty")
                {
                    pictureBoxBoard[i + 1, j - 1].Image = global::DraughtsWip.Properties.Resources.boardPossibleMove;
                    pictureBoxBoard[i + 1, j - 1].Click += new EventHandler(checkerMove_Click);
                    possibleMovesNumber++;
                    possibleMovesPictures.Add(pictureBoxBoard[i + 1, j - 1]);
                }
                //daca se poate captura in stanga jos
                else if ((boardPiece[i + 1, j - 1].getCheckerPieceColour() != boardPiece[initialI, initialJ].getCheckerPieceColour()) && (i < boardSize - 2 && j > 1) && (boardPiece[i + 2, j - 2].getCheckerPieceColour() == "empty"))
                {
                    canGoLeftDown = false;
                    capturedPieceI = i + 1;
                    capturedPieceJ = j - 1;
                    capturedCheckerIndex = boardPiece[i + 1, j - 1].getCheckerNumber();
                    pictureBoxBoard[i + 2, j - 2].Image = global::DraughtsWip.Properties.Resources.boardPossibleCapture;
                    pictureBoxBoard[i + 2, j - 2].Click += new EventHandler(checkerCapture_Click);
                    possibleCapturesNumber++;
                    possibleCapturesPictures.Add(pictureBoxBoard[i + 2, j - 2]);
                    possibleCaptureI.Add(i + 1);
                    possibleCaptureJ.Add(j - 1);
                    possibleCaptureIndex.Add(boardPiece[i + 1, j - 1].getCheckerNumber());
                }
                //nu se poate muta peste un checker din echipa
                if (boardPiece[i + 1, j - 1].getCheckerPieceColour() == boardPiece[initialI, initialJ].getCheckerPieceColour())
                {
                    canGoLeftDown = false;
                }
                //nu se poate captura peste 2 checkers
                if ((i < boardSize - 2 && j > 1) && (boardPiece[i + 1, j - 1].getCheckerPieceColour() == boardPiece[i + 2, j - 2].getCheckerPieceColour()) && boardPiece[i + 1, j - 1].getCheckerPieceColour() != "empty")
                {
                    canGoLeftDown = false;
                }
                //daca nu e rege poate muta doar un boardPiece
                if (boardPiece[initialI, initialJ].getCheckerType() != "king") { canGoLeftDown = false; }
                else
                {
                    i++; j--;
                }
            }

            //resetare i si j pentru verificare corecta
            i = initialI; j = initialJ;
            //////////////////////////////////////////////////////////////////////////////////////////////
            //dreapta sus ////////////////////////////////////////////////////////////////////////////////
            while ((i > 0 && j < boardSize - 1) && canGoRightUp && (boardPiece[initialI, initialJ].getCheckerPieceColour() == "white" || boardPiece[initialI, initialJ].getCheckerType() == "king"))
            {
                if (boardPiece[i - 1, j + 1].getCheckerPieceColour() == "empty")
                {
                    pictureBoxBoard[i - 1, j + 1].Image = global::DraughtsWip.Properties.Resources.boardPossibleMove;
                    pictureBoxBoard[i - 1, j + 1].Click += new EventHandler(checkerMove_Click);
                    possibleMovesNumber++;
                    possibleMovesPictures.Add(pictureBoxBoard[i - 1, j + 1]);
                }
                //daca se poate captura in dreapta sus
                else if ((boardPiece[i - 1, j + 1].getCheckerPieceColour() != boardPiece[initialI, initialJ].getCheckerPieceColour()) && (i > 1 && j < boardSize - 2) && (boardPiece[i - 2, j + 2].getCheckerPieceColour() == "empty"))
                {
                    canGoRightUp = false;
                    capturedPieceI = i - 1;
                    capturedPieceJ = j + 1;
                    capturedCheckerIndex = boardPiece[i - 1, j + 1].getCheckerNumber();
                    pictureBoxBoard[i - 2, j + 2].Image = global::DraughtsWip.Properties.Resources.boardPossibleCapture;
                    pictureBoxBoard[i - 2, j + 2].Click += new EventHandler(checkerCapture_Click);
                    possibleCapturesNumber++;
                    possibleCapturesPictures.Add(pictureBoxBoard[i - 2, j + 2]);
                    possibleCaptureI.Add(i - 1);
                    possibleCaptureJ.Add(j + 1);
                    possibleCaptureIndex.Add(boardPiece[i - 1, j + 1].getCheckerNumber());
                }
                //nu se poate muta peste un checker din echipa
                if (boardPiece[i - 1, j + 1].getCheckerPieceColour() == boardPiece[initialI, initialJ].getCheckerPieceColour())
                {
                    canGoRightUp = false;
                }
                //nu se poate captura peste 2 checkers
                if ((i > 1 && j < boardSize - 2) && (boardPiece[i - 1, j + 1].getCheckerPieceColour() == boardPiece[i - 2, j + 2].getCheckerPieceColour()) && boardPiece[i - 1, j + 1].getCheckerPieceColour() != "empty")
                {
                    canGoRightUp = false;
                }
                //daca nu e rege poate muta doar un boardPiece
                if (boardPiece[initialI, initialJ].getCheckerType() != "king") { canGoRightUp = false; }
                else
                {
                    i--; j++;
                }
            }

            //resetare i si j pentru verificare corecta
            i = initialI; j = initialJ;
            //////////////////////////////////////////////////////////////////////////////////////////////
            //dreapta jos ////////////////////////////////////////////////////////////////////////////////
            while ((i < boardSize - 1 && j < boardSize - 1) && canGoRightDown && (boardPiece[initialI, initialJ].getCheckerPieceColour() == "black" || boardPiece[initialI, initialJ].getCheckerType() == "king"))
            {
                if (boardPiece[i + 1, j + 1].getCheckerPieceColour() == "empty")
                {
                    pictureBoxBoard[i + 1, j + 1].Image = global::DraughtsWip.Properties.Resources.boardPossibleMove;
                    pictureBoxBoard[i + 1, j + 1].Click += new EventHandler(checkerMove_Click);
                    possibleMovesNumber++;
                    possibleMovesPictures.Add(pictureBoxBoard[i + 1, j + 1]);
                }
                //daca se poate captura in dreapta jos
                else if ((boardPiece[i + 1, j + 1].getCheckerPieceColour() != boardPiece[initialI, initialJ].getCheckerPieceColour()) && (i < boardSize - 2 && j < boardSize - 2) && (boardPiece[i + 2, j + 2].getCheckerPieceColour() == "empty"))
                {
                    canGoRightDown = false;
                    capturedPieceI = i + 1;
                    capturedPieceJ = j + 1;
                    capturedCheckerIndex = boardPiece[i + 1, j + 1].getCheckerNumber();
                    pictureBoxBoard[i + 2, j + 2].Image = global::DraughtsWip.Properties.Resources.boardPossibleCapture;
                    pictureBoxBoard[i + 2, j + 2].Click += new EventHandler(checkerCapture_Click);
                    possibleCapturesNumber++;
                    possibleCapturesPictures.Add(pictureBoxBoard[i + 2, j + 2]);
                    possibleCaptureI.Add(i + 1);
                    possibleCaptureJ.Add(j + 1);
                    possibleCaptureIndex.Add(boardPiece[i + 1, j + 1].getCheckerNumber());
                }
                //nu se poate muta peste un checker din echipa
                if (boardPiece[i + 1, j + 1].getCheckerPieceColour() == boardPiece[initialI, initialJ].getCheckerPieceColour())
                {
                    canGoRightDown = false;
                }
                //nu se poate captura peste 2 checkers
                if ((i < boardSize - 2 && j < boardSize - 2) && (boardPiece[i + 1, j + 1].getCheckerPieceColour() == boardPiece[i + 2, j + 2].getCheckerPieceColour()) && boardPiece[i + 1, j + 1].getCheckerPieceColour() != "empty")
                {
                    canGoRightDown = false;
                }
                //daca nu e rege poate muta doar un boardPiece
                if (boardPiece[initialI, initialJ].getCheckerType() != "king") { canGoRightDown = false; }
                else
                {
                    i++; j++;
                }
            }
            return (possibleMovesNumber, possibleCapturesNumber);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //captura piesa
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void checkerCapture_Click(object sender, EventArgs e)
        {
            string checkerColour = selectedChecker.GetCheckerColour();
            int checkerI = selectedChecker.GetCheckerI();
            int checkerJ = selectedChecker.GetCheckerJ();

            ResetPicturesBoard();
            PictureBox pictureBoxChecker = (PictureBox)sender;
            int index = pictureBoxChecker.TabIndex; //luam index pozitie board selectata pentru mutare

            int contor = 0, boardI = 0, boardJ = 0;
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (contor == index)
                    {
                        boardI = i;
                        boardJ = j;
                    }
                    contor++;
                }
            }

            boardPiece[checkerI, checkerJ].RemoveCheckerOnBoard();
            boardPiece[capturedPieceI, capturedPieceJ].RemoveCheckerOnBoard();

            boardPiece[boardI, boardJ].SetCheckerOnBoard(selectedChecker);

            Point checkerLocationOffset = new Point(25, 25);
            if (checkerColour == "white")
            {
                checkerBlack[capturedCheckerIndex].SetCheckerAsCaptured();
                scoreWhite++;
                pictureBoxCheckersBlack[capturedCheckerIndex].Visible = false;
                checkerWhite[selectedCheckerIndex].SetCheckerPosition(boardI, boardJ);
                pictureBoxCheckersWhite[selectedCheckerIndex].Location = pictureBoxBoard[boardI, boardJ].Location + (Size)checkerLocationOffset;
                label7.Text = scoreWhite.ToString();
                pictureBoxBoard[capturedPieceI, capturedPieceJ].Image = global::DraughtsWip.Properties.Resources.boardRedSkullBlack;

                //make white king
                if (boardI == 0)
                {
                    checkerWhite[selectedCheckerIndex].SetCheckerType("king");
                    pictureBoxCheckersWhite[selectedCheckerIndex].Image = global::DraughtsWip.Properties.Resources.checkerWhiteKing;
                }

                //conditie win
                if (scoreWhite == boardSize)
                {
                    DialogResult result = MessageBox.Show("White Player wins the game!", "WIN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result == DialogResult.OK)
                    {
                        this.Close();
                    }
                }
            }
            else
            {
                checkerWhite[capturedCheckerIndex].SetCheckerAsCaptured();
                scoreBlack++;
                pictureBoxCheckersWhite[capturedCheckerIndex].Visible = false;
                checkerBlack[selectedCheckerIndex].SetCheckerPosition(boardI, boardJ);
                pictureBoxCheckersBlack[selectedCheckerIndex].Location = pictureBoxBoard[boardI, boardJ].Location + (Size)checkerLocationOffset;
                label8.Text = scoreBlack.ToString();
                pictureBoxBoard[capturedPieceI, capturedPieceJ].Image = global::DraughtsWip.Properties.Resources.boardRedSkullWhite;

                //make black king
                if (boardI == boardSize - 1)
                {
                    checkerBlack[selectedCheckerIndex].SetCheckerType("king");
                    pictureBoxCheckersBlack[selectedCheckerIndex].Image = global::DraughtsWip.Properties.Resources.checkerBlackKing;
                }

                //conditie win
                if (scoreBlack == boardSize)
                {
                    DialogResult result = MessageBox.Show("Black Player wins the game!", "WIN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result == DialogResult.OK)
                    {
                        this.Close();
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //mutare piesa
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void checkerMove_Click(object sender, EventArgs e)
        {
            //schimba tura dupa mutare
            if (playerTurn == "white")
                playerTurn = "black";
            else playerTurn = "white";
            label3.Text = playerTurn;

            //proprietati checker selectat care se muta
            string checkerColour = selectedChecker.GetCheckerColour(); 
            int checkerI = selectedChecker.GetCheckerI();
            int checkerJ = selectedChecker.GetCheckerJ();

            ResetPicturesBoard();
            PictureBox pictureBoxChecker = (PictureBox)sender;
            int index = pictureBoxChecker.TabIndex; //luam index board selectata pentru mutare

            //parcurgem matricea (tabla) pentru aflare coordonate mutare folosind index
            int contor = 0, boardI = 0, boardJ = 0;
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (contor == index)
                    {
                        boardI = i;
                        boardJ = j;
                    }
                    contor++;
                }
            }

            boardPiece[checkerI, checkerJ].RemoveCheckerOnBoard();
            boardPiece[boardI, boardJ].SetCheckerOnBoard(selectedChecker);

            Point checkerLocationOffset = new Point(25, 25);
            if (checkerColour == "white")
            {
                checkerWhite[selectedCheckerIndex].SetCheckerPosition(boardI, boardJ);
                pictureBoxCheckersWhite[selectedCheckerIndex].Location = pictureBoxBoard[boardI, boardJ].Location + (Size)checkerLocationOffset;

                //make white king
                if (boardI == 0)
                {
                    checkerWhite[selectedCheckerIndex].SetCheckerType("king");
                    pictureBoxCheckersWhite[selectedCheckerIndex].Image = global::DraughtsWip.Properties.Resources.checkerWhiteKing;
                }

                //in caz de AI se ia o piesa random cu mutari
                if (conditionAI)
                {
                    while (playerTurn == "black")
                    {
                        AICheckAndMove();
                    }
                }
            }
            else
            {
                checkerBlack[selectedCheckerIndex].SetCheckerPosition(boardI, boardJ);
                pictureBoxCheckersBlack[selectedCheckerIndex].Location = pictureBoxBoard[boardI, boardJ].Location + (Size)checkerLocationOffset;

                //make black king
                if (boardI == boardSize - 1)
                {
                    checkerBlack[selectedCheckerIndex].SetCheckerType("king");
                    pictureBoxCheckersBlack[selectedCheckerIndex].Image = global::DraughtsWip.Properties.Resources.checkerBlackKing;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //AI verificare
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void AICheckAndMove()
        {
            ResetPicturesBoard();
            Random rand = new Random();
            int checkerRandom = rand.Next(0, boardSize);
            Checker randomMovedChecker = checkerBlack[checkerRandom];

            //if the random checker is still on the table (not captured)
            if (randomMovedChecker.GetCheckerCaptured() == false)
            {
                int howManyMoves = PossibleMovesForChecker(randomMovedChecker).possibleMoves;
                int howManyCaptures = PossibleMovesForChecker(randomMovedChecker).possibleCaptures;

                //if the random checker can capture
                if (howManyCaptures > 0)
                {
                    int whichCapture = rand.Next(0, howManyCaptures); //se ia captura random din capturile posibile
                    int indexPicture = possibleCapturesPictures.ElementAt(0).TabIndex; //se ia indexul unde se muta
                    int randomCapturedCheckerI = possibleCaptureI.ElementAt(0);
                    int randomCapturedCheckerJ = possibleCaptureJ.ElementAt(0);
                    int randomCapturedCheckerIndex = possibleCaptureIndex.ElementAt(0);

                    int possibleContor = 0, possibleBoardI = 0, possibleBoardJ = 0;

                    for (int i = 0; i < boardSize; i++)
                    {
                        for (int j = 0; j < boardSize; j++)
                        {
                            if (possibleContor == indexPicture)
                            {
                                possibleBoardI = i;
                                possibleBoardJ = j;
                            }
                            possibleContor++;
                        }
                    }
                    captureAI(randomMovedChecker, randomCapturedCheckerI, randomCapturedCheckerJ, randomCapturedCheckerIndex, possibleBoardI, possibleBoardJ);
                }

                //if the random checker can move
                else if (howManyMoves > 0)
                {
                    int whichMove = rand.Next(0, howManyMoves); //se ia mutare random din mutarile posibile
                    int indexPicture = possibleMovesPictures.ElementAt(whichMove).TabIndex; //se ia indexul unde se muta
                    int possibleContor = 0, possibleBoardI = 0, possibleBoardJ = 0;

                    for (int i = 0; i < boardSize; i++)
                    {
                        for (int j = 0; j < boardSize; j++)
                        {
                            if (possibleContor == indexPicture)
                            {
                                possibleBoardI = i;
                                possibleBoardJ = j;
                            }
                            possibleContor++;
                        }
                    }
                    moveAI(randomMovedChecker, possibleBoardI, possibleBoardJ);
                }
            }
            else { AICheckAndMove(); }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //AI capturare
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void captureAI(Checker checker, int randomCapturedPieceI, int randomCapturedPieceJ, int randomCapturedCheckerIndex, int possibleBoardI, int possibleBoardJ)
        {
            ResetPicturesEvents();

            int checkerNumber = checker.GetCheckerNumber();
            int checkerI = checker.GetCheckerI();
            int checkerJ = checker.GetCheckerJ();

            boardPiece[checkerI, checkerJ].RemoveCheckerOnBoard();
            boardPiece[randomCapturedPieceI, randomCapturedPieceJ].RemoveCheckerOnBoard();

            boardPiece[possibleBoardI, possibleBoardJ].SetCheckerOnBoard(checker);

            Point checkerLocationOffset = new Point(25, 25);

            checkerBlack[checkerNumber].SetCheckerPosition(possibleBoardI, possibleBoardJ);
            pictureBoxCheckersBlack[checkerNumber].Location = pictureBoxBoard[possibleBoardI, possibleBoardJ].Location + (Size)checkerLocationOffset;

            pictureBoxCheckersWhite[randomCapturedCheckerIndex].Visible = false;
            checkerWhite[randomCapturedCheckerIndex].SetCheckerAsCaptured();

            pictureBoxBoard[checkerI, checkerJ].Image = global::DraughtsWip.Properties.Resources.boardRedMovedFrom;
            pictureBoxBoard[randomCapturedPieceI, randomCapturedPieceJ].Image = global::DraughtsWip.Properties.Resources.boardRedSkullWhite;

            scoreBlack++;
            label8.Text = scoreBlack.ToString();

            //make black king
            if (possibleBoardI == boardSize - 1)
            {
                checkerBlack[checkerNumber].SetCheckerType("king");
                pictureBoxCheckersBlack[checkerNumber].Image = global::DraughtsWip.Properties.Resources.checkerBlackKing;
            }

            if (scoreBlack == boardSize)
            {
                DialogResult result = MessageBox.Show("Black Player wins the game!", "WIN", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    this.Close();
                }
            }
            playerTurn = "white";
            label3.Text = playerTurn;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //AI mutare
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void moveAI(Checker checker, int possibleBoardI, int possibleBoardJ)
        {
            ResetPicturesEvents();

            int checkerNumber = checker.GetCheckerNumber();
            int checkerI = checker.GetCheckerI();
            int checkerJ = checker.GetCheckerJ();

            boardPiece[checkerI, checkerJ].RemoveCheckerOnBoard();
            boardPiece[possibleBoardI, possibleBoardJ].SetCheckerOnBoard(checker);

            Point checkerLocationOffset = new Point(25, 25);

            checkerBlack[checkerNumber].SetCheckerPosition(possibleBoardI, possibleBoardJ);
            pictureBoxCheckersBlack[checkerNumber].Location = pictureBoxBoard[possibleBoardI, possibleBoardJ].Location + (Size)checkerLocationOffset;

            pictureBoxBoard[checkerI, checkerJ].Image = global::DraughtsWip.Properties.Resources.boardRedMovedFrom;
            
            //make black king
            if (possibleBoardI == boardSize - 1)
            {
                checkerBlack[checkerNumber].SetCheckerType("king");
                pictureBoxCheckersBlack[checkerNumber].Image = global::DraughtsWip.Properties.Resources.checkerBlackKing;
            }
            playerTurn = "white";
            label3.Text = playerTurn;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //apasare checker alb
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void checkerWhite_Click(object sender, EventArgs e)
        {
            if (playerTurn == "white")
            {
                ResetPicturesBoard();
                PictureBox pictureBoxChecker = (PictureBox)sender;
                int index = pictureBoxChecker.TabIndex; //luam index poza checker selectat

                selectedCheckerIndex = index;
                selectedChecker = checkerWhite[index];
                PossibleMovesForChecker(selectedChecker);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //apasare checker negru
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void checkerBlack_Click(object sender, EventArgs e)
        {
            if (playerTurn == "black")
            {
                ResetPicturesBoard();
                PictureBox pictureBoxCheckersBlack = (PictureBox)sender;
                int index = pictureBoxCheckersBlack.TabIndex; //luam index poza checker selectat

                selectedCheckerIndex = index;
                selectedChecker = checkerBlack[index];
                PossibleMovesForChecker(selectedChecker);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //initializare poze checkers alb
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void InitializePicturesCheckersWhite()
        {
            for (int i = 0; i < boardSize; i++)
            {
                int checkerRow = 0;
                pictureBoxCheckersWhite[i] = new PictureBox();
                pictureBoxCheckersWhite[i].Click += new EventHandler(checkerWhite_Click);
                pictureBoxCheckersWhite[i].Image = global::DraughtsWip.Properties.Resources.checkerWhite;
                pictureBoxCheckersWhite[i].Name = "pictureBoxCheckersWhite" + (i + 1);
                pictureBoxCheckersWhite[i].Size = new System.Drawing.Size(50, 50);
                pictureBoxCheckersWhite[i].TabIndex = i;
                pictureBoxCheckersWhite[i].TabStop = false;
                if (i % 2 == 1)
                {
                    checkerRow = -100;
                }
                else
                {
                    checkerRow = 0;
                }
                pictureBoxCheckersWhite[i].Location = new System.Drawing.Point(75 + (i * 100), (100 * boardSize) - 25 + checkerRow);
                this.Controls.Add(pictureBoxCheckersWhite[i]);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //initializare poze checkers negru
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void InitializePicturesCheckersBlack()
        {
            for (int i = 0; i < boardSize; i++)
            {
                int checkerRow = 0;
                pictureBoxCheckersBlack[i] = new PictureBox();
                pictureBoxCheckersBlack[i].Click += new EventHandler(checkerBlack_Click);
                pictureBoxCheckersBlack[i].Image = global::DraughtsWip.Properties.Resources.checkerBlack;
                pictureBoxCheckersBlack[i].Name = "pictureBoxCheckersBlack" + (i + 1);
                pictureBoxCheckersBlack[i].Size = new System.Drawing.Size(50, 50);
                pictureBoxCheckersBlack[i].TabIndex = i;
                pictureBoxCheckersBlack[i].TabStop = false;
                if (i % 2 == 1)
                {
                    checkerRow = -100;
                }
                else
                {
                    checkerRow = 0;
                }
                pictureBoxCheckersBlack[i].Location = new System.Drawing.Point(75 + (i * 100), 175 + checkerRow);
                this.Controls.Add(pictureBoxCheckersBlack[i]);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //initializare checkers alb
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void InitializeCheckersWhite(Checker[] checkerWhite)
        {
            int row = (boardSize - 1);
            for (int i = 0; i < boardSize; i++)
            {
                checkerWhite[i] = new Checker(i, "normal", "white", row, i);
                boardPiece[row, i].SetCheckerOnBoard(checkerWhite[i]);
                if (row == (boardSize - 1))
                {
                    row--;
                }
                else { row++; }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //initializare checkers negru
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void InitializeCheckersBlack(Checker[] checkerBlack)
        {
            int row = 1;
            for (int i = 0; i < boardSize; i++)
            {
                checkerBlack[i] = new Checker(i, "normal", "black", row, i);
                boardPiece[row, i].SetCheckerOnBoard(checkerBlack[i]);
                if (row == 1)
                {
                    row--;
                }
                else { row++; }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //resetare poze tabla dupa verificari mutari
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void ResetPicturesBoard()
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        pictureBoxBoard[i, j].Image = global::DraughtsWip.Properties.Resources.boardWhite;
                    }
                    else
                    {
                        pictureBoxBoard[i, j].Image = global::DraughtsWip.Properties.Resources.boardRed;
                    }
                    pictureBoxBoard[i, j].Click -= checkerMove_Click;
                    pictureBoxBoard[i, j].Click -= checkerCapture_Click;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //resetare evenimente poze tabla
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void ResetPicturesEvents()
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    pictureBoxBoard[i, j].Click -= checkerMove_Click;
                    pictureBoxBoard[i, j].Click -= checkerCapture_Click;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //initializare poze tabla
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void InitializePicturesBoard()
        {
            int pictureLocationColumn = 0, pictureLocationLine = 0;
            int contor = 0;
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    pictureBoxBoard[i, j] = new PictureBox();

                    if ((i + j) % 2 == 0)
                    {
                        pictureBoxBoard[i, j].Image = global::DraughtsWip.Properties.Resources.boardWhite;
                    }
                    else
                    {
                        pictureBoxBoard[i, j].Image = global::DraughtsWip.Properties.Resources.boardRed;
                    }

                    pictureBoxBoard[i, j].Location = new System.Drawing.Point(50 + pictureLocationColumn, 50 + pictureLocationLine);
                    pictureBoxBoard[i, j].Name = "pictureBox" + (i + 1);
                    pictureBoxBoard[i, j].Size = new System.Drawing.Size(100, 100);
                    pictureBoxBoard[i, j].TabIndex = contor;
                    pictureBoxBoard[i, j].TabStop = false;
                    this.Controls.Add(pictureBoxBoard[i, j]);

                    pictureLocationColumn += 100;
                    contor++;
                }
                pictureLocationColumn = 0;
                pictureLocationLine += 100;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //initializare tabla
        //////////////////////////////////////////////////////////////////////////////////////////////
        private void InitializeBoard(Board[,] boardPiece)
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if ((i + j) % 2 == 1)
                    {
                        boardPiece[i, j] = new Board("red");
                    }
                    else
                    {
                        boardPiece[i, j] = new Board("white");
                    }
                }
            }
        }
    }
}
