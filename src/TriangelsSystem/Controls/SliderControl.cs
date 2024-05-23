using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using System;

namespace TriangelsSystem.Controls
{
    internal class SliderControl : CompositeControl
    {
        public Action<int> OnValueChanged;

        public int MinValue { get; set; } = 0;
        public int MaxValue { get; set; } = 100;

        private int _value = 0;
        public int Value
        {
            get => _value;
            set
            {
                _value = value;

                if (_value < MinValue)
                    _value = MinValue;
                if (_value > MaxValue)
                    _value = MaxValue;

                _countLabel.Content = _value;

                var progressBarValue = (_value - MinValue) / (float)MaxValue;

                SetProgressBarValue(progressBarValue);
            }
        }

        public override object Content
        {
            get
            {
                return _contentLabel.Content;
            }
            set
            {
                _contentLabel.Content = value;
                if (value is Control control)
                    _contentLabel.Width = control.Width;
                OnSizeChanged();
            }
        }

        protected override Control Template => _stackPanel;

        private readonly Label _contentLabel;
        private readonly StackPanel _stackPanel;
        private readonly SliderProgressBar _progressBar;
        private readonly Label _countLabel;

        private bool _changingInProgress = false;

        public SliderControl()
        {
            this.BackgroundColor = Color.Transparent;
            this.BorderColor = Color.Transparent;

            _contentLabel = new Label();
            _progressBar = new SliderProgressBar()
            {
                Progress = 0.5f,
                MinWidth = 50,
                Width = 50,
                LeftBarColor = Color.Purple,
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };
            _progressBar.OnPointerDownEvent += ProgressBar_OnPointerDownEvent;
            _progressBar.OnPointerUpEvent += ProgressBar_OnPointerUpEvent;
            _progressBar.OnPointerMoveEvent += ProgressBar_OnPointerMoveEvent;
            _progressBar.OnPointerLeaveEvent += ProgressBar_OnPointerLeaveEvent;
            _countLabel = new Label()
            {
                Content = _progressBar.Progress.ToString(),
                Width = 55,
                HorizontalTextAlignment = HorizontalAlignment.Right
            };
            _stackPanel = new StackPanel()
            {
                BackgroundColor = this.BackgroundColor,
                BorderColor = this.BorderColor,
                Orientation = Orientation.Horizontal,
                Items =
                {
                    _progressBar,
                    _countLabel,
                    _contentLabel,
                }
            };
            _progressBar.Width = this.Width - _countLabel.Width - _contentLabel.Width;
        }

        public override Size CalculateActualSize(IGuiContext context)
        {
            return GetContentSize(context);
        }

        public override Size GetContentSize(IGuiContext context)
        {
            var width = _stackPanel.CalculateActualSize(context).Width;

            return new Size(width, _stackPanel.Height);
        }

        protected override void OnSizeChanged()
        {
            _stackPanel.Size = this.Size;
            _progressBar.Width = _stackPanel.Width - _countLabel.Width - _contentLabel.Width - 20;
            base.OnSizeChanged();
        }

        private bool ProgressBar_OnPointerMoveEvent(PointerEventArgs arg)
        {
            if (!_changingInProgress)
                return true;

            var koef = (arg.Position.X - _progressBar.ContentRectangle.X) / (float)_progressBar.ContentRectangle.Width;
            var value = (MaxValue - MinValue) * koef + MinValue;

            Value = (int)Math.Round(value);

            return true;
        }

        private bool ProgressBar_OnPointerLeaveEvent(PointerEventArgs arg)
        {
            return ProgressBar_OnPointerUpEvent(arg);
        }

        private bool ProgressBar_OnPointerUpEvent(PointerEventArgs args)
        {
            SendNewValueEvent();
            _changingInProgress = false;
            return true;
        }

        private bool ProgressBar_OnPointerDownEvent(PointerEventArgs args)
        {
            _changingInProgress = true;
            ProgressBar_OnPointerMoveEvent(args);
            return true;
        }

        private void SetProgressBarValue(float val)
        {
            if (val < 0)
                val = 0;
            if (val > 1)
                val = 1;

            _progressBar.Progress = val;
        }

        private void SendNewValueEvent()
        {
            if (!_changingInProgress)
                return;
            OnValueChanged?.Invoke(Value);
        }
    }
}
