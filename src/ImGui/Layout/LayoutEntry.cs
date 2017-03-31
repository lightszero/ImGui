﻿using System;
namespace ImGui
{
    class LayoutEntry
    {
        public Rect rect;//border-box
        public double contentWidth;//exact content width, pre-calculated from content and style
        public double contentHeight;//exact content height, pre-calculated from content and style
        public double minWidth = 1;//minimum width of content-box
        public double maxWidth = 9999;//maximum width of content-box
        public double minHeight = 1;//minimum height of content-box
        public double maxHeight = 9999;//maximum height of content-box
        public int horizontalStretchFactor = 1;//horizontal stretch factor
        public int verticalStretchFactor = 0;//vertical stretch factor

        public bool HorizontallyStretched { get { return !IsFixedWidth && horizontalStretchFactor > 0; } }
        public bool VerticallyStretched { get { return !IsFixedHeight && verticalStretchFactor > 0; } }

        public bool IsFixedWidth { get { return MathEx.AmostEqual(this.minWidth, this.maxWidth); } }
        public bool IsFixedHeight { get { return MathEx.AmostEqual(this.minHeight, this.maxHeight); } }

        public Style style;

        public LayoutGroup parent;

        public LayoutEntry(Style style, params LayoutOption[] options)
        {
            if (Utility.CurrentOS.IsAndroid)
            {
                this.verticalStretchFactor = 1;
            }
            this.style = style;
            if (options != null)
            {
                this.ApplyOptions(options);
            }
        }

        protected void ApplyOptions(LayoutOption[] options)
        {
            if (options == null)
            {
                return;
            }
            //TODO handle min/max width/height
            for (var i = 0; i < options.Length; i++)
            {
                var option = options[i];
                switch (option.type)
                {
                    case LayoutOption.Type.fixedWidth:
                        if ((double)option.value < this.style.PaddingHorizontal + this.style.BorderHorizontal)
                        {
                            throw new InvalidOperationException(
                                string.Format("The specified width is too small. It must bigger than the horizontal padding and border size ({0}).", this.style.PaddingHorizontal + this.style.BorderHorizontal));
                        }
                        this.minWidth = this.maxWidth = (double)option.value;
                        this.horizontalStretchFactor = 0;
                        break;
                    case LayoutOption.Type.fixedHeight:
                        if ((double)option.value < this.style.PaddingVertical + this.style.BorderVertical)
                        {
                            throw new InvalidOperationException(
                                string.Format("The specified height is too small. It must bigger than the vertical padding and border size ({0}).", this.style.PaddingVertical + this.style.BorderVertical));
                        }
                        this.minHeight = this.maxHeight = (double)option.value;
                        this.verticalStretchFactor = 0;
                        break;
                    case LayoutOption.Type.stretchWidth:
                        this.horizontalStretchFactor = (int)option.value;
                        break;
                    case LayoutOption.Type.stretchHeight:
                        this.verticalStretchFactor = (int)option.value;
                        break;
                }
            }
        }

        public virtual void CalcWidth(double unitPartWidth = -1d)
        {
            if (this.HorizontallyStretched)
            {
                if (unitPartWidth > 0)
                {
                    this.rect.Width = unitPartWidth * this.horizontalStretchFactor;
                    this.contentWidth = this.rect.Width - this.style.PaddingHorizontal - this.style.BorderHorizontal;
                }
                else
                {
                    throw new ArgumentException("The unit part width is invalid", "unitPartWidth");
                }
            }
            else if (this.IsFixedWidth)
            {
                this.rect.Width = this.minWidth;
                this.contentWidth = this.rect.Width - this.style.PaddingHorizontal - this.style.BorderHorizontal;
            }
            else
            {
                this.rect.Width = this.contentWidth + this.style.PaddingHorizontal + this.style.BorderHorizontal;
            }
        }

        public virtual void CalcHeight(double unitPartHeight = -1d)
        {
            if (this.VerticallyStretched)
            {
                if (unitPartHeight > 0)
                {
                    this.rect.Height = unitPartHeight * this.verticalStretchFactor;
                    this.contentHeight = this.rect.Height - this.style.PaddingVertical - this.style.BorderVertical;
                }
                else
                {
                    throw new ArgumentException("The unit part height is invalid", "unitPartHeight");
                }
            }
            else if (this.IsFixedHeight)
            {
                this.rect.Height = this.minHeight;
                this.contentHeight = this.rect.Height - this.style.PaddingVertical - this.style.BorderVertical;
            }
            else
            {
                this.rect.Height = this.contentHeight + this.style.PaddingVertical + this.style.BorderVertical;
            }
        }

        public virtual void SetX(double x)
        {
            this.rect.X = x;
        }

        public virtual void SetY(double y)
        {
            this.rect.Y = y;
        }


        #region Filled layout (not used)
        public virtual void CalcRect() { }
        #endregion
    }
}