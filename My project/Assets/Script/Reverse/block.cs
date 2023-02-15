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

    private Reverse reverse;

    public bool isReversed = false;
    // Start is called before the first frame update
    void Start()
    {
        reverse = GameObject.Find("gameSystem").GetComponent<Reverse>();
        collider = GetComponent<BoxCollider2D>();
        spriteRenderer= GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider == collider&& Input.GetMouseButtonUp(0))
        {
            Debug.Log(string.Format("{0} , {1} playerInput",row,column));
            reverse.playerInput(row, column);
        }

        spriteRenderer.color = reverse.getColor(row,column);
        
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
