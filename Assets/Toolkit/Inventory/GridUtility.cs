using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory
{
    public struct GridRect
    {
        public int x, y, w, h;

        public Vector2Int position {
            get => new Vector2Int(x, y);
            set {
                x = value.x;
                y = value.y;
            }
        }
        public Vector2Int size {
            get => new Vector2Int(w, h);
            set {
                w = value.x;
                h = value.y;
            }
        }

        public GridRect(Vector2Int position) {
            this.x = position.x;
            this.y = position.y;
            w = 1;
            h = 1;
        }

        public GridRect(Vector2Int position, Vector2Int size) {
            this.x = position.x;
            this.y = position.y;
            w = size.x;
            h = size.y;
        }

        public GridRect(int _x, int _y, int _w, int _h) {
            this.x = _x;
            this.y = _y;
            this.w = _w;
            this.h = _h;
        }

        public bool Contains(Vector2Int point) {
            return
                point.x >= x &&
                point.x < x + w &&
                point.y >= y &&
                point.y < y + h;
        }

        public bool Intersects(GridRect rect) {
            return (rect.x < this.x + this.w) &&
            (this.x < (rect.x + rect.w)) &&
            (rect.y < this.y + this.h) &&
            (this.y < rect.y + rect.h);
        }

        public override string ToString() {
            return $"({x}, {y}, {w}, {h})";
        }
    }

    public class GridHelper
    {
        private int width = 0;
        private int height = 0;
        private bool[,] occupied;

        public GridHelper(int width, int height) {
            this.width = width;
            this.height = height;
            occupied = new bool[width, height];
        }

        public void Clear() {
            occupied = new bool[width, height];
        }

        public void SetOccupied(IReadOnlyList<GridRect> rects) {
            Clear();
            for(int i = 0, length = rects.Count; i < length; i++) {
                var area = rects[i];
                int w = Math.Min(width, area.x + area.w);
                int h = Math.Min(height, area.y + area.h);
                for(int x = Math.Max(0, area.x); x < w; x++) {
                    for(int y = Math.Max(0, area.y); y < h; y++) {
                        occupied[x, y] = true;
                    }
                }
            }
        }

        public bool HasAvailableLocation(GridRect rect, out GridRect newArea) {
            if(rect.x >= 0 && rect.y >= 0 && rect.x + rect.w <= width && rect.y + rect.h <= height && Fits(rect.x, rect.y, rect.w, rect.h)) {
                newArea = rect;
                return true;
            }
            return SearchForLocation(rect.size, out newArea);
        }

        private bool SearchForLocation(Vector2Int size, out GridRect area) {
            int searchWidth = width - size.x;
            int searchHeight = height - size.y;
            for(int x = 0; x <= searchWidth; x++) {
                for(int y = 0; y <= searchHeight; y++) {
                    if(Fits(x, y, size.x, size.y)) {
                        area = new GridRect(x, y, size.x, size.y);
                        return true;
                    }
                }
            }
            area = new GridRect(Vector2Int.zero);
            return false;
        }

        private bool Fits(int _x, int _y, int _w, int _h) {
            _w = _x + _w;
            _h = _y + _h;
            for(int x = _x; x < _w; x++) {
                for(int y = _y; y < _h; y++) {
                    if(occupied[x, y]) {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
