using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ReverseTest
{
    PieceList pieceList = new PieceList();
    // A Test behaves as an ordinary method
    [Test]
    public void ReverseTestSimplePasses()
    {
        // Use the Assert class to test conditions

        for ( int x = 0; x < 10; x++ )
        {
            for (int y= 0; y < 10; y++ )
            {
                pieceList.Add(new Piece(x,y,Piece.Status.Void));
            }
        }


    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ReverseTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
