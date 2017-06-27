using System;
using System.Drawing;
using GTA;

namespace MapInfoTool.Helpers
{
    public class FrontendOutput
    {
        private const int TextActiveTime = 10000;

        private readonly UIContainer _backsplash;

        private readonly string[] _messageQueue = new string[100];

        private readonly UIText[] _text = new UIText[10];

        private bool _startFromTop = true;

        private bool _stayOnScreen;

        private int _scrollIndex;

        private int _shownTime;

        private int _linesCount;

        /// <summary>
        /// Write a new line to the message queue with the given format.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        /// <summary>
        /// Write a new line to the message queue.
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text)
        {
            Show();

            for (int i = _messageQueue.Length - 1; i > 0; i--)
            {
                _messageQueue[i] = _messageQueue[i - 1];
            }

            _messageQueue[0] = string.Format("~4~[{0}]:   {1}", DateTime.Now.ToShortTimeString(), text);

            _linesCount = Math.Min(_linesCount + 1, _messageQueue.Length);
        }

        private void SetTextColor(Color color)
        {
            for (int i = 0; i < _text.Length; i++)
            {
                _text[i].Color = color;
            }
        }

        public FrontendOutput()
        {
            _backsplash = new UIContainer(new Point(630, 460), new Size(600, 200), Color.Empty);
            CreateText();
        }

        public void Show()
        {
            _backsplash.Color = Color.FromArgb(140, Color.Black);

            SetTextColor(Color.White);

            _shownTime = Game.GameTime;
        }

        public void Hide()
        {
            _backsplash.Color = Color.Empty;

            SetTextColor(Color.Empty);

            _shownTime = 0;
        }

        public void ScrollUp()
        {
            if (_scrollIndex > 0)
                _scrollIndex--;
        }

        public void ScrollDown()
        {
            if (_scrollIndex < _linesCount - 10)
                _scrollIndex++;
        }

        public void DisableFadeOut()
        {
            _stayOnScreen = true;
        }

        public void EnableFadeOut()
        {
            _stayOnScreen = false;
        }

        private void CreateText()
        {
            for (int i = 0; i < _text.Length; i++)
            {
                _text[i] = new UIText(string.Empty, new Point(14, 11 + 18 * i), 0.3f, Color.Empty);
            }

            _backsplash.Items.AddRange(_text);
        }

        public void Update(int gameTime)
        {
            if (gameTime > _shownTime + TextActiveTime && !_stayOnScreen)
            {
                if (_backsplash.Color.A > 0)
                    _backsplash.Color = Color.FromArgb(Math.Max(0, _backsplash.Color.A - 2), _backsplash.Color);

                for (int i = 0; i < _text.Length; i++)
                {
                    if (_text[i].Color.A > 0)
                    {
                        _text[i].Color = Color.FromArgb(Math.Max(0, _text[i].Color.A - 4), _text[i].Color);
                    }
                }
            }

            else
            {
                for (int i = _text.Length - 1; i > -1; i--)
                {
                    _text[i].Caption = _messageQueue[
                                           _startFromTop
                                               ? i + _scrollIndex
                                               : (_text.Length - 1) - i + _scrollIndex] ?? string.Empty;
                }
            }

            _backsplash.Draw();
        }
    }
}
