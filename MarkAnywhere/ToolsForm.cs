using System;
using System.Drawing;
using System.Windows.Forms;


namespace MarkAnywhere
{
    public partial class ToolsForm : Form
    {
        private MainForm mainForm;

        private Color[] colors = new Color[]
        {
            Color.Red, Color.Green, Color.Blue, Color.Black,
            Color.Yellow, Color.Orange, Color.Purple, Color.White
        };

        public ToolsForm(MainForm mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
            SetupTools();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            BackColor = Color.FromArgb(45, 45, 48);
            ClientSize = new Size(420, 50);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ToolsBar";
            Text = "Tools";
            TopMost = true;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            ResumeLayout(false);
        }

        private void SetupTools()
        {
            int x = 10;
            int y = 10;
            int buttonSize = 30;

            foreach (var color in colors)
            {
                var colorButton = new Button();
                colorButton.BackColor = color;
                colorButton.Size = new Size(buttonSize, buttonSize);
                colorButton.Location = new Point(x, y);
                colorButton.FlatStyle = FlatStyle.Flat;
                colorButton.FlatAppearance.BorderSize = 2;
                colorButton.FlatAppearance.BorderColor = Color.White;
                colorButton.Click += (s, e) => SetColor(color);
                Controls.Add(colorButton);

                x += buttonSize + 5;
            }

            var eraserBtn = new Button();
            eraserBtn.Text = "E";
            eraserBtn.Size = new Size(40, 30);
            eraserBtn.Location = new Point(x, y);
            eraserBtn.BackColor = Color.Gray;
            eraserBtn.ForeColor = Color.White;
            eraserBtn.Click += (s, e) => SetEraser();
            Controls.Add(eraserBtn);

            x += 45;

            var clearBtn = new Button();
            clearBtn.Text = "C";
            clearBtn.Size = new Size(40, 30);
            clearBtn.Location = new Point(x, y);
            clearBtn.BackColor = Color.LightBlue;
            clearBtn.Click += (s, e) => ClearCanvas();
            Controls.Add(clearBtn);

            x += 45;

            var closeBtn = new Button();
            closeBtn.Text = "X";
            closeBtn.Size = new Size(40, 30);
            closeBtn.Location = new Point(x, y);
            closeBtn.BackColor = Color.Red;
            closeBtn.ForeColor = Color.White;
            closeBtn.Click += (s, e) => CloseAll();
            Controls.Add(closeBtn);
        }

        private void SetColor(Color color)
        {
            mainForm?.SetDrawingColor(color);
        }

        private void SetEraser()
        {
            mainForm?.SetEraser();
        }

        private void ClearCanvas()
        {
            mainForm?.ClearCanvas();
        }

        private void CloseAll()
        {
            Close();
            mainForm?.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var screen = Screen.PrimaryScreen;
            Location = new Point((screen.Bounds.Width - Width) / 2, 20);
        }
    }
}