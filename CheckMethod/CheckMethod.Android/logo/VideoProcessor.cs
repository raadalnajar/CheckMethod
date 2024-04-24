using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GANM.Droid.Logo;
using GANM.Interface.LogoInterface;
using Java.Nio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(VideoProcessor))]

namespace GANM.Droid.Logo
{
    public class VideoProcessor : ILogoAdder
    {
        int desiredWidth = 100;
        int desiredHeight = 100;
        public void AddLogoToVideoFromURLs(string videoUrl, string logoUrl, string outputVideoPath, int logoPositionX, int logoPositionY)
        {
            using (var webClient = new WebClient())
            {
                // Download the video from the cloud URL
                byte[] videoBytes = webClient.DownloadData(new Uri(videoUrl));

                // Download the logo from the cloud URL
                byte[] logoBytes = webClient.DownloadData(new Uri(logoUrl));

                // Load the video as a MemoryStream
                using (MemoryStream videoStream = new MemoryStream(videoBytes))
                {
                    // Load the logo image using BitmapFactory
                    Bitmap logoBitmap = BitmapFactory.DecodeByteArray(logoBytes, 0, logoBytes.Length);

                    // Add the logo to the video
                    AddLogoToVideo(videoStream, logoBitmap, outputVideoPath);
                }
            }
        }
        public void AddLogoToVideo(System.IO.Stream videoStream, Bitmap logoBitmap, string outputVideoPath)
        {
            // Convert the video stream to a byte array

            byte[] videoBytes = ConvertStreamToByteArray(videoStream);

            // Convert the logo bitmap to a byte array and resize it
            Bitmap resizedLogoBitmap = ResizeLogoBitmap(logoBitmap, 100, 100); // Adjust the width and height as per your requirement
            byte[] logoBytes = ConvertBitmapToByteArray(resizedLogoBitmap);

            // Save the video byte array to a temporary file
            string videoPath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "temp_video.mp4");
            System.IO.File.WriteAllBytes(videoPath, videoBytes);

            // Save the logo byte array to a temporary file
            string logoPath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "temp_logo.png");
            System.IO.File.WriteAllBytes(logoPath, logoBytes);

            // Define the output path for the video with the logo
            string outputFilePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, outputVideoPath);

            try
            {
                // Load the input video file
                MediaExtractor videoExtractor = new MediaExtractor();
                videoExtractor.SetDataSource(videoPath);

                // Get the video track format
                MediaFormat videoFormat = null;
                int videoTrackIndex = GetTrackIndex(videoExtractor, "video/");
                if (videoTrackIndex >= 0)
                {
                    videoFormat = videoExtractor.GetTrackFormat(videoTrackIndex);
                    videoExtractor.SelectTrack(videoTrackIndex);
                }

                // Load the logo image file
                Bitmap logoBitmapn = BitmapFactory.DecodeFile(logoPath);


                // Configure the output format
                MediaMuxer muxer = new MediaMuxer(outputFilePath, MuxerOutputType.Mpeg4);
                //MediaMuxer muxer = new MediaMuxer(outputFilePath, MediaMuxer.OutputFormat.MuxerOutputMpeg4);
                // Add the video track to the muxer
                int videoTrack = muxer.AddTrack(videoFormat);

                // Start the muxer
                muxer.Start();

                // Configure the video track with the logo
                MediaCodec.BufferInfo bufferInfo = new MediaCodec.BufferInfo();
                ByteBuffer buffer = ByteBuffer.Allocate(logoBitmapn.ByteCount);
                logoBitmapn.CopyPixelsToBuffer(buffer);
                buffer.Rewind();
                bufferInfo.Size = logoBitmapn.ByteCount;
                bufferInfo.PresentationTimeUs = 0;
                //bufferInfo.Flags = MediaCodec.BufferFlagKeyFrame;
                bufferInfo.Flags = MediaCodecBufferFlags.KeyFrame;
                muxer.WriteSampleData(videoTrack, buffer, bufferInfo);

                // Process the video frames and add them to the muxer
                MediaCodec.BufferInfo videoBufferInfo = new MediaCodec.BufferInfo();
                ByteBuffer videoBuffer = ByteBuffer.Allocate(videoFormat.GetInteger(MediaFormat.KeyMaxInputSize));
                while (true)
                {
                    int sampleSize = videoExtractor.ReadSampleData(videoBuffer, 0);
                    if (sampleSize < 0)
                        break;

                    videoBufferInfo.Size = sampleSize;
                    videoBufferInfo.PresentationTimeUs = videoExtractor.SampleTime;
                    videoBufferInfo.Flags = (MediaCodecBufferFlags.KeyFrame);

                    muxer.WriteSampleData(videoTrack, videoBuffer, videoBufferInfo);
                    videoExtractor.Advance();
                }

                // Stop and release the muxer
                muxer.Stop();
                muxer.Release();
            }
            catch (Exception ex)
            {
                // Handle any errors
                System.Console.WriteLine($"Error adding logo to video: {ex.Message}");
            }
            finally
            {
                // Delete the temporary video and logo files
                System.IO.File.Delete(videoPath);
                System.IO.File.Delete(logoPath);
            }
        }

        private byte[] ConvertStreamToByteArray(System.IO.Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private byte[] ConvertBitmapToByteArray(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                return stream.ToArray();
            }
        }

        private Bitmap ResizeLogoBitmap(Bitmap logoBitmap, int desiredWidth, int desiredHeight)
        {
            // Calculate the aspect ratio of the original logo bitmap
            float aspectRatio = (float)logoBitmap.Width / logoBitmap.Height;

            // Calculate the new width and height based on the desired width and aspect ratio
            int newWidth = desiredWidth;
            int newHeight = (int)(desiredWidth / aspectRatio);

            // Create a new bitmap with the desired dimensions
            Bitmap resizedBitmap = Bitmap.CreateScaledBitmap(logoBitmap, newWidth, newHeight, true);

            return resizedBitmap;
        }

        private int GetTrackIndex(MediaExtractor extractor, string trackType)
        {
            for (int i = 0; i < extractor.TrackCount; i++)
            {
                MediaFormat format = extractor.GetTrackFormat(i);
                string mimeType = format.GetString(MediaFormat.KeyMime);

                if (mimeType.StartsWith(trackType))
                {
                    return i;
                }
            }

            return -1;
        }
    }

}
