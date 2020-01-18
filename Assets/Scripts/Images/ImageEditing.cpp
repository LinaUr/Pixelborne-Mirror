/* This file serves as the basis for ImageEditing.dll.
 * If something needs to be changed in ImageEditing.dll, this must be done in this file.
 * Follow the instructions here: https://thomasmountainborn.com/2016/09/12/unity-and-opencv-part-two-project-setup/
 * Please name the project "ImageEditing".
 * As additional dependencies add opencv_core412.lib, opencv_highgui412.lib, 
 * opencv_objdetect412.lib, opencv_imgproc412.lib and opencv_imgcodecs412.lib.
 * Then build the x64 release version of the project and copy the resulting ImageEditing.dll in Assets/Plugins.
 * If you get errors, try adding "_CRT_SECURE_NO_WARNINGS" in the project properties under
 * C/C++ > Preprocessor > Preprocessor definitions and build the project again.
 * If you have any further questions, please contact alexander.junger@student.hpi.uni-potsdam.de.
 * 
 * Code modified from: https://amin-ahmadi.com/2019/06/01/how-to-pass-images-between-opencv-and-unity/
 */

#include <opencv2/opencv.hpp>
#include <Windows.h>
#include <iostream>
#include <stdio.h>
#include <chrono>
#include <ctime>

using namespace cv;

typedef Point3_<uint8_t> Pixel;

struct Color32
{
	uchar red;
	uchar green;
	uchar blue;
	uchar alpha;
};

extern "C" 
{
	// This allows the export of the function processImage() as .dll in other programs like Unity.
	__declspec(dllexport) void processImage(Color32 **rawImage, int width, int height)
	{
		// For debugging in Unity (only under Windows).
		//AllocConsole();
		//freopen_s((FILE**)stdout, "CONOUT$", "w", stdout);

		// CascadeClassifier is an OpenCV class to detect objects in a video stream or image.
		CascadeClassifier faceCascade;

		// This loads the external CascadeClassifier which it should be in the root directory of the Unity project.
		faceCascade.load("lbpcascade_frontalface.xml");

		// If there is any trouble with lbpcascade take haarcascade instead of lbpcascade.
		// Haarcascade is slower than lbpcascade but it is more precise.
		//faceCascade.load("haarcascade_frontalface_alt.xml");

		// Create an opencv object sharing the same data space.
		Mat image(height, width, CV_8UC4, *rawImage);

		// Start processing the image:
		// ********************************************************************************************************

		// Container for detected faces.
		std::vector<Rect> faces;

		// Convert the image to an BGR image and a grayscale image for cascade detection.
		Mat bgrImage, grayscaleImage;
		cvtColor(image, bgrImage, COLOR_RGBA2BGR);
		cvtColor(image, grayscaleImage, COLOR_RGBA2GRAY);

		// Detect faces.
		faceCascade.detectMultiScale(grayscaleImage, faces);

		// The container for the mask, the temporary foreground and the temporary background 
		// for future grabCut (foreground extraction).
		Mat mask, bgdModel, fgdModel;

		// For debugging if you want to know if a face was found.
		/*
		if (faces.size() == 0) {
			std::cout << "0 faces!!" << std::endl;
		}
		else {
		*/
		
		// The padding is used to cut out the found face a little larger.
		// If the result image should be larger, you are free to increase this variable.
		int padding = 20;

		// Cut faces.
		for (int i = 0; i < faces.size(); i++)
		{
			// For debugging to know, how many faces were found.
			//std::cout << faces.size() << " faces!!" << std::endl;

			// Check if the larger result image is still in the original for the extraction.
			// If true then set the height, width and the top left corner on the new values.
			if (faces[i].height + padding < image.rows && faces[i].y - padding / 2 < image.rows 
				&& faces[i].width + padding < image.cols && faces[i].x - padding / 2 < image.cols) {
				faces[i].height += padding;
				faces[i].y -= padding / 2;
				faces[i].width += padding;
				faces[i].x -= padding / 2;
			}

			// Foregroung extraction for every face and save the information of this algorithm in mask.
			// For further information of grabCut() visit:
			// https://docs.opencv.org/3.1.0/d7/d1b/group__imgproc__misc.html#ga909c1dda50efcbeaa3ce126be862b37f
			grabCut(bgrImage, mask, faces[i], bgdModel, fgdModel, 1, GC_INIT_WITH_RECT);

			// Create the result image with the size of the face.
			Mat resultImage(faces[i].height, faces[i].width, CV_8UC3);

			// Fill the result image with data of the original image 
			// but only where there is information in the mask.
			for (int row = faces[i].y; row < faces[i].y + faces[i].height; ++row) {
				uchar* ptrMask = mask.ptr(row, faces[i].x);
				Pixel* ptrResult = resultImage.ptr<Pixel>(row - faces[i].y, 0);
				Pixel* ptrImage = bgrImage.ptr<Pixel>(row, faces[i].x);
				const Pixel* ptr_end = ptrImage + faces[i].width;
				for (; ptrImage != ptr_end; ++ptrImage, ++ptrMask, ++ptrResult) {
					if(ptrMask[0] != 0) {
						ptrResult->x = ptrImage->x;
						ptrResult->y = ptrImage->y;
						ptrResult->z = ptrImage->z;
					}
					else {
						ptrResult->x = 0;
						ptrResult->y = 0;
						ptrResult->z = 0;
					}
				}
			}

			// Flip the image vertically to get the right orientation.
			flip(resultImage, resultImage, 0);

			// Create an unique timestamp for the result image file name with the index of the found face.
			// Code taken from: http://www.cplusplus.com/forum/general/205282/#msg972736
			time_t rawtime;
			struct tm * timeinfo;
			char buffer[80];

			time(&rawtime);
			timeinfo = localtime(&rawtime);

			strftime(buffer, 80, "%d-%m-%Y_%H-%M-%S", timeinfo);

			// Create the string which contains the filepath (Assets/photos) and the filename.
			// You can save the result image as .png, .jpg or .bmp.
			std::stringstream ss;
			ss << "Assets/photos/" << std::to_string(i) << "-" << std::string(buffer) << ".png";
			std::string filename = ss.str();

			// Save the result image.
			imwrite(filename, resultImage);
		}

		// Close else statement for debugging.
		//}

		// ********************************************************************************************************
		// End processing the image.

		// For debugging in Unity (to prevent a crash of Unity).
		//_fcloseall();
		//FreeConsole();
	}
}