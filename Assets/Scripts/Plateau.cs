

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType { NONE, RED, GREEN }

public struct GridPos { public int row, col; }

public class Plateau
{
    PlayerType[][] playerBoard;
    GridPos currentPos;

    // ponderations 

    int ponderationGagnante = 21;
    int ponderationPerdante = -20; 
    int ponderationComb = 100;
    int scorePour3 = 100;
    int scorePour4 = 5000;

    public Plateau()
    {
        playerBoard = new PlayerType[6][];
        for (int i = 0; i < playerBoard.Length; i++)
        {
            playerBoard[i] = new PlayerType[7];
            for (int j = 0; j < playerBoard[i].Length; j++)
            {
                playerBoard[i][j] = PlayerType.NONE;
            }
        }
    }

    public void UpdateBoard(int col, bool isPlayer)
    {
        int updatePos = 6;
        for (int i = 5; i >= 0; i--)
        {
            if (playerBoard[i][col] == PlayerType.NONE)
            {
                updatePos--;
            }
            else
            {
                break;
            }
        }

        playerBoard[updatePos][col] = isPlayer ? PlayerType.RED : PlayerType.GREEN;
        currentPos = new GridPos { row = updatePos, col = col };
    }

    public bool Result(bool isPlayer)
    {
        PlayerType current = isPlayer ? PlayerType.RED : PlayerType.GREEN;
        return IsHorizontal(current) || IsVertical(current) || IsDiagonal(current) || IsReverseDiagonal(current);
    }

    bool IsHorizontal(PlayerType current)
    {
        GridPos start = GetEndPoint(new GridPos { row = 0, col = -1 });
        List<GridPos> toSearchList = GetPlayerList(start, new GridPos { row = 0, col = 1 });
        return SearchResult(toSearchList, current);
    }

    bool IsVertical(PlayerType current)
    {
        GridPos start = GetEndPoint(new GridPos { row = -1, col = 0 });
        List<GridPos> toSearchList = GetPlayerList(start, new GridPos { row = 1, col = 0 });
        return SearchResult(toSearchList, current);
    }

    bool IsDiagonal(PlayerType current)
    {
        GridPos start = GetEndPoint(new GridPos { row = -1, col = -1 });
        List<GridPos> toSearchList = GetPlayerList(start, new GridPos { row = 1, col = 1 });
        return SearchResult(toSearchList, current);
    }

    bool IsReverseDiagonal(PlayerType current)
    {
        GridPos start = GetEndPoint(new GridPos { row = -1, col = 1 });
        List<GridPos> toSearchList = GetPlayerList(start, new GridPos { row = 1, col = -1 });
        return SearchResult(toSearchList, current);
    }

    GridPos GetEndPoint(GridPos diff)
    {
        GridPos result = new GridPos { row = currentPos.row, col = currentPos.col };
        while (result.row + diff.row < 6 &&
                result.col + diff.col < 7 &&
                result.row + diff.row >= 0 &&
                result.col + diff.col >= 0)
        {
            result.row += diff.row;
            result.col += diff.col;
        }
        return result;
    }

    List<GridPos> GetPlayerList(GridPos start, GridPos diff)
    {
        List<GridPos> resList;
        resList = new List<GridPos> { start };
        GridPos result = new GridPos { row = start.row, col = start.col };
        while (result.row + diff.row < 6 &&
                result.col + diff.col < 7 &&
                result.row + diff.row >= 0 &&
                result.col + diff.col >= 0)
        {
            result.row += diff.row;
            result.col += diff.col;
            resList.Add(result);
        }

        return resList;
    }

    bool SearchResult(List<GridPos> searchList, PlayerType current)
    {
        int counter = 0;

        for (int i = 0; i < searchList.Count; i++)
        {
            PlayerType compare = playerBoard[searchList[i].row][searchList[i].col];
            if (compare == current)
            {
                counter++;
                if (counter == 4)
                    break;
            }
            else
            {
                counter = 0;
            }
        }

        return counter >= 4;
    }



// ----------------------------------------------


public int MinMax(Plateau p, int depth, bool isMaximizing)
{
    if (depth == 0 || p.Result(isMaximizing))
    {
        return p.Evaluate();
    }

    int bestValue = isMaximizing ? int.MinValue : int.MaxValue;
    for (int i = 0; i < 7; i++)
    {
        if (p.IsColumnNotFull(i))
        {
            Plateau newP = p.Clone();
            newP.UpdateBoard(i, isMaximizing);
            int value = MinMax(newP, depth - 1, !isMaximizing);
            bestValue = isMaximizing ? Mathf.Max(bestValue, value) : Mathf.Min(bestValue, value);
        }
    }

    return bestValue;
}

public bool IsColumnNotFull(int col)
{
    return playerBoard[5][col] == PlayerType.NONE;
}

public int GetBestMove(Plateau p, int depth)
{
    int bestValue = int.MinValue;
    int bestMove = 0;
    for (int i = 0; i < 7; i++)
    {
        if (p.IsColumnNotFull(i))
        {
            Plateau newP = p.Clone();
            newP.UpdateBoard(i, true);
            int value = MinMax(newP, depth - 1, false);
            if (value > bestValue)
            {
                bestValue = value;
                bestMove = i;
            }
        }
    }

    return bestMove;
}

public int Evaluate()
{   
    int value = 0;
    for (int i = 0; i < 6; i++)
    {
        for (int j = 0; j < 7; j++)
        {
            if (playerBoard[i][j] != PlayerType.NONE)
            {
                value += EvaluateLine(i, j, 1, 0); // horizontal
                value += EvaluateLine(i, j, 0, 1); // vertical
                value += EvaluateLine(i, j, 1, 1); // diagonale
                value += EvaluateLine(i, j, -1, 1); //  diagonale inversée
                
            }
        }
    }

    return value;
}

private int EvaluateLine(int row, int col, int rowDiff, int colDiff)
{
    PlayerType player = playerBoard[row][col];    int counter = 0;
    int ponderationGagnante = 20;
    int ponderationPerdante = -21; 
    int ponderationComb = 100;
    int scorePour2 = 10;
    int scorePour3 = 100;
    int scorePour4 = 3000;
    for (int i = 0; i < 4; i++)
    {
        int r = row + i * rowDiff;
        int c = col + i * colDiff;
        if (r >= 0 && r < 6 && c >= 0 && c < 7 && playerBoard[r][c] == player)
        {
            counter++;
        }
        else
        {
            break;
        }
    }

    
    int value = 0;
    if (counter == 4)
    {
        value = scorePour4 * (player == PlayerType.RED ? ponderationGagnante : ponderationPerdante);
    }
    else
    {   
        int score = 0;
        if (counter == 3)
        {
            score = scorePour3;
        }
        value = score * (player == PlayerType.RED ? ponderationGagnante : ponderationPerdante);
    }

    return value;
}

public Plateau Clone()
{
    Plateau newP = new Plateau();
    for (int i = 0 ; i < 6; i++)
    {
        for (int j = 0; j < 7; j++)
        {
            newP.playerBoard[i][j] = playerBoard[i][j];
        }
    }
    return newP;
}
}
