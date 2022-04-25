using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Idemia.App.Eborders.ItalyKiosk.Controls
{
    public class AnimatedLabel : Label
    {

        #region - Properties -

        public new bool AutoSize { get; private set; }

        #endregion

        #region - Methods -

        #region - Common -

        private Size getTextSize(string text)
        {
            using (Graphics graphics = Graphics.FromImage(new Bitmap(1, 1)))
            {
                return graphics.MeasureString(text, Font).ToSize();
            }
        }

        private void invokeAction(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();
        }

        #endregion

        #region - Animation -

        public void AnimateScroll(int animationInterval)
        {
            Task.Run(() =>
            {
                var runAnimation = false;
                var text = Text;
                char chr = (char)160;
                invokeAction(delegate { Text = Text.Replace(' ', chr); });
                invokeAction(delegate { Text = string.Empty; });
                var padding = Padding;
                int usableWidth = Width - Padding.Left - Padding.Right;
                var textSize = getTextSize(Text);
                int textWidth = textSize.Width;
                int fillWidth = usableWidth - textWidth;
                if (fillWidth > 0)
                {
                    Task.WaitAll(Task.Delay(animationInterval));
                    invokeAction(delegate { Text = text; });
                    invokeAction(delegate { Text = Text.Replace(' ', chr); });
                    runAnimation = true;
                    if (RightToLeft == RightToLeft.Yes)
                    {
                        var right = padding.Right + fillWidth;
                        while (padding.Right < right)
                        {
                            invokeAction(delegate { Padding = new Padding(padding.Left, padding.Top, right, padding.Bottom); });
                            right--;
                            Task.WaitAll(Task.Delay(animationInterval));
                            if (!runAnimation) break;
                        }
                    }
                    else
                    {
                        var left = padding.Left + fillWidth;
                        while (padding.Left < left)
                        {
                            invokeAction(delegate { Padding = new Padding(left, padding.Top, padding.Right, padding.Bottom); });
                            left--;
                            Task.WaitAll(Task.Delay(animationInterval));
                            if (!runAnimation) break;
                        }
                    }
                    invokeAction(delegate
                        {
                            Padding = padding;
                            Text = text;
                        });
                }
            });
        }

        public void AnimateLetterByLetter(int animationInterval)
        {
            Task.Run(() =>
            {
                var runAnimation = false;
                var text = Text;
                invokeAction(delegate { Text = string.Empty; });
                Task.WaitAll(Task.Delay(animationInterval));
                runAnimation = true;
                for (int i = 0; i < text.Length; i++)
                {
                    if (!runAnimation)
                        break;

                    invokeAction(delegate { Text = text.Substring(0, i + 1); });
                    Task.WaitAll(Task.Delay(animationInterval));
                }
                invokeAction(delegate { Text = text; });
                runAnimation = false;
            });
        }

        public void AnimateBlink(int animationInterval, Color blinkColor)
        {
            Task.Run(() =>
            {
                var runAnimation = false;
                var text = Text;
                invokeAction(delegate { Text = string.Empty; });
                Task.WaitAll(Task.Delay(animationInterval));
                invokeAction(delegate { Text = text; });
                runAnimation = true;
                Color foreColor = ForeColor;
                do
                {
                    Task.WaitAll(Task.Delay(animationInterval));

                    invokeAction(delegate
                    {
                        if (ForeColor == foreColor)
                            ForeColor = blinkColor;
                        else
                            ForeColor = foreColor;
                    });
                } while (runAnimation);
                invokeAction(delegate { ForeColor = foreColor; });
            });
        }

        #endregion

        #endregion

    }
}
