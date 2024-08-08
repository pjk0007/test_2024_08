Console.Clear();
while (!GameManager.isExit)
{
    Console.WriteLine("      <<<  Main Menu  >>>       ");
    Console.WriteLine("         1: New Game");
    Console.WriteLine("         2: Load Game");
    Console.WriteLine("         exit: Exit Program");

    Console.Write("Select Menu: ");
    string? input = Console.ReadLine();
    Console.Clear();

    if (input == "exit")
    {
        break;
    }

    switch (input)
    {
        case "1":
            GameManager.SelectGame();
            break;
        case "2":
            Game? game = GameManager.LoadGame();
            game?.Start();
            break;
        default:
            Console.WriteLine("Invalid Input!!! Please retry.");
            break;
    }

}