using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using TransParentModule;

namespace Uni_Form_Trans_Test_01
{
    public partial class Form1 : TransparentForm
    {
        #region Enumerations

        enum Direction { LEFT, RIGHT, NULL };

        enum State { REST, WALK, ACTION_DANCE, FLY };

        enum SubState { RESTV1, RESTV2,
                        RESTV3S, RESTV3L1, RESTV3L2, RESTV3E,
                        RESTV4S, RESTV4L, RESTV4E,
                        WALKV1S, WALKV1L, WALKV1E,
                        WALKV2S, WALKV2L, WALKV2E,
                        FLYS1, FLYS2, FLYL, FLYE2, FLYE1};

        #endregion

        #region Declarations

        Timer timer;
        Canvas canvas;
        Direction direction = Direction.RIGHT;
        State state = State.REST, stateImposed = State.REST;
        SubState subState = SubState.RESTV3S, subStateImposed = SubState.RESTV1;
        
        Random rnd;
        Bitmap dinoBabyEyeBkgL, dinoBabyEyeBkgR, dinoBabyEyePupilL, dinoBabyEyePupilR;
        Animate action_Dance, restV1, restV2;
        Animate restV3S, restV3L1, restV3L2, restV3E;
        Animate restV4S, restV4L, restV4E;
        Animate walkV1S, walkV1L, walkV1E;
        Animate walkV2S, walkV2L, walkV2E;
        Animate flyS1, flyS2, flyL, flyE1, flyE2;

        bool isPaused = false;
        long cnt = 0;
        int yOffset = 0;
        int xPositionCanvas = 500, yPositionCanvas = 100, destX = 0, destY = 0, lim = 5;
        float scale = 1.0f;
        bool changeState = false, impose = false;
        float transparencyLevel = 1f;
        double cX = 0, xDestinationWalk = 0, yoffsetRest = 0;
		long currentStateCounter = 0, currentStateChangeCounter = 0;
		Point p1 = new Point(0, 0), p2 = new Point(0, 0);
        long counter = 0;
        #endregion

        private void DrawEye(Point p, ref Bitmap bkg, ref Bitmap pupil, Direction eye, bool flag)
        {
            double cX = Cursor.Position.X;
            double cY = Cursor.Position.Y;
            double dX = 0, dY = 0;
            Point offset = new Point(3, 4);

            if (direction == Direction.RIGHT)
            {
                dX = (cX - (canvas.Width - p.X) - xPositionCanvas) / 100;
                dY = (cY - p.Y - yPositionCanvas) / 100;
                dX = -Math.Atan(dX);
                dY = Math.Atan(dY);
            }
            else
            {
                dX = (cX - p.X - xPositionCanvas) / 100;
                dY = (cY - p.Y - yPositionCanvas) / 100;
                dX = Math.Atan(dX);
                dY = Math.Atan(dY);
            }
            if (eye == Direction.LEFT)
            {
                dX *= 1.5;
                dY *= 3;
            }
            else
            {
                dX *= 1.5;
                dY *= 3;
            }
            if (flag)
            {
                dX *= 1.2;
                offset.X = 5;
            }
            else
            {
                dX *= 2.5;
                dY *= 1.5;
                offset.X = 10;
            }
            canvas.AddBitmap(ref bkg, p.X + (int)((dX - offset.X) / scale),
                p.Y + (int)((dY - offset.Y - 3) / scale));
            canvas.AddBitmap(ref pupil, p.X + (int)((dX * 1.2f - offset.X + 2) / scale),
                            p.Y + (int)((dY - offset.Y) * 1.2f / scale));

        }

        private double GetProbability()
        {
            return rnd.NextDouble();
        }

        private int GetLoc()
        {
            if (direction == Direction.LEFT)
                return p1.X;
            else
            {
                if (subState == SubState.RESTV1 || subState == SubState.RESTV2)
                    return 250 - p1.X;
                else
                    return 300 - p1.X;
            }
        }

        private void ManageControl(object sender, EventArgs args)
        {
            if (isPaused)
                goto skip;

            // To control flow of program
            // Rest of the things will be handeled from here
            if (cnt % 1000 == 0)
                GC.Collect();
            cnt++;
            if(changeState == false && impose)
            {
                state = stateImposed;
                subState = subStateImposed;
                impose = false;
            }
            if (!changeState)
            {
                long currentNet = currentStateCounter + currentStateChangeCounter;
				switch (state)
				{
					case State.REST:
                        if (Math.Abs(Cursor.Position.X - cX) < 2)
                            currentStateCounter++;
                        else
                        {
                            currentStateCounter = 0;
                            currentStateChangeCounter++;
                        }

                        int limit = 300;
                        if(subState == SubState.RESTV4L || subState == SubState.RESTV3L2 || subState == SubState.RESTV3L1)
                        {
                            limit = 1000;
                        }
						if (currentNet >= limit && Math.Abs(Cursor.Position.X - xPositionCanvas - GetLoc()) > 100)
						{
							currentStateCounter = 0;
                            currentStateChangeCounter = 0;
                            changeState = true;
							xDestinationWalk = (int)cX;
							if (cX > xPositionCanvas)
								direction = Direction.RIGHT;
							else
								direction = Direction.LEFT;
						}
                        if(currentStateCounter >= 2000 && (subState == SubState.RESTV1 || subState == SubState.RESTV2))
                        {
                            currentStateCounter = 0;
                            double prob = GetProbability();
                            if (prob < 0.2)
                                subState = SubState.RESTV3S;
                            else if (prob > 0.4 && prob < 0.6)
                                subState = SubState.RESTV4S;
                            else if (prob > 0.7)
                            {
                                state = State.FLY;
                                prob = GetProbability();
                                if (prob > 0.5)
                                    subState = SubState.FLYS1;
                                else
                                    subState = SubState.FLYS2;
                            }
                        }
						if (currentNet % 100 == 0)
						{
							if (cX > xPositionCanvas)
								direction = Direction.RIGHT;
							else
								direction = Direction.LEFT;
						}
						break;
					case State.WALK:
						if (Math.Abs(xPositionCanvas + GetLoc() - xDestinationWalk) < 20 || 
                            xPositionCanvas < 20 || xPositionCanvas > Screen.PrimaryScreen.WorkingArea.Width * 0.8f)
							changeState = true;
						break;
				}
			}

			cX = Cursor.Position.X;
            int xOffset = 0;
            int taskBarLocation = Math.Max(0, Screen.PrimaryScreen.WorkingArea.Bottom - (int)canvas.Height + (int)(40 / scale));

            // This Part Controls What Will Be Displayed On Screen
            switch (state)
            {
                case State.REST:
                    {
                        switch (subState)
                        {
                            case SubState.RESTV1:
                                if(restV1.Update(cnt, 0, 0))
                                {
                                    if (GetProbability() > 0.4)
                                        subState = SubState.RESTV2;
                                    if (changeState)
                                    {
                                        state = State.WALK;
                                        if (GetProbability() > 0.8)
                                            subState = SubState.WALKV1S;
                                        else
                                            subState = SubState.WALKV2S;
                                        changeState = false;
                                    }
                                }
                                p1 = restV1.GetLocation(0);
                                p2 = restV1.GetLocation(1);
                                break;
                            case SubState.RESTV2:
                                if(restV2.Update(cnt, 0, 0))
                                {
                                    double prob = GetProbability();
                                    if (prob > 0.8)
                                        subState = SubState.RESTV1;
                                    else if (prob > 0.95)
                                    {
                                        state = State.ACTION_DANCE;
                                    }
                                    if (changeState)
                                    {
                                        state = State.WALK;
                                        if (GetProbability() > 0.8)
                                            subState = SubState.WALKV1S;
                                        else
                                            subState = SubState.WALKV2S;
                                        changeState = false;
                                    }
                                }
                                p1 = restV2.GetLocation(0);
                                p2 = restV2.GetLocation(1);
                                break;
                            case SubState.RESTV3S:
                                xOffset = -25;
                                yoffsetRest += 9.0 / 25.0;
                                if (restV3S.Update(cnt, 0, 0))
                                {
                                    subState = SubState.RESTV3L1;
                                }
                                p1 = restV3S.GetLocation(0);
                                p2 = restV3S.GetLocation(1);
                                break;
                            case SubState.RESTV3L1:
                                xOffset = -25;
                                yoffsetRest = 9;
                                if (restV3L1.Update(cnt, 0, 0))
                                {
                                    if (changeState)
                                        subState = SubState.RESTV3E;
                                    else
                                    {
                                        if (GetProbability() > 0.9)
                                            subState = SubState.RESTV3L2;
                                        else
                                            subState = SubState.RESTV3L1;
                                        changeState = false;
                                    }
                                }
                                p1 = restV3L1.GetLocation(0);
                                p2 = restV3L1.GetLocation(1);
                                break;
                            case SubState.RESTV3L2:
                                yoffsetRest = 9;
                                xOffset = -25;
                                if (restV3L2.Update(cnt, 0, 0))
                                {
                                    if (changeState)
                                        subState = SubState.RESTV3E;
                                    else
                                    {
                                        if (GetProbability() > 0.8)
                                            subState = SubState.RESTV3L2;
                                        else
                                            subState = SubState.RESTV3L1;
                                    }
                                }
                                p1 = restV3L2.GetLocation(0);
                                p2 = restV3L2.GetLocation(1);
                                break;
                            case SubState.RESTV3E:
                                xOffset = -25;
                                yoffsetRest -= 9.0 / 25.0;
                                if (restV3E.Update(cnt, 0, 0))
                                {
                                    state = State.WALK;
                                    if (GetProbability() > 0.8)
                                        subState = SubState.WALKV1S;
                                    else
                                        subState = SubState.WALKV2S;
                                    changeState = false;
                                }
                                p1 = restV3E.GetLocation(0);
                                p2 = restV3E.GetLocation(1);
                                break;
                            case SubState.RESTV4S:
                                xOffset = -25;
                                yoffsetRest -= 10.0 / 40.0;
                                if (yoffsetRest < -10)
                                    yoffsetRest = -10;
                                if (restV4S.Update(cnt, 0, 0))
                                    subState = SubState.RESTV4L;
                                p1 = restV4S.GetLocation(0);
                                p2 = restV4S.GetLocation(1);
                                break;
                            case SubState.RESTV4L:
                                xOffset = -25;
                                yoffsetRest = -10;
                                if (restV4L.Update(cnt, 0, 0) && changeState)
                                        subState = SubState.RESTV4E;
                                p1 = restV4L.GetLocation(0);
                                p2 = restV4L.GetLocation(1);
                                break;
                            case SubState.RESTV4E:
                                xOffset = -25;
                                yoffsetRest += 10.0 / 37.0;
                                if (restV4E.Update(cnt, 0, 0))
                                {
                                    state = State.WALK;
                                    if (GetProbability() > 0.8)
                                        subState = SubState.WALKV1S;
                                    else
                                        subState = SubState.WALKV2S;
                                    changeState = false;
                                }
                                p1 = restV4E.GetLocation(0);
                                p2 = restV4E.GetLocation(1);
                                break;
                        }
                        break;
                    }
                case State.WALK:
                    {
                        switch (subState)
                        {
                            case SubState.WALKV1S:
                                xOffset = -25;
                                if (walkV1S.Update(cnt, 0, 0))
                                    subState = SubState.WALKV1L;
                                p1 = walkV1S.GetLocation(0);
                                p2 = walkV1S.GetLocation(1);
                                break;
                            case SubState.WALKV1L:
                                if (direction == Direction.RIGHT)
                                    xPositionCanvas += 6;
                                else
                                    xPositionCanvas -= 6;
                                xOffset = -25;
                                if (walkV1L.Update(cnt, 0, 0) && changeState)
                                    subState = SubState.WALKV1E;
                                p1 = walkV1L.GetLocation(0);
                                p2 = walkV1L.GetLocation(1);
                                break;
                            case SubState.WALKV1E:
                                xOffset = -25;
                                if (walkV1E.Update(cnt, 0, 0))
                                {
                                    subState = SubState.RESTV2;
                                    state = State.REST;
                                    changeState = false;
                                }
                                p1 = walkV1E.GetLocation(0);
                                p2 = walkV1E.GetLocation(1);
                                break;
                            case SubState.WALKV2S:
                                xOffset = -25;
                                if (walkV2S.Update(cnt, 0, 0))
                                    subState = SubState.WALKV2L;
                                p1 = walkV2S.GetLocation(0);
                                p2 = walkV2S.GetLocation(1);
                                break;
                            case SubState.WALKV2L:
                                if (direction == Direction.RIGHT)
                                    xPositionCanvas += 3;
                                else
                                    xPositionCanvas -= 3;
                                xOffset = -25;
                                if (walkV2L.Update(cnt, 0, 0) && changeState)
                                    subState = SubState.WALKV2E;
                                p1 = walkV2L.GetLocation(0);
                                p2 = walkV2L.GetLocation(1);
                                break;
                            case SubState.WALKV2E:
                                xOffset = -25;
                                if (walkV2E.Update(cnt, 0, 0))
                                {
                                    state = State.REST;
                                    subState = SubState.RESTV2;
                                    changeState = false;
                                }
                                p1 = walkV2E.GetLocation(0);
                                p2 = walkV2E.GetLocation(1);
                                break;
                        }
                        break;
                    }
                case State.ACTION_DANCE:
                    {
                        xOffset = -40;
                        if (action_Dance.Update(cnt, 0, 0))
                        {
                            changeState = false;
                            state = State.REST;
                            subState = SubState.RESTV2;
                        }
                        p1 = action_Dance.GetLocation(0);
                        p2 = action_Dance.GetLocation(1);
                        break;
                    }
                case State.FLY:
                    {
                        xOffset = -40;
                        switch (subState)
                        {
                            case SubState.FLYS1:
                                if(flyS1.Update(cnt, 0, 0))
                                    subState = SubState.FLYS2;
                                p1 = flyS1.GetLocation(0);
                                p2 = flyS1.GetLocation(1);
                                yOffset = 0;
                                destY = 0;
                                break;
                            case SubState.FLYS2:
                                if (flyS2.Update(cnt, 0, 0))
                                    subState = SubState.FLYL;
                                p1 = flyS2.GetLocation(0);
                                p2 = flyS2.GetLocation(1);
                                yOffset -= 3;
                                break;
                            case SubState.FLYL:
                                {
                                    
                                    bool stable = true;
                                    if (yOffset >= -30)
                                        subState = SubState.FLYE2;
                                    flyL.Update(cnt, 0, 0);
                                    
                                    // To change the direction of Dino
                                    if (Math.Abs(destX - xPositionCanvas) > 50)
                                    {
                                        if (destX > xPositionCanvas && counter == 0)     // Counter is used for adding delay
                                            direction = Direction.RIGHT;                 // But in next part of code its also
                                        else if(counter == 0)                            // used as input to Sin function
                                            direction = Direction.LEFT;

                                        if (direction == Direction.RIGHT)
                                        {
                                            xPositionCanvas += 3;
                                        }
                                        else
                                        {
                                            xPositionCanvas -= 3;
                                        }
                                        xPositionCanvas = (int)Math.Min(xPositionCanvas, Screen.PrimaryScreen.WorkingArea.Width * 0.8f);
                                        stable = false;
                                    }

                                    if (changeState)
                                        yOffset += 2;
                                    else if (yPositionCanvas > destY && Math.Abs(destY - yPositionCanvas) > lim)
                                    {
                                        lim = 5;
                                        yOffset -= 2;
                                        if (yOffset < -taskBarLocation)
                                            yOffset = -taskBarLocation;
                                        stable = false;
                                    }
                                    else if (yPositionCanvas < destY && Math.Abs(destY - yPositionCanvas) > lim)
                                    {
                                        lim = 5;
                                        yOffset += 2;
                                        if (yOffset > 0)
                                            yOffset = 0;
                                        stable = false;
                                    }
                                    
                                    counter += 10;
                                    if (stable) // This gives effect of hovering using sin function
                                    {
                                        yOffset += (int)(2 * Math.Sin(counter / 100.0));
                                        lim = 50;
                                    }
                                    if (counter > 628)
                                        counter = 0;
                                    p1 = flyL.GetLocation(0);
                                    p2 = flyL.GetLocation(1);

                                    if (counter == 0 && (!stable || Math.Abs(Cursor.Position.X - xPositionCanvas) > 500))
                                    {
                                        destX = Cursor.Position.X;
                                        destY = Cursor.Position.Y;
                                    }
                                    break;
                                }

                            case SubState.FLYE2:
                                if (flyE2.Update(cnt, 0, 0))
                                    subState = SubState.FLYE1;
                                p1 = flyE2.GetLocation(0);
                                p2 = flyE2.GetLocation(1);
                                yOffset += 4;
                                break;
                            case SubState.FLYE1:
                                if (flyE1.Update(cnt, 0, 0))
                                {
                                    state = State.REST;
                                    subState = SubState.RESTV2;
                                    changeState = false;
                                }
                                    
                                p1 = flyE1.GetLocation(0);
                                p2 = flyE1.GetLocation(1);
                                break;
                        }
                        break;
                    }
            }
			
            DrawEye(p1, ref dinoBabyEyeBkgL, ref dinoBabyEyePupilL, Direction.LEFT, true);
            DrawEye(p2, ref dinoBabyEyeBkgR, ref dinoBabyEyePupilR, Direction.RIGHT, false);
            yPositionCanvas = Math.Min(taskBarLocation, taskBarLocation + yOffset);

			if (direction == Direction.RIGHT)
			{
				xOffset *= -1;
				xOffset -= 80;
			}
			this.Location = new Point(xPositionCanvas + xOffset, (int)(yPositionCanvas + yoffsetRest));

            if (direction == Direction.RIGHT)
                canvas.FlipBitmap();
            int xC, yC;
            xC = Cursor.Position.X;
            yC = Cursor.Position.Y;
            if (Properties.Settings.Default.FADE_ON_HOVER)
            {
                if ((xC > xPositionCanvas && xC < xPositionCanvas + canvas.Width) &&
                      (yC > yPositionCanvas && yC < yPositionCanvas + canvas.Height))
                {
                    if (transparencyLevel > 0.1f)
                        transparencyLevel -= 0.05f;
                }
                else if (transparencyLevel < 1)
                    transparencyLevel += 0.008f;
                else
                    transparencyLevel = 1f;
            }
            else
                transparencyLevel = 1f;
            if (transparencyLevel != 1f)
                canvas.SetTransparency(transparencyLevel);
            SetBitmap(canvas.GetBitmap());
            canvas.Clear();
            cnt %= 10000000;

            skip:
            {
                // Just to skip
                if (isPaused)
                {
                    SetBitmap(canvas.GetBitmap());
                    canvas.Clear();
                }
            }
        }
    }
}