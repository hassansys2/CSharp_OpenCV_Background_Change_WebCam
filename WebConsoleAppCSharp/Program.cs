using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;

namespace WebcamBackgroundReplacement
{
    class Program
    {
        static void Main(string[] args)
        {
            // Open the webcam
            VideoCapture capture = new VideoCapture(0);
            if (!capture.IsOpened)
            {
                Console.WriteLine("Error: Could not open webcam.");
                return;
            }

            capture.Set(CapProp.FrameWidth, 640);
            capture.Set(CapProp.FrameHeight, 480);

            

            string[] imgPaths = { "1.jpg", "2.jpg", "3.jpg" };
            Mat[] imgList = new Mat[imgPaths.Length];
            int i  = 0;
            
            for (i = 0; i < imgPaths.Length; i++)
            {
                imgList[i] = CvInvoke.Imread(imgPaths[i], ImreadModes.Color);
                CvInvoke.Resize(imgList[i], imgList[i], new Size(640, 480));
                Console.WriteLine("Loop i = " + i);
                CvInvoke.Imshow("Webcam_" + i, imgList[i]);
                CvInvoke.WaitKey(30);

            }
            Console.WriteLine("Read i = " + i);

            int indexImg = 0;
            Mat frame = new Mat();
            Mat mask = new Mat();
            Mat imgOut = new Mat();
            

            while (true)
            {
                // Capture frame from webcam
                capture.Read(frame);
                if (frame.IsEmpty)
                {
                    Console.WriteLine("Error: Could not read frame from webcam.");
                    break;
                }

                
                // Convert frame to HSV
                Mat hsv = new Mat();
                CvInvoke.CvtColor(frame, hsv, ColorConversion.Bgr2Hsv);

                // Create a mask based on color (for green screen removal)
                CvInvoke.InRange(hsv, new ScalarArray(new MCvScalar(35, 100, 100)),
                                      new ScalarArray(new MCvScalar(85, 255, 255)), mask);

                // Invert mask
                CvInvoke.BitwiseNot(mask, mask);

                // Combine original frame and background image using the mask
                imgOut = new Mat(frame.Size, DepthType.Cv8U, 3);
                imgList[indexImg].CopyTo(imgOut, mask);

                // Combine frame and imgOut
                Mat result = new Mat();
                CvInvoke.BitwiseAnd(frame, frame, result, mask);
                CvInvoke.Add(result, imgOut, imgOut);
                
                // Display the output
                CvInvoke.Imshow("Webcam", imgOut);

                // Handle user input for changing background
                var key = CvInvoke.WaitKey(30);
                if (key == 'a')
                {
                    if (indexImg > 0) indexImg--;
                }
                else if (key == 'd')
                {
                    if (indexImg < imgList.Length - 1) indexImg++;
                }
                else if (key == 'q')
                {
                    break;
                }
            }

            // Cleanup
            capture.Dispose();
            CvInvoke.DestroyAllWindows();
        }
    }
}
