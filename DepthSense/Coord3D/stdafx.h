// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <windows.h>



// TODO: reference additional headers your program requires here


////////////////////////////////////////////////////////////////////////////////
// SoftKinetic DepthSense SDK
//
// COPYRIGHT AND CONFIDENTIALITY NOTICE - SOFTKINETIC CONFIDENTIAL
// INFORMATION
//
// All rights reserved to SOFTKINETIC SENSORS NV (a
// company incorporated and existing under the laws of Belgium, with
// its principal place of business at Boulevard de la Plainelaan 15,
// 1050 Brussels (Belgium), registered with the Crossroads bank for
// enterprises under company number 0811 341 454 - "Softkinetic
// Sensors").
//
// The source code of the SoftKinetic DepthSense Camera Drivers is
// proprietary and confidential information of Softkinetic Sensors NV.
//
// For any question about terms and conditions, please contact:
// info@softkinetic.com Copyright (c) 2002-2012 Softkinetic Sensors NV
////////////////////////////////////////////////////////////////////////////////


#ifdef _MSC_VER
#include <windows.h>
#endif

#include <stdio.h>
#include <vector>
#include <exception>
#include <thread>

#include <DepthSense.hxx>

void start();
void run();
void pause();
void stop();

DepthSense::Context& GetContext();
DepthSense::DepthNode& GetDepthNode();
DepthSense::ColorNode& GetColorNode();

namespace ColorExport
{
	struct NewSampleReceivedData
	{
		/// The color map. If \c captureConfiguration::compression is DepthSense::COMPRESSION_TYPE_MJPEG, the output format is BGR, otherwise the output format is YUY2.
		const uint8_t *colorMap;
		int ncolorMap;
		/// The compressed data. If \c captureConfiguration::compression is DepthSense::COMPRESSION_TYPE_MJPEG, this array contains the compmressed data.
		const uint8_t *compressedData;
		int ncompressedData;
		/// the camera configuration that was in effect at the time of capture
		//DepthSense::ColorNode::Configuration captureConfiguration;
		/// the time of capture of the sample, expressed in ?s
		uint64_t timeOfCapture;
		/// the time of arrival of the sample in the library, expressed in ?s
		uint64_t timeOfArrival;
		/// the number of dropped samples since the last \c newSampleReceived event was raised
		int32_t droppedSampleCount;
		/// the number of dropped samples since the streaming was started
		int32_t cumulativeDroppedSampleCount;
	};

	DepthSense::ColorNode::NewSampleReceivedData& GetNodeNewSampleReceivedData();

	void LockData();
	void UnlockData();
}

namespace DepthExport
{
	struct NewSampleReceivedData
	{
		/// the confidence map
		const int16_t *confidenceMap;
		int nconfidenceMap;
		/// The phase map. This map represents the radial phase ([0 - 2£k[) with respect to the center of the depth camera. Valid values lie in the range [0 - 32767]. Saturated pixels are given the special value \c -32767.
		const int16_t *phaseMap;
		int nphaseMap;
		/// The depth map in fixed point format. This map represents the cartesian depth of each pixel, expressed in millimeters. Valid values lies in the range [0 - 31999]. Saturated pixels are given the special value \c 32002.
		const int16_t *depthMap;
		int ndepthMap;
		/// The depth map in floating point format. This map represents the cartesian depth of each pixel, expressed in meters. Saturated pixels are given the special value \c -2.0.
		const float *depthMapFloatingPoint;
		int ndepthMapFloatingPoint;
		/// The vertices in fixed point format. This map represents the cartesian 3D coordinates of each pixel, expressed in millimeters. Saturated pixels are given the special value \c 32002.
		const DepthSense::Vertex *vertices;
		int nvertices;
		/// The vertices in floating point format. This map represents the cartesian 3D coordinates of each pixel, expressed in meters. Saturated pixels are given the special value \c -2.0.
		const DepthSense::FPVertex *verticesFloatingPoint;
		int nverticesFloatingPoint;
		/// The UV mapping. This map represents the normalized coordinates of each pixel in the color map. Invalid pixels are given the special value \c -FLT_MAX.
		const DepthSense::UV *uvMap;
		int nuvMap;
		/// The acceleration of the camera when the frame was captured. The sampling frequency of this value is 1 Hz.
		//DepthSense::DepthNode::Acceleration acceleration;
		/// the system model parameters that were in effect at the time of capture
		//DepthSense::StereoCameraParameters stereoCameraParameters;
		/// the camera configuration that was in effect at the time of capture
		//DepthSense::DepthNode::Configuration captureConfiguration;
		/// the time of capture of the sample, expressed in ?s
		uint64_t timeOfCapture;
		/// the time of arrival of the sample in the library, expressed in ?s
		uint64_t timeOfArrival;
		/// the number of dropped samples since the last \c newSampleReceived event was raised
		int32_t droppedSampleCount;
		/// the number of dropped samples since the streaming was started
		int32_t cumulativeDroppedSampleCount;
	};

	DepthSense::DepthNode::NewSampleReceivedData & GetNodeNewSampleReceivedData();

	void LockData();
	void UnlockData();
}