using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using PlasticGui.WorkspaceWindow.PendingChanges;

public class Piece
{
    public enum Status
    {
        Void,
        Red,
        Blue
    }


    public readonly int x;
    public readonly int y;
    private Status status;
    public Status changeStatus { get; set; }

    public Status getStatus() 
    {
        return status; 
    }

    public Piece setStatus(Status status)
    {
        this.status = status;
        return this;
    }


    GameObject piece;
    block pieceScript;
    PieceList pieceList;

    public Piece(int x, int y, Status status)
    {
        this.x = x;
        this.y = y;
        this.status = status;
    }

    public Piece(int x, int y, GameObject piece, PieceList pieceList, Status status = Status.Void)
        : this(x, y, status)
    {

        this.piece = MonoBehaviour.Instantiate(piece);
        this.pieceList = pieceList;
    }
    public Piece instanceSetting(float xPos, float yPos , float size)
    {
        this.piece.transform.position = new Vector2(xPos, yPos);
        this.piece.transform.localScale = Vector3.one * size;

        this.pieceScript = this.piece.GetComponent<block>();
        this.pieceScript.piece = this;
        return this;
    }

    public void apply()
    {
        pieceScript.changesetColorMode(status);
    }

    public bool cehckXY(int x, int y)
    {
        return this.x== x && this.y == y;
    }

    public void playerInput()
    {
        this.changeStatus = this.status;
        this.status = pieceList.playerStatus;
        pieceList.setFocus(this)
            .fipDirectionAction();

        if (this.status == this.changeStatus)
            pieceList.changePlayerStatus();
        else 
            this.status = this.changeStatus;
    }
}

public class PieceList
{
    private List<Piece> pieceList;
    private Piece focus;

    public Piece.Status playerStatus{get ; set;} 

    private int[,] direction = new int[8,2] { 
        {1, 1 }, {1, 0}, {1,-1},

        {0, 1 }, {0,-1}, //����� ����

        {-1, 1 }, {-1, 0}, {-1,-1}, 
    };

    public PieceList ()
    {
        this.pieceList = new List<Piece>();
        this.playerStatus = Piece.Status.Blue; 
    }

    public Piece Add (Piece piece)
    {
        pieceList.Add(piece);
        return piece;
    }

    public Piece getPiece(int x, int y)
    {
        foreach(Piece piece in pieceList)
            if (piece.cehckXY(x, y))
                return piece;
        return null;
    }

    public PieceList setFocus(Piece focus)
    {
        this.focus = focus;
        return this; 
    }
    public Piece getFocus()
    {
        return focus;
    }

    //direction���� �巯�� �������� status�� ���ö����� Ž�� �����ϸ� ture, flip�� ���� ������ ���°� �ƴϸ� flase
    public bool searchDirection(int directionX, int directionY, Piece piece)
    {
        piece = getPiece(directionX + piece.x, directionY + piece.y);

        //Debug.Log($"{focus.getStatus()} - {piece.getStatus()} ({piece.x} , {piece.y}) ");
        if (piece == null || piece.getStatus() == Piece.Status.Void ||
            (playerStatus == piece.getStatus()&&
            piece.x - focus.x == directionX&&
            piece.y - focus.y == directionY))
            return false;
        else if (playerStatus == piece.getStatus())
            return true;
        else
            return searchDirection(directionX, directionY, piece);
    }

    //flipDirection ��������ִ³�
    public void fipDirectionAction()
    {
        for (int i = 0; i < direction.GetLength(0); i++)
            flipDirection(direction[i,0], direction[i,1]);
    }

    // Ư���������� flip�������� üũ��flip�����ϸ� flip����
    public void flipDirection(int directionX, int directionY)
    {
        if (searchDirection(directionX, directionY, focus ))
        {
            flip(directionX, directionY, focus);
            // �����Ⱑ ������ ��ҿ����� �÷��̾ �а͵� ����
            focus.changeStatus = focus.getStatus();
        }

    }

    //����ڰ� ���� ����setfocus�ϰ� �״��� flip����
    public PieceList flip(int directionX, int directionY, Piece piece)
    {
        if (focus.getStatus() == piece.getStatus() && piece != focus)
            return this;
        piece.setStatus(focus.getStatus()).apply();
        return flip(directionX, directionY, getPiece(piece.x + directionX, piece.y + directionY));
    }

    public void changePlayerStatus()
    {
        playerStatus = (playerStatus == Piece.Status.Blue) ? Piece.Status.Red : Piece.Status.Blue;
    }
}

public class Reverse : MonoBehaviour
{
    [SerializeField]
    private int size;

    [SerializeField]
    private GameObject block;

    PieceList pieceList;

    private float correctionPiecePos = 7f;
    const float pieceSize = 1.3f;

    private void Start()
    {
        correctionPiecePos = correctionPiecePos / size;
        float correctionHalfSize = pieceSize * size / 2 * correctionPiecePos;

        pieceList= new PieceList();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                pieceList.Add(new Piece(x, y, block, pieceList))
                    .instanceSetting(x - correctionHalfSize,y - correctionHalfSize, correctionPiecePos);
            }
        }

        pieceList.getPiece(4,4).setStatus(Piece.Status.Red).apply();
        pieceList.getPiece(4,5).setStatus(Piece.Status.Blue).apply();
        pieceList.getPiece(5,4).setStatus(Piece.Status.Blue).apply();
        pieceList.getPiece(5,5).setStatus(Piece.Status.Red).apply();
    }
}

