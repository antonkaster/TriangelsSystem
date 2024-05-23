using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TriangelsSystem.CommonGraphics.Constants;
using TriangelsSystem.CommonGraphics.Helpers;
using TriangelsSystem.Logic.GameObjects;

namespace TriangelsSystem.Logic
{
    public class WorldManager
    {
        public bool IsPaused { get; set; }

        private readonly ContentManager _contentManager;
        private readonly List<UnitGameObject> _units = new();

        private int _unitsCount = 30;

        public WorldManager(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public void SetNewCount(int newCount)
        {
            _unitsCount = newCount;
            _units.Clear();
            Load();
        }

        public void Load()
        {
            for (var i = 0; i < _unitsCount; i++)
            {
                Vector2 pos;
                var isIntersects = false;

                do
                {
                    pos = SpaceHelper.GetRandomScreenCoords(ScreenConsts.ScreenBound).ToVector2();

                    foreach (var unit in _units)
                    {
                        isIntersects = GeometriaExtensions.IsIntersect(unit.Position, unit.Radius, pos, unit.Radius);
                        if (isIntersects)
                            break;
                    }
                }
                while (isIntersects);

                _units.Add(new UnitGameObject(pos));
            }

            foreach (var unit in _units)
            {
                UnitGameObject first;

                do
                {
                    first = _units.TakeRandom();
                }
                while (first == unit);

                UnitGameObject second;

                do
                {
                    second = _units.TakeRandom();
                }
                while (second == unit || first == second);

                unit.TakeFriends(first, second);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (IsPaused)
                return;

            foreach (var unit in _units)
                unit.Move();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            if (Globals.DebugOn)
                spriteBatch.DrawScreenBounds();

            if (Globals.ShowLinks)
                foreach (var unit in _units)
                    unit.DrawLines(spriteBatch);

            foreach (var unit in _units)
                unit.DrawBody(spriteBatch);

            if (Globals.DebugOn)
                foreach (var unit in _units)
                    unit.DebugDraw(spriteBatch);

            spriteBatch.End();
        }

        private UnitGameObject? _dragUnit = null;
        public void TryStartDrag(Point mousePos)
        {
            var newPos = mousePos.ToVector2();

            foreach (var unit in _units)
            {
                if (GeometriaExtensions.IsIntersect(unit.Position, unit.Radius + 10, newPos))
                {
                    _dragUnit = unit;
                    _dragUnit.IsDragged = true;
                    break;
                }
            }
        }

        public void Drag(Point mousePos)
        {
            if (_dragUnit == null)
                return;

            _dragUnit.DragTo(mousePos.ToVector2());
        }

        public void EndDrag()
        {
            if (_dragUnit == null)
                return;

            _dragUnit.IsDragged = false;
            _dragUnit = null;
        }
    }
}
