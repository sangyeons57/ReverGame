using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

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
    public Piece instanceSetting(float xPos, float yPos , float size, GameObject parent)
    {
        this.piece.transform.position = new Vector2(xPos, yPos);
        this.piece.transform.localScale = Vector3.one * size;

        this.pieceScript = this.piece.GetComponent<block>();
        this.pieceScript.piece = this;

        this.piece.transform.SetParent(parent.transform);
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
        Debug.Log($"playeriput {x}, {y}");
        if (status != Status.Void)
            return;

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

public class CounterClass
{
    public int Red = 0;
    public int Blue = 0;

    public void addRed()
    {
        Red++;
    }
    
    public void addBlue()
    {
        Blue++;
    }
}

public class PieceList
{
    private List<Piece> pieceList;
    private Piece focus;

    public Piece.Status playerStatus{get ; set;} 

    private int[,] direction = new int[8,2] { 
        {1, 1 }, {1, 0}, {1,-1},

        {0, 1 }, {0,-1}, //가운데는 빠짐

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

    public CounterClass checkAllPieceChanged()
    {
        CounterClass counter = new CounterClass();
        foreach(Piece piece in pieceList)
        {
            if (piece.getStatus() == Piece.Status.Blue)
                counter.addBlue();
            else if (piece.getStatus() == Piece.Status.Red)
                counter.addRed();
            else
                return null;
        }
        return counter;
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

    //direction으로 드러온 방향으로 status가 날올때까지 탐색 존재하면 ture, flip할 만한 적합한 상태가 아니면 flase
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

    //flipDirection 실행시켜주는놈
    public void fipDirectionAction()
    {
        for (int i = 0; i < direction.GetLength(0); i++)
            flipDirection(direction[i,0], direction[i,1]);
    }

    // 특정방향으로 flip가능한지 체크후flip가능하면 flip실행
    public void flipDirection(int directionX, int directionY)
    {
        if (searchDirection(directionX, directionY, focus ))
        {
            flip(directionX, directionY, focus);
            // 뒤집기가 가능한 장소였으면 플레이어가 둔것도 고정
            focus.changeStatus = focus.getStatus();
        }

    }

    //사용자가 놓은 점을setfocus하고 그다음 flip실행
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
        Camera.main.backgroundColor = Reverse.backgroundColor[playerStatus];
    }
}

public class Reverse : MonoBehaviour
{
    [SerializeField]
    private int size;

    [SerializeField]
    private GameObject block;

    [SerializeField]
    private GameObject parentOjbect;

    public static Dictionary<Piece.Status, Color> backgroundColor = new Dictionary<Piece.Status, Color>()
    {
        {Piece.Status.Void, new Color(50/255f, 50/255f, 50/255f) },
        {Piece.Status.Red, new Color(110/255f, 60/255f, 60/255f) },
        {Piece.Status.Blue, new Color(50/255f, 80/255f, 120/255f) },
    };

    PieceList pieceList;

    private float correctionPiecePos = 7f;
    const float pieceSize = 1.3f;


    private void Start()
    {
        //Screen.SetResolution(640, 360, false);

        correctionPiecePos = correctionPiecePos / size;
        float correctionHalfSize = pieceSize * size / 2 * correctionPiecePos;

        pieceList= new PieceList();

        Camera.main.backgroundColor = backgroundColor[pieceList.playerStatus];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                pieceList.Add(new Piece(x, y, block, pieceList))
                    .instanceSetting(x - correctionHalfSize, y - correctionHalfSize, correctionPiecePos, parentOjbect);
            }
        }

        pieceList.getPiece(4,4).setStatus(Piece.Status.Red).apply();
        pieceList.getPiece(4,5).setStatus(Piece.Status.Blue).apply();
        pieceList.getPiece(5,4).setStatus(Piece.Status.Blue).apply();
        pieceList.getPiece(5,5).setStatus(Piece.Status.Red).apply();
    }

    private void Update()
    {
        //턴 넘기기
        if(Input.GetKeyDown(KeyCode.Space))
        {
            pieceList.changePlayerStatus();
        }

        CounterClass counter = pieceList.checkAllPieceChanged(); 
        if(counter != null)
        {
            if(counter.Red > counter.Blue)
                Camera.main.backgroundColor=new Color(0,0,200/255f);
            else
                Camera.main.backgroundColor=new Color(200/255f,0,0);
        }

    }
}

