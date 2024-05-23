using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TriangelsSystem.CommonGraphics.Helpers;
using TriangelsSystem.Controls;
using TriangelsSystem.Logic;

namespace TriangelsSystem
{
    public class GameMain : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private WorldManager _worldManager;
        private GuiSystemController _guiSystemController;

        public GameMain()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 2550,
                PreferredBackBufferHeight = 1440
            };
            Window.AllowUserResizing = true;
            SpaceHelper.Size = new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Globals.SetGraphipsDevice(GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            GlobalContent.Init();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _guiSystemController = new GuiSystemController(GraphicsDevice, Content, Window);
            _guiSystemController.Load();
            _guiSystemController.OnCountChanged += (val) => _worldManager.SetNewCount(val);
            Window.ClientSizeChanged += _guiSystemController.WindowOnClientSizeChanged;

            _worldManager = new WorldManager(Content);
            _worldManager.Load();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            CheckPause(keyboardState);
            CheckDrag(mouseState);

            _worldManager.Update(gameTime);
            _guiSystemController.Update(gameTime);

            base.Update(gameTime);
        }

        private bool _mousePressed = false;
        private void CheckDrag(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (_mousePressed)
                {
                    _worldManager.Drag(mouseState.Position);
                }
                else
                {
                    _mousePressed = true;
                    _worldManager.TryStartDrag(mouseState.Position);
                }
            }
            else
            {
                _mousePressed = false;
                _worldManager.EndDrag();
            }
        }

        private bool _spacePressed = false;
        private void CheckPause(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Space) && !_spacePressed)
            {
                _spacePressed = true;
                _worldManager.IsPaused = !_worldManager.IsPaused;
            }
            else if (keyboardState.IsKeyUp(Keys.Space) && _spacePressed)
            {
                _spacePressed = false;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _worldManager.Draw(gameTime, _spriteBatch);
            _guiSystemController.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
