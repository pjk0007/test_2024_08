public abstract class Rule
{
    public Rule(string ruleName, string[] stoneMarks, int numberOfBoards, int size)
    {
        RuleName = ruleName;
        StoneMarks = stoneMarks;
        NumberOfBoards = numberOfBoards;
        Size = size;
    }
    public string RuleName { get; set; }
    public string[] StoneMarks { get; set; }
    public int NumberOfBoards { get; set; }
    public int Size { get; set; }


    public string GetStoneMark(bool isFirstPlayerTurn)
    {
        return isFirstPlayerTurn ? StoneMarks[0] : StoneMarks[1];
    }

    public static Rule GetRule(string ruleName)
    {
        switch (ruleName)
        {
            case "notakto":
                return new NotaktoRule();
            case "gomoku":
                return new GomokuRule();
            default:
                return new NotaktoRule();
        }
    }


    public static bool IsValidNumber(State state, int boardIndex, int x, int y)
    {
        bool validNumber = x >= 1 && x <= state.Size && y >= 1 && y <= state.Size && boardIndex >= 1 && boardIndex <= state.NumberOfBoards;
        return validNumber;
    }

    public static bool IsEmpty(State state, int boardIndex, int x, int y)
    {
        bool isEmpty = state.Boards[(boardIndex - 1) * state.Size * state.Size + (x - 1) * state.Size + (y - 1)] == " ";
        return isEmpty;
    }


    /* 추상 메소드 */
    public abstract bool IsValidPlace(State state, int boardIndex, int x, int y);

    public abstract bool IsGameOver(State state);
    public abstract bool GetWinner(State state);
}

public class NotaktoRule : Rule
{

    public NotaktoRule() : base("notakto", new string[] { "X", "X" }, 3, 3)
    { }

    public override bool IsValidPlace(State state, int boardIndex, int x, int y)
    {
        bool validNumber = IsValidNumber(state, boardIndex, x, y);
        bool isEmpty = IsEmpty(state, boardIndex, x, y);
        int disabledBoard = 0;

        // 가로줄 검사
        for (int i = 0; i < Size; i++)
        {
            if (state.Boards[(boardIndex - 1) * Size * Size + i * Size] == "X" && state.Boards[(boardIndex - 1) * Size * Size + i * Size + 1] == "X" && state.Boards[(boardIndex - 1) * Size * Size + i * Size + 2] == "X")
            {
                disabledBoard = 1;
            }
        }

        // 세로줄 검사
        for (int i = 0; i < Size; i++)
        {
            if (state.Boards[(boardIndex - 1) * Size * Size + i] == "X" && state.Boards[(boardIndex - 1) * Size * Size + Size + i] == "X" && state.Boards[(boardIndex - 1) * Size * Size + 2 * Size + i] == "X")
            {
                disabledBoard = 1;
            }
        }

        // 대각선 검사
        if (state.Boards[(boardIndex - 1) * Size * Size] == "X" && state.Boards[(boardIndex - 1) * Size * Size + Size + 1] == "X" && state.Boards[(boardIndex - 1) * Size * Size + 2 * Size + 2] == "X")
        {
            disabledBoard = 1;
        }

        return validNumber && isEmpty && disabledBoard == 0;
    }

    // 모든 보드에 3줄이 만들어지면 게임 종료
    public override bool IsGameOver(State state)
    {
        string[] boards = state.Boards;
        int numberOfBoards = state.NumberOfBoards;
        int size = state.Size;
        // 각 보드에 3줄이 만들어지면 1
        int[] disabledBoard = { 0, 0, 0 };

        for (int b = 0; b < numberOfBoards; b++)
        {
            // 가로줄 검사
            for (int i = 0; i < size; i++)
            {
                if (boards[b * size * size + i * size] == "X" && boards[b * size * size + i * size + 1] == "X" && boards[b * size * size + i * size + 2] == "X")
                {
                    disabledBoard[b] = 1;
                }
            }

            // 세로줄 검사
            for (int i = 0; i < size; i++)
            {
                if (boards[b * size * size + i] == "X" && boards[b * size * size + size + i] == "X" && boards[b * size * size + 2 * size + i] == "X")
                {
                    disabledBoard[b] = 1;
                }
            }

            // 대각선 검사
            if (boards[b * size * size] == "X" && boards[b * size * size + size + 1] == "X" && boards[b * size * size + 2 * size + 2] == "X")
            {
                disabledBoard[b] = 1;
            }
            if (boards[b * size * size + 2] == "X" && boards[b * size * size + size + 1] == "X" && boards[b * size * size + 2 * size] == "X")
            {
                disabledBoard[b] = 1;
            }
        }


        return disabledBoard[0] == 1 && disabledBoard[1] == 1 && disabledBoard[2] == 1;
    }

    public override bool GetWinner(State state)
    {
        return state.IsFirstPlayerTurn;
    }
}

public class GomokuRule : Rule
{
    public GomokuRule() : base("gomoku", new string[] { "O", "X" }, 1, 10)
    {
    }

    // 한개의 보드라도 5줄이 같은문자로 만들어지면 게임 종료
    public override bool IsGameOver(State state)
    {
        string[] boards = state.Boards;
        int size = state.Size;

        // 가로줄 검사
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size - 4; j++)
            {
                if (boards[i * size + j] == "O" && boards[i * size + j + 1] == "O" && boards[i * size + j + 2] == "O" && boards[i * size + j + 3] == "O" && boards[i * size + j + 4] == "O")
                {
                    return true;
                }
                if (boards[i * size + j] == "X" && boards[i * size + j + 1] == "X" && boards[i * size + j + 2] == "X" && boards[i * size + j + 3] == "X" && boards[i * size + j + 4] == "X")
                {
                    return true;
                }
            }
        }

        // 세로줄 검사
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size - 4; j++)
            {
                if (boards[j * size + i] == "O" && boards[(j + 1) * size + i] == "O" && boards[(j + 2) * size + i] == "O" && boards[(j + 3) * size + i] == "O" && boards[(j + 4) * size + i] == "O")
                {
                    return true;
                }
                if (boards[j * size + i] == "X" && boards[(j + 1) * size + i] == "X" && boards[(j + 2) * size + i] == "X" && boards[(j + 3) * size + i] == "X" && boards[(j + 4) * size + i] == "X")
                {
                    return true;
                }
            }
        }

        // 대각선 검사
        for (int i = 0; i < size - 4; i++)
        {
            for (int j = 0; j < size - 4; j++)
            {
                if (boards[i * size + j] == "O" && boards[(i + 1) * size + j + 1] == "O" && boards[(i + 2) * size + j + 2] == "O" && boards[(i + 3) * size + j + 3] == "O" && boards[(i + 4) * size + j + 4] == "O")
                {
                    return true;
                }
                if (boards[i * size + j] == "X" && boards[(i + 1) * size + j + 1] == "X" && boards[(i + 2) * size + j + 2] == "X" && boards[(i + 3) * size + j + 3] == "X" && boards[(i + 4) * size + j + 4] == "X")
                {
                    return true;
                }
            }
        }
        for (int i = 0; i < size - 4; i++)
        {
            for (int j = 4; j < size; j++)
            {
                if (boards[i * size + j] == "O" && boards[(i + 1) * size + j - 1] == "O" && boards[(i + 2) * size + j - 2] == "O" && boards[(i + 3) * size + j - 3] == "O" && boards[(i + 4) * size + j - 4] == "O")
                {
                    return true;
                }
                if (boards[i * size + j] == "X" && boards[(i + 1) * size + j - 1] == "X" && boards[(i + 2) * size + j - 2] == "X" && boards[(i + 3) * size + j - 3] == "X" && boards[(i + 4) * size + j - 4] == "X")
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override bool IsValidPlace(State state, int boardIndex, int x, int y)
    {
        bool validNumber = IsValidNumber(state, boardIndex, x, y);
        bool isEmpty = IsEmpty(state, boardIndex, x, y);
        return validNumber && isEmpty;
    }

    public override bool GetWinner(State state)
    {
        return !state.IsFirstPlayerTurn;
    }
}

