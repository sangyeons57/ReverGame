using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class block : MonoBehaviour
{
    private int row;
    private int column;

    private RaycastHit2D hit;
    private BoxCollider2D collider;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Reverse reverse;

    public bool isReversed = false;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        spriteRenderer= GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider == collider&& Input.GetMouseButtonUp(0)&& Reverse.grid[row, column] == 0)
        {
            Reverse.grid[row, column] = Reverse.player_turn;
            Reverse.player_turn = (Reverse.player_turn == 1) ? 2 : 1;
            Reverse.flip_calculate(row,column);
        }

        if (Reverse.grid[row,column] == 1)
        {
            spriteRenderer.color =  new Color(200 / 255f, 0 / 255f, 0 / 255f); ;
        } else if (Reverse.grid[row,column] == 2){
            spriteRenderer.color = new Color(0 / 255f, 0 / 255f, 255 / 255f);
        }
        
    }


    public void setRowColumn(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public int[] getRowColumn()
    {
        return new int[] {row, column}; 
    }
}
