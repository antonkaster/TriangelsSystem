using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TriangelsSystem.CommonGraphics.Helpers;
using TriangelsSystem.CommonGraphics.Primitive2d;

namespace TriangelsSystem.Logic.GameObjects
{
    internal class UnitGameObject
    {
        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set
            {
                _isMoved = _position != value;
                _position = value;

                if (IsTargetMoved)
                    RecalculateTarget();
            }
        }
        public int Radius => _radius;

        private int _minSpeed = 3;
        private int _maxAdditionalSpeed = 6;
        public int Speed
        {
            get
            {
                var range = Position.PowRange(_target);
                var additionaSpeed = (int)(range / 250000f * _maxAdditionalSpeed);
                if (additionaSpeed > _maxAdditionalSpeed)
                    additionaSpeed = _maxAdditionalSpeed;

                return _minSpeed + additionaSpeed;
            }
        }

        public bool IsTargetMoved => _lastFirstPos != _firstFriend.Position || _lastSecondPos != _secondFriend.Position;

        public bool IsDragged { get; set; } = false;

        private int _radius = 20;

        private UnitGameObject _firstFriend;
        private UnitGameObject _secondFriend;

        private Vector2 _target;

        private bool _isMoved = false;
        private int _moveCounter = 0;

        private Vector2 _lastFirstPos = new();
        private Vector2 _lastSecondPos = new();

        public UnitGameObject(Vector2 startPosition)
        {
            _position = startPosition;
            _target = startPosition;
        }

        public void TakeFriends(UnitGameObject first, UnitGameObject second)
        {
            _firstFriend = first;
            _secondFriend = second;
        }

        public void Move()
        {
            Position = SpaceHelper.CalculateNextPosition(Position, _target, Speed);

            if (_isMoved)
                _moveCounter = 10;

            _lastFirstPos = _firstFriend.Position;
            _lastSecondPos = _secondFriend.Position;
        }

        public void DragTo(Vector2 position)
        {
            Position = position;
            RecalculateTarget();
        }

        private void RecalculateTarget()
        {
            var firstPos = _firstFriend.Position;
            var secondPos = _secondFriend.Position;

            (var p1, var p2) = firstPos.GetPerpendicular(secondPos);

            var newTarget = Position.FindNearestPointOnLineOnInfiniteLine(p1, p2);

            if (newTarget.InRecatangle(SpaceHelper.GetBoundedScreen()))
            {
                _target = newTarget;
                return;
            }

            var screenPoints = SpaceHelper.GetTargetBorderPoints(firstPos, secondPos, p1, p2);

            if (screenPoints == null)
                return;

            _target = Position.FindNearestPointOnLineOnFiniteLine(screenPoints.Value.p1, screenPoints.Value.p2);
        }

        public void DrawLines(SpriteBatch spriteBatch)
        {
            var firstPos = _firstFriend.Position;
            var secondPos = _secondFriend.Position;

            if (IsDragged)
            {
                GlobalContent.FriendLineSelected.DrawLine(spriteBatch, Position, firstPos);
                GlobalContent.FriendLineSelected.DrawLine(spriteBatch, Position, secondPos);
            }
            else
            {
                if (_moveCounter > 0)
                {
                    GlobalContent.FriendLine.DrawLine(spriteBatch, Position, firstPos);
                    GlobalContent.FriendLine.DrawLine(spriteBatch, Position, secondPos);
                    _moveCounter--;
                }
            }
        }

        public void DrawBody(SpriteBatch spriteBatch)
        {
            if (IsDragged)
            {
                GlobalContent.BodySelected.DrawCircle(spriteBatch, Position, Radius);
            }
            else
            {
                GlobalContent.Body.DrawCircle(spriteBatch, Position, Radius);
            }
        }

        public void DebugDraw(SpriteBatch spriteBatch)
        {
            if (!IsDragged)
                return;

            if (_isMoved)
                GlobalContent.TargetLine.DrawLine(spriteBatch, Position, _target);

            var firstPos = _firstFriend.Position;
            var secondPos = _secondFriend.Position;

            spriteBatch.DrawLine(firstPos, secondPos, 2, Color.White);
            var center = new Vector2((firstPos.X + secondPos.X) / 2, (firstPos.Y + secondPos.Y) / 2);
            spriteBatch.DrawCircle(center, 4, 4, Color.White, Color.White);

            (var p1, var p2) = firstPos.GetPerpendicular(secondPos);

            p1 -= center;
            p1.Normalize();
            p1 = center + p1 * 10000;

            p2 -= center;
            p2.Normalize();
            p2 = center + p2 * 10000;

            spriteBatch.DrawLine(p1, p2, 2, Color.White);

            var screenPoints = SpaceHelper.GetTargetBorderPoints(firstPos, secondPos, p1, p2);

            if (screenPoints != null)
            {
                spriteBatch.DrawCircle(screenPoints.Value.p1, 4, 4, Color.Blue, Color.Black);
                spriteBatch.DrawCircle(screenPoints.Value.p2, 4, 4, Color.Red, Color.Black);
            }
        }
    }
}
