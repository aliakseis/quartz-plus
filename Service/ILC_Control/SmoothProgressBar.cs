using System;
using System.Drawing;
using System.Windows.Forms;

namespace ILC_ControlPanel
{
    /// <summary>
    /// Implements smooth progress bar with text
    /// </summary>
    public partial class SmoothProgressBar : UserControl
    {
        /// <summary>
        /// ctor
        /// </summary>
        public SmoothProgressBar()
        {
            InitializeComponent();
        }

        int min = 0;	// Minimum value for progress range
        int max = 100;	// Maximum value for progress range
        int val = 0;		// Current progress
        Color barColor = Color.Blue;		// Color of progress meter
        string progressText = "";

        protected override void OnResize(EventArgs e)
        {
            // Invalidate the control to get a repaint.
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            float percent = (float)(val - min) / (float)(max - min);
            Rectangle rect = this.ClientRectangle;

            // Calculate area for drawing the progress.
            rect.Width = (int)((float)rect.Width * percent);

            using (SolidBrush brush = new SolidBrush(barColor))
            {
                // Draw the progress meter.
                e.Graphics.FillRectangle(brush, rect);

                // Draw a three-dimensional border around the control.
                Draw3DBorder(e.Graphics);

                TextRenderer.DrawText(e.Graphics, progressText, Font, ClientRectangle, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        /// <summary>
        /// Minimum value for progress range
        /// </summary>
        public int Minimum
        {
            get
            {
                return min;
            }

            set
            {
                // Prevent a negative value.
                if (value < 0)
                {
                    min = 0;
                }

                // Make sure that the minimum value is never set higher than the maximum value.
                if (value > max)
                {
                    min = value;
                    min = value;
                }

                // Ensure value is still in range
                if (val < min)
                {
                    val = min;
                }

                // Invalidate the control to get a repaint.
                Invalidate();
            }
        }

        /// <summary>
        /// Maximum value for progress range
        /// </summary>
        public int Maximum
        {
            get
            {
                return max;
            }

            set
            {
                // Make sure that the maximum value is never set lower than the minimum value.
                if (value < min)
                {
                    min = value;
                }

                max = value;

                // Make sure that value is still in range.
                if (val > max)
                {
                    val = max;
                }

                // Invalidate the control to get a repaint.
                Invalidate();
            }
        }

        /// <summary>
        /// Current progress
        /// </summary>
        public int Value
        {
            get
            {
                return val;
            }

            set
            {
                if (val == value)
                    return;

                int oldValue = val;

                // Make sure that the value does not stray outside the valid range.
                if (value < min)
                {
                    val = min;
                }
                else val = value > max ? max : value;

                // Invalidate only the changed area.

                Rectangle newValueRect = ClientRectangle;
                Rectangle oldValueRect = ClientRectangle;

                // Use a new value to calculate the rectangle for progress.
                float percent = (float)(val - min) / (float)(max - min);
                newValueRect.Width = (int)(newValueRect.Width * percent);

                // Use an old value to calculate the rectangle for progress.
                percent = (float)(oldValue - min) / (float)(max - min);
                oldValueRect.Width = (int)(oldValueRect.Width * percent);

                Rectangle updateRect = new Rectangle();

                // Find only the part of the screen that must be updated.
                if (newValueRect.Width > oldValueRect.Width)
                {
                    updateRect.X = oldValueRect.Size.Width;
                    updateRect.Width = newValueRect.Width - oldValueRect.Width;
                }
                else
                {
                    updateRect.X = newValueRect.Size.Width;
                    updateRect.Width = oldValueRect.Width - newValueRect.Width;
                }

                updateRect.Height = Height;

                // Invalidate the intersection region only.
                Invalidate(updateRect);
            }
        }

        /// <summary>
        /// Color of progress meter
        /// </summary>
        public Color ProgressBarColor
        {
            get
            {
                return barColor;
            }

            set
            {
                barColor = value;

                // Invalidate the control to get a repaint.
                Invalidate();
            }
        }

        /// <summary>
        /// Progress bar text
        /// </summary>
        public string ProgressText
        {
            get { return progressText; }
            set 
            {
                if (progressText != value)
                {
                    progressText = value;
                    Invalidate();
                }
            }
        }

        private void Draw3DBorder(Graphics g)
        {
            int PenWidth = (int)Pens.White.Width;

            g.DrawLine(Pens.DarkGray,
                new Point(ClientRectangle.Left, ClientRectangle.Top),
                new Point(ClientRectangle.Width - PenWidth, ClientRectangle.Top));
            g.DrawLine(Pens.DarkGray,
                new Point(ClientRectangle.Left, ClientRectangle.Top),
                new Point(ClientRectangle.Left, ClientRectangle.Height - PenWidth));
            g.DrawLine(Pens.White,
                new Point(ClientRectangle.Left, ClientRectangle.Height - PenWidth),
                new Point(ClientRectangle.Width - PenWidth, ClientRectangle.Height - PenWidth));
            g.DrawLine(Pens.White,
                new Point(ClientRectangle.Width - PenWidth, ClientRectangle.Top),
                new Point(ClientRectangle.Width - PenWidth, ClientRectangle.Height - PenWidth));
        } 

    }
}
