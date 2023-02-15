using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Reverse : MonoBehaviour
{

    public static readonly int PLAYER1 = 1;
    public static readonly int PLAYER2 = 2;

    [SerializeField]
    private int size;

    [SerializeField]
    private GameObject block;

    private const float block_size = 1.3f;
    private float correction_size = 7f;

    private float half_size;

    //0-�÷��̾�1 1-�÷�����2
    private static Cell.State player_turn;

    private LineList line_list;

    // Start is called before the first frame update
    void Start()
    {
        player_turn = Cell.State.AbleBlue;


        line_list = new LineList();

        //generate row line
        for (int i = 0; i < size; i++)
        {
            List<Cell> cells = new List<Cell>();
            for ( int j = 0;j < size; j++ )
                cells.Add(new Cell(j,i,i));
            line_list.add(new Line(cells));

            List<Cell> cells_B = new List<Cell>();
            for ( int j = size-1; j >= 0; j--)
                cells_B.Add(new Cell(j,i,size - 1 - j));
            line_list.add(new Line(cells_B));
        }

        //generate colum line
        for (int i = 0; i < size; i++)
        {
            List<Cell> cells = new List<Cell>();
            for (int j = 0; j < size; j++)
                cells.Add(new Cell(i, j, j));
            line_list.add(new Line(cells));

            List<Cell> cells_B = new List<Cell>();
            for ( int j = size -1; j >= 0; j--)
                cells_B.Add(new Cell(i,j, size -1 -j));
            line_list.add(new Line(cells_B));
        }

        //diagnoal  left down -> right up
        for (int i = 0; i < size; i++)
        {
            List<Cell> cells = new List<Cell>();
            for (int j = 0; j < i+1; j++)
                cells.Add(new Cell(j, j - i, j));
            line_list.add(new Line(cells));

            List<Cell> cells_B = new List<Cell>();
            for ( int j = i+1; j >= 0; j--)
                cells_B.Add(new Cell(j,j - i, i + 1 - j ));
            line_list.add(new Line(cells_B));
        }
        for (int i = 0; i < size; i++)
        {
            List<Cell> cells = new List<Cell>();
            for (int j = 0; j < size - i; j++)
                cells.Add(new Cell(j, j + i, j));
            line_list.add(new Line(cells));

            List<Cell> cells_B = new List<Cell>();
            for ( int j = size - 1 - i; j >= 0; j--)
                cells_B.Add(new Cell(j + i, j, size - 1- i - j));
            line_list.add(new Line(cells_B));
        }

        //diagnoal  left up -> right down
        for (int i = 0; i < size; i++)
        {
            List<Cell> cells = new List<Cell>();
            for (int j = 0; j < size - i; j++)
                cells.Add(new Cell(j + i, j, j));
            line_list.add(new Line(cells));

            List<Cell> cells_B = new List<Cell>();
            for ( int j = i+1; j >= 0; j--)
                cells_B.Add(new Cell(size - j, size - (j + i), i + 1 - j ));
            line_list.add(new Line(cells_B));
        }
        for (int i = size - 1; i >= 0; i--)
        {
            List<Cell> cells = new List<Cell>();
            for (int j = 0; j < i+1; j++)
                cells.Add(new Cell(size - (i - j), size - j, j));
            line_list.add(new Line(cells));

            List<Cell> cells_B = new List<Cell>();
            for ( int j = i+1; j >= 0; j--)
                cells_B.Add(new Cell(size - (i - j),size - j, i+1 - j));
            line_list.add(new Line(cells_B));
        }

        int focus = (size - 1) / 2 ;
        line_list.baseSetting(
            new BasePoint(focus, focus, Cell.State.Red),
            new BasePoint(focus + 1, focus, Cell.State.Blue),
            new BasePoint(focus, focus + 1, Cell.State.Blue),
            new BasePoint(focus + 1, focus + 1, Cell.State.Red));

        //������
        int outline_focus = focus - 1; 
        line_list.baseSetting(
            //��
            new BasePoint(outline_focus + 1, outline_focus, Cell.State.AbleBlue),
            new BasePoint(outline_focus + 2, outline_focus, Cell.State.AbleRed),
            //�Ʒ�
            new BasePoint(outline_focus + 1, outline_focus + 3, Cell.State.AbleRed),
            new BasePoint(outline_focus + 2, outline_focus + 3, Cell.State.AbleBlue),
            //����
            new BasePoint(outline_focus, outline_focus + 1, Cell.State.AbleBlue),
            new BasePoint(outline_focus, outline_focus + 2, Cell.State.AbleRed),
            //������
            new BasePoint(outline_focus + 3, outline_focus + 1, Cell.State.AbleRed),
            new BasePoint(outline_focus + 3, outline_focus + 2, Cell.State.AbleBlue) );

        //Debug.Log(outline_focus);

        //�����ũ�� ����
        correction_size = correction_size / size;

        half_size = block_size * size / 2 * correction_size;

        //blockũ�� ������ ���� ��ȯ
        block.transform.localScale = Vector3.one * correction_size;

        playerInputSetup(size);

    }

    private void playerInputSetup(int size)
    {
        for (int i = 0; i < size;i++)
        {
            for (int j = 0; j < size;j++)
            {
                block instance = Instantiate(block, new Vector2(i - half_size,j - half_size), Quaternion.identity).GetComponent<block>();
                instance.setRowColumn(i, j);
            }
        }
    }

    public bool playerInputCheck(int x, int y)
    {
        return line_list.playerInputCheck(x, y, player_turn);
    }

    public void changePlayer()
    {
        player_turn = (player_turn == Cell.State.AbleBlue) ? Cell.State.AbleRed : Cell.State.AbleBlue;
    }

    public Color getColor(int x, int y)
    {
        if (line_list.playerInputCheck(x,y,Cell.State.Blue))
        {
            return new Color(0 / 255f, 0 / 255f, 255 / 255f);
        } 
        else if (line_list.playerInputCheck(x,y,Cell.State.Red))
        {
            return new Color(255 / 255f, 0 / 255f, 0 / 255f);
        }
        else
        {
            return new Color(1, 1, 1);
        }
    }

    public void playerInput(int x, int y)
    {
        if( line_list.playerInput(x, y, player_turn) )
            changePlayer();
    }

}

class BasePoint
{
    public int x,y;
    public Cell.State state;

    public BasePoint(int x, int y, Cell.State state)
    {
        this.x = x;
        this.y = y;
        this.state = state;
    }
}

class LineList
{
    List<Line> AllLines;

    public LineList()
    {
        AllLines = new List<Line>();
    }
    public void add(Line lineList)
    {
        AllLines.Add(lineList);
    }

    public void baseSetting(params BasePoint[] basePoints)
    {
        foreach (BasePoint basePoint in basePoints)
        {
            foreach(Line line in AllLines)
            {
                Cell cell = line.getCell(basePoint.x, basePoint.y);
                if(cell != null)
                {
                    cell.setState(basePoint.state);
                }
            }
        }
    }

    public bool playerInput(int x, int y, Cell.State state)
    {
        if (!(state == Cell.State.AbleBlue || state == Cell.State.AbleRed))
        {
            Debug.Log(string.Format("Incorrect state {0}",state));
            return false ;

        }
        foreach(Line line in AllLines)
        {
            if(line.checkState(x, y, state))
            {
                Cell cell = line.getCell(x, y);
                line.flip(cell.getNumber(), state);
            }
        }

        return true ;
    }

    public bool playerInputCheck(int x, int y, Cell.State state)
    {
        foreach (Line line in AllLines)
        {
            if( line.checkState(x, y, state) )
            {
                return true;
            }
        }

        return false;
    }

    public List<Cell.State> getState(int x, int y)
    {
        List< Cell.State > states = new List< Cell.State >();
        foreach (Line line in AllLines)
        {
            Cell.State state = line.getState(x, y);
            if(state != Cell.State.Null)
                states.Add(state);
        }

        return states;
    }
}


class Line
{
    List<Cell> line;

    public Line (List<Cell> line)
    {
        this.line = line;
    }

    //���� Cell�� ������ flip
    public void flip(int number, Cell.State state)
    {
        if (Cell.State.AbleBlue == state)
            state = Cell.State.Blue;
        else if (Cell.State.AbleRed == state)
            state = Cell.State.Red;
        else
            return;
        do
        {
            Debug.Log(number);
            Debug.Log(line[number].getNumber());
            line[number].setState(state);
            number += 1;
        } while (line[number].getState() == state);
    }


    //���ο��� (x,y)���� Ư�������� Ȯ�� �˻�
    public bool checkState(int x, int y, Cell.State state)
    {
        foreach (Cell cell in line)
        {
            if (cell.checkState(x, y, state))
            {
                return true;
            }
        }
        return false;
    }

    //Ư����ǥ�� ���� cell���������
    public Cell getCell(int x, int y)
    {
        foreach (Cell cell in line)
            if(cell.checkCoordinate(x, y))    
                return cell;
        return null;
    }

    public Cell.State getState(int x,int y)
    {
        foreach (Cell cell in line)
            if (cell.checkCoordinate(x, y))
                return cell.getState();
        return Cell.State.Null;
    }

    //Ư�� ��ġ���� ������ �ش� ��ġ���� state���� ����
    //player input �޴°� State - REDAble,BLUEAble ��
    public Cell playerInputCoordinate(int x, int y, Cell.State state)
    {
        foreach (Cell cell in line)
        {
            if( cell.checkCoordinate(x, y) )
            {
                return cell;
            }
        }
        return null;
    }

    //cell ���� ���ִ� ������ ������ ��������� State - Able��
    public int getCellAbleValue(Cell cell, Cell.State state)
    {
        return (state == Cell.State.AbleBlue) ? cell.getLastBlue() : cell.getLastRed();
    }


    //line��ü Able�� �缳��
    public void resetAblesetting()
    {
        Cell beforeCell = line[0];

        foreach (Cell cell in line)
        {
            //���尡��� �����ǻ��� ���� �缳��
            int[] distance = cell.calculateLast(beforeCell);
            cell.setLast(distance[Cell.BLUE], distance[Cell.RED]);
            //cell�� �δ°��� �������� �缳��
            cell.resetAble();

            beforeCell= cell;
        }
    }


}



class Cell
{
    public static readonly int INIT_DISTANCE = 1; 
    public static readonly int NOT_EXIST = 0; 

    public static readonly int BLUE = 0;
    public static readonly int RED = 1;
    public enum State
    {
        Null,
        Void,
        Red,
        Blue,
        AbleBlue,
        AbleRed,
    }
    private int x;
    private int y;

    private State state;

    private int lastBlue;
    private int lastRed;

    private int number;

    public Cell(int x, int y, int number)
    {
        state = State.Void;
        this.x = x;
        this.y = y;


        lastBlue = NOT_EXIST;
        lastRed = NOT_EXIST;

        this.number= number;

    }

    public Cell(int x, int y, int number,State state, int lastBlue, int lastRed) 
        : this(x,y,number)
    {
        this.state = state;

        this.lastBlue = lastBlue;
        this.lastRed = lastRed;
    }

    public bool checkState(int x, int y, State state)
    {
        //Debug.Log(string.Format("celllcheckstate {0} - {1} / {2} - {3} / {4} - {5}",x,this.x, y,this.y,state,this.state));
        return this.x == x && this.y == y && this.state == state;
    }
    public bool checkCoordinate(int x, int y)
    {
        return this.x == x && this.y == y;
    }

    public State getState()
    {
        return state;
    }
    public void setState(State state) 
    { 
        this.state=state;
    }

    public int getLastBlue()
    {
        return lastBlue;
    }

    public int getLastRed()
    {
        return lastRed;
    }

    //LastRed, LastBlue �� �����Ѵ�
    public void setLast(int lastBlue, int lastRed)
    {
        this.lastBlue = lastBlue; this.lastRed = lastRed;
    }

    //Last �� ���
    public int[] calculateLast(Cell lastCell)
    {

        //���� Cell�� Blue �ϰ��
        if (lastCell.getState() == State.Blue)
        {
            //Blue�� ������ �Ÿ��� 1
            //����Cell�� Red���� �ִٰ� �ϸ� �װ� + 1
            return new int[]
            { 
                INIT_DISTANCE, (lastCell.getLastBlue() == NOT_EXIST) ? NOT_EXIST : lastCell.getLastRed() + 1
            };
        }
        //���� Cell�� Red �ϰ��
        else if (lastCell.getState() == State.Red)
        {
            return new int[]
            {
                (lastCell.getLastBlue() == NOT_EXIST) ? NOT_EXIST : lastCell.getLastBlue() + 1,
                INIT_DISTANCE
            };
        }
        //���� Cell�� void�� ���
        else
        {
            return new int[] { NOT_EXIST, NOT_EXIST };
        }
    }

    //able�缳��
    public void resetAble()
    {
        //����� ���������� ����ϴ°��̱⶧����
        //���࿡ ����̾ƴ϶�� ƨ�ܳ���
        if (state == State.Blue || state == State.Red)
            return ;
        //blue�� �����ϰ� lsatRed�� ������ �ʱ�ȭ�Ǿ�����
        else if (lastBlue != NOT_EXIST && lastRed == INIT_DISTANCE)
            state = State.Blue;
        //Red�� �����ϰ� lastBlue�� ������ �ʱ�ȭ �Ǿ�����
        else if (lastRed != NOT_EXIST && lastBlue == INIT_DISTANCE)
            state = State.Red;
    }

    public int getNumber()
    {
        return number;
    }
}
