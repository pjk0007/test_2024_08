public class State
{
    public bool IsFirstPlayerTurn { get; set; }
    public int NumberOfBoards { get; set; }
    public int Size { get; set; }
    public String[] Boards { get; set; }

    // Constructor
    public State(bool isFirstPlayerTurn, int numberOfBoards, int size)
    {
        // 처음 플레이어 턴
        IsFirstPlayerTurn = isFirstPlayerTurn;

        // 보드 개수
        NumberOfBoards = numberOfBoards;

        // 보드 크기
        Size = size;

        // 보드 생성 및 초기화
        Boards = new string[numberOfBoards * size * size];
        for (int i = 0; i < numberOfBoards * size * size; i++)
        {
            Boards[i] = " ";
        }
    }

    // 상태 복사하여 반환 (undo, redo 할 때 사용)
    public State Copy()
    {
        State newState = new State(IsFirstPlayerTurn, NumberOfBoards, Size);
        for (int i = 0; i < NumberOfBoards * Size * Size; i++)
        {
            newState.Boards[i] = Boards[i];
        }
        return newState;
    }


    // 돌 놓기
    public void PlaceStone(int boardIndex, int x, int y, string stone)
    {

        Boards[(boardIndex - 1) * Size * Size + (x - 1) * Size + (y - 1)] = stone;
    }

    // 보드 출력
    public void PrintBoard()
    {
        Console.WriteLine();
        for (int i = 0; i < Size; i++)
        {
            for (int b = 0; b < NumberOfBoards; b++)
            {
                for (int j = 0; j < Size; j++)
                {
                    int index = b * Size * Size + i * Size + j;
                    Console.Write($" {Boards[index]} ");
                    if (j < Size - 1) Console.Write("|");
                }
                if (b < NumberOfBoards - 1) Console.Write("   "); // 보드 사이의 공백
            }
            Console.WriteLine();
            if (i < Size - 1)
            {
                for (int b = 0; b < NumberOfBoards; b++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        Console.Write("---");
                        if (j < Size - 1) Console.Write("+");
                    }
                    if (b < NumberOfBoards - 1) Console.Write("   "); // 보드 사이의 공백
                }
                Console.WriteLine();
            }
        }
    }

    // 컴퓨터 플레이
    public void ComputerPlaceStone(Rule rule)
    {
        Console.WriteLine("Computer is thinking...");
        List<int> availableIndex = new List<int>();

        for (int i = 0; i < NumberOfBoards; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                for (int k = 0; k < Size; k++)
                {
                    if (rule.IsValidPlace(this, i + 1, j + 1, k + 1))
                    {
                        availableIndex.Add(i * Size * Size + j * Size + k);
                    }
                }
            }
        }

        Random random = new Random();
        int randomIndex = random.Next(availableIndex.Count);

        Boards[availableIndex[randomIndex]] = rule.GetStoneMark(IsFirstPlayerTurn);
    }
}