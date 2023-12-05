using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using lorcana.Cards;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace lorcanaApp
{
    public partial class CardDetailPage : ContentPage
    {
        const int fps = 60;
        const float maxRot = 0.01f;
        const string KEY_ENABLE_VIEWER = "KEY_ENABLE_VIEWER";
        const string KEY_ENABLE_GYRO = "KEY_ENABLE_GYRO";

        private SKBitmap rawPlaceholderBitmap;
        private SKBitmap placeholderBitmap;
        private SKBitmap rawResourceBitmap;
        private SKBitmap resourceBitmap;
        private SKBitmap oldResourceBitmap;
        private float xRotation;
        private float yRotation;
        private float canvasWidth;
        private double skiaViewHeight;
        private float skiaViewCanvasHeight;
        private float canvasHeight;
        private float updateDelta;
        private float gyroX;
        private float gyroY;
        private DateTime lastUpdate;
        private List<Card> cards;
        private int index;
        private bool forwardSwitch;
        private float oldResourceX;
        private float resourceX;
        private double oldWidth;
        private double oldHeight;
        private float resBitmapAlpha;
        private Card currentCard;
        private bool viewerEnabled;
        private bool gyroEnabled;
        private int drawsLeft = 100;
        private float glareIntensity = 1;

        public Card CurrentCard { get => currentCard; set { currentCard = value; OnPropertyChanged(); } }

        public CardDetailPage(List<Card> cards, int index)
        {
            this.cards = cards;
            this.index = index;
            BindingContext = this;
            lastUpdate = DateTime.Now;
            CurrentCard = cards[index];

            InitializeComponent();

            scrollView.SwipeLeft += ScrollView_SwipeLeft;
            scrollView.SwipeRight += ScrollView_SwipeRight;
            scrollView.Scrolled += ScrollView_Scrolled;
            skiaView.PaintSurface += SkiaView_PaintSurface;

            viewerEnabled = Preferences.Get(KEY_ENABLE_VIEWER, true);
            skiaView.Opacity = viewerEnabled ? 1 : 0;
            enableViewer.IsToggled = viewerEnabled;
            enableViewer.Toggled += EnableViewer_Toggled;

            gyroEnabled = Preferences.Get(KEY_ENABLE_GYRO, false);
            if (gyroEnabled)
            {
                glareIntensity = 1;
            }
            else
            {
                glareIntensity = 0;
            }
            enableGyro.IsToggled = gyroEnabled;
            enableGyro.Toggled += EnableGyro_Toggled;
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;

            SwitchImage();
            rawPlaceholderBitmap = SKBitmap.Decode(EmbeddedResources.GetResourceStream("Resources.card.png"));

            if (viewerEnabled) StartDrawing();
        }

        private void StartDrawing()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(1000 / fps), () =>
            {
                var ms = DateTime.Now.Subtract(lastUpdate).TotalMilliseconds;
                updateDelta = (float)ms / (1000 / fps);
                lastUpdate = DateTime.Now;
                if (drawsLeft > 0)
                {
                    drawsLeft--;
                    xRotation = Clamp(xRotation + (gyroY * -0.0004f * updateDelta), -maxRot, maxRot);
                    yRotation = Clamp(yRotation + (gyroX * -0.0004f * updateDelta), -maxRot, maxRot);
                    skiaView.InvalidateSurface();
                }
                return viewerEnabled;
            });
        }

        private void EnableViewer_Toggled(object sender, ToggledEventArgs e)
        {
            viewerEnabled = !viewerEnabled;
            Preferences.Set(KEY_ENABLE_VIEWER, viewerEnabled);
            if (viewerEnabled)
            {
                Task.Run(() => DownloadImage(CurrentCard.Number, Helpers.SetcodeToNumber(CurrentCard.SetCode)));
                skiaView.FadeTo(1);
                StartDrawing();
            }
            else
            {
                skiaView.FadeTo(0);
                enableGyro.IsToggled = false;
            }
            SetSpacerHeight();
        }

        private void SetSpacerHeight()
        {
            skSpacer.HeightRequest = viewerEnabled ? skiaView.HeightRequest : 0;
        }

        private void EnableGyro_Toggled(object sender, ToggledEventArgs e)
        {
            gyroEnabled = !gyroEnabled;
            Preferences.Set(KEY_ENABLE_GYRO, gyroEnabled);
            drawsLeft = 10;
            if (gyroEnabled)
            {
                TryStartGyro();
            }
            else
            {
                gyroX = 0;
                gyroY = 0;
                TryStopGyro();
            }
        }

        private void ScrollView_SwipeRight(object sender, EventArgs e)
        {
            SwipeRight();
        }

        private void ScrollView_SwipeLeft(object sender, EventArgs e)
        {
            SwipeLeft();
        }

        private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            skiaView.HeightRequest = skiaViewHeight - e.ScrollY;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (oldWidth == width && oldHeight == height)
            {
                return;
            }
            oldWidth = width;
            oldHeight = height;
            if (width > 0)
            {
                skiaView.HeightRequest = height * 0.8;
                skiaViewHeight = -1;
                SetSpacerHeight();
            }
        }

        private void SwipeLeft()
        {
            if (index == cards.Count - 1)
            {
                return;
            }
            drawsLeft = 60;
            index++;
            CurrentCard = cards[index];
            forwardSwitch = true;
            SwitchImage();
            resourceX = float.MinValue;
        }

        private void SwipeRight()
        {
            if (index == 0)
            {
                return;
            }
            drawsLeft = 60;
            index--;
            CurrentCard = cards[index];
            forwardSwitch = false;
            SwitchImage();
            resourceX = float.MaxValue;
        }

        private void SwitchImage()
        {
            oldResourceX = 0;
            oldResourceBitmap = resourceBitmap;
            resourceBitmap = null;
            Title = CurrentCard.NumberDisplay + " - " + CurrentCard.Title;
            Task.Run(() => DownloadImage(CurrentCard.Number, Helpers.SetcodeToNumber(CurrentCard.SetCode)));
        }

        private void DownloadImage(string numberStr, int setNumber)
        {
            if (!viewerEnabled)
            {
                return;
            }
            int.TryParse(numberStr, out int number);
            HttpWebResponse response = null;
            string url = $"https://images.dreamborn.ink/cards/en/{setNumber:D3}-{number:D3}_1468x2048.webp";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    byte[] stream = null;
                    using (var webClient = new WebClient())
                    {
                        stream = webClient.DownloadData(url);
                    }
                    resBitmapAlpha = 0;
                    rawResourceBitmap = SKBitmap.Decode(stream);
                    var resized = ResizeBitmap(rawResourceBitmap, (int)(skiaView.CanvasSize.Width * 0.9f), (int)(skiaViewCanvasHeight * 0.9f)); ;
                    resourceBitmap = AddRoundedCorners(resized, resized.Width * 0.055f);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        public SKBitmap AddRoundedCorners(SKBitmap sourceBitmap, float cornerRadius)
        {
            int width = sourceBitmap.Width;
            int height = sourceBitmap.Height;

            // Erstellen einer leeren Bitmap für das Ergebnis
            SKBitmap roundedBitmap = new SKBitmap(width, height);

            using (SKCanvas canvas = new SKCanvas(roundedBitmap))
            {
                canvas.Clear(SKColors.Transparent);

                // Erstellen eines abgerundeten Rechtecks mit dem gewünschten Eckradius
                SKRect rect = new SKRect(0, 0, width, height);

                // Festlegen der Farbe und des Pinsels für das abgerundete Rechteck
                using (SKPaint paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.Color = SKColors.Black; // Farbe für das Rechteck ändern, falls gewünscht
                    paint.Style = SKPaintStyle.Fill;
                    canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                }

                // Zeichnen Sie die ursprüngliche Bitmap auf die abgerundete Bitmap, wobei nur der Bereich innerhalb des abgerundeten Rechtecks sichtbar ist.
                using (SKPaint sourcePaint = new SKPaint())
                {
                    sourcePaint.IsAntialias = true;
                    sourcePaint.BlendMode = SKBlendMode.SrcIn;
                    sourcePaint.FilterQuality = SKFilterQuality.High; // Anpassen der Filterqualität nach Bedarf

                    canvas.DrawBitmap(sourceBitmap, rect, sourcePaint);
                }
            }

            return roundedBitmap;
        }

        public SKBitmap ResizeBitmap(SKBitmap sourceBitmap, int maxWidth, int maxHeight)
        {
            if (sourceBitmap == null || maxWidth <= 0 || maxHeight <= 0)
            {
                return null; // Invalid input
            }

            int sourceWidth = sourceBitmap.Width;
            int sourceHeight = sourceBitmap.Height;

            float aspectRatio = (float)sourceWidth / sourceHeight;

            int targetWidth, targetHeight;

            float widthRatio = (float)maxWidth / sourceWidth;
            float heightRatio = (float)maxHeight / sourceHeight;

            if (widthRatio < heightRatio)
            {
                targetWidth = maxWidth;
                targetHeight = (int)(maxWidth / aspectRatio);
            }
            else
            {
                targetHeight = maxHeight;
                targetWidth = (int)(maxHeight * aspectRatio);
            }

            // Create a new SKBitmap with the target dimensions
            SKBitmap resizedBitmap = new SKBitmap(targetWidth, targetHeight);

            using (SKCanvas canvas = new SKCanvas(resizedBitmap))
            using (SKPaint paint = new SKPaint())
            {
                paint.FilterQuality = SKFilterQuality.High;
                canvas.Clear(SKColors.Transparent);

                // Draw the source bitmap onto the resized canvas
                SKRect destRect = new SKRect(0, 0, targetWidth, targetHeight);
                canvas.DrawBitmap(sourceBitmap, destRect, paint);
            }

            return resizedBitmap;
        }

        private float Clamp(float value, float min, float max) => Math.Min(max, Math.Max(min, value));

        private void SkiaView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {

            var canvas = e.Surface.Canvas;

            if (canvasWidth != canvas.LocalClipBounds.Width || canvasHeight != canvas.LocalClipBounds.Height)
            {
                canvasWidth = canvas.LocalClipBounds.Width;
                canvasHeight = canvas.LocalClipBounds.Height;
                if (skiaViewHeight == -1)
                {
                    skiaViewHeight = skiaView.Height;
                    skiaViewCanvasHeight = skiaView.CanvasSize.Height;
                    resourceBitmap = ResizeBitmap(rawResourceBitmap, (int)(skiaView.CanvasSize.Width * 0.9f), (int)(skiaViewCanvasHeight * 0.9f));
                    placeholderBitmap = ResizeBitmap(rawPlaceholderBitmap, (int)(skiaView.CanvasSize.Width * 0.9f), (int)(skiaViewCanvasHeight * 0.9f));
                }
            }

            canvas.Clear(SKColors.White);
            
            {
                SKPoint startPoint = new SKPoint(0, 0);
                SKPoint endPoint = new SKPoint(e.Info.Width * 0.8f, e.Info.Height);
                SKColor.TryParse("#181A3D", out SKColor blue);
                SKColor.TryParse("#3C1D3C", out SKColor purple);
                SKColor[] colors = { blue, purple, blue };
                float[] colorPos = { 0.0f, 0.6f, 1.0f };
                SKShader shader = SKShader.CreateLinearGradient(startPoint, endPoint, colors, colorPos, SKShaderTileMode.Clamp);

                SKPaint paint = new SKPaint
                {
                    Shader = shader
                };

                canvas.DrawRect(new SKRect(0, 0, e.Info.Width, e.Info.Height), paint);
            }

            float centerX = canvasWidth / 2f;
            float centerY = canvasHeight / 2f;

            SKMatrix perspectiveMatrix = SKMatrix.CreateIdentity();
            perspectiveMatrix.Persp0 = xRotation / 100;
            perspectiveMatrix.Persp1 = yRotation / 100;

            SKMatrix matrix = SKMatrix.CreateTranslation(-centerX, -centerY);
            matrix = matrix.PostConcat(perspectiveMatrix);
            float maxPerspective = Math.Max(Math.Abs(perspectiveMatrix.Persp0), Math.Abs(perspectiveMatrix.Persp1)) * 500;
            matrix = matrix.PostConcat(SKMatrix.CreateScale(1 - maxPerspective, 1 - maxPerspective));
            float heightFactor = (float)(skiaView.Height / skiaViewHeight);
            matrix = matrix.PostConcat(SKMatrix.CreateScale(heightFactor, heightFactor));
            matrix = matrix.PostConcat(SKMatrix.CreateTranslation(centerX, centerY));

            canvas.SetMatrix(matrix);
            if (oldResourceBitmap != null)
            {
                // Coordinates to center bitmap on canvas
                float x = centerX - oldResourceBitmap.Width / 2;
                float y = centerY - oldResourceBitmap.Height / 2;
                float lerpEnd = forwardSwitch ? -skiaView.CanvasSize.Width : skiaView.CanvasSize.Width;
                oldResourceX = Lerp(oldResourceX, lerpEnd / heightFactor, 0.3f);
                canvas.DrawBitmap(oldResourceBitmap,  x + oldResourceX, y, new SKPaint { });
            }
            if (resourceBitmap != null || placeholderBitmap != null)
            {
                SKBitmap currentBitmap = resourceBitmap ?? placeholderBitmap;
                // Coordinates to center bitmap on canvas
                float x = centerX - currentBitmap.Width / 2;
                float y = centerY - currentBitmap.Height / 2;
                if (resourceX != 0)
                {
                    if (resourceX == float.MaxValue)
                    {
                        resourceX = - skiaView.CanvasSize.Width / heightFactor;
                    }
                    else if (resourceX == float.MinValue)
                    {
                        resourceX = skiaView.CanvasSize.Width / heightFactor;
                    }
                    resourceX = Lerp(resourceX, 0, 0.25f);
                }
                if (placeholderBitmap != null)
                {
                    canvas.DrawBitmap(placeholderBitmap, x + resourceX, y, new SKPaint { Color = SKColors.White.WithAlpha(100) });
                }
                if (resourceBitmap != null)
                {
                    resBitmapAlpha = Lerp(resBitmapAlpha, 255, 0.23f);
                    canvas.DrawBitmap(resourceBitmap, x + resourceX, y, new SKPaint { IsAntialias = true, Color = SKColors.White.WithAlpha((byte)resBitmapAlpha) });
                }

                SKPoint startPoint = new SKPoint(x, y);
                SKPoint endPoint = new SKPoint(currentBitmap.Width + x, currentBitmap.Height + y);
                SKColor.TryParse("#00FFFFFF", out SKColor transparent);
                SKColor.TryParse("#FFFFFF", out SKColor highlight);
                float shineAddition = (xRotation + yRotation) * 100;
                SKColor[] colors = { transparent, highlight.WithAlpha(Convert.ToByte(glareIntensity * Math.Max(0, Math.Min(50, 50 - (Math.Abs(shineAddition) * 30))))), transparent };
                float[] colorPos = { 0.05f + shineAddition, 0.4f + shineAddition, 0.9f + shineAddition };
                SKShader shader = SKShader.CreateLinearGradient(startPoint, endPoint, colors, colorPos, SKShaderTileMode.Clamp);

                SKPaint paint = new SKPaint
                {
                    Shader = shader
                };

                SKRect rect = new SKRect(x + resourceX, y, currentBitmap.Width + x + resourceX, currentBitmap.Height + y);
                var cornerRadius = currentBitmap.Width * 0.055f;
                canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
            }

            glareIntensity = Lerp(glareIntensity, gyroEnabled ? 1 : 0, 0.03f);
            xRotation = Lerp(xRotation, 0, 0.03f, 0.0000001f);
            yRotation = Lerp(yRotation, 0, 0.03f, 0.0000001f);

            if (Math.Abs(xRotation) > 0f ||
                Math.Abs(yRotation) > 0f ||
                Math.Abs(resourceX) > 0f ||
                Math.Abs(resBitmapAlpha) < 250 ||
                (glareIntensity > 0 &&
                glareIntensity < 1))
            {
                drawsLeft = 60;
            }

            canvas.ResetMatrix();
#if DEBUG
            canvas.DrawText(drawsLeft.ToString(), 8, 40, new SKPaint { Color = SKColors.White, TextSize = 40 });
            canvas.DrawText(gyroX .ToString(), 8, 90, new SKPaint { Color = SKColors.White, TextSize = 40 });
            canvas.DrawText(gyroY.ToString(), 8, 140, new SKPaint { Color = SKColors.White, TextSize = 40 });
            canvas.DrawText(resourceX.ToString(), 8, 190, new SKPaint { Color = SKColors.White, TextSize = 40 });
            canvas.DrawText(xRotation.ToString(), 8, 240, new SKPaint { Color = SKColors.White, TextSize = 40 });
            canvas.DrawText(yRotation.ToString(), 8, 290, new SKPaint { Color = SKColors.White, TextSize = 40 });
#endif
        }

        public static float Lerp(float start, float end, float t, float endAccuracy = 0.001f)
        {
            t = Math.Max(0, Math.Min(1, t));
            var change = (end - start) * t;
            if (Math.Abs(change) < endAccuracy) return end;
            return start + change;
        }

        private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            if (!gyroEnabled)
            {
                gyroX = 0;
                gyroY = 0;
                return;
            }
            gyroX = e.Reading.AngularVelocity.X;
            gyroY = e.Reading.AngularVelocity.Y;
            if (Math.Abs(gyroX) > 0.1f || Math.Abs(gyroY) > 0.1f)
            {
                drawsLeft = 60;
            }
        }

        private void TryStartGyro()
        {
            try
            {
                Gyroscope.Start(SensorSpeed.UI);
            }
            catch (Exception ex)
            {

            }
        }

        private void TryStopGyro()
        {
            try
            {
                Gyroscope.Stop();
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnAppearing()
        {
            if (!gyroEnabled)
            {
                return;
            }
            TryStartGyro();
        }

        protected override void OnDisappearing()
        {
            if (!gyroEnabled)
            {
                return;
            }
            TryStopGyro();
        }

        void Prev_Clicked(System.Object sender, System.EventArgs e)
        {
            SwipeRight();
        }

        void Next_Clicked(System.Object sender, System.EventArgs e)
        {
            SwipeLeft();
        }
    }
}

