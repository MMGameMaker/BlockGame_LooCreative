using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Germ : MonoBehaviour
{
    private static Germ selected;
    private SpriteRenderer sr;

    private Vector2Int _position;
    public Vector2Int Position
    {
        get { return this._position; }
        set { this._position = value; }
    }
    

    // Start is called before the first frame update
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Select()
    {
        SoundManager.Instance.PlaySound(SoundType.TypeSelect);
        sr.color = Color.grey;
    }

    public void Unselect()
    {
        sr.color = Color.white;
    }

    private void OnMouseDown()
    {
        if (sr.sprite == null) return;
        if (sr.sprite == null)
        {
            SoundManager.Instance.PlaySound(SoundType.TypePop);
            return;
        }
        if (selected != null)
        {
            if (selected == this)
                return;
            selected.Unselect();
            if(Vector2Int.Distance(selected.Position, Position) == 1)
            {
                SoundManager.Instance.PlaySound(SoundType.TypeMove);
                GridManager.Instance.SwapGerms(selected.Position, this.Position);
                selected = null;
            }
            else
            {
                selected = this;
                Select();
            }

        }
        else
        {
            selected = this;
            Select();
        }
    }
}
