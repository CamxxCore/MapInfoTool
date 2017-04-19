using System.Drawing;
using GTA;
using GTA.Native;
using MapInfoTool.Interfaces;

namespace MapInfoTool.Helpers
{
    /// <summary>
    /// Lightweight class for displaying user input on the screen.
    /// </summary>
    public class FrontendInput : IUpdatable
    {
        private const int CursorPulseSpeed = 300;

        private bool _cursorState;

        private int _lastCursorPulse;

        private int _currentTextWidth;

        private bool _active;

        private readonly UIContainer _backsplash;

        private readonly UIText _uitext;

        private readonly UIRectangle _cursor;

        private string _text = "";

        private const int TextActiveTime = 30000;

        private int _shownTime;

        /// <summary>
        /// Gets the current text in the input box.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return _text;
        }

        /// <summary>
        /// Add a line of text to the input box.
        /// </summary>
        /// <param name="text"></param>
        public void AddLine(string text)
        {
            Show();

            _text = text;

            _currentTextWidth = GetTextWidth();
        }

        /// <summary>
        /// Add a single character to the input box.
        /// </summary>
        public void AddChar(char c)
        {
            Show();

            _text += c;

            _currentTextWidth = GetTextWidth();
        }

        /// <summary>
        /// Remove the last character from the input box.
        /// </summary>
        public void RemoveLastChar()
        {
            if (_text.Length <= 0) return;

            _text = _text.Substring(0, _text.Length - 1);

            _currentTextWidth = GetTextWidth();
        }

        /// <summary>
        /// Show the input box.
        /// </summary>
        public void Show()
        {
            _backsplash.Color = Color.FromArgb(140, Color.Black);

            SetCursorColor(Color.White);

            SetTextColor(Color.White);

            _shownTime = Game.GameTime;

            _active = true;
        }

        /// <summary>
        /// Hide the input box.
        /// </summary>
        public void Hide()
        {
            _backsplash.Color = Color.Empty;

            SetCursorColor(Color.Empty);

            SetTextColor(Color.Empty);

            _active = false;
        }

        /// <summary>
        /// Clear the active text.
        /// </summary>
        public void Clear()
        {
            _text = string.Empty;

            _currentTextWidth = GetTextWidth();
        }

        /// <summary>
        /// Set the text color.
        /// </summary>
        /// <param name="color"></param>
        private void SetTextColor(Color color)
        {
            _uitext.Color = color;
        }

        /// <summary>
        /// Set the cursor color.
        /// </summary>
        /// <param name="color"></param>
        private void SetCursorColor(Color color)
        {
            _cursor.Color = color;
        }

        private int GetTextWidth()
        {
            Function.Call(CustomNative.SetTextEntryForWidth, "CELL_EMAIL_BCON");

            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, _text);

            Function.Call(Hash.SET_TEXT_FONT, (int)_uitext.Font);
            Function.Call(Hash.SET_TEXT_SCALE, _uitext.Scale, _uitext.Scale);

            return (int)(UI.WIDTH * Function.Call<float>((Hash)0x85F061DA64ED2F67, (int)_uitext.Font));
        }

        /// <summary>
        /// Initialize the class.
        /// </summary>
        public FrontendInput()
        {
            _backsplash = new UIContainer(new Point(630, 670), new Size(600, 30), Color.Empty);
            _backsplash.Items.Add(_cursor = new UIRectangle(new Point(14, 5), new Size(1, 15), Color.Empty));
            _backsplash.Items.Add(_uitext = new UIText("", new Point(14, 5), 0.3f));
        }

        /// <summary>
        /// Update and draw the this <see cref="FrontendInput"/> box.
        /// </summary>
        public void Update(int gameTime)
        {
            if (!_active) return;

            /*   if (gameTime > shownTime + TextActiveTime)
                {
                    active = false;

                    for (int i = 0; i < backsplash.Items.Count; i++)
                    {
                        if (backsplash.Items[i].Color.A > 0)
                        {
                            backsplash.Items[i].Color = Color.FromArgb(Math.Max(0, backsplash.Items[i].Color.A - 2), backsplash.Items[i].Color);

                            active = true;
                        }
                    }
                }

                else
                {*/
            Game.DisableAllControlsThisFrame(0);

            _uitext.Caption = _text;

            _cursor.Position = new Point(14 + _currentTextWidth, 7);

            if (gameTime - _lastCursorPulse > CursorPulseSpeed && _uitext.Color.A > 0)
            {
                _cursorState = !_cursorState;

                _cursor.Color = _cursorState ? Color.FromArgb(255, _cursor.Color) : Color.FromArgb(0, _cursor.Color);

                _lastCursorPulse = gameTime;
            }

            //}

            _backsplash.Draw();
        }
    }
}


