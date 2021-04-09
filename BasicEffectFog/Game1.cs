using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BasicEffectFog
{
	/// <summary>
	/// ゲームメインクラス
	/// </summary>
	public class Game1 : Game
	{
    /// <summary>
    /// グラフィックデバイス管理クラス
    /// </summary>
    private readonly GraphicsDeviceManager _graphics = null;

    /// <summary>
    /// スプライトのバッチ化クラス
    /// </summary>
    private SpriteBatch _spriteBatch = null;

    /// <summary>
    /// スプライトでテキストを描画するためのフォント
    /// </summary>
    private SpriteFont _font = null;

    /// <summary>
    /// 直前のキーボード入力の状態
    /// </summary>
    private KeyboardState _oldKeyboardState = new KeyboardState();

    /// <summary>
    /// 直前のマウスの状態
    /// </summary>
    private MouseState _oldMouseState = new MouseState();

    /// <summary>
    /// 直前のゲームパッド入力の状態
    /// </summary>
    private GamePadState _oldGamePadState = new GamePadState();

    /// <summary>
    /// モデル
    /// </summary>
    private Model _model = null;

    /// <summary>
    /// フォグの有効フラグ
    /// </summary>
    private bool _fogEnabled = true;

    /// <summary>
    /// フォグの色
    /// </summary>
    private Vector3 _fogColor = Vector3.One;

    /// <summary>
    /// フォグの開始距離
    /// </summary>
    private float _fogStart = 60.0f;

    /// <summary>
    /// フォグの終了距離
    /// </summary>
    private float _fogEnd = 100.0f;

    /// <summary>
    /// 選択しているメニューのインデックス
    /// </summary>
    private int _selectedMenuIndex = 0;

    /// <summary>
    /// メニューリスト
    /// </summary>
    private static string[] MenuNameList = new string[]
        {
                "Enabled",
                "Color (Red)",
                "Color (Green)",
                "Color (Blue)",
                "Start",
                "End"
        };

    /// <summary>
    /// パラメータテキストリスト
    /// </summary>
    private string[] parameters = new string[6];


    /// <summary>
    /// GameMain コンストラクタ
    /// </summary>
    public Game1()
    {
      // グラフィックデバイス管理クラスの作成
      _graphics = new GraphicsDeviceManager(this);

      // ゲームコンテンツのルートディレクトリを設定
      Content.RootDirectory = "Content";

      // ウインドウ上でマウスのポインタを表示するようにする
      IsMouseVisible = true;
    }

    /// <summary>
    /// ゲームが始まる前の初期化処理を行うメソッド
    /// グラフィック以外のデータの読み込み、コンポーネントの初期化を行う
    /// </summary>
    protected override void Initialize()
    {
      // TODO: ここに初期化ロジックを書いてください

      // コンポーネントの初期化などを行います
      base.Initialize();
    }

    /// <summary>
    /// ゲームが始まるときに一回だけ呼ばれ
    /// すべてのゲームコンテンツを読み込みます
    /// </summary>
    protected override void LoadContent()
    {
      // テクスチャーを描画するためのスプライトバッチクラスを作成します
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      // フォントをコンテンツパイプラインから読み込む
      _font = Content.Load<SpriteFont>("Font");

      // モデルを作成
      _model = Content.Load<Model>("Model");

      // ライトとビュー、プロジェクションはあらかじめ設定しておく
      foreach (ModelMesh mesh in _model.Meshes)
      {
        foreach (BasicEffect effect in mesh.Effects)
        {
          // デフォルトのライト適用
          effect.EnableDefaultLighting();

          // ビューマトリックスをあらかじめ設定
          effect.View = Matrix.CreateLookAt(
              new Vector3(0.0f, 30.0f, 50.0f),
              new Vector3(0.0f, -10.0f, 0.0f),
              Vector3.Up
          );

          // プロジェクションマトリックスをあらかじめ設定
          effect.Projection = Matrix.CreatePerspectiveFieldOfView(
              MathHelper.ToRadians(45.0f),
              (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height,
              1.0f,
              100.0f
          );
        }
      }
    }

    /// <summary>
    /// ゲームが終了するときに一回だけ呼ばれ
    /// すべてのゲームコンテンツをアンロードします
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: ContentManager で管理されていないコンテンツを
      //       ここでアンロードしてください
    }

    /// <summary>
    /// 描画以外のデータ更新等の処理を行うメソッド
    /// 主に入力処理、衝突判定などの物理計算、オーディオの再生など
    /// </summary>
    /// <param name="gameTime">このメソッドが呼ばれたときのゲーム時間</param>
    protected override void Update(GameTime gameTime)
    {
      // 入力デバイスの状態取得
      KeyboardState keyboardState = Keyboard.GetState();
      MouseState mouseState = Mouse.GetState();
      GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

      // ゲームパッドの Back ボタン、またはキーボードの Esc キーを押したときにゲームを終了させます
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      {
        Exit();
      }

      // メニューの選択
      if ((keyboardState.IsKeyDown(Keys.Up) && _oldKeyboardState.IsKeyUp(Keys.Up)) ||
          (gamePadState.ThumbSticks.Left.Y >= 0.5f &&
              _oldGamePadState.ThumbSticks.Left.Y < 0.5f))
      {
        // 選択メニューをひとつ上に移動
        _selectedMenuIndex =
            (_selectedMenuIndex + parameters.Length - 1) % parameters.Length;
      }
      if ((keyboardState.IsKeyDown(Keys.Down) && _oldKeyboardState.IsKeyUp(Keys.Down)) ||
          (gamePadState.ThumbSticks.Left.Y <= -0.5f &&
              _oldGamePadState.ThumbSticks.Left.Y > -0.5f) ||
          (_oldMouseState.LeftButton == ButtonState.Pressed &&
           mouseState.LeftButton == ButtonState.Released))
      {
        // 選択メニューをひとつ下に移動
        _selectedMenuIndex =
            (_selectedMenuIndex + parameters.Length + 1) % parameters.Length;
      }

      // 各マテリアルの値を操作
      float moveValue = 0.0f;
      if (keyboardState.IsKeyDown(Keys.Left))
      {
        moveValue -= (float)gameTime.ElapsedGameTime.TotalSeconds;
      }
      if (keyboardState.IsKeyDown(Keys.Right))
      {
        moveValue += (float)gameTime.ElapsedGameTime.TotalSeconds;
      }
      if (mouseState.LeftButton == ButtonState.Pressed)
      {
        moveValue += (mouseState.X - _oldMouseState.X) * 0.005f;
      }
      if (gamePadState.IsConnected)
      {
        moveValue += gamePadState.ThumbSticks.Left.X *
                     (float)gameTime.ElapsedGameTime.TotalSeconds;
      }

      if (moveValue != 0.0f)
      {
        switch (_selectedMenuIndex)
        {
        case 0: // フォグの有効フラグ
          _fogEnabled = (moveValue > 0.0f);
          break;
        case 1: // フォグの色 (赤)
          _fogColor.X = MathHelper.Clamp(_fogColor.X + moveValue,
                                             0.0f,
                                             1.0f);
          break;
        case 2: // フォグの色 (緑)
          _fogColor.Y = MathHelper.Clamp(_fogColor.Y + moveValue,
                                             0.0f,
                                             1.0f);
          break;
        case 3: // フォグの色 (青)
          _fogColor.Z = MathHelper.Clamp(_fogColor.Z + moveValue,
                                             0.0f,
                                             1.0f);
          break;
        case 4: // フォグの開始距離
          _fogStart = MathHelper.Clamp(_fogStart + moveValue * 10.0f,
                                           0.0f,
                                           100.0f);
          break;
        case 5: // フォグの終了距離
          _fogEnd = MathHelper.Clamp(_fogEnd + moveValue * 10.0f,
                                         0.0f,
                                         100.0f);
          break;
        }
      }

      // フォグを設定
      foreach (ModelMesh mesh in _model.Meshes)
      {
        foreach (BasicEffect effect in mesh.Effects)
        {
          // フォグの有効フラグ
          effect.FogEnabled = _fogEnabled;

          // フォグの色
          effect.FogColor = _fogColor;

          // フォグの開始距離
          effect.FogStart = _fogStart;

          // フォグの終了距離
          effect.FogEnd = _fogEnd;
        }
      }

      // 入力情報を記憶
      _oldKeyboardState = keyboardState;
      _oldMouseState = mouseState;
      _oldGamePadState = gamePadState;

      // 登録された GameComponent を更新する
      base.Update(gameTime);
    }

    /// <summary>
    /// 描画処理を行うメソッド
    /// </summary>
    /// <param name="gameTime">このメソッドが呼ばれたときのゲーム時間</param>
    protected override void Draw(GameTime gameTime)
    {
      // 画面を指定した色でクリアします
      GraphicsDevice.Clear(Color.CornflowerBlue);

      // 深度バッファを有効にする
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;

      // モデルを描画
      foreach (ModelMesh mesh in _model.Meshes)
      {
        mesh.Draw();
      }

      // スプライトの描画準備
      _spriteBatch.Begin();

      // 操作
      _spriteBatch.DrawString(_font,
          "Up, Down : Select Menu",
          new Vector2(20.0f, 20.0f), Color.Black);
      _spriteBatch.DrawString(_font,
          "Left, right : Change Value",
          new Vector2(20.0f, 45.0f), Color.Black);
      _spriteBatch.DrawString(_font,
          "MouseClick & Drag :",
          new Vector2(20.0f, 70.0f), Color.Black);
      _spriteBatch.DrawString(_font,
          "    Select Menu & Change Value",
          new Vector2(20.0f, 95.0f), Color.Black);

      _spriteBatch.DrawString(_font,
          "Up, Down : Select Menu",
          new Vector2(19.0f, 19.0f), Color.White);
      _spriteBatch.DrawString(_font,
          "Left, right : Change Value",
          new Vector2(19.0f, 44.0f), Color.White);
      _spriteBatch.DrawString(_font,
          "MouseClick & Drag :",
          new Vector2(19.0f, 69.0f), Color.White);
      _spriteBatch.DrawString(_font,
          "    Select Menu & Change Value",
          new Vector2(19.0f, 94.0f), Color.White);

      // 各メニュー //
      for (int i = 0; i < MenuNameList.Length; i++)
      {
        _spriteBatch.DrawString(_font,
            MenuNameList[i],
            new Vector2(40.0f, 120.0f + i * 20.0f), Color.Black);
        _spriteBatch.DrawString(_font,
            MenuNameList[i],
            new Vector2(39.0f, 119.0f + i * 20.0f), Color.White);
      }

      // 各パラメータ //

      // フォグの有効フラグ
      parameters[0] = _fogEnabled.ToString();

      // フォグの色 (赤)
      parameters[1] = _fogColor.X.ToString();

      // フォグの色 (緑)
      parameters[2] = _fogColor.Y.ToString();

      // フォグの色 (青)
      parameters[3] = _fogColor.Z.ToString();

      // フォグの開始距離
      parameters[4] = _fogStart.ToString();

      // フォグの終了距離
      parameters[5] = _fogEnd.ToString();

      for (int i = 0; i < parameters.Length; i++)
      {
        _spriteBatch.DrawString(_font,
            parameters[i],
            new Vector2(220.0f, 120.0f + i * 20.0f), Color.Black);
        _spriteBatch.DrawString(_font,
            parameters[i],
            new Vector2(219.0f, 119.0f + i * 20.0f), Color.White);
      }

      // 選択インデックス
      _spriteBatch.DrawString(_font, "*",
          new Vector2(20.0f, 120.0f + _selectedMenuIndex * 20.0f), Color.Black);
      _spriteBatch.DrawString(_font, "*",
          new Vector2(19.0f, 119.0f + _selectedMenuIndex * 20.0f), Color.White);

      // スプライトの一括描画
      _spriteBatch.End();

      // 登録された DrawableGameComponent を描画する
      base.Draw(gameTime);
    }
  }
}
