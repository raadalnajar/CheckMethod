using AVFoundation;
using CoreAnimation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using GANIOS.iOS.Logo;
using GANM.Interface.LogoInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(VideoProcessor))]

namespace GANIOS.iOS.Logo
{
    public class VideoProcessor : ILogoAdder
    {
        public void AddLogoToVideoFromURLs(string videoUrl, string logoUrl, string outputVideoPath, int logoPositionX, int logoPositionY)
        {
            using (var webClient = new WebClient())
            {
                // Download the video from the cloud URL
                byte[] videoBytes = webClient.DownloadData(new Uri(videoUrl));

                // Download the logo from the cloud URL
                byte[] logoBytes = webClient.DownloadData(new Uri(logoUrl));

                // Load the video as NSData
                NSData videoData = NSData.FromArray(videoBytes);

                // Load the logo image using UIImage
                UIImage logoImage = UIImage.LoadFromData(NSData.FromArray(logoBytes));

                // Add the logo to the video
                AddLogoToVideo(videoData, logoImage, outputVideoPath);
            }
        }

        public void AddLogoToVideo(NSData videoData, UIImage logoImage, string outputVideoPath)
        {
            // Convert the video data and logo image to byte arrays
            byte[] videoBytes = videoData.ToArray();

            // Convert the logo image to a byte array and resize it
            UIImage resizedLogoImage = ResizeLogoImage(logoImage, 100, 100); // Adjust the width and height as per your requirement
            byte[] logoBytes = resizedLogoImage.AsPNG().ToArray();

            // Save the video byte array to a temporary file
            string videoPath = Path.Combine(Path.GetTempPath(), "temp_video.mp4");
            File.WriteAllBytes(videoPath, videoBytes);

            // Save the logo byte array to a temporary file
            string logoPath = Path.Combine(Path.GetTempPath(), "temp_logo.png");
            File.WriteAllBytes(logoPath, logoBytes);

            // Define the output path for the video with the logo
            string outputFilePath = Path.Combine(Path.GetTempPath(), outputVideoPath);

            try
            {
                // Load the video asset
                AVAsset videoAsset = AVAsset.FromUrl(NSUrl.FromFilename(videoPath));

                // Create the composition
                AVMutableComposition composition = AVMutableComposition.Create();
                AVMutableCompositionTrack videoTrack = composition.AddMutableTrack(AVMediaType.Video, 0);
                AVMutableCompositionTrack audioTrack = composition.AddMutableTrack(AVMediaType.Audio, 0);

                // Extract the video track from the video asset
                AVAssetTrack sourceVideoTrack = videoAsset.TracksWithMediaType(AVMediaType.Video)[0];
                AVAssetTrack sourceAudioTrack = videoAsset.TracksWithMediaType(AVMediaType.Audio)[0];

                CMTimeRange CreateCMTimeRange(CMTime thestartTime, CMTime theduration)
                {
                    CMTimeRange timeRange;
                    timeRange.Start = thestartTime;
                    timeRange.Duration = theduration;
                    return timeRange;
                }

                // Usage
                CMTime startTime = new CMTime(0, 1); // Start time (0 seconds)
                CMTime duration = videoAsset.Duration; // Duration of the video asset

                /*       CMTimeRange timeRange = new CMTimeRange()
          //     ///        {
                           Start = CMTime.Zero,
                           Duration = videoAsset.Duration
           //     ///       };
                */
                // Insert the video and audio tracks into the composition
                videoTrack.InsertTimeRange(CreateCMTimeRange(startTime, duration), sourceVideoTrack, CMTime.Zero, out _);
                audioTrack.InsertTimeRange(CreateCMTimeRange(startTime, duration), sourceAudioTrack, CMTime.Zero, out _);
                ///   //  videoTrack.InsertTimeRange(timeRange, sourceVideoTrack, CMTime.Zero, out _);
                ///  //   audioTrack.InsertTimeRange(timeRange, sourceAudioTrack, CMTime.Zero, out _);
                // Create the video layer instruction
                AVMutableVideoCompositionLayerInstruction layerInstruction = AVMutableVideoCompositionLayerInstruction.FromAssetTrack(videoTrack);

                // Create the video composition instruction
                AVMutableVideoCompositionInstruction videoCompositionInstruction = (AVMutableVideoCompositionInstruction)AVMutableVideoCompositionInstruction.Create();
                {



                    // Set the theduration of the video composition instruction to the theduration of the video asset
                    CMTimeRange timeRange = CreateCMTimeRange(startTime, duration);
                    videoCompositionInstruction.TimeRange = timeRange;

                };


                // Set the video track and layer instruction to the video composition instruction
                videoCompositionInstruction.LayerInstructions = new[] { layerInstruction };

                // Create the video composition
                AVMutableVideoComposition videoComposition = AVMutableVideoComposition.Create();
                {
                    videoComposition.RenderSize = sourceVideoTrack.NaturalSize;
                    videoComposition.FrameDuration = sourceVideoTrack.MinFrameDuration;
                };

                // Set the video composition instruction to the video composition
                videoComposition.Instructions = new[] { videoCompositionInstruction };

                // Create the logo layer
                CALayer logoLayer = new CALayer()
                {
                    Contents = resizedLogoImage.CGImage,
                    Frame = new CGRect(10, 10, resizedLogoImage.Size.Width, resizedLogoImage.Size.Height)
                };

                // Create the parent layer for the video and logo layers
                CALayer parentLayer = new CALayer()
                {
                    Frame = new CGRect(0, 0, videoComposition.RenderSize.Width, videoComposition.RenderSize.Height)
                };
                // Create an instance of AVPlayerItem with the videoAsset
                AVPlayerItem playerItem = new AVPlayerItem(videoAsset);

                // Create an instance of AVPlayer with the playerItem
                AVPlayer player = new AVPlayer(playerItem);

                // Create an instance of AVPlayerLayer with the player
                AVPlayerLayer playerLayer = AVPlayerLayer.FromPlayer(player);

                // Set the video gravity for the playerLayer (optional)
                playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;

                // Set the frame of the playerLayer to match the parentLayer's bounds
                playerLayer.Frame = parentLayer.Bounds;

                // Add the playerLayer as a sublayer to the parentLayer
                parentLayer.AddSublayer(playerLayer);

                // Add the video and logo layers to the parent layer
                ////////  parentLayer.AddSublayer(AVCaptureVideoPreviewLayer.FromAsset(videoAsset).Layer); // Add video layer
                parentLayer.AddSublayer(logoLayer); // Add logo layer

                // Set the parent layer as the video composition's animation tool

                AVVideoCompositionCoreAnimationTool animationTool = AVVideoCompositionCoreAnimationTool.FromLayer(playerLayer, parentLayer);






                // Create the exporter
                AVAssetExportSession exporter = new AVAssetExportSession(composition, AVAssetExportSessionPreset.HighestQuality)
                {
                    OutputFileType = AVFileType.Mpeg4,
                    OutputUrl = NSUrl.FromFilename(outputFilePath),
                    VideoComposition = videoComposition
                };

                // Perform the export
                exporter.ExportAsynchronously(() =>
                {
                    if (exporter.Status == AVAssetExportSessionStatus.Completed)
                    {
                        // Export completed successfully
                        Console.WriteLine("Logo added to video: " + outputFilePath);
                    }
                    else if (exporter.Status == AVAssetExportSessionStatus.Failed)
                    {
                        // Export failed
                        Console.WriteLine("Error adding logo to video: " + exporter.Error.LocalizedDescription);
                    }

                    // Delete the temporary video and logo files
                    File.Delete(videoPath);
                    File.Delete(logoPath);
                });
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error adding logo to video: " + ex.Message);

                // Delete the temporary video and logo files
                File.Delete(videoPath);
                File.Delete(logoPath);
            }
        }
        public UIImage ResizeLogoImage(UIImage image, nfloat width, nfloat height)
        {
            UIGraphics.BeginImageContextWithOptions(new CGSize(width, height), false, 0.0f);
            image.Draw(new CGRect(0, 0, width, height));
            UIImage resizedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return resizedImage;
        }
    }
}