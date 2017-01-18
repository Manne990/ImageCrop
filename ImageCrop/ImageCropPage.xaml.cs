using Xamarin.Forms;

namespace ImageCrop
{
    public partial class ImageCropPage : ContentPage
    {
        private const int HandleSize = 30;

        private Rectangle _startRect;

        public ImageCropPage()
        {
            InitializeComponent();

            var boxPan = new PanGestureRecognizer();
            boxPan.PanUpdated += BoxPanUpdated;
            box.GestureRecognizers.Add(boxPan);

            var topLeftPan = new PanGestureRecognizer();
            topLeftPan.PanUpdated += TopLeftHandlePanUpdated;
            topLeft.GestureRecognizers.Add(topLeftPan);

            var topRightPan = new PanGestureRecognizer();
            topRightPan.PanUpdated += TopRightHandleUpdated;
            topRight.GestureRecognizers.Add(topRightPan);

            var bottomLeftPan = new PanGestureRecognizer();
            bottomLeftPan.PanUpdated += BottomLeftHandlePanUpdated;
            bottomLeft.GestureRecognizers.Add(bottomLeftPan);

            var bottomRightPan = new PanGestureRecognizer();
            bottomRightPan.PanUpdated += BottomRightHandlePanUpdated;
            bottomRight.GestureRecognizers.Add(bottomRightPan);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            var handleModifier = HandleSize / 2;

            AbsoluteLayout.SetLayoutBounds(box, new Rectangle(100, 100, width - 200, height - 200));

            AbsoluteLayout.SetLayoutBounds(topLeft, new Rectangle(box.X - handleModifier, box.Y - handleModifier, HandleSize, HandleSize));
            AbsoluteLayout.SetLayoutBounds(topRight, new Rectangle(box.X + box.Width - handleModifier, box.Y - handleModifier, HandleSize, HandleSize));
            AbsoluteLayout.SetLayoutBounds(bottomLeft, new Rectangle(box.X - handleModifier, box.Y + box.Height - handleModifier, HandleSize, HandleSize));
            AbsoluteLayout.SetLayoutBounds(bottomRight, new Rectangle(box.X + box.Width - handleModifier, box.Y + box.Height - handleModifier, HandleSize, HandleSize));
        }

        private void BoxPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _startRect = new Rectangle(box.X, box.Y, box.Width, box.Height);
                    break;

                case GestureStatus.Running:
                    ChangeBoxRect(new Rectangle(e.TotalX, e.TotalY, 0, 0));
                    break;

                case GestureStatus.Completed:
                    PrintCropSize();
                    break;
            }
        }

        private void TopLeftHandlePanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _startRect = new Rectangle(box.X, box.Y, box.Width, box.Height);
                    break;

                case GestureStatus.Running:
                    ChangeBoxRect(new Rectangle(e.TotalX, e.TotalY, -e.TotalX, -e.TotalY));
                    break;

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    PrintCropSize();
                    break;
            }
        }

        private void TopRightHandleUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _startRect = new Rectangle(box.X, box.Y, box.Width, box.Height);
                    break;

                case GestureStatus.Running:
                    ChangeBoxRect(new Rectangle(0, e.TotalY, e.TotalX, -e.TotalY));
                    break;

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    PrintCropSize();
                    break;
            }
        }

        private void BottomLeftHandlePanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _startRect = new Rectangle(box.X, box.Y, box.Width, box.Height);
                    break;

                case GestureStatus.Running:
                    ChangeBoxRect(new Rectangle(e.TotalX, 0, -e.TotalX, e.TotalY));
                    break;

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    PrintCropSize();
                    break;
            }
        }

        private void BottomRightHandlePanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _startRect = new Rectangle(box.X, box.Y, box.Width, box.Height);
                    break;

                case GestureStatus.Running:
                    ChangeBoxRect(new Rectangle(0, 0, e.TotalX, e.TotalY));
                    break;

                case GestureStatus.Completed:
                case GestureStatus.Canceled:
                    PrintCropSize();
                    break;
            }
        }

        private void ChangeBoxRect(Rectangle deltaRect)
        {
            var adjustedRect = AdjustBoxRect(deltaRect);

            box.Layout(adjustedRect);

            var handleModifier = HandleSize / 2;

            topLeft.Layout(new Rectangle(box.X - handleModifier, box.Y - handleModifier, HandleSize, HandleSize));
            topRight.Layout(new Rectangle(box.X + box.Width - handleModifier, box.Y - handleModifier, HandleSize, HandleSize));
            bottomLeft.Layout(new Rectangle(box.X - handleModifier, box.Y + box.Height - handleModifier, HandleSize, HandleSize));
            bottomRight.Layout(new Rectangle(box.X + box.Width - handleModifier, box.Y + box.Height - handleModifier, HandleSize, HandleSize));
        }

        private Rectangle AdjustBoxRect(Rectangle deltaRect)
        {
            var newX = _startRect.X + deltaRect.X;
            var newY = _startRect.Y + deltaRect.Y;
            var newWidth = _startRect.Width + deltaRect.Width;
            var newHeight = _startRect.Height + deltaRect.Height;
            var newRect = new Rectangle(newX, newY, newWidth, newHeight);

            if (newX + newWidth > Width)
            {
                newRect.X = box.X;
                newRect.Width = box.Width;
            }

            if (newX < 0)
            {
                newRect.X = 0;
                newRect.Width = box.Width;
            }

            if (newWidth < 50)
            {
                newRect.Width = box.Width;
            }

            if (newY + newHeight > Height)
            {
                newRect.Y = box.Y;
                newRect.Height = box.Height;
            }

            if (newY < 0)
            {
                newRect.Y = 0;
                newRect.Height = box.Height;
            }

            if (newHeight < 50)
            {
                newRect.Height = box.Height;
            }

            return new Rectangle((int)newRect.Left, (int)newRect.Top, (int)newRect.Width, (int)newRect.Height);
        }

        private void PrintCropSize()
        { 
            System.Diagnostics.Debug.WriteLine($"Crop Rect: {box.X}, {box.Y}, {box.Width}, {box.Height}");
        }
    }
}