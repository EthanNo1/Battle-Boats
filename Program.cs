using System;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace BattleBoats
{
    internal class Program
    {
        //global variables
        static bool quit;

        static void Main(string[] args)
        {
            
            Title();
            while (!quit) //Loops game until quit answered in Menu()
            {
                Menu();
            }
        }
        static void Menu() //Outputs options menu & directs program to other functions per the user's selection
        {
            Console.ForegroundColor = ConsoleColor.Red;
            int choice = 0;
            Console.WriteLine("Choose an option from the menu\n1 - New Game\n2 - Resume\n3 - Instructions\n4 - Quit");
            try { choice = Convert.ToInt32(Console.ReadLine()); } //takes user's choice and validates it
            catch { Console.WriteLine("Choose a valid input"); } //"
            switch(choice)
            {
                case 1: //creates user & computer boards and places ships
                    char[,] NewComputerBoard = PlaceComputerShips();
                    char[,] NewUserBoard = PlaceShips();
                    char[,] NewAttackBoard = CreateBoard();
                    bool goFirst = RockPaperScissors();
                    Play(NewUserBoard, NewComputerBoard, NewAttackBoard, goFirst);
                    break;
                case 2: //gets user & computer boards from text file
                    string[] board2d = File.ReadAllText("SavedBoard.txt").Split("/");
                    char[,] userBoard = new char[8, 8];
                    char[,] computerBoard = new char[8, 8];
                    char[,] attackBoard = new char[8, 8];
                    bool turn;
                    for (int i = 0; i < 24; i++) //loops through rows of saved board
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (i < 8) { userBoard[i, j] = board2d[i][j]; } //the 1st 8 lines belong to the user
                            else if (i < 16) { computerBoard[i - 8, j] = board2d[i][j]; } //the 2nd 8 belong to the computer, but for the computerBoard's first index the "i" count must restart (hence the -8)
                            else attackBoard[i - 16, j] = board2d[i][j];
                        }
                    }
                    turn = board2d[16][0] == '1';
                    Play(userBoard, computerBoard, attackBoard, turn);
                    break;
                case 3: //outputs instructions
                    Console.WriteLine(File.ReadAllText("instructions.txt"));
                    break;
                case 4: //quits the game
                    System.Environment.Exit(0);
                    break;
            }
        }
        static void Title()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            string title = File.ReadAllText("Title.txt");
            Console.WriteLine(title); //Font is "Small" from https://patorjk.com/software/taag/#p=display&f=Small&t=BATTLE%20BOATS
        } //Prints the title
        static void Play(char[,] userBoard, char[,] computerBoard, char[,] attackBoard, bool goFirst) //Loops the main game
        {
            
            bool finished = false;
            string attack;
            saveGame(userBoard, computerBoard, attackBoard, goFirst);
            Console.Clear();
            while (!finished) //loops until there is a winner
            {
                if (CheckWin(attackBoard)||CheckWin(userBoard)) { finished = true; break; }//checks for a win
                RenderAllBoards(userBoard, attackBoard);
                if (goFirst) //if the user goes first then this set of code is run
                {
                    attackBoard = playerAttack(computerBoard, attackBoard);
                    System.Threading.Thread.Sleep(1000);
                    RenderAllBoards(userBoard, attackBoard);
                    System.Threading.Thread.Sleep(500);
                    saveGame(userBoard, computerBoard, attackBoard, false);
                    if (CheckWin(attackBoard)) { finished = true; break; }
                    Console.Clear();
                    Console.WriteLine("Computer attacking...");
                    System.Threading.Thread.Sleep(1500);
                    RenderAllBoards(userBoard, attackBoard);
                    userBoard = computerAttack(userBoard);
                    saveGame(userBoard, computerBoard, attackBoard, true);
                    if (CheckWin(userBoard)) { finished = true; break; }
                    RenderAllBoards(userBoard, attackBoard) ;
                }
                else //if the computer goes first then this set of code is run
                {
                    Console.Clear();
                    Console.WriteLine("Computer attacking...");
                    System.Threading.Thread.Sleep(1500);
                    RenderAllBoards(userBoard, attackBoard);
                    userBoard = computerAttack(userBoard);
                    saveGame(userBoard, computerBoard, attackBoard, true);
                    if (CheckWin(userBoard)) { finished = true; break; }
                    RenderAllBoards(userBoard, attackBoard);
                    attackBoard = playerAttack(computerBoard, attackBoard);
                    saveGame(userBoard, computerBoard, attackBoard, false);
                    if (CheckWin(attackBoard)) { finished = true; break; }
                    System.Threading.Thread.Sleep(1000);
                    RenderAllBoards (userBoard, attackBoard);
                    System.Threading.Thread.Sleep(500);
                }
            }
            if (CheckWin(attackBoard))
            {
                RenderAllBoards(userBoard, attackBoard);
                Console.ForegroundColor = ConsoleColor.Black; Console.BackgroundColor = ConsoleColor.Magenta;
                Console.WriteLine("You Win!!!");
                Console.ForegroundColor = ConsoleColor.Red; Console.BackgroundColor = ConsoleColor.Black;
                System.Threading.Thread.Sleep(1000);
            }
            else
            {
                RenderAllBoards(userBoard, attackBoard);
                Console.WriteLine("You Lose :(");
            }
        }
        static bool RockPaperScissors()//Plays rock, paper, scissors to determine who attacks first
        {
            Console.Clear();
            bool win = false;
            string[] choiceList = { "ROCK", "PAPER", "SCISSORS" };
            Console.WriteLine("You will play \"rock, paper scissors\" with the computer to determine who goes first.");
            Random random = new Random();
            while (win == false)
            {
                Console.WriteLine("Rock, Paper or Scissors?");
                string userPlay = Console.ReadLine().ToUpper();
                if (Array.IndexOf(choiceList, userPlay) == -1)
                {
                    Console.WriteLine("Choose from \"Rock\", \"Paper\" or \"Scissors\"");
                    continue;
                }
                string compPlay = choiceList[random.Next(3)];
                int compIndex = Array.IndexOf(choiceList, compPlay);
                int userIndex = Array.IndexOf(choiceList, userPlay);
                if (compPlay == userPlay) //if the user and computer give the same answer
                {
                    Console.WriteLine($"User: {choiceList[userIndex]}\nComputer: {choiceList[compIndex]}\nDraw.");
                    System.Threading.Thread.Sleep(500);
                    continue;
                }
                else if ((compIndex == 1 && userIndex == 2)||(compIndex == 0 && userIndex == 1) || (compIndex==2&&userIndex==0)) //if the user wins
                {
                    Console.WriteLine($"User: {choiceList[userIndex]}\nComputer: {choiceList[compIndex]}\nYou win!");
                    System.Threading.Thread.Sleep(2000);
                    return true;
                }
                else //if the user loses
                {
                    Console.WriteLine($"User: {choiceList[userIndex]}\nComputer: {choiceList[compIndex]}\nYou lose!");
                    System.Threading.Thread.Sleep(2000);
                    return false;
                }
            }
            return true;
        } 
        static void saveGame(char[,] userBoard, char[,] computerBoard, char[,] attackBoard, bool turn)//Saves the state of the game
        {
            string formattedData = "";
            for(int i = 0;  i < 24; i++) //the two for loops loop through the three boards one after another. As there are three boards with 8 rows each, there are 3*8=24 rows to be looped through
            {
                for(int j = 0; j < 8; j++)
                {
                    if (i < 8) formattedData += userBoard[i, j];
                    else if (i < 16) formattedData += computerBoard[i-8, j];
                    else formattedData+= attackBoard[i-16,j];
                }
                formattedData += "/";
            }
            if (turn) formattedData += "1/"; //depending on whose turn it is a 1 or a 0 is added to the end of the file
            else formattedData += "0/";
            File.WriteAllText("SavedBoard.txt", formattedData);

        } 
        static void RenderAllBoards(char[,] userBoard, char[,] attackBoard)//Prints the user's and computer's board from the user's point of view
        {
            Console.Clear();
            Console.WriteLine("\nYour board:");
            RenderBoard(userBoard);
            Console.WriteLine("\nComputer's board:");
            RenderBoard(attackBoard);
            
        } 
        static char[,] playerAttack(char[,] computerBoard, char[,] attackBoard)//Edits the computer's gameboard according to the inputted coordinates
        {
            bool attacking = true;
            string attack;
            while (attacking)
            {
                Console.WriteLine("\nInput a coordinate");
                attack = Console.ReadLine().ToUpper();
                string[] attackCoords = new string[2];
                char targetedCoord = ' ';
                try { attackCoords = GetCoords(attack); targetedCoord = computerBoard[int.Parse(attackCoords[0]), int.Parse(attackCoords[1])]; } //the input is translated into usable indexes and "targetedCoord" is set to 'B','C','U','D' or 'P' based on the boat
                catch { Console.WriteLine("Invalid input."); continue; }
                char hitCheck = attackBoard[int.Parse(attackCoords[0]), int.Parse(attackCoords[1])]; //the input is translated into usable indexes and "targetedCoord" is set to 'M','H','S' or '~' depending on what is there

                if (hitCheck == 'H'|| hitCheck == 'S' ||hitCheck=='M') //if the space has been targeted already, the user is prompted to input again
                {
                    Console.WriteLine("Already attacked.");
                    continue;
                }
                if (targetedCoord == 'B'|| targetedCoord == 'C' || targetedCoord == 'U' || targetedCoord == 'D' || targetedCoord == 'P') //if a boat is there, the board is edited appropriately.
                {
                    Console.BackgroundColor = ConsoleColor.Green; Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("You hit!");
                    attackBoard[int.Parse(attackCoords[0]), int.Parse(attackCoords[1])] = 'H';
                    Console.BackgroundColor = ConsoleColor.Black; Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (targetedCoord == '~')//if a boat is not there, the board is edited appropriately.
                {
                    Console.BackgroundColor = ConsoleColor.Yellow; Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("You missed!");
                    attackBoard[int.Parse(attackCoords[0]), int.Parse(attackCoords[1])] = 'M';
                    Console.BackgroundColor = ConsoleColor.Black; Console.ForegroundColor = ConsoleColor.Red;
                }
                
                attacking = false;
            }
            attackBoard = CheckSunk(computerBoard, attackBoard);
            return attackBoard;
        } 
        static bool CheckWin(char[,] board) //Checks if the user or computer have won the game
        {
            int hitCount = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i,j] == 'H'|| board[i, j] == 'S')
                    {
                        hitCount++; //counts how many boats have been hit, and if it equals the starting boats then someone has won 
                    }
                }
            }
            if (hitCount == 9) return true;
            else return false;
        }
        static char[,] CheckSunk(char[,] computerBoard, char[,] attackBoard)//Checks if any of the computer's boats are sunk
        {
            char boatID;
            char checkChar;
            int uCount=0;
            int pCount=0;
            int cCount = 0;
            for (int k = 0; k < 3; k++) //itereates through the board and checks if tall of one type of boat are sunk (by checking if the user views a hit where a type of boat is)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        boatID = computerBoard[i, j];
                        checkChar = attackBoard[i, j];
                        if (checkChar == 'H' && boatID == 'D') attackBoard[i, j] = 'S';
                        if (checkChar == 'H' && boatID == 'U')
                        {
                            if (uCount >= 4) attackBoard[i, j] = 'S';
                            uCount++;
                            
                        }
                        if (checkChar == 'H' && boatID == 'P')
                        {
                            if (pCount >= 4) attackBoard[i, j] = 'S';
                            pCount++;
                            
                        }
                        if (checkChar == 'H' && boatID == 'C')
                        {
                            if (cCount >= 6) attackBoard[i, j] = 'S';
                            cCount++;
                        }
                    }
                }
            }
            return attackBoard;
        } 
        static char[,] computerAttack( char[,] playerBoard)//Generates coordinates for the computer's attack, and edits the user's gameboard accordingly
        {
            Random random = new Random();
            bool uniqueAttack = false;
            int[] attackCoords = new int[2];
            while (!uniqueAttack)
            {
                attackCoords[0] = random.Next(8);
                attackCoords[1] = random.Next(8);
                if (playerBoard[attackCoords[0], attackCoords[1]] == 'B')
                {
                    Console.BackgroundColor = ConsoleColor.Yellow; Console.ForegroundColor = ConsoleColor.Black;
                    playerBoard[attackCoords[0], attackCoords[1]] = 'H';
                    Console.WriteLine("\nComputer hit!");
                    System.Threading.Thread.Sleep(2000);
                    uniqueAttack = true;
                }
                else if (playerBoard[attackCoords[0], attackCoords[1]] == '~')
                {
                    Console.BackgroundColor = ConsoleColor.Green; Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\nComputer missed!");
                    System.Threading.Thread.Sleep(2000);
                    uniqueAttack = true;
                }
                else
                {
                    continue;
                }
            }
            Console.BackgroundColor = ConsoleColor.Black; Console.ForegroundColor = ConsoleColor.Red;
            return playerBoard;
        } 
        static char[,] PlaceShips()//Prompts the user to place their ships
        {
            
            char[,] board = CreateBoard();
            bool ready = false;
            string[] placeCoords = new string[2];
            string[] otherCoords = new string[4];
            string answer;
            string[] allowedCoords = new string[4];
            string[] eraseCoords = new string[2];
            bool carrierPlaced = false;
            int persistentCounter = 0; //counts the number of ships placed so that the program knows when to move on
            while(persistentCounter<2)//placing destroyers
            {
                Console.Clear();
                RenderBoard(board);
                Console.WriteLine($"It's time to place your destroyers!\nYou have {2 - persistentCounter}x Destroyers\nInput the coordinates of a destroyer.");
                try { placeCoords = GetCoords(Console.ReadLine()); }
                catch { Console.WriteLine("Invalid input."); continue; }
                if ((String.IsNullOrEmpty(placeCoords[0]) || String.IsNullOrEmpty(placeCoords[1]))) //checks if the input is valid
                {
                    Console.WriteLine("Invalid input.");
                    continue; 
                }
                else if (board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] == 'B')//checks if the coordinates are new
                {
                    Console.WriteLine("Space occupied.");
                    continue;
                }
                board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] = 'B'; //places the ship
                persistentCounter++;
                RenderBoard(board);
                Console.WriteLine("Is this right? (Y/N)");
                answer = Console.ReadLine().ToUpper();
                if (answer != "Y" && answer != "YES") //checks if this is what the user wanted
                {
                    if (answer != "N" || answer != "NO") Console.WriteLine("Invalid input");
                    board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] = '~';
                    persistentCounter--;
                    continue;
                }
            }
            persistentCounter = 0;
            while (persistentCounter < 2)//placing submarines
            {
                Console.Clear();
                RenderBoard(board);
                allowedCoords = new string[4];
                Console.WriteLine($"It's time to place your submarines!\nYou have {2 - persistentCounter}x Submarines\nInput the coordinate of a part of a submarine.");
                try { placeCoords = GetCoords(Console.ReadLine()); if (board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] == 'B') { } }
                catch { Console.WriteLine("Invalid input."); continue; }
                string[] firstCoords = placeCoords;
                if (board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] == 'B')
                {
                    Console.WriteLine("Space already occupied.");
                    continue;
                }
                eraseCoords = placeCoords;
                board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] = 'B';//places part of the ship
                RenderBoard(board);
                otherCoords = OtherCoords(placeCoords);
                //above code is near identical to that of the destroyer placement
                Console.WriteLine($"Where would you like second part to go?");
                for (int j = 0; j < otherCoords.Length; j++)
                {
                    if (!(String.IsNullOrEmpty(otherCoords[j]) || board[int.Parse(GetCoords(otherCoords[j])[0]), int.Parse(GetCoords(otherCoords[j])[1])] =='B'))
                    {
                        allowedCoords[j]=(otherCoords[j]); //adds to a list of acceptable coordinates if it meets the criteria
                    }
                }
                foreach (string coord in allowedCoords) { if (!(String.IsNullOrEmpty(coord))){ Console.Write(coord + " "); } }
                Console.WriteLine();
                string testCoords = Console.ReadLine().ToUpper();
                
                
                if (Array.IndexOf(allowedCoords, testCoords) == -1) //checks if the inputted coordinate is acceptable
                {
                    Console.WriteLine("Please choose an adjacent coordinate.");
                    board[int.Parse(placeCoords[1]), int.Parse(placeCoords[0])] = '~';
                    continue;
                }
                try { placeCoords = GetCoords(testCoords); }
                catch { Console.WriteLine("Invalid input."); continue; }
                board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] = 'B'; //places part of the ship
                persistentCounter++;
                RenderBoard(board);
                Console.WriteLine("Is this right? (Y/N)");
                answer = Console.ReadLine().ToUpper();
                if (answer != "Y" && answer != "YES")//checks if this is what the user wanted
                {
                    if (answer != "N" || answer != "NO") Console.WriteLine("Invalid input");
                    board[int.Parse(eraseCoords[1]), int.Parse(eraseCoords[0])] = '~';
                    board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] = '~';
                    persistentCounter--;
                    continue;
                }
            }
            
            //placing the carrier
            while(carrierPlaced == false)
            {
                Console.Clear();
                RenderBoard(board);
                allowedCoords = new string[4];
                Console.WriteLine($"It's time to place your Carrier!\nYou have 1x Carriers\nInput the coordinate of a part of a carrier.");
                try { placeCoords = GetCoords(Console.ReadLine()); if (board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] == 'B') { } }
                catch { Console.WriteLine("Invalid input."); }
                if ((String.IsNullOrEmpty(placeCoords[0]) || String.IsNullOrEmpty(placeCoords[1])))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }
                else if (board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] == 'B')
                {
                    Console.WriteLine("Space occupied.");
                    continue;
                }
                string[] firstPart = placeCoords;
                board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] = 'B';
                RenderBoard(board);
                otherCoords = OtherCoords(placeCoords);
                Console.WriteLine($"Where would you like second part to go?");
                for (int j = 0; j < otherCoords.Length; j++)
                {
                    if (!(String.IsNullOrEmpty(otherCoords[j]) || board[int.Parse(GetCoords(otherCoords[j])[0]), int.Parse(GetCoords(otherCoords[j])[1])] == 'B'))
                    {
                        allowedCoords[j] = (otherCoords[j]);
                    }
                }
                foreach (string coord in allowedCoords) { if (!(String.IsNullOrEmpty(coord))) { Console.Write(coord + " "); } }
                Console.WriteLine();
                string testCoords = Console.ReadLine().ToUpper();
                
                if (Array.IndexOf(allowedCoords, testCoords) == -1)
                {
                    Console.WriteLine("Please choose an adjacent coordinate.");
                    board[int.Parse(placeCoords[1]), int.Parse(placeCoords[0])] = '~';
                    continue;
                }
                try { placeCoords = GetCoords(testCoords); }
                catch { Console.WriteLine("Invalid input."); }

                board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] = 'B';
                string[] secondPart = placeCoords;
                RenderBoard(board);
                //above code is near identical to submarine placing

                Console.WriteLine($"Where would you like third part to go?");
                string[] thirdPart1 = new string[2];
                string[] thirdPart2 = new string[2];
                Array.Reverse(secondPart);

                allowedCoords = new string[2];
                if (firstPart[0] == secondPart[0])
                {
                    thirdPart1[0] = firstPart[0];
                    thirdPart2[0] = firstPart[0];
                    if (int.Parse(firstPart[1]) > int.Parse(secondPart[1]))
                    {
                        thirdPart1[1] = Convert.ToString(int.Parse(secondPart[1]) - 1);
                        thirdPart2[1] = Convert.ToString(int.Parse(firstPart[1]) + 1);
                    }
                    else if (int.Parse(firstPart[1]) < int.Parse(secondPart[1]))
                    {
                        thirdPart1[1] = Convert.ToString(int.Parse(secondPart[1]) + 1);
                        thirdPart2[1] = Convert.ToString(int.Parse(firstPart[1]) - 1);
                    }
                }
                else if (firstPart[1] == secondPart[1])
                {
                    thirdPart1[1] = firstPart[1];
                    thirdPart2[1] = firstPart[1];
                    if (int.Parse(firstPart[0]) > int.Parse(secondPart[0]))
                    {

                        if (int.Parse(secondPart[0]) != 0) thirdPart1[0] = Convert.ToString(int.Parse(secondPart[0]) - 1);
                        thirdPart2[0] = Convert.ToString(int.Parse(firstPart[0]) + 1);
                    }
                    else if (int.Parse(firstPart[0]) < int.Parse(secondPart[0]))
                    {
                        thirdPart1[0] = Convert.ToString(int.Parse(secondPart[0]) + 1);
                        if (int.Parse(firstPart[0]) != 0) thirdPart2[0] = Convert.ToString(int.Parse(firstPart[0]) - 1);
                    }
                }
                if (!(String.IsNullOrEmpty(thirdPart1[0]))&& board[int.Parse(thirdPart1[0]), int.Parse(thirdPart1[1])] != 'B')allowedCoords[0] = ReverseCoords(thirdPart1);
                if (!(String.IsNullOrEmpty(thirdPart2[0])) && board[int.Parse(thirdPart2[0]), int.Parse(thirdPart2[1])] != 'B')allowedCoords[1] = ReverseCoords(thirdPart2);
                foreach (string coord in allowedCoords) { if (!(String.IsNullOrEmpty(coord))) { Console.Write(coord + " "); } }

                testCoords = Console.ReadLine().ToUpper();
                if (Array.IndexOf(allowedCoords, testCoords) == -1)
                {
                    Console.WriteLine("Please choose a coordinate on one end.");
                    board[int.Parse(firstPart[1]), int.Parse(firstPart[0])] = '~';
                    board[int.Parse(secondPart[1]), int.Parse(secondPart[0])] = '~';
                    continue;
                }
                try { placeCoords = GetCoords(testCoords); }
                catch { Console.WriteLine("Invalid input."); }
                if (board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] == 'B')
                {
                    Console.WriteLine("Space already occupied.");
                    continue;
                }
                board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] = 'B';
                RenderBoard(board);

                Console.WriteLine("Is this right? (Y/N)");
                answer = Console.ReadLine().ToUpper();
                if (answer != "Y" && answer != "YES")
                {
                    if (answer != "N" || answer != "NO") Console.WriteLine("Invalid input");
                    board[int.Parse(placeCoords[0]), int.Parse(placeCoords[1])] = '~';
                    board[int.Parse(firstPart[1]), int.Parse(firstPart[0])] = '~';
                    board[int.Parse(secondPart[1]), int.Parse(secondPart[0])] = '~';
                    continue;
                }
                
                carrierPlaced = true;
                
            }
            return board;
        } 
        static char[,] PlaceComputerShips()//Generates locations for the computer's ships
        {
            //Each ship has a unique character (excluding destroyers) so that they can be distinguished from eachother when the program checks if they are sunk


            char[,] board = CreateBoard();
            Random random = new Random();
            
            int[] coords = new int[2];
            
            
            coords[0] = random.Next(1, 7);
            coords[1] = random.Next(1, 7);
            board[coords[0], coords[1]] = 'C';
            if (random.Next(2) == 0)
            {
                board[coords[0]-1, coords[1]] = 'C';
                board[coords[0]+1, coords[1]] = 'C';
            }
            else
            {
                board[coords[0], coords[1]-1] = 'C';
                board[coords[0], coords[1]+1] = 'C';
            }

            for (int i = 0; i < 2; i++)
            {
                char shipID = 'U';
                if (i == 1) shipID = 'P';
                do
                {
                    coords[0] = random.Next(7);
                    coords[1] = random.Next(7);
                }
                while (board[coords[0], coords[1]] == 'U'|| board[coords[0], coords[1]] == 'C' || board[coords[0], coords[1]] == 'P');
                board[coords[0], coords[1]] = shipID;
                if (random.Next(2) == 0)
                {
                    if (board[coords[0] + 1, coords[1]] == 'U'|| board[coords[0]+1, coords[1]] == 'C' || board[coords[0] + 1, coords[1]] == 'P') { i--; continue; }
                    board[coords[0] + 1, coords[1]] = shipID;
                }
                else
                {
                    if (board[coords[0], coords[1] + 1]== 'U' || board[coords[0], coords[1] + 1] == 'C' || board[coords[0], coords[1] + 1] == 'P') { i--;continue; }
                    board[coords[0], coords[1] + 1] = shipID;
                }
            }
            for(int i = 0; i < 2; i++)
            {
                do
                {
                    coords[0] = random.Next(8);
                    coords[1] = random.Next(8);

                    if (board[coords[0], coords[1]] != 'D') board[coords[0], coords[1]] = 'D';
                    else { i--;}
                }
                while (board[coords[0], coords[1]] == 'U' || board[coords[0], coords[1]] == 'C' || board[coords[0], coords[1]] == 'P');
            }

            return board;
        } 
        static void RenderBoard(char[,] gameboard) //Prints the inputted board - spaced and in colour
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" A B C D E F G H");
            for (int i = 0; i < gameboard.GetLength(0); i++)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(i + 1);
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Blue;
                for (int j = 0; j < gameboard.GetLength(1); j++)
                {
                    if (gameboard[i, j] == 'H' || gameboard[i, j] == 'S')
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.Write(gameboard[i, j]);
                        Console.BackgroundColor = ConsoleColor.Blue;
                    }
                    else
                    {
                        Console.Write(gameboard[i, j]);
                    }
                    if (j < gameboard.GetLength(1) - 1) Console.Write(" ");
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.Red;
        }
        static char[,] CreateBoard()//Creates an empty gameboard
        {

            char[,] board = new char[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                    board[i, j] = '~';
            }
            return board;
        } 
        static string[] GetCoords(string attack)//Converts inputted coordinates (e.g. "B6") to an array to be used as the indexes of a gameboard array
        {
            string[] coordOutput = new string[2];
            string[] coords = { "A", "B", "C", "D", "E", "F", "G", "H" };
            attack = attack.ToUpper();
            for (int i = 0; i < coords.Length; i++)
            {
                if (Convert.ToString(attack[0]) == coords[i]) //finds the position of the letter in the array to determine its value
                {
                    coordOutput[1] = Convert.ToString(i);
                    coordOutput[0] = Convert.ToString(int.Parse(Convert.ToString(attack[1])) - 1);
                }
            }
            if (int.Parse(coordOutput[0]) > 7 || int.Parse(coordOutput[1]) > 7)
            {
                coordOutput[0] = "";
                coordOutput[1] = "";
            }
            return coordOutput;
        } 
        static string[] OtherCoords(string[] coordInput)//Finds adjacent coordinates
        {
            string[] coordOutput = new string[4];
            string[] coords = { "A", "B", "C", "D", "E", "F", "G", "H" };
            Array.Reverse(coordInput);

            if (int.Parse(coordInput[0]) > 0) coordOutput[0] = (coords[Convert.ToInt32(coordInput[0]) - 1] + (int.Parse(coordInput[1]) + 1));
            if (int.Parse(coordInput[0]) < 7) coordOutput[1] = (coords[Convert.ToInt32(coordInput[0]) + 1] + (int.Parse(coordInput[1]) + 1));
            if (int.Parse(coordInput[1]) > 0) coordOutput[2] = (coords[Convert.ToInt32(coordInput[0])] + (int.Parse(coordInput[1])));
            if (int.Parse(coordInput[1]) < 7) coordOutput[3] = (coords[Convert.ToInt32(coordInput[0])] + (int.Parse(coordInput[1]) + 2));
            return coordOutput;
        } 
        static string ReverseCoords(string[] coordInput)//Converts the numbers used as indexes into coordinates (e.g. "B6")
        {
            string attack = "";
            string[] coords = { "A", "B", "C", "D", "E", "F", "G", "H" };
            attack += coords[int.Parse(coordInput[0])];
            attack += int.Parse(coordInput[1])+1;
            return attack;
        } 
    }
}
