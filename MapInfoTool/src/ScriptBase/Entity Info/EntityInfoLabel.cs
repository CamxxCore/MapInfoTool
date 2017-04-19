using System.Drawing;
using GTA;
using GTA.Math;
using GTA.Native;

namespace MapInfoTool.ScriptBase.Entity_Info
{
    public class EntityInfoLabel
    {
        private readonly UIText[] _lines = new UIText[5];

        private readonly UIContainer _container;

        public Color Color {
            get { return _container.Color; }
            set { _container.Color = value; }
        }     

        public EntityInfoLabel()
        {
            SetupText();
            _container = new UIContainer(new Point(), new Size(200, 90), Color.FromArgb(158, Color.Black));
            _container.Items.AddRange(_lines);
        }

        private void SetupText()
        {
            for (int i = 0; i < _lines.Length; i++)
            {
                _lines[i] = new UIText("", new Point(20, 20 * i), 1.0f, Color.White, GTA.Font.ChaletComprimeCologne, false, true, true);
            }
        }

        public void SetText(int lineIndex, string str)
        {
            if (lineIndex > _lines.Length || lineIndex < 0) return;
            _lines[lineIndex].Caption = str;
        }

        public void DrawAtPosition(Vector3 position, float distFromCamera, int stackedIdx = 0)
        {
            var scale = 3.0f / distFromCamera;

            for (int i = 0; i < _lines.Length; i++)
            {
                _lines[i].Scale = scale * 0.37f;
                _lines[i].Position = new Point((int)(10 * scale), (int)((5 + 20 * i) * scale));
            }

            _container.Size = new Size((int)(200 * scale), (int)(90 * scale));

            var drawPos = new Vector3(position.X - 0.87f, position.Y, position.Z + 0.4f * stackedIdx);

            Function.Call(Hash.SET_DRAW_ORIGIN, drawPos.X, drawPos.Y, drawPos.Z, 0);

            _container.Draw();

            Function.Call(Hash.CLEAR_DRAW_ORIGIN);
        }
    }
}
