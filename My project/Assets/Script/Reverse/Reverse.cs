using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Reverse : MonoBehaviour
{

    public static readonly int PLAYER1 = 1;
    public static readonly int PLAYER2 = 2;

    [SerializeField]
    private int row;

    [SerializeField]
    private int column;

    [SerializeField]
    private GameObject block;

    private const float block_size = 1.3f;
    private float correction_size = 7f;

    private float width_half;
    private float height_half;

    //����ڰ� �и��� ǥ���ҿ뵵�� ��
    public static int[,] grid;

    //0-�÷��̾�1 1-�÷�����2
    public static int player_turn;

    // Start is called before the first frame update
    void Start()
    {
        player_turn = 1;
        grid = new int[row, column];
        Array.Clear(grid,0, grid.Length);

        //������ �⺻����
        grid[(row - 1) / 2, (column - 1) / 2] = PLAYER1;
        grid[(row - 1) / 2 + 1, (column - 1) / 2] = PLAYER2;
        grid[(row - 1) / 2, (column - 1) / 2 + 1] = PLAYER2;
        grid[(row - 1) / 2 + 1, (column - 1) / 2 + 1] = PLAYER1;

        //�����ũ�� ����
        correction_size = correction_size / ((column > row) ? column : row);

        //���μ��� ���ݱ��ϱ�
        width_half = block_size * (float)row / 2;
        height_half = block_size * (float)column / 2;

        //blockũ�� ������ ���� ��ȯ
        block.transform.localScale = Vector3.one * correction_size;

        //�ʻ���
        make_map(row, column, block);

    }


    private void make_map(int row, int column, GameObject block)
    {
        for (int i = 0; i < row; i++)
        {
            List<GameObject> line = new List<GameObject>();
            for (int j = 0; j < column; j++) {
                GameObject instance = Instantiate(block, new Vector2((block_size * i + block_size / 2 - width_half) * correction_size, (block_size * j + block_size / 2 - height_half) * correction_size), Quaternion.identity);
                instance.GetComponent<block>().setRowColumn(i,j);
            }
        }
    }
    
    public static void flip_calculate(int x, int y)
    {
        bool check = false;
        //�ڽ��� ������
        for (int i = x + 1; i < grid.GetLength(0); i++)
        {
            if (grid[i,y] == 0 ||
            (i == (x + 1)&& grid[i, y] == player_turn))
            {
                break;
            } 
            else if (grid[i, y] == player_turn)
            {
                flip(x,y,i,y);
                check= true;
                break;
            }
        }
        //�ڽ��� ����
        for (int i = x - 1; i >= 0; i--)
        {
            if (grid[i,y] == 0 ||
            (x - 1) == i&& grid[i, y] == player_turn)
            {
                break;
            } 
            else if (grid[i, y] == player_turn)
            {
                flip(i,y,x,y);
                check= true;
                break;
            }
        }

        // �ڽ��� �Ʒ���
        for (int i = y + 1; i < grid.GetLength(1); i++)
        {
            if (grid[x,i] == 0 ||
            i == (y + 1) && grid[x, i] == player_turn)
            {
                break;
            } 
            else if (i != (y + 1) && grid[x, i] == player_turn)
            {
                flip(x,y,x,i);
                check= true;
                break;
            }
        }
        //�ڽ��� ����
        for (int i = y - 1; i >= 0; i--)
        {
            if (grid[x,i] == 0 ||
            (y - 1) == i && grid[x, i] == player_turn)
            {
                break;
            } 
            else if ((y - 1) != i && grid[x, i] == player_turn)
            {
                flip(x,i,x,y);
                check= true;
                break;
            }
        }

        if (!check)
        {
            grid[x, y] = 0;
            player_turn = (player_turn == 1) ? 2 : 1;;
        }
    }

    //�׻� x1 < x2 , y1 < y2 �����Ѵ�
    private static void flip(int x1, int y1, int x2, int y2)
    {
        for(int i = x1; i <= x2; i++)
        {
            for (int j = y1; j <= y2; j++)
            {
                grid[i, j] = player_turn;
            }
        }
    }


}
