// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include <iostream>
#include <thread>
#include <mutex>
#include <atomic>
#include <windows.h>

using namespace DepthSense;
using namespace std;

static recursive_mutex colorMutex;
static recursive_mutex depthMutex;

static Context g_context;
static DepthNode g_dnode;
static ColorNode g_cnode;
static AudioNode g_anode;

static ColorNode::NewSampleReceivedData colorData;
static DepthNode::NewSampleReceivedData depthData;

static bool g_bDeviceFound = false;

static thread *g_thd = nullptr;
static atomic<bool> g_stop(true);
static atomic<bool> g_start(false);

DepthSense::ColorNode::NewSampleReceivedData& ColorExport::GetNodeNewSampleReceivedData()
{
	return colorData;
}
DepthSense::DepthNode::NewSampleReceivedData& DepthExport::GetNodeNewSampleReceivedData()
{
	return depthData;
}

void ColorExport::LockData()
{
	colorMutex.lock();
}

void DepthExport::LockData()
{
	depthMutex.lock();
}

void ColorExport::UnlockData()
{
	colorMutex.unlock();
}

void DepthExport::UnlockData()
{
	depthMutex.unlock();
}

Context& GetContext()
{
	return g_context;
}

DepthNode& GetDepthNode()
{
	return g_dnode;
}

ColorNode& GetColorNode()
{
	return g_cnode;
}

/*----------------------------------------------------------------------------*/
// New audio sample event handler
void onNewAudioSample(AudioNode node, AudioNode::NewSampleReceivedData data)
{
}

/*----------------------------------------------------------------------------*/
// New color sample event handler
void onNewColorSample(ColorNode node, ColorNode::NewSampleReceivedData data)
{
	colorMutex.lock();
	try
	{
		colorData = data;
	}
	catch (...)
	{
	}
	colorMutex.unlock();
}

/*----------------------------------------------------------------------------*/
// New depth sample event handler
void onNewDepthSample(DepthNode node, DepthNode::NewSampleReceivedData data)
{
	depthMutex.lock();
	try
	{
		depthData = data;
	}
	catch (...)
	{
	}
	depthMutex.unlock();

	if (g_stop)
	{
		g_context.quit();
	}
}

/*----------------------------------------------------------------------------*/
void configureAudioNode()
{
	g_anode.newSampleReceivedEvent().connect(&onNewAudioSample);

	AudioNode::Configuration config = g_anode.getConfiguration();
	config.sampleRate = 44100;

	try
	{
		g_context.requestControl(g_anode, 0);

		g_anode.setConfiguration(config);

		g_anode.setInputMixerLevel(0.5f);
	}
	catch (ArgumentException& e)
	{
		printf("Argument Exception: %s\n", e.what());
	}
	catch (UnauthorizedAccessException& e)
	{
		printf("Unauthorized Access Exception: %s\n", e.what());
	}
	catch (ConfigurationException& e)
	{
		printf("Configuration Exception: %s\n", e.what());
	}
	catch (StreamingException& e)
	{
		printf("Streaming Exception: %s\n", e.what());
	}
	catch (TimeoutException&)
	{
		printf("TimeoutException\n");
	}
}

/*----------------------------------------------------------------------------*/
void configureDepthNode()
{
	g_dnode.newSampleReceivedEvent().connect(&onNewDepthSample);

	DepthNode::Configuration config = g_dnode.getConfiguration();
	config.frameFormat = FRAME_FORMAT_QVGA;
	config.framerate = 30;
	config.mode = DepthNode::CAMERA_MODE_CLOSE_MODE;
	config.saturation = true;

	g_dnode.setEnableDepthMap(true);
	g_dnode.setEnableVertices(true);
	g_dnode.setEnableAccelerometer(true);
	g_dnode.setEnableUvMap(true);
	g_dnode.setEnableConfidenceMap(true);

	try
	{
		g_context.requestControl(g_dnode, 0);

		g_dnode.setConfiguration(config);
	}
	catch (ArgumentException& e)
	{
		printf("Argument Exception: %s\n", e.what());
	}
	catch (UnauthorizedAccessException& e)
	{
		printf("Unauthorized Access Exception: %s\n", e.what());
	}
	catch (IOException& e)
	{
		printf("IO Exception: %s\n", e.what());
	}
	catch (InvalidOperationException& e)
	{
		printf("Invalid Operation Exception: %s\n", e.what());
	}
	catch (ConfigurationException& e)
	{
		printf("Configuration Exception: %s\n", e.what());
	}
	catch (StreamingException& e)
	{
		printf("Streaming Exception: %s\n", e.what());
	}
	catch (TimeoutException&)
	{
		printf("TimeoutException\n");
	}

}

/*----------------------------------------------------------------------------*/
void configureColorNode()
{
	// connect new color sample handler
	g_cnode.newSampleReceivedEvent().connect(&onNewColorSample);

	ColorNode::Configuration config = g_cnode.getConfiguration();
	config.frameFormat = FRAME_FORMAT_VGA;
	config.compression = COMPRESSION_TYPE_MJPEG;
	config.powerLineFrequency = POWER_LINE_FREQUENCY_50HZ;
	config.framerate = 30;

	g_cnode.setEnableColorMap(true);

	try
	{
		g_context.requestControl(g_cnode, 0);

		g_cnode.setConfiguration(config);
	}
	catch (ArgumentException& e)
	{
		printf("Argument Exception: %s\n", e.what());
	}
	catch (UnauthorizedAccessException& e)
	{
		printf("Unauthorized Access Exception: %s\n", e.what());
	}
	catch (IOException& e)
	{
		printf("IO Exception: %s\n", e.what());
	}
	catch (InvalidOperationException& e)
	{
		printf("Invalid Operation Exception: %s\n", e.what());
	}
	catch (ConfigurationException& e)
	{
		printf("Configuration Exception: %s\n", e.what());
	}
	catch (StreamingException& e)
	{
		printf("Streaming Exception: %s\n", e.what());
	}
	catch (TimeoutException&)
	{
		printf("TimeoutException\n");
	}
}

/*----------------------------------------------------------------------------*/
void configureNode(Node node)
{
	if ((node.is<DepthNode>()) && (!g_dnode.isSet()))
	{
		g_dnode = node.as<DepthNode>();
		configureDepthNode();
		g_context.registerNode(node);
	}

	if ((node.is<ColorNode>()) && (!g_cnode.isSet()))
	{
		g_cnode = node.as<ColorNode>();
		configureColorNode();
		g_context.registerNode(node);
	}

	if ((node.is<AudioNode>()) && (!g_anode.isSet()))
	{
		g_anode = node.as<AudioNode>();
		configureAudioNode();
		g_context.registerNode(node);
	}
}

/*----------------------------------------------------------------------------*/
void onNodeConnected(Device device, Device::NodeAddedData data)
{
	configureNode(data.node);
}

/*----------------------------------------------------------------------------*/
void onNodeDisconnected(Device device, Device::NodeRemovedData data)
{
	if (data.node.is<AudioNode>() && (data.node.as<AudioNode>() == g_anode))
		g_anode.unset();
	if (data.node.is<ColorNode>() && (data.node.as<ColorNode>() == g_cnode))
		g_cnode.unset();
	if (data.node.is<DepthNode>() && (data.node.as<DepthNode>() == g_dnode))
		g_dnode.unset();
	printf("Node disconnected\n");
}

/*----------------------------------------------------------------------------*/
void onDeviceConnected(Context context, Context::DeviceAddedData data)
{
	if (!g_bDeviceFound)
	{
		data.device.nodeAddedEvent().connect(&onNodeConnected);
		data.device.nodeRemovedEvent().connect(&onNodeDisconnected);
		g_bDeviceFound = true;
	}
}

/*----------------------------------------------------------------------------*/
void onDeviceDisconnected(Context context, Context::DeviceRemovedData data)
{
	g_bDeviceFound = false;
	printf("Device disconnected\n");
}

void contextRun()
{
	return g_context.run();
}

/*----------------------------------------------------------------------------*/
void start(void)
{
	if (g_start)
		return;

	g_context = Context::create("localhost");

	g_context.deviceAddedEvent().connect(&onDeviceConnected);
	g_context.deviceRemovedEvent().connect(&onDeviceDisconnected);
	// Get the list of currently connected devices
	vector<Device> da = g_context.getDevices();

	// We are only interested in the first device
	if (da.size() >= 1)
	{
		g_bDeviceFound = true;

		da[0].nodeAddedEvent().connect(&onNodeConnected);
		da[0].nodeRemovedEvent().connect(&onNodeDisconnected);

		vector<Node> na = da[0].getNodes();

		printf("Found %u nodes\n", na.size());

		for (int n = 0; n < (int)na.size(); n++)
			configureNode(na[n]);
	}

	g_context.startNodes();

	g_start.exchange(true);
}

void run()
{
	if (!g_start)
		start();

	if (!g_stop)
		return;

	g_stop.exchange(false);
	g_thd = new thread(&contextRun);
}

void pause()
{
	if (g_stop)
		return;

	g_stop.exchange(true);
	g_thd->join();
	delete g_thd;
}

void stop()
{
	g_context.stopNodes();

	while (!g_context.getRegisteredNodes().empty())
	{
		g_context.unregisterNode(g_context.getRegisteredNodes().back());
	}
}

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
	)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		break;
	case DLL_PROCESS_DETACH:
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	}
	return TRUE;
}