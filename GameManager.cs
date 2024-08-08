using System.Text.Json;
using System.Text.Json.Serialization;

public static class GameManager
{
    /* 필드 */
    // 게임 종료 여부
    public static bool isExit = false;

    /* 메소드 */
    public static void SelectGame()
    {
        Console.WriteLine("      <<<  Game Select  >>>       ");
        Console.WriteLine("         1: notakto");
        Console.WriteLine("         2: gomoku");
        Console.Write("Select Game: ");
        string? input = Console.ReadLine();
        Console.Clear();

        switch (input)
        {
            case "1":
                Console.WriteLine("Notakto Game");
                new Game(new NotaktoRule()).Start();
                break;
            case "2":
                Console.WriteLine("Gomoku Game");
                new Game(new GomokuRule()).Start();
                break;
            default:
                Console.WriteLine("Invalid Input!!! Please retry.");
                break;
        }
    }

    public static Game? LoadGame()
    {
        if (File.Exists("./data/state.json") && File.Exists("./data/rule.json"))
        {
            string stateString = File.ReadAllText("./data/state.json");
            string ruleString = File.ReadAllText("./data/rule.json");
            string modeString = File.ReadAllText("./data/mode.json");
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            State? state = JsonSerializer.Deserialize<State>(stateString, options);
            string? ruleName = JsonSerializer.Deserialize<string>(ruleString, options);
            GameMode? mode = JsonSerializer.Deserialize<GameMode>(modeString, options);

            if (state == null || ruleName == null || mode == null)
            {
                Console.WriteLine("There is no saved data.");
                return null;
            }

            return new Game(Rule.GetRule(ruleName), state, mode);
        }
        else
        {
            Console.WriteLine("There is no saved data.");
            return null;
        }
    }
}