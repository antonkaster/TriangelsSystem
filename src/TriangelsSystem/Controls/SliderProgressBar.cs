using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TriangelsSystem.Controls
{
    internal class SliderProgressBar : Control
    {
        public event EventHandler ProgressChanged;

        public event Func<PointerEventArgs, bool> OnPointerDownEvent;
        public event Func<PointerEventArgs, bool> OnPointerUpEvent;
        public event Func<PointerEventArgs, bool> OnPointerMoveEvent;
        public event Func<PointerEventArgs, bool> OnPointerLeaveEvent;

        private float _progress = 1f;

        public float Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    this.ProgressChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public Color LeftBarColor { get; set; } = Color.White;
        public Color RightBarColor { get; set; } = Color.Gray;
        public Color PipColor { get; set; } = Color.White;
        public override IEnumerable<Control> Children { get; } = Enumerable.Empty<Control>();

        public SliderProgressBar()
        {
        }

        public override bool OnPointerDown(IGuiContext context, PointerEventArgs args)
        {
            if (OnPointerDownEvent != null)
                return OnPointerDownEvent(args);
            return base.OnPointerDown(context, args);
        }

        public override bool OnPointerUp(IGuiContext context, PointerEventArgs args)
        {
            if (OnPointerUpEvent != null)
                return OnPointerUpEvent(args);
            return base.OnPointerUp(context, args);
        }

        public override bool OnPointerMove(IGuiContext context, PointerEventArgs args)
        {
            if (OnPointerMoveEvent != null)
                return OnPointerMoveEvent(args);
            return base.OnPointerMove(context, args);
        }

        public override bool OnPointerLeave(IGuiContext context, PointerEventArgs args)
        {
            if (OnPointerLeaveEvent != null)
                return OnPointerLeaveEvent(args);
            return base.OnPointerLeave(context, args);
        }

        public override Size GetContentSize(IGuiContext context)
        {
            return new Size(5, 5);
        }

        public override void Draw(IGuiContext context, IGuiRenderer renderer, float deltaSeconds)
        {
            base.Draw(context, renderer, deltaSeconds);
            var contentRectangle = base.ContentRectangle;

            var width = (int)(contentRectangle.Width * Progress);

            var leftBar = new Rectangle(contentRectangle.X, contentRectangle.Y + 5, width, contentRectangle.Height - 10);
            renderer.FillRectangle(base.BoundingRectangle, LeftBarColor, leftBar);

            var rightBar = new Rectangle(contentRectangle.X + width, contentRectangle.Y + 5, contentRectangle.Width - width, contentRectangle.Height - 10);
            renderer.FillRectangle(base.BoundingRectangle, RightBarColor, rightBar);

            var pipValue = new Rectangle(contentRectangle.X + width, contentRectangle.Y, 17, contentRectangle.Height);
            renderer.FillRectangle(base.BoundingRectangle, PipColor, pipValue);
        }
    }
}
