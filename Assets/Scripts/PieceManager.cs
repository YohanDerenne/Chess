﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceManager : MonoBehaviour
{
    [HideInInspector]
    public bool isKingAlive;

    [HideInInspector]
    public bool isWhiteTurn;

    public ClockManager clockManager;

    public GameObject piecePrefab;

    private List<BasePiece> whitePieces = null;
    private List<BasePiece> blackPieces = null;

    private string[] pieceOrder = { "P", "P", "P", "P", "P", "P", "P", "P",
        "R", "KN", "B", "K", "Q", "B", "KN", "R" };


    private Dictionary<string, Type> pieceDico = new Dictionary<string, Type>()
    {
        {"P", typeof(Pawn)},
        {"R", typeof(Rook)},
        {"KN", typeof(Knight)},
        {"B", typeof(Bishop)},
        {"K", typeof(King)},
        {"Q", typeof(Queen)}
    };

    private Dictionary<string, int> coordA = new Dictionary<string, int>()
    {
        {"a", 0},
        {"b", 1},
        {"c", 2},
        {"d", 3},
        {"e", 4},
        {"f", 5},
        {"g", 6},
        {"h", 7}
    };

    private Dictionary<string, int> coordB = new Dictionary<string, int>()
    {
        {"1", 0},
        {"2", 1},
        {"3", 2},
        {"4", 3},
        {"5", 4},
        {"6", 5},
        {"7", 6},
        {"8", 7}
    };

    public void Setup(Board board)
    {
        isKingAlive = true;

        whitePieces = CreatePieces(true, board);
        blackPieces = CreatePieces(false, board);

        PlacePieces("2", "1", whitePieces, board);
        PlacePieces("7", "8", blackPieces, board);

        SetColor(blackPieces, Color.grey);
        SetTurn(true);
    }


    private List<BasePiece> CreatePieces(bool isWhite, Board board)
    {
        List<BasePiece> pieceList = new List<BasePiece>();

        float board_width = board.GetComponent<RectTransform>().rect.width;
        float board_height = board.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < pieceOrder.Length; i++)
        {
            GameObject newPieceObject = Instantiate(piecePrefab);
            newPieceObject.transform.SetParent(transform);

            newPieceObject.transform.localScale = new Vector3(1, 1, 1);
            newPieceObject.transform.localRotation = Quaternion.identity;

            
            float piece_width = board_width / board.Column - BasePiece.CellPadding;
            float piece_height = board_height / board.Row - BasePiece.CellPadding;
            newPieceObject.GetComponent<RectTransform>().sizeDelta = new Vector2(piece_width, piece_height);
            

            string key = pieceOrder[i];
            Type pieceType = pieceDico[key];

            BasePiece newPiece = (BasePiece)newPieceObject.AddComponent(pieceType);
            pieceList.Add(newPiece);

            newPiece.Setup(isWhite, this);
        }

        return pieceList;
    }

    private void PlacePieces(string pawnRow, string royaltyRow, List<BasePiece> pieces, Board board)
    {
        for (int i = 0; i < board.Column; i++)
        {
            pieces[i].Place(board.allCells[i][coordB[pawnRow]]);
            pieces[i + 8].Place(board.allCells[i][coordB[royaltyRow]]);
        }
    }

    private void SetInteractive(List<BasePiece> pieces, bool state)
    {
        foreach(BasePiece piece in pieces)
        {
            piece.enabled = state;
        }
    }

    public void SetTurn(bool isWhiteTurn)
    {
        if (isKingAlive == false)
            return;

        SetInteractive(whitePieces, isWhiteTurn);
        SetInteractive(blackPieces, !isWhiteTurn);

        clockManager.setTurn(isWhiteTurn);
    }

    public void SetColor(List<BasePiece> pieces, Color col)
    {
        foreach(BasePiece piece in pieces)
        {
            piece.GetComponent<Image>().color = col;
        }
    }

    public void PawnPromotion(Pawn pawn, Cell promotionCell)
    {
        promotionCell.RemovePiece();
        GameObject newPieceObject = Instantiate(piecePrefab);
        newPieceObject.transform.SetParent(transform);

        newPieceObject.transform.localScale = new Vector3(1, 1, 1);
        newPieceObject.transform.localRotation = Quaternion.identity;

        float board_width = promotionCell.board.GetComponent<RectTransform>().rect.width;
        float board_height = promotionCell.board.GetComponent<RectTransform>().rect.height;

        float piece_width = board_width / promotionCell.board.Column - BasePiece.CellPadding;
        float piece_height = board_height / promotionCell.board.Row - BasePiece.CellPadding;
        newPieceObject.GetComponent<RectTransform>().sizeDelta = new Vector2(piece_width, piece_height);

        Queen queen = (Queen)newPieceObject.AddComponent(typeof(Queen));
        //base.pieceManager.pieceList.Add(newPiece);

        queen.Setup(pawn.isWhite, this);
        queen.Place(promotionCell);
        if (pawn.isWhite)
        {
            whitePieces.Add(queen);
        } else
        {
            blackPieces.Add(queen);
            queen.GetComponent<Image>().color = Color.grey;
        }
        queen.gameObject.SetActive(true);

    }
}


