using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.ViewportAdapters;
using System;
using TriangelsSystem.CommonGraphics.Helpers;

namespace TriangelsSystem.Controls
{
    internal class GuiSystemController
    {
        public Action<int> OnCountChanged;

        private const string _checkLinksName = "CheckLinks";
        private const string _checkGeometryName = "CheckDebug";
        private const string _sliderCount = "countSlider";

        private readonly GraphicsDevice _graphicsDevice;
        private readonly ContentManager _contentManager;
        private readonly GameWindow _gameWindow;
        private GuiSystem _guiSystem;

        public GuiSystemController(
            GraphicsDevice graphicsDevice,
            ContentManager contentManager,
            GameWindow gameWindow
            )
        {
            _graphicsDevice = graphicsDevice;
            _contentManager = contentManager;
            _gameWindow = gameWindow;
        }

        public void Load()
        {
            var viewportAdapter = new DefaultViewportAdapter(_graphicsDevice);
            var guiRenderer = new GuiSpriteBatchRenderer(_graphicsDevice, () => Matrix.Identity);

            var font = _contentManager.Load<BitmapFont>("font");
            BitmapFont.UseKernings = false;
            Skin.CreateDefault(font);

            var screen = LoadControls();
            LoadControlsLogic(screen);

            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer) { ActiveScreen = screen };
        }

        private void LoadControlsLogic(Screen screen)
        {
            screen.FindControl<EventsCheckBox>(_checkLinksName).CheckedStateChanged += (val) => Globals.ShowLinks = val;
            screen.FindControl<EventsCheckBox>(_checkGeometryName).CheckedStateChanged += (val) => Globals.DebugOn = val;
            screen.FindControl<SliderControl>(_sliderCount).OnValueChanged += (val) => OnCountChanged?.Invoke(val);
        }

        private static Screen LoadControls()
        {
            return new Screen
            {
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Height = 40,
                    MaxHeight = 100,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Items =
                    {
                        new SliderControl
                        {
                            Name = _sliderCount,
                            Width = 700,
                            Content = new Label()
                            {
                                Content = "шт.",
                                Width = 50
                            },
                            MinValue = 3,
                            MaxValue = 100,
                            Value = 30,
                            BackgroundColor = Color.Transparent,
                        },
                        new EventsCheckBox
                        {
                            Name = _checkLinksName,
                            Content = new Label{ Content = "Показать связи" },
                            IsChecked = Globals.ShowLinks
                        },
                        new EventsCheckBox
                        {
                            Name = _checkGeometryName,
                            Content = new Label{ Content = "Показать геометрию" },
                            IsChecked = Globals.DebugOn,
                            IsVisible = false
                        }
                    }
                }
            };
        }

        public void WindowOnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            _guiSystem.ClientSizeChanged();
            SpaceHelper.Size = _gameWindow.ClientBounds.Size;
        }

        public void Update(GameTime gameTime)
        {
            _guiSystem.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            _guiSystem.Draw(gameTime);
        }
    }
}
