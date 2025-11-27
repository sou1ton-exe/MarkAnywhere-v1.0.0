using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace MarkAnywhere
{
    public partial class MainForm : Form
    {
        private bool isDrawing = false;
        private Point lastPoint;
        private Pen currentPen;
        private Bitmap canvas;
        private Graphics canvasGraphics;
        private Color currentColor = Color.Red;
        private int penSize = 3;
        private bool isEraser = false;

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_LAYERED = 0xFFFFFFF;
        const int LWA_ALPHA = 0x2;

        public MainForm()
        {
            InitializeComponent();
            InitializeDrawing();

            Bounds = Screen.PrimaryScreen.Bounds;
            Location = new Point(0, 0);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            BackColor = Color.Magenta;
            FormBorderStyle = FormBorderStyle.None;
            Name = "MainForm";
            Text = "DrawAnywhere";
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;
            KeyPreview = true;
            TopMost = false;
            ShowInTaskbar = false;
            TransparencyKey = Color.Magenta;

            MouseDown += MainForm_MouseDown;
            MouseMove += MainForm_MouseMove;
            MouseUp += MainForm_MouseUp;
            Paint += MainForm_Paint;
            KeyDown += MainForm_KeyDown;

            ResumeLayout(false);
        }

        private void InitializeDrawing()
        {

            canvas = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            canvasGraphics = Graphics.FromImage(canvas);
            canvasGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            canvasGraphics.Clear(Color.Transparent);

            UpdatePen();
            SetupTransparency();
        }

        private void SetupTransparency()
        {
            if (IsHandleCreated)
            {
                int style = GetWindowLong(Handle, GWL_EXSTYLE);
                SetWindowLong(Handle, GWL_EXSTYLE, style | WS_EX_LAYERED);
                SetLayeredWindowAttributes(Handle, 0xFFFFFFF, 200, LWA_ALPHA);
            }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                lastPoint = e.Location;

                Console.WriteLine($"Mouse DOWN at {e.Location}");

                if (isEraser) ErasePoint(e.Location);
                else canvasGraphics.DrawEllipse(currentPen, e.X, e.Y, 1, 1);

                Invalidate();
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                Console.WriteLine($"Drawing from {lastPoint} to {e.Location}");

                if (isEraser)
                {
                    EraseLine(lastPoint, e.Location);
                }
                else
                {
                    canvasGraphics.DrawLine(currentPen, lastPoint, e.Location);
                }
                lastPoint = e.Location;
                this.Invalidate();
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = false;
                Console.WriteLine("Mouse UP");
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Transparent);
            e.Graphics.DrawImage(canvas, 0, 0);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
                case Keys.C:
                    ClearCanvas();
                    break;
                case Keys.E:
                    SetEraser();
                    break;
                case Keys.R:
                    SetDrawingColor(Color.Red);
                    break;
                case Keys.G:
                    SetDrawingColor(Color.Green);
                    break;
                case Keys.B:
                    SetDrawingColor(Color.Blue);
                    break;
                case Keys.Add:
                case Keys.Oemplus:
                    penSize = Math.Min(20, penSize + 1);
                    UpdatePen();
                    break;
                case Keys.Subtract:
                case Keys.OemMinus:
                    penSize = Math.Max(1, penSize - 1);
                    UpdatePen();
                    break;
            }
        }

        private void UpdatePen()
        {
            currentPen?.Dispose();
            if (isEraser)
            {
                currentPen = new Pen(Color.Transparent, penSize * 4);
            }
            else
            {
                currentPen = new Pen(currentColor, penSize);
            }

            currentPen.StartCap = LineCap.Round;
            currentPen.EndCap = LineCap.Round;
            currentPen.LineJoin = LineJoin.Round;
        }

        public void SetDrawingColor(Color color)
        {
            isEraser = false;
            currentColor = color;
            UpdatePen();
            Console.WriteLine($"Color changed to {color}");
        }

        public void SetEraser()
        {
            isEraser = true;
            UpdatePen();
            Console.WriteLine("Eraser activated");
        }

        public void ClearCanvas()
        {
            canvasGraphics.Clear(Color.Transparent);
            Invalidate();
            Console.WriteLine("Canvas cleared");
        }

        private void ErasePoint(Point point)
        {
            using (var erasePen = new Pen(Color.Transparent, penSize * 4))
            {
                erasePen.StartCap = LineCap.Round;
                erasePen.EndCap = LineCap.Round;
                canvasGraphics.DrawEllipse(erasePen, point.X, point.Y, 1, 1);
            }
        }

        private void EraseLine(Point start, Point end)
        {
            using (var erasePen = new Pen(Color.Transparent, penSize * 4))
            {
                erasePen.StartCap = LineCap.Round;
                erasePen.EndCap = LineCap.Round;
                canvasGraphics.DrawLine(erasePen, start, end);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            currentPen?.Dispose();
            canvasGraphics?.Dispose();
            canvas?.Dispose();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            System.Threading.Thread.Sleep(100);
            SetupTransparency();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Bounds = Screen.PrimaryScreen.Bounds;
            Location = new Point(0, 0);
        }
    }
}