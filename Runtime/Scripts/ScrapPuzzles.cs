using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapPuzzles : MonoBehaviour
{
    [SerializeField] ScrapCursorManager scrapManager;

    [SerializeField] DragAndDrop[] pieces;
    [SerializeField] Transform[] targets;

    [SerializeField] float speed;

    bool finished = false;

    int piecesDone;

    void Update()
    {
        if (!finished)
        {
            for (int i = 0; i < pieces.Length; i++)
            {
                var piece = pieces[i];
                var target = targets[i];

                piece.transform.position = Vector3.MoveTowards(piece.transform.position, target.position, speed * Time.deltaTime);

                if (piece.transform.position == target.position)
                {
                    finished = true;

                    for (int n = 0; n < pieces.Length; n++)
                    {
                        pieces[n].transform.position = targets[n].position;
                        pieces[n].enabled = true;
                    }    
                }
            }
        }
    }

    public void Resolve()
    {
        piecesDone++;

        if (piecesDone == pieces.Length)
        {
            scrapManager.WinGame();
        }
    }

}