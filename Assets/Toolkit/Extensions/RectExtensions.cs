using System;
using UnityEngine;

namespace Toolkit {
    public static class RectExtensions {

        #region Translate

        public static Rect Translate(this Rect rect, float x, float y) {
            return new Rect(rect.x + x, rect.y + y, rect.width, rect.height);
        }

        public static Rect Translate(this Rect rect, Vector2 offset) {
            return new Rect(rect.x + offset.x, rect.y + offset.y, rect.width, rect.height);
        }

        public static void TranslateRef(this ref Rect rect, float x, float y) {
            rect.x += x;
            rect.y += y;
        }

        public static void TranslateRef(this ref Rect rect, Vector2 offset) {
            rect.x += offset.x;
            rect.y += offset.y;
        }

        #endregion

        #region Padding

        public static Rect Pad(this Rect rect, float left, float right, float up, float down) {
            return new Rect(rect.x + left, rect.y + up, rect.width - (right + left), rect.height - (down + up));
        }

        public static void PadRef(this ref Rect rect, float left, float right, float up, float down) {
            rect.x += left;
            rect.y += up;
            rect.width -= right + left;
            rect.height -= down + up;
        }

        #endregion

        #region Shrinking

        public enum ShrinkSide {
            Left,
            Right,
            Top,
            Bot,
            All
        }

        public static Rect Shrink(this Rect rect, float amount) {
            rect.x += amount;
            rect.y += amount;
            rect.width -= amount * 2;
            rect.height -= amount * 2;
            return rect;
        }

        public static Rect Shrink(this Rect rect, float amount, ShrinkSide side) {
            switch(side) {
                case ShrinkSide.All:
                    rect.x += amount;
                    rect.y += amount;
                    rect.width -= amount * 2;
                    rect.height -= amount * 2;
                    return rect;
                case ShrinkSide.Left:
                    rect.x += amount;
                    rect.width -= amount;
                    return rect;
                case ShrinkSide.Right:
                    rect.width -= amount;
                    return rect;
                case ShrinkSide.Top:
                    rect.y += amount;
                    rect.height -= amount;
                    return rect;
                case ShrinkSide.Bot:
                    rect.height -= amount;
                    return rect;
            }
            return rect;
        }

        public static void ShrinkRef(ref this Rect rect, float amount) {
            rect.x += amount;
            rect.y += amount;
            rect.width -= amount * 2;
            rect.height -= amount * 2;
        }

        public static void ShrinkRef(ref this Rect rect, float amount, ShrinkSide side) {
            switch(side) {
                case ShrinkSide.All:
                    rect.x += amount;
                    rect.y += amount;
                    rect.width -= amount * 2;
                    rect.height -= amount * 2;
                    break;
                case ShrinkSide.Left:
                    rect.x += amount;
                    rect.width -= amount;
                    break;
                case ShrinkSide.Right:
                    rect.width -= amount;
                    break;
                case ShrinkSide.Top:
                    rect.y += amount;
                    rect.height -= amount;
                    break;
                case ShrinkSide.Bot:
                    rect.height -= amount;
                    break;
            }
        }

        #endregion

        #region Splitting

        public static void SplitVertical(this Rect rect, out Rect up, out Rect down, float upPercentage)
            => SplitVertical(rect, out up, out down, upPercentage, 0f);

        public static void SplitVertical(this Rect rect, out Rect up, out Rect down, float upPercentage, float spacing) {
            float upHeight = rect.height * upPercentage - spacing / 2f;
            up = new Rect(rect.x, rect.y, rect.width, upHeight);
            down = new Rect(rect.x, rect.y + upHeight + spacing, rect.width, rect.height - (upHeight + spacing));
        }

        public static void SplitVertical(this Rect rect, out Rect up, out Rect mid, out Rect down, float upPercentage, float midPercentage, float spacing) {
            float upHeight = rect.height * upPercentage - spacing / 2f;
            var midHeight = rect.height * midPercentage - spacing;
            var botHeight = rect.height * (1f - (upPercentage + midPercentage)) - spacing / 2f;
            up = new Rect(rect.x, rect.y, rect.width, upHeight);
            mid = new Rect(rect.x, rect.y + upHeight + spacing, rect.width, midHeight);
            down = new Rect(rect.x, rect.y + upHeight + midHeight + spacing * 2f, rect.width, botHeight);
        }

        public static Rect[] SplitVertical(this Rect rect, int splits, float spacing) {
            var totalSpacing = (splits - 1) * spacing;
            var chunkSize = (rect.height - totalSpacing) / splits;
            Rect[] areas = new Rect[splits];
            var chunk = new Rect(rect);
            chunk.height = chunkSize;
            for(int i = 0; i < splits; i++) {
                areas[i] = chunk;
                chunk.y += chunkSize + spacing;
            }
            return areas;
        }

        public static void SplitHorizontal(this Rect rect, out Rect left, out Rect right, float leftPercentage)
            => SplitHorizontal(rect, out left, out right, leftPercentage, 0f);

        public static void SplitHorizontal(this Rect rect, out Rect left, out Rect right, float leftPercentage, float spacing) {
            float leftWidth = rect.width * leftPercentage - spacing / 2f;
            left = new Rect(rect.x, rect.y, leftWidth, rect.height);
            right = new Rect(rect.x + leftWidth + spacing, rect.y, rect.width - (leftWidth + spacing), rect.height);
        }

        public static void SplitHorizontal(this Rect rect, out Rect left, out Rect mid, out Rect right, float leftPercentage)
            => SplitHorizontal(rect, out left, out mid, out right, leftPercentage, (1f - leftPercentage) / 2f, 0f);

        public static void SplitHorizontal(this Rect rect, out Rect left, out Rect mid, out Rect right, float leftPercentage, float spacing)
            => SplitHorizontal(rect, out left, out mid, out right, leftPercentage, (1f - leftPercentage) / 2f, spacing);

        public static void SplitHorizontal(this Rect rect, out Rect left, out Rect mid, out Rect right, float leftPercentage, float midPercentage, float spacing) {
            var leftWidth = rect.width * leftPercentage - spacing / 2f;
            var midWidth = rect.width * midPercentage - spacing;
            var rightWidth = rect.width * (1f - (leftPercentage + midPercentage)) - spacing / 2f;
            left = new Rect(rect.x, rect.y, leftWidth, rect.height);
            mid = new Rect(rect.x + leftWidth + spacing, rect.y, midWidth, rect.height);
            right = new Rect(rect.x + leftWidth + midWidth + spacing * 2f, rect.y, rightWidth, rect.height);
        }

        public static Rect[] SplitHorizontal(this Rect rect, int splits, float spacing) {
            var totalSpacing = (splits - 1) * spacing;
            var chunkSize = (rect.width - totalSpacing) / splits;
            Rect[] areas = new Rect[splits];
            var chunk = new Rect(rect);
            chunk.width = chunkSize;
            for(int i = 0; i < splits; i++) {
                areas[i] = chunk;
                chunk.x += chunkSize + spacing;
            }
            return areas;
        }

        #endregion

        #region Intersect

        public static bool Intersects(this Rect rect, Rect other) {
            return !(other.x > rect.x + rect.width || other.y > rect.y + rect.height || other.x + other.width < rect.x || other.y + other.height < rect.y);
        }

        public static bool Intersects(this Rect rect, Rect other, out Rect intersectionArea) {
            var doesIntersect = !(other.x > rect.x + rect.width || other.y > rect.y + rect.height || other.x + other.width < rect.x || other.y + other.height < rect.y);
            if(!doesIntersect) {
                intersectionArea = default;
                return false;
            }
            var x1 = Mathf.Min(rect.x + rect.width, other.x + other.width);
            var x2 = Mathf.Max(rect.x, other.x);
            var y1 = Mathf.Min(rect.y + rect.height, other.y + other.height);
            var y2 = Mathf.Max(rect.y, other.y);
            intersectionArea = new Rect(Mathf.Min(x1, x2), Mathf.Min(y1, y2), Mathf.Max(0f, x1 - x2), Mathf.Max(0f, y1 - y2));
            return true;
        }

        #endregion

        #region Subdivide

        public static Rect[] Subdivide(this Rect rect) {
            Rect[] result = new Rect[4];
            var halfx = rect.width / 2f;
            var halfy = rect.height / 2f;
            result[0] = new Rect(rect.x, rect.y, halfx, halfy);
            result[1] = new Rect(rect.x + halfx, rect.y, halfx, halfy);
            result[2] = new Rect(rect.x, rect.y + halfy, halfx, halfy);
            result[3] = new Rect(rect.x + halfx, rect.y + halfy, halfx, halfy);
            return result;
        }

        public static Rect[] Subdivide(this Rect rect, int amount) {
            if(amount > 8) {
                throw new Exception("Unable to subdivide this more than 8 times due to memory");
            }
            if(amount < 1)
                return null;

            var arrW = 1 << amount;
            Rect[] result = new Rect[arrW * arrW];
            var w = rect.width / (float)arrW;
            var h = rect.height / (float)arrW;
            var x = rect.x;
            var y = rect.y;

            for(int iy = 0; iy < arrW; iy++) {
                for(int ix = 0; ix < arrW; ix++) {
                    result[iy * arrW + ix] = new Rect(x + w * ix, y + h * iy, w, h);
                }
            }
            return result;
        }

        public static void Subdivide(this Rect rect, out Rect nw, out Rect ne, out Rect sw, out Rect se) {
            var halfx = rect.width / 2f;
            var halfy = rect.height / 2f;
            nw = new Rect(rect.x, rect.y + halfy, halfx, halfx);
            ne = new Rect(rect.x + halfx, rect.y + halfy, halfx, halfx);
            sw = new Rect(rect.x, rect.y, halfx, halfx);
            se = new Rect(rect.x + halfx, rect.y, halfx, halfx);
        }

        #endregion

        #region Random

        public static Vector2 RandomPoint(this Rect rect) {
            return new Vector2(rect.x + UnityEngine.Random.value * rect.width, rect.y + UnityEngine.Random.value * rect.height);
        }

        public static Vector2 RandomPoint(this Rect rect, System.Random random) {
            return new Vector2(rect.x + random.NextFloat() * rect.width, rect.y + random.NextFloat() * rect.height);
        }

        #endregion

        #region Subset

        public static Toolkit.Mathematics.Line Subset(this Rect rect, Toolkit.Mathematics.Line line) {
            var width = rect.x + rect.width;
            var height = rect.y + rect.height;
            var sp = line.StartPoint;
            var ep = line.EndPoint;

            var spOutside = !rect.Contains(sp);
            var epOutside = !rect.Contains(ep);

            if(spOutside) {
                if(sp.x < rect.x)
                    sp = line.GetPointAtIntersectionX(rect.x) ?? sp;
                if(sp.x > width)
                    sp = line.GetPointAtIntersectionX(width) ?? sp;
                if(sp.y < rect.y)
                    sp = line.GetPointAtIntersectionY(rect.y) ?? sp;
                if(sp.y > height)
                    sp = line.GetPointAtIntersectionY(height) ?? sp;
            }
            if(epOutside) {
                if(ep.x < rect.x)
                    ep = line.GetPointAtIntersectionX(rect.x) ?? ep;
                if(ep.x > width)
                    ep = line.GetPointAtIntersectionX(width) ?? ep;
                if(ep.y < rect.y)
                    ep = line.GetPointAtIntersectionY(rect.y) ?? ep;
                if(ep.y > height)
                    ep = line.GetPointAtIntersectionY(height) ?? ep;
            }

            return new Mathematics.Line(sp, ep);
        }

        // Returns a point that is clamped
        public static Vector2 Subset(this Rect rect, Vector2 point) {
            if(rect.x > point.x)
                point.x = rect.x;
            if(rect.x + rect.width < point.x)
                point.x = rect.x + rect.width;
            if(rect.y > point.y)
                point.y = rect.y;
            if(rect.y + rect.height < point.y)
                point.y = rect.y + rect.height;
            return point;
        }

        // Returns a point that is clamped
        public static bool Subset(this Rect rect, Vector2 input, out Vector2 output) {
            output = input;
            if(rect.x > input.x)
                output.x = rect.x;
            if(rect.x + rect.width < input.x)
                output.x = rect.x + rect.width;
            if(rect.y > input.y)
                output.y = rect.y;
            if(rect.y + rect.height < input.y)
                output.y = rect.y + rect.height;
            return input != output;
        }

        // Returns a point that is clamped
        public static bool Subset(this Rect rect, Vector2 input, out Vector3 output) {
            output = input;
            if(rect.x > input.x)
                output.x = rect.x;
            if(rect.x + rect.width < input.x)
                output.x = rect.x + rect.width;
            if(rect.y > input.y)
                output.y = rect.y;
            if(rect.y + rect.height < input.y)
                output.y = rect.y + rect.height;
            return input.x != output.x || input.y != output.y;
        }

        #endregion

        #region Fits Inside

        public static bool FitsInside(this Rect rect, Rect otherRect) {
            return rect.width <= otherRect.width && rect.height <= otherRect.height;
        }

        #endregion

        #region Clamp

        public static Rect ClampInside(this Rect rect, Rect container) {
            var x = rect.width < container.width ? Mathf.Clamp(rect.x, container.x, container.x + container.width - rect.width) : (container.x - (rect.width - container.width) / 2f);
            var y = rect.height < container.height ? Mathf.Clamp(rect.y, container.y, container.y + container.height - rect.height) : (container.y - (rect.height - container.height) / 2f);
            return new Rect(x, y, rect.width, rect.height);
        }

        public static Rect ClampInside(this Rect rect, Rect container, bool shrinkToFit) {
            if(!shrinkToFit)
                return ClampInside(rect, container);
            var x = rect.width < container.width ? Mathf.Clamp(rect.x, container.x, container.x + container.width - rect.width) : (container.x - (rect.width - container.width) / 2f);
            var y = rect.height < container.height ? Mathf.Clamp(rect.y, container.y, container.y + container.height - rect.height) : (container.y - (rect.height - container.height) / 2f);
            return new Rect(x, y, Mathf.Min(rect.width, container.width), Mathf.Min(rect.height, container.height));
        }

        #endregion

        #region NextLine

        public static Rect NextLine(this ref Rect rect) {
            rect.y += rect.height;
            return rect;
        }

        public static Rect NextLine(this ref Rect rect, float spacing) {
            rect.y += rect.height + spacing;
            return rect;
        }

        #endregion
    }
}
