using System.Drawing;
using System.Windows.Forms;

class GameForm : Form
{
    public bool check_K_W = false;
    public bool check_K_A = false;
    public bool check_K_S = false;
    public bool check_K_D = false;
    public bool check_K_P = false;
    public bool check_K_Up = false;
    public bool check_K_Down = false;
    public bool check_K_Return = false;
    public bool check_K_Escape = false;
    public bool check_K_Space = false;
    public bool check_K_Shift = false;
    public bool check_M1 = false;
    public double[] mouse = new double[] { 0, 0 };
    public bool release_state = false;
    public PictureBox picbox;
    public GameForm(string txt) : base()
    {
        Size = new Size(1600, 1000);
        FormBorderStyle = FormBorderStyle.Fixed3D;
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = false;
        Text = txt;
        Font = new Font("Arial", 14, FontStyle.Bold);
        KeyPreview = true;
        DoubleBuffered = true;
        picbox = new PictureBox();
        picbox.SetBounds(5, 5, this.ClientSize.Width - 10, this.ClientSize.Height - 10);
        Controls.Add(picbox);
        this.KeyDown += (obj, ea) => {
            if (ea.KeyCode == Keys.Up) check_K_Up = true;
            if (ea.KeyCode == Keys.Down) check_K_Down = true;
            if (ea.KeyCode == Keys.Return) check_K_Return = true;
            if (ea.KeyCode == Keys.Escape) check_K_Escape = true;
            if (ea.KeyCode == Keys.Space) check_K_Space = true;
            if (ea.KeyCode == Keys.ShiftKey) check_K_Shift = true;
            if (ea.KeyCode == Keys.W) check_K_W = true;
            if (ea.KeyCode == Keys.A) check_K_A = true;
            if (ea.KeyCode == Keys.S) check_K_S = true;
            if (ea.KeyCode == Keys.D) check_K_D = true;
            if (ea.KeyCode == Keys.P)
            {
                if (check_K_P) check_K_P = false;
                else check_K_P = true;
            }
        };
        this.KeyUp += (obj, ea) => {
            if (ea.KeyCode == Keys.Up) check_K_Up = false;
            if (ea.KeyCode == Keys.Down) check_K_Down = false;
            if (ea.KeyCode == Keys.Return) check_K_Return = false;
            if (ea.KeyCode == Keys.Escape) check_K_Escape = false;
            if (ea.KeyCode == Keys.Space) check_K_Space = false;
            if (ea.KeyCode == Keys.ShiftKey) check_K_Shift = false;
            if (ea.KeyCode == Keys.W) check_K_W = false;
            if (ea.KeyCode == Keys.A) check_K_A = false;
            if (ea.KeyCode == Keys.S) check_K_S = false;
            if (ea.KeyCode == Keys.D) check_K_D = false;
        };
        picbox.MouseMove += (obj, ea) => {
            mouse[0] = ea.X;
            mouse[1] = ea.Y;
        };
        picbox.MouseDown += (obj, ea) => {
            check_M1 = true;
        };
        picbox.MouseUp += (obj, ea) => {
            check_M1 = false;
            release_state = true;
        };
    }
}