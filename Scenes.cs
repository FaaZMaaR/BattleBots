using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System;

class GameScene
{
    private int dtime;
    private double[] display_center, map_center;
    private Map map;
    private Minimap minimap;
    private Bot player,bot_1;
    private Bot_Handler bh_1;
    private ArrayList walls;
    private Box edge;
    private PhysHandler phys;
    private float[] edge_min = new float[] { 800, 800 };
    private float[] edge_max = new float[] { 20 * 160 + 800, 20 * 160 + 800 };
    private double[] mouse = new double[] { 0, 0 };
    private string imgpath = "Images/";
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
        player = new Bot(dtime, imgpath + "BB_Red.png");
        bot_1=new Bot(dtime,imgpath+"BB_Cyan.png");
        bh_1 = new Bot_Handler(bot_1);
        phys=new PhysHandler();
        edge = new Box(800, 800, 20 * 160, 20 * 160);
        walls = new ArrayList();
        int wcell = 160;
        walls.Add(new Box(11*wcell, 5 * wcell, 1 * wcell, 2 * wcell));
        walls.Add(new Box(5 * wcell, 7 * wcell, 2 * wcell, 1 * wcell));
        walls.Add(new Box(9 * wcell, 7 * wcell, 3 * wcell, 1 * wcell));
        walls.Add(new Box(14 * wcell, 7 * wcell, 3 * wcell, 1 * wcell));
        walls.Add(new Box(17 * wcell, 7 * wcell, 1 * wcell, 8 * wcell));
        walls.Add(new Box(19 * wcell, 7 * wcell, 3 * wcell, 2 * wcell));
        walls.Add(new Box(22 * wcell, 8 * wcell, 1 * wcell, 9 * wcell));
        walls.Add(new Box(13 * wcell, 9 * wcell, 3 * wcell, 2 * wcell));
        walls.Add(new Box(6 * wcell, 10 * wcell, 2 * wcell, 2 * wcell));
        walls.Add(new Box(9 * wcell, 10 * wcell, 2 * wcell, 2 * wcell));
        walls.Add(new Box(18 * wcell, 10 * wcell, 3 * wcell, 1 * wcell));
        walls.Add(new Box(12 * wcell, 12 * wcell, 2 * wcell, 1 * wcell));
        walls.Add(new Box(16 * wcell, 12 * wcell, 1 * wcell, 1 * wcell));
        walls.Add(new Box(19 * wcell, 12 * wcell, 3 * wcell, 1 * wcell));
        walls.Add(new Box(6 * wcell, 13 * wcell, 2 * wcell, 2 * wcell));
        walls.Add(new Box(9 * wcell, 13 * wcell, 4 * wcell, 1 * wcell));
        walls.Add(new Box(18 * wcell, 14 * wcell, 3 * wcell, 1 * wcell));
        walls.Add(new Box(9 * wcell, 15 * wcell, 4 * wcell, 2 * wcell));
        walls.Add(new Box(5 * wcell, 16 * wcell, 3 * wcell, 1 * wcell));
        walls.Add(new Box(17 * wcell, 16 * wcell, 5 * wcell, 1 * wcell));
        walls.Add(new Box(12 * wcell, 17 * wcell, 2 * wcell, 2 * wcell));
        walls.Add(new Box(16 * wcell, 17 * wcell, 2 * wcell, 1 * wcell));
        walls.Add(new Box(6 * wcell, 18 * wcell, 1 * wcell, 3 * wcell));
        walls.Add(new Box(11 * wcell, 18 * wcell, 1 * wcell, 1 * wcell));
        walls.Add(new Box(8 * wcell, 19 * wcell, 4 * wcell, 1 * wcell));
        walls.Add(new Box(16 * wcell, 19 * wcell, 4 * wcell, 4 * wcell));
        walls.Add(new Box(21 * wcell, 19 * wcell, 3 * wcell, 1 * wcell));
        walls.Add(new Box(23 * wcell, 20 * wcell, 1 * wcell, 3 * wcell));
        walls.Add(new Box(13 * wcell, 21 * wcell, 1 * wcell, 2 * wcell));
        walls.Add(new Box(6 * wcell, 22 * wcell, 2 * wcell, 2 * wcell));
        walls.Add(new Box(8 * wcell, 22 * wcell, 1 * wcell, 1 * wcell));
        walls.Add(new Box(12 * wcell, 22 * wcell, 1 * wcell, 1 * wcell));
        walls.Add(new Box(10 * wcell, 23 * wcell, 1 * wcell, 1 * wcell));
        walls.Add(new Box(22 * wcell, 23 * wcell, 2 * wcell, 1 * wcell));
        walls.Add(new Box(12 * wcell, 24 * wcell, 6 * wcell, 1 * wcell));
        display_center = new double[] { pb.Width / 2, pb.Height / 2 };
        map_center = new double[] { map.img.Width / 2, map.img.Height / 2 };
        map.x = -map_center[0] + display_center[0];
        map.y = -map_center[1] + display_center[1];
        player.body.x = map_center[0];
        player.body.y = map_center[1];
        bot_1.body.x = map_center[0] - 240;
        bot_1.body.y = map_center[1] - 240;
        bh_1.Reach(12, 12);
        bot_instructions = new string[] { "turn 90", "forward 2", "turn 90", "back 2", "turn 90", "back 1", "forward 2", "turn -90", "forward 2", "turn 90", "forward 1", "turn 90" };
    }
    public void ProcessInput(GameForm mf)
    {
        mouse = mf.mouse;
        player.body.angle = Math.Atan2(mouse[1] - player.body.y - map.y, mouse[0] - player.body.x - map.x);
        if (mf.check_K_W && mf.check_K_A) player.direction = Direction.UPLEFT;
        else if (mf.check_K_W && mf.check_K_D) player.direction = Direction.UPRIGHT;
        else if (mf.check_K_S && mf.check_K_A) player.direction = Direction.DOWNLEFT;
        else if (mf.check_K_S && mf.check_K_D) player.direction = Direction.DOWNRIGHT;
        else if (mf.check_K_W) player.direction = Direction.UP;
        else if (mf.check_K_A) player.direction = Direction.LEFT;
        else if (mf.check_K_S) player.direction = Direction.DOWN;
        else if (mf.check_K_D) player.direction = Direction.RIGHT;
        else player.direction = Direction.NONE;
        if (mf.check_K_P) freecam = true;
        else freecam = false;
        if (mf.check_K_Shift) player.speed = 300;
        else player.speed = 100;
        if (mf.check_M1)
        {
            player.Fire();
        }
    }
    public void Update()
    {
        UpdateConsole();
        //bot_1.Act(bot_instructions[instr_index]);
        //if(bot_1.ready) instr_index=(instr_index+1)%bot_instructions.Length;

        bh_1.UpdateVision(player);
        bh_1.Update(player);        
        if (bot_1.hp <= 0) bot_1.hp = 100;

        player.Set_Velocity();
        phys.ResolveCircleInsideBoxCollision(player.body, edge);
        foreach (Box v in walls)
        {
            if (!player.body.velocity.IsNull()) phys.ResolveCircleBoxCollision(player.body, v);
            else break;
        }
        player.Update();
        if (player.hp <= 0) player.hp = 100;

        RocketsUpdate(player, bot_1, walls, edge);
        RocketsUpdate(bot_1, player, walls, edge);

        if (!freecam)
        {
            map.x = -player.body.x + display_center[0];
            map.y = -player.body.y + display_center[1];
        }
    }
    public void Render(Graphics g)
    {
        g.Clear(Color.White);
        map.Draw(g);
        foreach (Box v in walls)
        {
            v.Draw(g, new float[] { (float)map.x, (float)map.y });
        }
        bot_1.Draw(g, new float[] { (float)map.x, (float)map.y });
        player.Draw(g, new float[] { (float)map.x, (float)map.y });
        for(int i = 0; i < player.rockets.Length; i++)
        {
            player.rockets[i].Draw(g, new float[] { (float)map.x, (float)map.y });
        }
        for (int i = 0; i < bot_1.rockets.Length; i++)
        {
            bot_1.rockets[i].Draw(g, new float[] { (float)map.x, (float)map.y });
        }
        g.DrawString("direction: " + player.direction + player.speed, new Font("Arial", 14, FontStyle.Bold), new SolidBrush(Color.Yellow), new RectangleF(600, 10, 200, 300));
        g.DrawString(console, new Font("Arial", 14, FontStyle.Bold), new SolidBrush(Color.Orange), new RectangleF(1350, 500, 200, 300));
        bot_1.DrawHealthBar(g, map);
        player.DrawHealthBar(g, map);
        minimap.Draw(g);
        g.FillRectangle(new SolidBrush(Color.Red), 1300 + ((int)(player.body.x - 640) / 160) * 10, 50 + ((int)(player.body.y - 640) / 160) * 10, 10, 10);
        g.FillRectangle(new SolidBrush(Color.Cyan), 1300 + ((int)(bot_1.body.x - 640) / 160) * 10, 50 + ((int)(bot_1.body.y - 640) / 160) * 10, 10, 10);
    }

    public void RocketsUpdate(Bot b1,Bot b2,ArrayList ws,Box e)
    {
        for(int i=0;i<b1.rockets.Length; i++)
        {
            if (b1.rockets[i].destroyed) continue;
            phys.ResolveCircleInsideBoxCollision(b1.rockets[i].body, e);
            for(int j = 0; j < ws.Count; j++)
            {
                phys.ResolveCircleBoxCollision(b1.rockets[i].body, (Box)ws[j]);
            }
            phys.ResolveRocketHit(b1.rockets[i], b2);
            b1.rockets[i].Update();
        }
    }

    public void Control_Bot(Bot b)
    {
        int[] position = new int[] { 0, 0 };
        position[0] = (int)(b.body.x - 640) / 160;
        position[1] = (int)(b.body.y - 640) / 160;
        console = "" + position[1] + " x " + position[0];
        console += " | " + b.body.x + " x " + b.body.y;
        console += " | " + b.body.angle;
    }
    public void UpdateConsole()
    {
        Control_Bot(bot_1);
        console += "queue: " + bh_1.cmds.Count;
        //if(bot_1.path.Count!=0) console+=" next_pos: "+((int[])(bot_1.path[bot_1.next_pos_i]))[0]+" x "+((int[])(bot_1.path[bot_1.next_pos_i]))[1];
        console += " path: " + bh_1.path.Count;
        //console+=" cur_pos: "+bot_1.cur_position[1]+" x "+bot_1.cur_position[0];
        foreach (int[] i in bh_1.path)
        {
            console += " - " + i[0] + "x" + i[1];
        }
        console += " - ";
        console += " dura: " + bh_1.duration;
        //console+=" | "+player.angle;
    }
}