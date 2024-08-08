using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

public enum GameMode
{
    HumanVsHuman,
    HumanVsComputer,
    ComputerVsHuman,
}

public class Game
{

    // Constructor
    public Game(Rule rule, State? currentState = null, GameMode? mode = null)
    {
        Rule = rule;

        if (currentState == null || mode == null)
        {
            CurrentState = new State(true, rule.NumberOfBoards, rule.Size);

            Console.WriteLine("1. Human vs Human");
            Console.WriteLine("2. Human vs Computer");
            Console.WriteLine("3. Computer vs Human");
            Console.Write("Select Mode: ");
            string? input = Console.ReadLine();
            Console.Clear();

            switch (input)
            {
                case "1":
                    Mode = GameMode.HumanVsHuman;
                    break;
                case "2":
                    Mode = GameMode.HumanVsComputer;
                    break;
                case "3":
                    Mode = GameMode.ComputerVsHuman;
                    break;
                default:
                    Console.WriteLine("Invalid Input!!! Please retry.");
                    break;
            }
        }
        else
        {
            CurrentState = currentState;
            Mode = mode.Value;
        }

    }

    public GameMode Mode { get; set; }
    public Stack<State> UndoStack { get; set; } = new Stack<State>();
    public Stack<State> RedoStack { get; set; } = new Stack<State>();
    public State CurrentState { get; set; }
    public Rule Rule { get; set; }
    public void Start()
    {
        while (true)
        {
            CurrentState.PrintBoard();
            if (Rule.IsGameOver(CurrentState))
            {
                string winner = GetPlayerName(Rule.GetWinner(CurrentState));
                Console.WriteLine(winner + " is the winner!!!");
                Console.WriteLine();
                break;
            }
            string currentTurn = GetPlayerName(CurrentState.IsFirstPlayerTurn);

            if (currentTurn == "Computer")
            {
                CurrentState.ComputerPlaceStone(Rule);
                CurrentState.IsFirstPlayerTurn = !CurrentState.IsFirstPlayerTurn;
                continue;
            }

            Console.Write(currentTurn + "(\"help\" for command list): ");
            string? input = Console.ReadLine();
            Console.Clear();

            if (input == "help")
            {
                CommandInfo();
                continue;
            }
            else if (input == "save")
            {
                Save();
                continue;
            }
            else if (input == "exit")
            {
                break;
            }
            else if (input == "undo")
            {
                Undo();
                continue;
            }
            else if (input == "redo")
            {
                Redo();
                continue;
            }

            string[] inputs = input!.Split(" ");

            // 커맨드가 place가 아니거나 인자가 4개가 아닌 경우
            if (inputs.Length != 4 || inputs[0] != "place")
            {
                Console.WriteLine("Input is not valid!!! Please retry.");
                continue;
            }

            // place [board number] [x] [y]
            int boardIndex;
            int x;
            int y;

            // 인자가 정수가 아닌 경우 체크
            try
            {
                boardIndex = int.Parse(inputs[1]);
                x = int.Parse(inputs[2]);
                y = int.Parse(inputs[3]);
            }
            catch (System.Exception)
            {
                Console.WriteLine("Invalid Input!!! Please retry.");
                continue;
            }

            // place가 가능한지 체크
            if (Rule.IsValidPlace(CurrentState, boardIndex, x, y))
            {
                UndoStack.Push(CurrentState.Copy());
                CurrentState.PlaceStone(boardIndex, x, y, Rule.GetStoneMark(CurrentState.IsFirstPlayerTurn));
                CurrentState.IsFirstPlayerTurn = !CurrentState.IsFirstPlayerTurn;
                RedoStack.Clear();

            }
            else
            {
                Console.WriteLine("Invalid Input!!! Please retry.");
            }
        }
    }

    public static void CommandInfo()
    {
        Console.WriteLine("      <<<   Command List   >>>");
        Console.WriteLine("         place [board number] [x] [y]");
        Console.WriteLine("         save");
        Console.WriteLine("         undo");
        Console.WriteLine("         redo");
        Console.WriteLine("         exit");
    }

    // 현재 턴의 플레이어 이름 반환
    public string GetPlayerName(bool isFirstPlayerTurn)
    {
        if (Mode == GameMode.HumanVsHuman)
        {
            return isFirstPlayerTurn ? "Player-1" : "Player-2";
        }
        else if (Mode == GameMode.HumanVsComputer)
        {
            return isFirstPlayerTurn ? "Player" : "Computer";
        }
        else
        {
            return isFirstPlayerTurn ? "Computer" : "Player";
        }
    }

    // 현재 상태 저장
    public void Save()
    {
        // Reset RedoStack, UndoStack
        RedoStack.Clear();
        UndoStack.Clear();

        // Save current state
        if (!Directory.Exists("./data"))
        {
            Directory.CreateDirectory("./data");
        }

        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve };
            string stateString = JsonSerializer.Serialize(CurrentState, options);
            File.WriteAllText("./data/state.json", stateString);

            string ruleString = JsonSerializer.Serialize(Rule.RuleName, options);
            File.WriteAllText("./data/rule.json", ruleString);

            string modeString = JsonSerializer.Serialize(Mode, options);
            File.WriteAllText("./data/mode.json", modeString);
            Console.WriteLine("Game is saved.");
        }
        catch (System.Exception)
        {
            Console.WriteLine("Failed to save the game.");
        }


    }

    // Undo
    public void Undo()
    {
        // UndoStack이 비어있지 않은 경우
        if (UndoStack.Count > 0)
        {
            RedoStack.Push(CurrentState.Copy());
            CurrentState = UndoStack.Pop();
            if (Mode == GameMode.HumanVsHuman)
            {
                CurrentState.IsFirstPlayerTurn = !CurrentState.IsFirstPlayerTurn;
            }

        }
        else
        {
            Console.WriteLine("Undo is not available.");
        }
    }

    // Redo
    public void Redo()
    {
        // RedoStack이 비어있지 않은 경우
        if (RedoStack.Count > 0)
        {
            UndoStack.Push(CurrentState.Copy());
            CurrentState = RedoStack.Pop();
            if (Mode == GameMode.HumanVsHuman)
            {
                CurrentState.IsFirstPlayerTurn = !CurrentState.IsFirstPlayerTurn;
            }
        }
        else
        {
            Console.WriteLine("Redo is not available.");
        }
    }

}
