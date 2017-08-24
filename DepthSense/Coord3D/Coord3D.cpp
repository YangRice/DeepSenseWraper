// Coord3D.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include <thread>
#include <atomic>

extern "C" __declspec(dllexport) void WINAPI DoNothing()
{
	// donothing
}

using namespace std;
using namespace DepthSense;

template<class T> bool IsNodeSet()
{
	bool ret = false;
	Context &context = GetContext();
	std::vector<DepthSense::Node> nodes = context.getRegisteredNodes();

	for (std::vector<DepthSense::Node>::iterator it = nodes.begin(); it != nodes.end(); it++)
	{
		if (it->is<T>())
		{
			ret = it->isSet();
			break;
		}
	}
	return ret;
}

extern "C" __declspec(dllexport) void WINAPI Initialize()
{
	run();
}

extern "C" __declspec(dllexport) void WINAPI Finalize()
{
	pause();
	//stop();
}

extern "C" __declspec(dllexport) void WINAPI LockColorMutex()
{
	ColorExport::LockData();
}

extern "C" __declspec(dllexport) void WINAPI LockDepthMutex()
{
	DepthExport::LockData();
}

extern "C" __declspec(dllexport) void WINAPI UnlockColorMutex()
{
	ColorExport::UnlockData();
}

extern "C" __declspec(dllexport) void WINAPI UnlockDepthMutex()
{
	DepthExport::UnlockData();
}

extern "C" __declspec(dllexport) int WINAPI IsDepthNodeSet()
{
	return IsNodeSet<DepthNode>();
}

extern "C" __declspec(dllexport) int WINAPI IsColorNodeSet()
{
	return IsNodeSet<ColorNode>();
}

extern "C" __declspec(dllexport) int WINAPI SizeofStruct(char *structName)
{
	if (strcmp(structName, "ColorExport::NewSampleReceivedData") == 0)
		return sizeof(ColorExport::NewSampleReceivedData);
	else if (strcmp(structName, "DepthExport::NewSampleReceivedData") == 0)
		return sizeof(DepthExport::NewSampleReceivedData);
	else if (strcmp(structName, "DepthSense::Vertex") == 0)
		return sizeof(DepthSense::Vertex);
	else if (strcmp(structName, "DepthSense::FPVertex") == 0)
		return sizeof(DepthSense::FPVertex);
	else if (strcmp(structName, "DepthSense::UV") == 0)
		return sizeof(DepthSense::UV);
	else
		return 0;
}

#define ASSIGN_MEMBER(src, dst, field)	(dst).field = (src).field
#define ASSIGN_ARRAY(src, dst, field)	\
	(dst).field = (src).field;\
	(dst).n##field = (src).field.size();

extern "C" __declspec(dllexport) ColorExport::NewSampleReceivedData WINAPI GetColorData(void)
{
	static ColorNode::NewSampleReceivedData data;
	ColorExport::NewSampleReceivedData ret;

	ColorExport::LockData();
	try
	{
		ColorNode::NewSampleReceivedData newData = ColorExport::GetNodeNewSampleReceivedData();
		ASSIGN_ARRAY(newData, ret, colorMap);
		ASSIGN_ARRAY(newData, ret, compressedData);
		ASSIGN_MEMBER(newData, ret, timeOfCapture);
		ASSIGN_MEMBER(newData, ret, timeOfArrival);
		ASSIGN_MEMBER(newData, ret, droppedSampleCount);
		ASSIGN_MEMBER(newData, ret, cumulativeDroppedSampleCount);
	}
	catch (...)
	{
	}
	ColorExport::UnlockData();
	return ret;
}

extern "C" __declspec(dllexport) DepthExport::NewSampleReceivedData WINAPI GetDepthData(void)
{
	static DepthNode::NewSampleReceivedData data;
	DepthExport::NewSampleReceivedData ret;

	DepthExport::LockData();
	try
	{
		DepthNode::NewSampleReceivedData newData = DepthExport::GetNodeNewSampleReceivedData();
		ASSIGN_ARRAY(newData, ret, confidenceMap);
		ASSIGN_ARRAY(newData, ret, phaseMap);
		ASSIGN_ARRAY(newData, ret, depthMap);
		ASSIGN_ARRAY(newData, ret, depthMapFloatingPoint);
		ASSIGN_ARRAY(newData, ret, vertices);
		ASSIGN_ARRAY(newData, ret, verticesFloatingPoint);
		ASSIGN_ARRAY(newData, ret, uvMap);
		ASSIGN_MEMBER(newData, ret, timeOfCapture);
		ASSIGN_MEMBER(newData, ret, timeOfArrival);
		ASSIGN_MEMBER(newData, ret, droppedSampleCount);
		ASSIGN_MEMBER(newData, ret, cumulativeDroppedSampleCount);
	}
	catch (...)
	{
	}
	DepthExport::UnlockData();
	return ret;
}