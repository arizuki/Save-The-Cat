using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game5
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        
        SpriteBatch spriteBatch;
        private Rectangle _viewPortRectangle; // Границы игрового поля
        private Texture2D _background; //фон

        
        
        private Game1 _paddle;
        private Game1 _ball;
        SpriteFont font;


        private int _brickPaneWidth = 10; // Сколько кипричей рисовать в ширину
        private int _brickPaneHeight = 5; // Сколько кипричей рисовать в высоту
        private Texture2D _brickSprite; // Спрайт кирпича
        private Game1[,] _bricks; // Массив кирпичей

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        public Texture2D Sprite { get; set; } // Спрайт

        public Vector2 Position; // Положение
        public Vector2 Velocity; // Скорость
        public int Width { get { return Sprite.Width; } } // Ширина
        public int Height { get { return Sprite.Height; } } // Высота
        public bool IsAlive { get; set; } // Жив ли обьект
        public Rectangle Bounds // Границы обьекта
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        // Разворачивание движения по горизонтальной оси
        public void ReflectHorizontal()
        {
            Velocity.Y = -Velocity.Y;
        }

        // Разворачивание движения по вертикальной оси
        public void ReflectVertical()
        {
            Velocity.X = -Velocity.X;
        }

        public Game1(Texture2D sprite)
        {
            Sprite = sprite;
            IsAlive = true;
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
        }
    

/// <summary>
/// Allows the game to perform any initialization it needs to before starting to run.
/// This is where it can query for any required services and load any non-graphic
/// related content.  Calling base.Initialize will enumerate through any components
/// and initialize them as well.
/// </summary>
protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphics.IsFullScreen = true;
            _viewPortRectangle = new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            //_viewPortRectangle = new Rectangle(0, 0, graphics.IsFullScreen = true, graphics.IsFullScreen = true);

            font = Content.Load<SpriteFont>("Game");

            _background = Content.Load<Texture2D>(@"background1");

            _paddle = new Game1(Content.Load<Texture2D>(@"platform"));
            _paddle.Position = new Vector2((_viewPortRectangle.Width - _paddle.Width) / 2, _viewPortRectangle.Height - _paddle.Height + 70);

            _brickSprite = Content.Load<Texture2D>(@"brick");
            _bricks = new Game1[_brickPaneWidth, _brickPaneHeight];

            for (int i = 0; i < _brickPaneWidth; i++)
            {
                for (int j = 0; j < _brickPaneHeight; j++)
                {
                    _bricks[i, j] = new Game1(_brickSprite)
                    {
                        Position = new Vector2(i * 55  + 100, j * 30 + 100)
                        //Position = new Rectangle(i * 45 + 120, j * 25 + 100, _bric, _brickSprite.Height);
                    };
                }
            }


            // Создание мячика, начальное положение в середине на ракетке,
            // начальное направление - вправо, вверх
            _ball = new Game1(Content.Load<Texture2D>(@"Ball"));
            _ball.Position = new Vector2((_viewPortRectangle.Width - _ball.Width) / 2, _viewPortRectangle.Height - _paddle.Height - _ball.Height - 10 );
            _ball.Velocity = new Vector2(-3, -3);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        private void UpdateBall()
        {
            _ball.Position += _ball.Velocity;
            // Будущее положение мяча, нужно для предотвращения "залипания" мяча на поверхности обьекта
            Rectangle nextRect = new Rectangle((int)(_ball.Position.X + _ball.Velocity.X), (int)(_ball.Position.Y + _ball.Velocity.Y), _ball.Width, _ball.Height);

            // Столкновение с верхним краем игрового поля
            if (nextRect.Y <= 0)
                _ball.ReflectHorizontal();

            // При сталкивании мяча с нижним краем игрового поля, мячик "умирает"
            if (nextRect.Y >= _viewPortRectangle.Height - nextRect.Height)
            {
                _ball.IsAlive = false;
            }
            

            // Столкновение мячика с левым или правым краем игрового поля
            if ((nextRect.X >= _viewPortRectangle.Width - nextRect.Width) || nextRect.X <= 0)
            {
                _ball.ReflectVertical();
            }

            // Столкновение мячика с ракеткой
            if (nextRect.Intersects(_paddle.Bounds))
                Collide(_ball, _paddle.Bounds);

            // Столкновение мячика с кирпичами
            foreach (var brick in _bricks)
            {
                if (nextRect.Intersects(brick.Bounds) && brick.IsAlive)
                {
                    brick.IsAlive = false;
                    Collide(_ball, brick.Bounds);
                }
            }

            _ball.Position += _ball.Velocity;
        }
        public void Collide(Game1 gameObject, Rectangle rect2)
        {

            // Обьект столкнулся сверху или снизу, отражаем направление полета по горизонтали
            if (rect2.Left <= gameObject.Bounds.Center.X && gameObject.Bounds.Center.X <= rect2.Right)
                gameObject.ReflectHorizontal();

            // Обьект столкнулся слева или справа, отражаем направление полета по вертикали
            else if (rect2.Top <= gameObject.Bounds.Center.Y && gameObject.Bounds.Center.Y <= rect2.Bottom)
                gameObject.ReflectVertical();
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // TODO: Add your update logic here
            KeyboardState keyboardState = Keyboard.GetState();

            // Двигаем ракетку вправо
            if (keyboardState.IsKeyDown(Keys.Right))
                _paddle.Position.X += 10f;

            // Двигаем ракетку влево
            if (keyboardState.IsKeyDown(Keys.Left))
                _paddle.Position.X -= 10f;

            // Ограничиваем движение ракетки игровым полем
            _paddle.Position.X = MathHelper.Clamp(_paddle.Position.X, 0, _viewPortRectangle.Width - _paddle.Width);

            IsMouseVisible = true;
            if (keyboardState.IsKeyDown(Keys.Space))
                UpdateBall();
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            // Рисуем фон
            spriteBatch.Draw(_background, _viewPortRectangle, Color.White);
            spriteBatch.Draw(_paddle.Sprite, _paddle.Position, Color.White);

            foreach (var brick in _bricks)
                if (brick.IsAlive)
                    spriteBatch.Draw(brick.Sprite, brick.Position, Color.White);
            spriteBatch.Draw(_ball.Sprite, _ball.Position, Color.White);
            if (_ball.IsAlive == false)
            {
                spriteBatch.DrawString(font, "GAME OVER", new Vector2(_viewPortRectangle.Height / 2, _viewPortRectangle.Width / 2), Color.White);
               
            }



            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        
     /*
        Осталось сделать:
        1. Дезигн (масштабы, цветовая гамма);
        2. Пробелы между кирпичиками;
        3. Пофиксить управление (чтобы мяч вылетал на старте от пробела, не задерживался на ракетке, "не зажимало в углу", ракетка ездит реще, активнее, быстрее);
        4. Таймер;
        5. Текст с началом и концом;
        6. Подсчет очков;
        7. Сделать код красивым и читабельным;
        8.* Траектория мячика.
     */ 
    }


     
}
