using System;
using System.Net;
using System.Threading.Tasks;
using lorcana.Cards;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace lorcanaApp
{
    public class CardDetailPage : ContentPage
    {
        private SKCanvasView skiaView;
        private SKBitmap resourceBitmap;
        private float xRotation;
        private float yRotation;
        private float canvasWidth;
        private float canvasHeight;
        private DateTime lastReading;

        public CardDetailPage(Card card)
        {
            lastReading = DateTime.Now;
            Title = card.Display;
            Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
            Content = skiaView = new SKCanvasView();
            skiaView.PaintSurface += SkiaView_PaintSurface;
            Task.Run(() => DownloadImage(card.Image));
        }

        private void DownloadImage(string url)
        {
            HttpWebResponse response = null;

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
                    resourceBitmap = ResizeBitmap(SKBitmap.Decode(stream), (int)(skiaView.CanvasSize.Width * 0.9f), (int)(skiaView.CanvasSize.Height * 0.9f));
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

        private void SkiaView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            if (canvasWidth != canvas.LocalClipBounds.Width || canvasHeight != canvas.LocalClipBounds.Height)
            {
                canvasWidth = canvas.LocalClipBounds.Width;
                canvasHeight = canvas.LocalClipBounds.Height;
                resourceBitmap = ResizeBitmap(resourceBitmap, (int)(skiaView.CanvasSize.Width * 0.9f), (int)(skiaView.CanvasSize.Height * 0.9f));
            }

            canvas.Clear(SKColors.White);

            // Definieren Sie den Farbverlauf
            SKPoint startPoint = new SKPoint(0, 0);
            SKPoint endPoint = new SKPoint(e.Info.Width, e.Info.Height);
            SKColor[] colors = { SKColors.DarkBlue, SKColors.Purple };
            float[] colorPos = { 0.0f, 1.0f };
            SKShader shader = SKShader.CreateLinearGradient(startPoint, endPoint, colors, colorPos, SKShaderTileMode.Clamp);

            // Zeichnen Sie einen Rechteck mit dem Farbverlauf
            SKPaint paint = new SKPaint
            {
                Shader = shader
            };

            canvas.DrawRect(new SKRect(0, 0, e.Info.Width, e.Info.Height), paint);

            float centerX = canvasWidth / 2f;
            float centerY = canvasHeight / 2f;

            SKMatrix perspectiveMatrix = SKMatrix.CreateIdentity();
            perspectiveMatrix.Persp0 = xRotation / 100;
            perspectiveMatrix.Persp1 = yRotation / 100;

            SKMatrix matrix = SKMatrix.CreateTranslation(-centerX, -centerY);
            matrix = matrix.PostConcat(perspectiveMatrix);
            float maxPerspective = Math.Max(Math.Abs(perspectiveMatrix.Persp0), Math.Abs(perspectiveMatrix.Persp1)) * 500;
            matrix = matrix.PostConcat(SKMatrix.CreateScale(1 - maxPerspective, 1 - maxPerspective));
            matrix = matrix.PostConcat(SKMatrix.CreateTranslation(centerX, centerY));

            canvas.SetMatrix(matrix);
            if (resourceBitmap != null)
            {
                // Coordinates to center bitmap on canvas
                float x = centerX - resourceBitmap.Width / 2;
                float y = centerY - resourceBitmap.Height / 2;
                canvas.DrawBitmap(resourceBitmap, x, y);
            }

            xRotation = Lerp(xRotation, 0, 0.03f);
            yRotation = Lerp(yRotation, 0, 0.03f);
        }

        public static float Lerp(float start, float end, float t)
        {
            t = Math.Max(0, Math.Min(1, t));

            return start + (end - start) * t;
        }

        private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            var ms = DateTime.Now.Subtract(lastReading).TotalMilliseconds;
            float delta = (float)ms / 16f;
            xRotation += e.Reading.AngularVelocity.Y * -0.0004f * delta;
            yRotation += e.Reading.AngularVelocity.X * -0.0004f * delta;
            lastReading = DateTime.Now;
            skiaView.InvalidateSurface();
        }

        protected override void OnAppearing()
        {
            try
            {
                Gyroscope.Start(SensorSpeed.Game);
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnDisappearing()
        {
            try
            {
                Gyroscope.Stop();
            }
            catch (Exception ex)
            {

            }
        }
    }
}

