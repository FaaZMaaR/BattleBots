using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Threading;

class Vector
{
    public float[] coords;
    public Vector()
    {
        coords = new float[] { 0, 0 };
    }
    public float Scalar
    {
        get
        {
            return (float)Math.Sqrt(coords[0] * coords[0] + coords[1] * coords[1]);
        }
    }
    public float Scalar2
    {
        get
        {
            return coords[0] * coords[0] + coords[1] * coords[1];
        }
    }
    public float Angle
    {
        get
        {
            return (float)(Math.Atan2(coords[1], coords[0]) * 180 / Math.PI);
        }
    }
    public void Set_From_Ang(float ang, float s)
    {
        coords[0] = (float)(s * Math.Cos(ang * Math.PI / 180));
        coords[1] = (float)(s * Math.Sin(ang * Math.PI / 180));
    }
    public void Set_From_Coords(float x, float y)
    {
        coords[0] = x;
        coords[1] = y;
    }
    public void Set_Null()
    {
        coords = new float[] { 0, 0 };
    }
    public void Scale(float x, float y)
    {
        coords[0] *= x;
        coords[1] *= y;
    }
    public void Translate(float x, float y)
    {
        coords[0] += x;
        coords[1] += y;
    }
    public void Rotate(float ang)
    {
        float x = coords[0];
        float y = coords[1];
        coords[0] = (float)(x * Math.Cos(ang * Math.PI / 180) - y * Math.Sin(ang * Math.PI / 180));
        coords[1] = (float)(x * Math.Sin(ang * Math.PI / 180) + y * Math.Cos(ang * Math.PI / 180));
    }
    public Vector Normalize()
    {
        Vector v = new Vector();
        v.coords[0] = coords[0];
        v.coords[1] = coords[1];
        v.Scale(1 / Scalar, 1 / Scalar);
        return v;
    }
    public static Vector operator +(Vector a, Vector b)
    {
        Vector v = new Vector();
        v.Set_From_Coords(a.coords[0] + b.coords[0], a.coords[1] + b.coords[1]);
        return v;
    }
    public static Vector operator -(Vector a, Vector b)
    {
        Vector v = new Vector();
        v.Set_From_Coords(a.coords[0] - b.coords[0], a.coords[1] - b.coords[1]);
        return v;
    }
    public static float operator *(Vector a, Vector b)
    {
        return a.coords[0] * b.coords[0] + a.coords[1] * b.coords[1];
    }
}

class Bot
{
    public float[] coords = new float[] { 0, 0 };
    public Vector velocity = new Vector();
    public float angle = -90;
    public float hp, damage, speed;
    public Image img;
    public char direction = 'n';
    public int time;
    public Rocket rocket;
    public Bot(float h, float d, float s, int t, string i)
    {
        hp = h;
        damage = d;
        speed = s;
        time = t;
        img = Image.FromFile(i);
    }
    public void Set_Velocity()
    {
        switch (direction)
        {
            case 'f':
                velocity.Set_From_Ang(angle, speed * time / 1000);
                break;
            case 'b':
                velocity.Set_From_Ang(angle - 180, speed * time / 1000);
                break;
            case 'r':
                velocity.Set_From_Ang(angle + 90, speed * time / 1000);
                break;
            case 'l':
                velocity.Set_From_Ang(angle - 90, speed * time / 1000);
                break;
            default:
                velocity.Set_Null();
                break;
        }
    }
    public void Move()
    {
        coords[0] += velocity.coords[0];
        coords[1] += velocity.coords[1];
    }
    public void Fire()
    {
        rocket = new Rocket(this);
    }
    public void Draw(Graphics g, float[] map)
    {
        System.Drawing.Drawing2D.GraphicsState state = g.Save();
        g.TranslateTransform(coords[0] + map[0], coords[1] + map[1]);
        g.RotateTransform(angle + 90);
        g.DrawImage(img, -img.Width / 2, -img.Height / 2);
        g.Restore(state);
    }
}

class Rocket
{
    public float[] start_coords = new float[] { 0, 0 };
    public float[] end_coords = new float[] { 0, 0 };
    public Vector velocity = new Vector();
    public Vector distance = new Vector();
    public float damage = 0;
    public float range = 800;
    public bool fire = false;
    public bool destroyed = false;
    public Rocket(Bot bot)
    {
        Vector v = new Vector();
        v.Set_From_Ang(bot.angle, 25);
        start_coords[0] = bot.coords[0] + v.coords[0];
        start_coords[1] = bot.coords[1] + v.coords[1];
        end_coords[0] = start_coords[0];
        end_coords[1] = start_coords[1];
        velocity = v.Normalize();
        velocity.Scale(300 * bot.time / 1000, 300 * bot.time / 1000);
        damage = bot.damage;
        fire = true;
    }
    public void Move()
    {
        if (this != null)
        {
            distance.Set_From_Coords(end_coords[0] - start_coords[0], end_coords[1] - start_coords[1]);
            if (distance.Scalar2 <= range * range)
            {
                end_coords[0] += velocity.coords[0];
                end_coords[1] += velocity.coords[1];
            }
            else
            {
                destroyed = true;
            }
        }
    }
    public void Draw(Graphics g, float[] map)
    {
        g.FillEllipse(new SolidBrush(Color.Magenta), end_coords[0] + map[0] - 5, end_coords[1] + map[1] - 5, 10, 10);
    }
}

class Bot_Handler
{
    public Bot bot;
    public Minimap map, cur_map;
    public float turn_speed = 90;
    public float duration = 0;
    public float counter = 0;
    public float new_angle = 0;
    public float angle_vision = 45;
    public float radius_vision = 300;
    public int next_pos_i = 0;
    public string state = "idle";
    public bool ready = true;
    public bool enemy_spotted = false;
    public bool enemy_lost = false;
    public int[] destination;
    public int[] cur_position;
    public float[] next_position = new float[] { 0, 0 };
    public ArrayList path;
    public Queue cmds;
    public Bot_Handler(float h, float d, float s, int t, string i)
    {
        bot = new Bot(h, d, s, t, i);
        map = new Minimap();
    }
    public void Act(string s)
    {
        if (ready)
        {
            string[] cur_state = s.Split();
            state = cur_state[0];
            duration = Int32.Parse(cur_state[1]);
            counter = 0;
            ready = false;
            if (state == "turn")
            {
                new_angle = (bot.angle + duration) % 360;
                if (new_angle > 180) new_angle -= 360;
                if (new_angle < -180) new_angle += 360;
                next_position[0] = bot.coords[0];
                next_position[1] = bot.coords[1];
            }
            if (state == "rotate")
            {
                duration = new_angle;
            }
            if (state == "regroup")
            {
                float x = ((int)(bot.coords[0] - 640) / 160) * 160 + 720;
                float y = ((int)(bot.coords[1] - 640) / 160) * 160 + 720;
                if (new_angle == 0)
                {
                    if ((x - bot.coords[0]) > 0) duration = x - bot.coords[0];
                    else if ((x - bot.coords[0]) < 0) duration = x + 160 - bot.coords[0];
                    else duration = 0;
                }
                else if (new_angle == 90)
                {
                    if ((y - bot.coords[1]) > 0) duration = y - bot.coords[1];
                    else if ((y - bot.coords[1]) < 0) duration = y + 160 - bot.coords[1];
                    else duration = 0;
                }
                else if (new_angle == -90)
                {
                    if ((bot.coords[1] - y) > 0) duration = bot.coords[1] - y;
                    else if ((bot.coords[1] - y) < 0) duration = bot.coords[1] - y + 160;
                    else duration = 0;
                }
                else
                {
                    if ((bot.coords[0] - x) > 0) duration = bot.coords[0] - x;
                    else if ((bot.coords[0] - x) < 0) duration = bot.coords[0] - x + 160;
                    else duration = 0;
                }
            }
        }
        else
        {
            if (state == "turn")
            {
                if (duration >= 0)
                {
                    if (counter < duration)
                    {
                        bot.angle += turn_speed * bot.time / 1000;
                        counter += turn_speed * bot.time / 1000;
                    }
                    else
                    {
                        ready = true;
                        bot.angle = new_angle;
                    }
                }
                else
                {
                    if (counter > duration)
                    {
                        bot.angle -= turn_speed * bot.time / 1000;
                        counter -= turn_speed * bot.time / 1000;
                    }
                    else
                    {
                        ready = true;
                        bot.angle = new_angle;
                    }
                }
            }
            else if (state == "rotate")
            {
                if (duration - bot.angle > 1) bot.angle += turn_speed * bot.time / 1000;
                else if (duration - bot.angle < -1) bot.angle -= turn_speed * bot.time / 1000;
                else
                {
                    bot.angle = duration;
                    ready = true;
                }
            }
            else if (state == "idle")
            {
                if (counter < duration)
                {
                    counter += 1;
                }
                else ready = true;
            }
            else if (state == "forward")
            {
                if (counter < duration * 160)
                {
                    bot.velocity.Set_From_Ang(bot.angle, bot.speed * bot.time / 1000);
                    bot.Move();
                    counter += bot.velocity.Scalar;
                }
                else
                {
                    ready = true;
                    next_pos_i += 1;
                }
            }
            else if (state == "back")
            {
                if (counter < duration * 160)
                {
                    bot.velocity.Set_From_Ang(bot.angle - 180, bot.speed * bot.time / 1000);
                    bot.Move();
                    counter += bot.velocity.Scalar;
                }
                else
                {
                    ready = true;
                }
            }
            else if (state == "regroup")
            {
                if (counter < duration)
                {
                    bot.velocity.Set_From_Ang(bot.angle, bot.speed * bot.time / 1000);
                    bot.Move();
                    counter += bot.velocity.Scalar;
                }
                else
                {
                    ready = true;
                }
            }
        }
    }
    public bool Check_Cell(int x, int y)
    {
        if (cur_map.cells[x, y])
        {
            cur_map.cells[cur_position[0], cur_position[1]] = false;
            cur_position[0] = x;
            cur_position[1] = y;
            path.Add(new int[] { x, y });
            return true;
        }
        else return false;
    }
    public void Reach(int m, int n)
    {
        next_pos_i = 0;
        destination = new int[] { m, n };
        cur_position = new int[] { (int)(bot.coords[1] - 640) / 160, (int)(bot.coords[0] - 640) / 160 };
        map = new Minimap();
        cur_map = new Minimap();
        path = new ArrayList();
        cmds = new Queue();
        int i, j;
        if (destination[0] == cur_position[0] && destination[1] == cur_position[1]) return;
        while (cur_position[0] != destination[0] || cur_position[1] != destination[1])
        {
            i = destination[0] - cur_position[0];
            if (i > 0)
            {
                if (Check_Cell(cur_position[0] + 1, cur_position[1])) continue;
            }
            if (i < 0)
            {
                if (Check_Cell(cur_position[0] - 1, cur_position[1])) continue;
            }
            j = destination[1] - cur_position[1];
            if (j > 0)
            {
                if (Check_Cell(cur_position[0], cur_position[1] + 1)) continue;
            }
            if (j < 0)
            {
                if (Check_Cell(cur_position[0], cur_position[1] - 1)) continue;
            }
            if (i > 0)
            {
                if (Check_Cell(cur_position[0] - 1, cur_position[1])) continue;
            }
            if (i < 0)
            {
                if (Check_Cell(cur_position[0] + 1, cur_position[1])) continue;
            }
            if (i == 0)
            {
                if (Check_Cell(cur_position[0] + 1, cur_position[1])) continue;
                if (Check_Cell(cur_position[0] - 1, cur_position[1])) continue;
            }
            if (j > 0)
            {
                if (Check_Cell(cur_position[0], cur_position[1] - 1)) continue;
            }
            if (j < 0)
            {
                if (Check_Cell(cur_position[0], cur_position[1] + 1)) continue;
            }
            if (j == 0)
            {
                if (Check_Cell(cur_position[0], cur_position[1] + 1)) continue;
                if (Check_Cell(cur_position[0], cur_position[1] - 1)) continue;
            }
            map.cells[cur_position[0], cur_position[1]] = false;
            for (int k = 0; k < map.cells.GetLength(0); k++)
            {
                for (int l = 0; l < map.cells.GetLength(1); l++)
                {
                    cur_map.cells[k, l] = map.cells[k, l];
                }
            }
            cur_position = new int[] { (int)(bot.coords[1] - 640) / 160, (int)(bot.coords[0] - 640) / 160 };
            path.Clear();
        }
        cur_position = new int[] { (int)(bot.coords[1] - 640) / 160, (int)(bot.coords[0] - 640) / 160 };
        int ang = 0;
        new_angle = bot.angle;
        foreach (int[] v in path)
        {
            ang = (int)(Math.Atan2(v[0] - cur_position[0], v[1] - cur_position[1]) * 180 / Math.PI);
            if (ang - new_angle == 0 || ang - new_angle == 360)
            {
                cmds.Enqueue("forward 1");
            }
            else if (ang - new_angle == -90 || ang - new_angle == 270)
            {
                cmds.Enqueue("turn -90");
                cmds.Enqueue("forward 1");
                new_angle = (new_angle - 90) % 360;
                if (new_angle > 180) new_angle -= 360;
                if (new_angle < -180) new_angle += 360;
            }
            else if (ang - new_angle == 90 || ang - new_angle == -270)
            {
                cmds.Enqueue("turn 90");
                cmds.Enqueue("forward 1");
                new_angle = (new_angle + 90) % 360;
                if (new_angle > 180) new_angle -= 360;
                if (new_angle < -180) new_angle += 360;
            }
            else if (ang - new_angle == 180)
            {
                cmds.Enqueue("turn -180");
                cmds.Enqueue("forward 1");
                new_angle = (new_angle - 180) % 360;
                if (new_angle > 180) new_angle -= 360;
                if (new_angle < -180) new_angle += 360;
            }
            else
            {
                cmds.Enqueue("turn 180");
                cmds.Enqueue("forward 1");
                new_angle = (new_angle + 180) % 360;
                if (new_angle > 180) new_angle -= 360;
                if (new_angle < -180) new_angle += 360;
            }
            cur_position[0] = v[0];
            cur_position[1] = v[1];
        }
        cmds.Enqueue("idle 20");
    }
    public void Check_Vision(Bot enemy)
    {
        Vector v = new Vector();
        float d = 0;
        v.Set_From_Coords(enemy.coords[0] - bot.coords[0], enemy.coords[1] - bot.coords[1]);
        d = bot.angle - v.Angle;
        if (d < -180) d += 360;
        else if (d > 180) d -= 360;
        if (d >= -angle_vision && d <= angle_vision)
        {
            if (v.Scalar2 <= (radius_vision + 20) * (radius_vision + 20))
            {
                enemy_spotted = true;
            }
            else
            {
                if (enemy_spotted) enemy_lost = true;
                enemy_spotted = false;
            }
        }
        else
        {
            if (enemy_spotted) enemy_lost = true;
            enemy_spotted = false;
        }
    }
}

class Map
{
    public float[] coords = new float[] { 0, 0 };
    public Image img;
    public Map(string i)
    {
        img = Image.FromFile(i);
    }
    public void Draw(Graphics g)
    {
        g.DrawImage(img, coords[0], coords[1]);
    }
}

class Wall
{
    public float posX, posY, width, height;
    public int a = 160;
    public Color color = Color.Green;
    public Wall(float x, float y, float w, float h)
    {
        posX = x * a;
        posY = y * a;
        width = w * a;
        height = h * a;
    }
    public void Draw(Graphics g, float[] map)
    {
        g.FillRectangle(new SolidBrush(color), posX + map[0], posY + map[1], width, height);
    }
}

class Minimap
{
    public bool[,] cells;
    public Minimap()
    {
        cells = new bool[,]{
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
            {false,true,true,true,true,true,true,false,true,true,true,true,true,true,true,true,true,true,true,true,true,false},
            {false,true,true,true,true,true,true,false,true,true,true,true,true,true,true,true,true,true,true,true,true,false},
            {false,false,false,true,true,false,false,false,true,true,false,false,false,false,true,false,false,false,true,true,true,false},
            {false,true,true,true,true,true,true,true,true,true,true,true,true,false,true,false,false,false,false,true,true,false},
            {false,true,true,true,true,true,true,true,true,false,false,false,true,false,true,true,true,true,false,true,true,false},
            {false,true,false,false,true,false,false,true,true,false,false,false,true,false,false,false,false,true,false,true,true,false},
            {false,true,false,false,true,false,false,true,true,true,true,true,true,false,true,true,true,true,false,true,true,false},
            {false,true,true,true,true,true,true,true,false,false,true,true,false,false,true,false,false,false,false,true,true,false},
            {false,true,false,false,true,false,false,false,false,true,true,true,true,false,true,true,true,true,false,true,true,false},
            {false,true,false,false,true,true,true,true,true,true,true,true,true,false,false,false,false,true,false,true,true,false},
            {false,true,true,true,true,false,false,false,false,true,true,true,true,true,true,true,true,true,false,true,true,false},
            {false,false,false,false,true,false,false,false,false,true,true,true,true,false,false,false,false,false,false,true,true,false},
            {false,true,true,true,true,true,true,true,false,false,true,true,false,false,true,true,true,true,true,true,true,false},
            {false,true,false,true,true,true,true,false,false,false,true,true,true,true,true,true,true,true,true,true,true,false},
            {false,true,false,true,false,false,false,false,true,true,true,true,false,false,false,false,true,false,false,false,true,false},
            {false,true,false,true,true,true,true,true,true,true,true,true,false,false,false,false,true,true,true,false,true,false},
            {false,true,true,true,true,true,true,true,true,false,true,true,false,false,false,false,true,true,true,false,true,false},
            {false,true,false,false,false,true,true,true,false,false,true,true,false,false,false,false,true,true,true,false,true,false},
            {false,true,false,false,true,true,false,true,true,true,true,true,true,true,true,true,true,true,false,false,true,false},
            {false,true,true,true,true,true,true,true,false,false,false,false,false,false,true,true,true,true,true,true,true,false},
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false}
        };
    }
    public void Draw(Graphics g)
    {
        int side = 10;
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (cells[i, j]) g.FillRectangle(new SolidBrush(Color.White), 1300 + j * side, 50 + i * side, side, side);
                else g.FillRectangle(new SolidBrush(Color.Black), 1300 + j * side, 50 + i * side, side, side);
            }
        }
    }
}

class GameScene
{
    private int dtime;
    private float[] display_center, map_center;
    private Map map;
    private Minimap minimap;
    private Bot player;
    private Bot_Handler bot_1;
    private ArrayList walls;
    private DateTime t_now = DateTime.Now;
    private int t_delta = 0;
    private float[] edge_min = new float[] { 800, 800 };
    private float[] edge_max = new float[] { 20 * 160 + 800, 20 * 160 + 800 };
    private double[] mouse = new double[] { 0, 0 };
    private string imgpath = "../../Images/";
    private bool freecam = false;
    private string console = "";
    private string instruction = "";
    private string[] bot_instructions;
    private int instr_index = 0;
    public GameScene(int t, PictureBox pb)
    {
        dtime = t;
        map = new Map(imgpath + "BBMap.png");
        minimap = new Minimap();
        player = new Bot(100, 5, 100, dtime, imgpath + "BB_Red.png");
        bot_1 = new Bot_Handler(100, 5, 100, dtime, imgpath + "BB_Cyan.png");
        walls = new ArrayList();
        walls.Add(new Wall(11, 5, 1, 2));
        walls.Add(new Wall(5, 7, 2, 1));
        walls.Add(new Wall(9, 7, 3, 1));
        walls.Add(new Wall(14, 7, 3, 1));
        walls.Add(new Wall(17, 7, 1, 8));
        walls.Add(new Wall(19, 7, 3, 2));
        walls.Add(new Wall(22, 8, 1, 9));
        walls.Add(new Wall(13, 9, 3, 2));
        walls.Add(new Wall(6, 10, 2, 2));
        walls.Add(new Wall(9, 10, 2, 2));
        walls.Add(new Wall(18, 10, 3, 1));
        walls.Add(new Wall(12, 12, 2, 1));
        walls.Add(new Wall(16, 12, 1, 1));
        walls.Add(new Wall(19, 12, 3, 1));
        walls.Add(new Wall(6, 13, 2, 2));
        walls.Add(new Wall(9, 13, 4, 1));
        walls.Add(new Wall(18, 14, 3, 1));
        walls.Add(new Wall(9, 15, 4, 2));
        walls.Add(new Wall(5, 16, 3, 1));
        walls.Add(new Wall(17, 16, 5, 1));
        walls.Add(new Wall(12, 17, 2, 2));
        walls.Add(new Wall(16, 17, 2, 1));
        walls.Add(new Wall(6, 18, 1, 3));
        walls.Add(new Wall(11, 18, 1, 1));
        walls.Add(new Wall(8, 19, 4, 1));
        walls.Add(new Wall(16, 19, 4, 4));
        walls.Add(new Wall(21, 19, 3, 1));
        walls.Add(new Wall(23, 20, 1, 3));
        walls.Add(new Wall(13, 21, 1, 2));
        walls.Add(new Wall(6, 22, 2, 2));
        walls.Add(new Wall(8, 22, 1, 1));
        walls.Add(new Wall(12, 22, 1, 1));
        walls.Add(new Wall(10, 23, 1, 1));
        walls.Add(new Wall(22, 23, 2, 1));
        walls.Add(new Wall(12, 24, 6, 1));
        display_center = new float[] { pb.Width / 2, pb.Height / 2 };
        map_center = new float[] { map.img.Width / 2, map.img.Height / 2 };
        map.coords[0] = -map_center[0] + display_center[0];
        map.coords[1] = -map_center[1] + display_center[1];
        player.coords[0] = map_center[0];
        player.coords[1] = map_center[1];
        bot_1.bot.coords[0] = map_center[0] - 240;
        bot_1.bot.coords[1] = map_center[1] - 240;
        bot_1.Reach(12, 12);
        bot_instructions = new string[] { "turn 90", "forward 2", "turn 90", "back 2", "turn 90", "back 1", "forward 2", "turn -90", "forward 2", "turn 90", "forward 1", "turn 90" };
    }
    public void ProcessInput(BBForm mf)
    {
        mouse = mf.mouse;
        player.angle = (float)(Math.Atan2(mouse[1] - player.coords[1] - map.coords[1], mouse[0] - player.coords[0] - map.coords[0]) * 180 / Math.PI);
        if (mf.check_K_W) player.direction = 'f';
        else if (mf.check_K_A) player.direction = 'l';
        else if (mf.check_K_S) player.direction = 'b';
        else if (mf.check_K_D) player.direction = 'r';
        else player.direction = 'n';
        if (mf.check_K_P) freecam = true;
        else freecam = false;
        if (mf.check_K_Shift) player.speed = 300;
        else player.speed = 100;
        if (mf.check_M1)
        {
            if (player.rocket == null)
            {
                player.Fire();
            }
        }
    }
    public void Update()
    {
        Control_Bot(bot_1.bot);
        console += "queue: " + bot_1.cmds.Count;
        //if(bot_1.path.Count!=0) console+=" next_pos: "+((int[])(bot_1.path[bot_1.next_pos_i]))[0]+" x "+((int[])(bot_1.path[bot_1.next_pos_i]))[1];
        console += " path: " + bot_1.path.Count;
        //console+=" cur_pos: "+bot_1.cur_position[1]+" x "+bot_1.cur_position[0];
        foreach (int[] i in bot_1.path)
        {
            console += " - " + i[0] + "x" + i[1];
        }
        console += " - ";
        console += " dura: " + bot_1.duration;
        //console+=" | "+player.angle;
        //bot_1.Act(bot_instructions[instr_index]);
        //if(bot_1.ready) instr_index=(instr_index+1)%bot_instructions.Length;
        bot_1.Check_Vision(player);
        if (bot_1.enemy_spotted)
        {
            bot_1.bot.angle = (float)(Math.Atan2(player.coords[1] - bot_1.bot.coords[1], player.coords[0] - bot_1.bot.coords[0]) * 180 / Math.PI);
            if (bot_1.bot.rocket == null)
            {
                bot_1.bot.Fire();
            }
        }
        else
        {
            if (bot_1.enemy_lost)
            {
                bot_1.cmds.Clear();
                bot_1.cmds.Enqueue("rotate 0");
                bot_1.cmds.Enqueue("regroup 0");
                bot_1.ready = true;
                bot_1.enemy_lost = false;
            }
        }
        if (bot_1.cmds.Count != 0 && !bot_1.enemy_spotted)
        {
            if (bot_1.ready) instruction = (string)bot_1.cmds.Dequeue();
        }
        else
        {
            instruction = "idle 1";
            if (!bot_1.enemy_spotted) bot_1.Reach((int)(player.coords[1] - 640) / 160, (int)(player.coords[0] - 640) / 160);
        }
        if (!bot_1.enemy_spotted) bot_1.Act(instruction);
        if (bot_1.bot.hp <= 0) bot_1.bot.hp = 100;
        player.Set_Velocity();
        Resolve_Edge_Collision(player, edge_min, edge_max);
        foreach (Wall v in walls)
        {
            if (player.direction != 'n') Resolve_Wall_Collision(player, v);
            else break;
        }
        player.Move();
        /*
        foreach(Wall v in walls){
            if(player.rocket!=null || console=="Collision") Resolve_Wall_Collision(player.rocket,v);
            else break;
        }
        */
        if (player.rocket != null) Resolve_Rocket_Collisions(player.rocket, walls, bot_1.bot);
        if (player.rocket != null) player.rocket.Move();
        if (player.rocket != null)
        {
            if (player.rocket.destroyed) player.rocket = null;
        }
        if (bot_1.bot.rocket != null) Resolve_Rocket_Collisions(bot_1.bot.rocket, walls, player);
        if (bot_1.bot.rocket != null) bot_1.bot.rocket.Move();
        if (bot_1.bot.rocket != null)
        {
            if (bot_1.bot.rocket.destroyed) bot_1.bot.rocket = null;
        }
        /*
        t_delta=DateTime.Now.Millisecond-t_now.Millisecond;
        t_now=DateTime.Now;
        */
        if (!freecam)
        {
            map.coords[0] = -player.coords[0] + display_center[0];
            map.coords[1] = -player.coords[1] + display_center[1];
        }
    }
    public void Render(Graphics g)
    {
        g.Clear(Color.White);
        map.Draw(g);
        foreach (Wall v in walls)
        {
            v.Draw(g, map.coords);
        }
        bot_1.bot.Draw(g, map.coords);
        player.Draw(g, map.coords);
        if (player.rocket != null)
        {
            player.rocket.Draw(g, map.coords);
        }
        if (bot_1.bot.rocket != null)
        {
            bot_1.bot.rocket.Draw(g, map.coords);
        }
        g.DrawString("direction: " + player.direction + player.speed, new Font("Arial", 14, FontStyle.Bold), new SolidBrush(Color.Yellow), new RectangleF(600, 10, 200, 300));
        g.DrawString(console, new Font("Arial", 14, FontStyle.Bold), new SolidBrush(Color.Orange), new RectangleF(1350, 500, 200, 300));
        g.FillRectangle(new SolidBrush(Color.Red), bot_1.bot.coords[0] - 25 + map.coords[0], bot_1.bot.coords[1] + 35 + map.coords[1], (float)(0.5 * bot_1.bot.hp), 10);
        g.FillRectangle(new SolidBrush(Color.Black), (float)(bot_1.bot.coords[0] - 25 + 0.5 * bot_1.bot.hp + map.coords[0]), bot_1.bot.coords[1] + 35 + map.coords[1], (float)(50 - 0.5 * bot_1.bot.hp), 10);
        g.FillRectangle(new SolidBrush(Color.Red), 20, 20, (float)(2 * player.hp), 30);
        g.FillRectangle(new SolidBrush(Color.Black), (float)(2 * player.hp + 20), 20, (float)(200 - 2 * player.hp), 30);
        minimap.Draw(g);
        g.FillRectangle(new SolidBrush(Color.Red), 1300 + ((int)(player.coords[0] - 640) / 160) * 10, 50 + ((int)(player.coords[1] - 640) / 160) * 10, 10, 10);
        g.FillRectangle(new SolidBrush(Color.Cyan), 1300 + ((int)(bot_1.bot.coords[0] - 640) / 160) * 10, 50 + ((int)(bot_1.bot.coords[1] - 640) / 160) * 10, 10, 10);
    }
    public void Resolve_Wall_Collision(Bot b, Wall w)
    {
        if (((b.coords[0] + 20 + b.velocity.coords[0]) > (w.posX) && (b.coords[0] - 20 + b.velocity.coords[0]) < (w.posX + w.width)) &&
            ((b.coords[1] + 20 + b.velocity.coords[1]) > (w.posY) && (b.coords[1] - 20 + b.velocity.coords[1]) < (w.posY + w.height)))
        {
            b.direction = 'n';
            b.velocity.Set_Null();
        }
    }
    public void Resolve_Edge_Collision(Bot b, float[] min, float[] max)
    {
        if ((b.coords[0] - 20 + b.velocity.coords[0]) < min[0] || (b.coords[0] + 20 + b.velocity.coords[0]) > max[0] ||
            (b.coords[1] - 20 + b.velocity.coords[1]) < min[1] || (b.coords[1] + 20 + b.velocity.coords[1]) > max[1])
        {
            b.direction = 'n';
            b.velocity.Set_Null();
        }
    }
    public void Resolve_Wall_Collision(Rocket r, Wall w)
    {
        if (((r.end_coords[0] + 5 + r.velocity.coords[0]) > (w.posX) && (r.end_coords[0] - 5 + r.velocity.coords[0]) < (w.posX + w.width)) &&
            ((r.end_coords[1] + 5 + r.velocity.coords[1]) > (w.posY) && (r.end_coords[1] - 5 + r.velocity.coords[1]) < (w.posY + w.height)))
        {
            r.range = r.distance.Scalar;
        }
    }
    public void Resolve_Bot_Collision(Rocket r, Bot b)
    {
        if (((b.coords[0] + b.velocity.coords[0]) - (r.end_coords[0] + r.velocity.coords[0])) * ((b.coords[0] + b.velocity.coords[0]) - (r.end_coords[0] + r.velocity.coords[0])) +
            ((b.coords[1] + b.velocity.coords[1]) - (r.end_coords[1] + r.velocity.coords[1])) * ((b.coords[1] + b.velocity.coords[1]) - (r.end_coords[1] + r.velocity.coords[1])) <=
            (20 + 5) * (20 + 5))
        {
            r.range = r.distance.Scalar;
            b.hp -= r.damage;
        }
    }
    public void Resolve_Rocket_Collisions(Rocket r, ArrayList w, Bot b)
    {
        foreach (Wall v in w)
        {
            if (r != null) Resolve_Wall_Collision(r, v);
            else break;
        }
        if (r != null)
        {
            Resolve_Bot_Collision(r, b);
        }
    }
    public void Control_Bot(Bot b)
    {
        int[] position = new int[] { 0, 0 };
        position[0] = (int)(b.coords[0] - 640) / 160;
        position[1] = (int)(b.coords[1] - 640) / 160;
        console = "" + position[1] + " x " + position[0];
        console += " | " + b.coords[0] + " x " + b.coords[1];
        console += " | " + b.angle;
    }
}

class BBForm : Form
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
    public BBForm() : base()
    {
        Size = new Size(1600, 1000);
        FormBorderStyle = FormBorderStyle.Fixed3D;
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = false;
        Text = "BattleBots";
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

class BattleBots
{
    public static void StartPI(GameScene s, BBForm mf, int time)
    {
        while (true)
        {
            s.ProcessInput(mf);
            Thread.Sleep(time);
        }
    }
    public static void StartU(GameScene s, int time)
    {
        while (true)
        {
            s.Update();
            Thread.Sleep(time);
        }
    }
    public static void StartR(BBForm mf, int time)
    {
        while (true)
        {
            mf.picbox.Invalidate();
            Thread.Sleep(time);
        }
    }
    public static void Main()
    {
        int time = 15;
        BBForm myform = new BBForm();
        GameScene scene = new GameScene(time, myform.picbox);
        myform.picbox.Paint += (obj, ea) => {
            scene.Render(ea.Graphics);
        };
        Thread PIthread = new Thread(() => {
            StartPI(scene, myform, time);
        });
        Thread Uthread = new Thread(() => {
            StartU(scene, time);
        });
        Thread Rthread = new Thread(() => {
            StartR(myform, time);
        });
        PIthread.Start();
        Uthread.Start();
        Rthread.Start();
        Application.Run(myform);
    }
}