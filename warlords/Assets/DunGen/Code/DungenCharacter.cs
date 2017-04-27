using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DunGen
{
    public delegate void CharacterTileChangedEvent(DungenCharacter character, Tile previousTile, Tile newTile);

    /// <summary>
    /// Contains information about the dungeon the character is in
    /// </summary>
    [AddComponentMenu("DunGen/Character")]
    public class DungenCharacter : MonoBehaviour
    {
        public Tile CurrentTile { get { return currentTile; } set { currentTile = value; } }
        public event CharacterTileChangedEvent OnTileChanged;

        [SerializeField, HideInInspector]
        private Tile currentTile;


        internal void ForceRecheckTile()
        {
            foreach(var tile in Component.FindObjectsOfType<Tile>())
                if (tile.Placement.Bounds.Contains(transform.position))
                {
                    HandleTileChange(tile);
                    break;
                }
        }

        protected virtual void OnTileChangedEvent(Tile previousTile, Tile newTile) { }

        internal void HandleTileChange(Tile newTile)
        {
            if (currentTile == newTile)
                return;

            var previousTile = currentTile;
            currentTile = newTile;

            if (OnTileChanged != null)
                OnTileChanged(this, previousTile, newTile);

            OnTileChangedEvent(previousTile, newTile);
        }
    }
}
