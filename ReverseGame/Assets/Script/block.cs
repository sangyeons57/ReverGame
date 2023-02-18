using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class block : MonoBehaviour
{
    readonly Dictionary<Piece.Status, Color> colorDict =
        new Dictionary<Piece.Status, Color>() 
        {
            {Piece.Status.Void, new Color(1, 1, 1) },
            {Piece.Status.Red, new Color(200/255f, 0/255f, 0/255f) },
            {Piece.Status.Blue, new Color(0/255f, 0/255f, 200/255f) },
        }; 

    private RaycastHit2D hit;
    private BoxCollider2D collider;
    private SpriteRenderer spriteRenderer;

    public Piece piece{ get; set; }

    public bool isReversed = false;
    // Start is called before the first frame update
    void Awake()
    {
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
            piece.playerInput();
        }
    }

    public void changesetColorMode(Piece.Status status)
    {
        spriteRenderer.color = (Color)colorDict[status];
    }

}
