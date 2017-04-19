using System.Drawing;
using System.Runtime.CompilerServices;
using GTA;
using MapInfoTool.Interfaces;

namespace MapInfoTool.Helpers
{
    /// <summary>
    /// Represents a fast- changing text array that is drawn on the screen.
    /// </summary>
    public class FastUiArray : IDrawable
    {
        public const int NumTextLines = 20;

        public const int NumTextColumns = 2;

        public const int TextActiveTime = 5000;

        private int _textAddedTime;

        private UIText[,] _columns;

        private readonly UIContainer _backdrop;

        private readonly UIText _titleText;

        private readonly Color _backdropColour;

        private readonly Color _textColour;

        public FastUiArray(string title)
        {
            _backdropColour = Color.FromArgb(190, Color.Black);
            _textColour = Color.White;
            _titleText = new UIText("", new Point(16, 8), 0.52f, Color.Empty, GTA.Font.ChaletComprimeCologne, false);
            _backdrop = new UIContainer(new Point((int)(UI.WIDTH * 0.75f), UI.HEIGHT / 2 - 200), new Size(264, 400), Color.Empty);
            _backdrop.Items.Add(_titleText);
            SetTitle(title);
            SetupText();
        }

        private void SetupText()
        {
            _columns = new UIText[NumTextColumns, NumTextLines];

            for (int i = 0; i < NumTextColumns; i++)
            {
                for (int x = 0; x < NumTextLines; x++)
                {
                    _columns[i, x] = new UIText("", new Point(16 + (180 * i), 20 + (18 * x)), 0.3f, Color.White);

                    _backdrop.Items.Add(_columns[i, x]);
                }
            }
        }

        public void SetTitle(string str)
        {
            _titleText.Caption = str;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetText(int lineIndex, params string[] text)
        {
            var gameTime = Game.GameTime;

            if (gameTime > _textAddedTime + TextActiveTime)
            {
                Show();
            }

            for (int i = 0; i < System.Math.Min(text.Length, NumTextColumns); i++)
            {
                _columns[i, lineIndex].Caption = 
                    text[i].Length > 24 ? text[i].Substring(0, 21) + "..." : text[i];
            }     

            _textAddedTime = gameTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetText(int columnIndex, int lineIndex, string text)
        {
            var gameTime = Game.GameTime;

            if (gameTime > _textAddedTime + TextActiveTime)
            {
                Show();
            }

            _columns[columnIndex, lineIndex].Caption = 
                text.Length > 24 ? text.Substring(0, 21) + "..." : text;

            _textAddedTime = gameTime;
        }

        public void Show()
        {
            _backdrop.Color = _backdropColour;

            _titleText.Color = _textColour;

            for (int i = 0; i < NumTextColumns; i++)
            {
                for (int x = 0; x < NumTextLines; x++)
                {
                    _columns[i, x].Color = _textColour;
                }
            }
        }

        public void Draw()
        {
            if (Game.GameTime > _textAddedTime + TextActiveTime)
            {
                if (_titleText.Color.A > 0)
                    _titleText.Color = Color.FromArgb(System.Math.Max(0, _titleText.Color.A - 8), _titleText.Color);

                if (_backdrop.Color.A > 0)
                    _backdrop.Color = Color.FromArgb(System.Math.Max(0, _backdrop.Color.A - 8), _backdrop.Color);

                for (int i = 0; i < NumTextColumns; i++)
                {
                    for (int x = 0; x < NumTextLines; x++)
                    {
                        if (_columns[i, x].Color.A > 0)
                        {
                            _columns[i, x].Color = Color.FromArgb(System.Math.Max(0, _columns[i, x].Color.A - 8), _columns[i, x].Color);
                        }
                    }
                }
            }

            _backdrop.Draw();
        }        

        public void Clear()
        {
            for (int i = 0; i < NumTextColumns; i++)
            {
                for (int x = 0; x < NumTextLines; x++)
                {
                    _columns[i, x].Caption = string.Empty;
                }
            }
        }
    }
}
