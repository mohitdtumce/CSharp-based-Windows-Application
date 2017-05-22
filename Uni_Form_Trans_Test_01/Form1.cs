using System;
using System.Drawing;
using System.Resources;
using TransParentModule;
using System.Windows.Forms;

namespace Uni_Form_Trans_Test_01
{
    public partial class Form1 : TransparentForm
    {
        public Form1()
        {
            InitializeComponent();
            Console.WriteLine("Main Program Started!!");
        }

        public void ChangeSetting(string settingName, float arg = 0)
        {
            if (settingName == "SIZE")
            {
                scale = arg;
                canvas.Resize((int)(325 / scale), (int)(250 / scale));
                Animate.SetScale(arg);

                if (dinoBabyEyeBkgL == null)
                    dinoBabyEyeBkgL.Dispose();
                if (dinoBabyEyeBkgR == null)
                    dinoBabyEyeBkgR.Dispose();
                if (dinoBabyEyePupilL == null)
                    dinoBabyEyePupilL.Dispose();
                if (dinoBabyEyePupilR == null)
                    dinoBabyEyePupilR.Dispose();
                Animate.LoadBitmap("DinoBabyEyeBkgL", ref dinoBabyEyeBkgL, scale);
                Animate.LoadBitmap("DinoBabyEyeBkgR", ref dinoBabyEyeBkgR, scale);
                Animate.LoadBitmap("DinoBabyEyePupilL", ref dinoBabyEyePupilL, scale);
                Animate.LoadBitmap("DinoBabyEyePupilR", ref dinoBabyEyePupilR, scale);
            }
            else if (settingName == "HIDE|SHOW")
            {
                if (arg == 0)
                    isPaused = true;
                else
                    isPaused = false;
            }
            else if(settingName == "FADE_ON_HOVER")
            {
                Properties.Settings.Default.FADE_ON_HOVER = !Properties.Settings.Default.FADE_ON_HOVER;
            }
            else if(settingName == "FLY" && state != State.FLY)
            {
                changeState = true;
                impose = true;
                stateImposed = State.FLY;
                subStateImposed = SubState.FLYS1;
            }
            else if (settingName == "LAY_DOWN" && (subState != SubState.RESTV4L && subState != SubState.RESTV4S && subState != SubState.RESTV4E))
            {
                changeState = true;
                impose = true;
                stateImposed = State.REST;
                subStateImposed = SubState.RESTV4S;
            }
            else if (settingName == "SIT_DOWN" && subState != SubState.RESTV3S && subState != SubState.RESTV3L1 && subState != SubState.RESTV3L2 && subState != SubState.RESTV3E)
            {
                changeState = true;
                impose = true;
                stateImposed = State.REST;
                subStateImposed = SubState.RESTV3S;
            }
            else if(settingName == "DANCE" && state != State.ACTION_DANCE)
            {
                changeState = true;
                impose = true;
                stateImposed = State.ACTION_DANCE;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = new Point(xPositionCanvas, yPositionCanvas);
            this.ShowInTaskbar = false;

            ResourceManager rm = Properties.Resources.ResourceManager;
            canvas = new Canvas((int)(325 / scale), (int)(250 / scale));

            timer = new System.Windows.Forms.Timer()
            {
                Enabled = true,
                Interval = 25
            };
            timer.Start();
            timer.Tick += new EventHandler(ManageControl);

            rnd = new Random();

            action_Dance = new Animate("Action_Dance", 5, 179, 1, scale, ref canvas, ref rm);
            restV1 = new Animate("RestV1", 5, 74, 1, scale, ref canvas, ref rm);
            restV2 = new Animate("RestV2", 5, 69, 1, scale, ref canvas, ref rm);

            restV3S = new Animate("RestV3S", 5, 24, 1, scale, ref canvas, ref rm);
            restV3L1 = new Animate("RestV3L1", 5, 29, 1, scale, ref canvas, ref rm);
            restV3L2 = new Animate("RestV3L2", 5, 75, 1, scale, ref canvas, ref rm);
            restV3E = new Animate("RestV3E", 5, 25, 1, scale, ref canvas, ref rm);

            restV4S = new Animate("RestV4S", 5, 73, 1, scale, ref canvas, ref rm);
            restV4L = new Animate("RestV4L", 5, 65, 1, scale, ref canvas, ref rm);
            restV4E = new Animate("RestV4E", 5, 37, 1, scale, ref canvas, ref rm);

            walkV1S = new Animate("WalkV1S", 5, 19, 1, scale, ref canvas, ref rm);
            walkV1L = new Animate("WalkV1L", 5, 18, 1, scale, ref canvas, ref rm);
            walkV1E = new Animate("WalkV1E", 5, 50, 1, scale, ref canvas, ref rm);

            walkV2S = new Animate("WalkV2S", 5, 11, 1, scale, ref canvas, ref rm);
            walkV2L = new Animate("WalkV2L", 5, 28, 1, scale, ref canvas, ref rm);
            walkV2E = new Animate("WalkV2E", 5, 25, 1, scale, ref canvas, ref rm);

            flyS1 = new Animate("FlyS1", 5, 19, 1, scale, ref canvas, ref rm);
            flyS2 = new Animate("FlyS2", 5, 13, 1, scale, ref canvas, ref rm);
            flyL = new Animate("FlyL", 5, 32, 1, scale, ref canvas, ref rm);
            flyE1 = new Animate("FlyE1", 5, 19, 1, scale, ref canvas, ref rm);
            flyE2 = new Animate("FlyE2", 5, 13, 1, scale, ref canvas, ref rm);

            Animate.LoadBitmap("DinoBabyEyeBkgL", ref dinoBabyEyeBkgL, scale);
            Animate.LoadBitmap("DinoBabyEyeBkgR", ref dinoBabyEyeBkgR, scale);
            Animate.LoadBitmap("DinoBabyEyePupilL", ref dinoBabyEyePupilL, scale);
            Animate.LoadBitmap("DinoBabyEyePupilR", ref dinoBabyEyePupilR, scale);
            
            Console.WriteLine("Loading Completed");
        }
    }

}
