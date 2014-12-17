#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2010 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX 
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ws.winx.platform.osx
{

	using CFRunLoopRef = IntPtr;
	using CFAllocatorRef=System.IntPtr;
	using CFDictionaryRef=System.IntPtr;
	using CFStringRef=System.IntPtr;
	using CFNumberRef=System.IntPtr;
	using CFArrayCallBacks=System.IntPtr;
	using CFArrayRef = System.IntPtr;
	using CFTypeRef = System.IntPtr;
	using IOHIDDeviceRef = System.IntPtr;
	using IOHIDElementRef = System.IntPtr;
	using IOHIDManagerRef = System.IntPtr;
	using IOHIDValueRef = System.IntPtr;
	using IOOptionBits = System.Int32;//System.UInt32;
	//using IOReturn = //System.IntPtr;
	using IOHIDElementCookie = System.UInt32;
	using CFTypeID=System.Int32;//System.UInt64;
	using CFIndex =System.Int32;
	using CFTimeInterval=System.Double;
	using mach_port_t=System.UInt32;
	using UInt8=System.Byte;

   



	#region NativeMethods
	
	internal static class Native
	{
		const string hid = "/System/Library/Frameworks/IOKit.framework/Versions/Current/IOKit";
		const string appServices = "/System/Library/Frameworks/ApplicationServices.framework/Versions/Current/ApplicationServices";
		const string coreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";


		public static readonly String IOHIDLocationIDKey="LocationID";
		public static readonly String IOHIDTransportKey="Transport";
		public static readonly String IOHIDVendorIDKey = "VendorID";
		public static readonly String IOHIDVendorIDSourceKey = "VendorIDSource";
		public static readonly String IOHIDProductIDKey = "ProductID";
		public static readonly String IOHIDVersionNumberKey = "VersionNumber";
		public static readonly String IOHIDManufacturerKey = "Manufacturer";
		public static readonly String IOHIDProductKey = "Product";
		public static readonly String IOHIDDeviceUsageKey = "DeviceUsage";
		public static readonly String IOHIDDeviceUsagePageKey = "DeviceUsagePage";
		public static readonly String IOHIDDeviceUsagePairsKey = "DeviceUsagePairs";
		public static readonly String IOHIDMaxInputReportSizeKey= "MaxInputReportSize";
		public static readonly String IOHIDDeviceKey ="IOHIDDevice";


		public static readonly IntPtr kIOCFPlugInInterfaceID=Native.CFUUIDGetConstantUUIDWithBytes(IntPtr.Zero,	
		                                                      0xC2, 0x44, 0xE8, 0x58, 0x10, 0x9C, 0x11, 0xD4,			
		                                                                                   0x91, 0xD4, 0x00, 0x50, 0xE4, 0xC6, 0x42, 0x6F);


		public static readonly IntPtr kIOUSBDeviceUserClientTypeID= Native.CFUUIDGetConstantUUIDWithBytes(IntPtr.Zero,
		                                                                                   
		                                                                                   0x9d,
		                                                                                   0xc7,
		                                                                                   0xb7,
		                                                                                   0x80,
		                                                                                   0x9e,
		                                                                                   0xc0,
		                                                                                   0x11,
		                                                                                   0xD4,
		                                                                                 
		                                                                                   0xa5,
		                                                                                   0x4f,
		                                                                                   0x00,
		                                                                                   0x0a,
		                                                                                   0x27,
		                                                                                   0x05,
		                                                                                   0x28,
		                                                                                   0x61);
			

		

	
	
		
		[DllImport(appServices)]
		internal static extern CFRunLoopRef CFRunLoopGetCurrent();
		
		[DllImport(appServices)]
		internal static extern CFRunLoopRef CFRunLoopGetMain();
		
		[DllImport(appServices)]
		internal static extern CFRunLoopExitReason CFRunLoopRunInMode(
			CFStringRef cfstrMode, CFTimeInterval interval, bool returnAfterSourceHandled);

	

		[DllImport(appServices)]
		internal static extern void CFRunLoopRun();

		[DllImport(appServices)]
		internal static extern void CFRunLoopStop(CFRunLoopRef rl);


		#region CFArray Func
		[DllImport(coreFoundationLibrary)]
		//extern static IntPtr CFArrayCreate (IntPtr allocator, IntPtr values, CFIndex numValues, IntPtr callbacks);
		internal static extern IntPtr CFArrayCreate (CFAllocatorRef allocator, IntPtr[] values, CFIndex numValues, CFArrayCallBacks callbacks);
		
		[DllImport(coreFoundationLibrary)]
		public static extern int CFArrayGetCount(IntPtr theArray);
		
		[DllImport(coreFoundationLibrary)]
		public static extern IntPtr CFArrayGetValueAtIndex(IntPtr theArray, int index);


		[DllImport(coreFoundationLibrary)]
		internal static extern CFTypeID CFArrayGetTypeID ();
		#endregion

		#region CFBoolean Func
		
		[DllImport(coreFoundationLibrary)]
		public static extern bool CFBooleanGetValue(IntPtr theBoolean);






	

		[DllImport(coreFoundationLibrary)]
		internal static extern CFTypeID CFStringGetTypeID ();

		[DllImport(coreFoundationLibrary)]
		internal static extern CFTypeID CFDictionaryGetTypeID ();

		[DllImport(coreFoundationLibrary)]
		internal static extern CFTypeID CFBooleanGetTypeID ();

		[DllImport(coreFoundationLibrary)]
		internal static extern IntPtr CFUUIDGetConstantUUIDWithBytes (
			CFAllocatorRef alloc,
			UInt8 byte0,
			UInt8 byte1,
			UInt8 byte2,
			UInt8 byte3,
			UInt8 byte4,
			UInt8 byte5,
			UInt8 byte6,
			UInt8 byte7,
			UInt8 byte8,
			UInt8 byte9,
			UInt8 byte10,
			UInt8 byte11,
			UInt8 byte12,
			UInt8 byte13,
			UInt8 byte14,
			UInt8 byte15
			);



		#endregion

		#region CFData Func
		
		[DllImport(coreFoundationLibrary)]
		public static   extern int CFDataGetLength(IntPtr theData);
		
		[DllImport(coreFoundationLibrary)]
		public static   extern IntPtr CFDataGetBytePtr(IntPtr theData);
		
		[DllImport(coreFoundationLibrary)]
		public static   extern void CFDataGetBytes(IntPtr theData, CFRange range, IntPtr buffer);
		
		[DllImport(coreFoundationLibrary)]
		public static   extern IntPtr CFDataCreate(IntPtr theAllocator, IntPtr bytes, int bytelength);

		[DllImport(coreFoundationLibrary)]
		internal static extern CFTypeID CFDataGetTypeID ();
		#endregion
		#region CFDictionary Func

		[DllImport(coreFoundationLibrary)]
		internal static extern int CFDictionaryGetCount(IntPtr theDictionary);
		
		[DllImport(coreFoundationLibrary)]
		internal static extern IntPtr CFDictionaryGetValue(IntPtr theDictionary, IntPtr theKey);

		[DllImport(coreFoundationLibrary)]
		internal static extern void CFDictionarySetValue (CFDictionaryRef theDict, IntPtr key, IntPtr value);

		
		[DllImport(coreFoundationLibrary)]
		public static   extern IntPtr CFDictionaryGetKeysAndValues(IntPtr theDict, IntPtr[] keys, IntPtr[] values);
		
	
		[DllImport(coreFoundationLibrary)]
		internal static extern CFDictionaryRef CFDictionaryCreate (
			CFAllocatorRef allocator,
			CFStringRef[] keys,
			CFNumberRef[] values,
			CFIndex numValues,
			ref CFDictionary.CFDictionaryKeyCallBacks keyCallBacks,
			ref CFDictionary.CFDictionaryValueCallBacks valueCallBacks
			);







#endregion
		#region CFNumber Func
		[DllImport(coreFoundationLibrary)]
		internal unsafe static extern CFNumberRef CFNumberCreate (CFAllocatorRef allocator,CFNumberType theType,int* valuePtr);
		//CFNumberRef CFNumberCreate (CFAllocatorRef allocator,CFNumberType theType,const void *valuePtr);

	
		[DllImport(coreFoundationLibrary)]
		internal static extern CFNumberRef CFNumberCreate (CFAllocatorRef allocator,CFNumberType theType,IntPtr valuePtr);
		//CFNumberRef CFNumberCreate (CFAllocatorRef allocator,CFNumberType theType,const void *valuePtr);
		
		[DllImport(coreFoundationLibrary)]
		internal unsafe static extern bool CFNumberGetValue (IntPtr number, CFNumberType theType, IntPtr valuePtr);
		//		internal static extern bool CFNumberGetValue (IntPtr number, CFNumberType theType, int* valuePtr);	
		
		[DllImport(coreFoundationLibrary)]
		public static  extern bool CFNumberGetValue(IntPtr theNumber, IntPtr theType, IntPtr valuePtr);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern CFNumberType CFNumberGetType(IntPtr theNumber);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern int CFNumberGetByteSize(IntPtr theNumber);

		[DllImport(coreFoundationLibrary)]
		internal static extern CFTypeID CFNumberGetTypeID ();
		#endregion
		#region CFPropertyList Fun
		
		[DllImport(coreFoundationLibrary)]
		public static  extern bool CFPropertyListIsValid(IntPtr theList, IntPtr theFormat);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern IntPtr CFPropertyListCreateXMLData(IntPtr theAllocator, IntPtr theList);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern IntPtr CFPropertyListCreateWithData(IntPtr theAllocator, IntPtr theData, int options, int format,IntPtr errorString);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern IntPtr CFPropertyListCreateFromStream(IntPtr allocator,IntPtr stream, int length,int options,int format,IntPtr errorString);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern int CFPropertyListWriteToStream(IntPtr propertyList, IntPtr stream, int format,IntPtr errorString);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern bool CFPropertyListIsValid(IntPtr plist,int format);
		#endregion
		#region CFString Func
		[DllImport(coreFoundationLibrary)]
		static extern IntPtr __CFStringMakeConstantString(string cStr);
		internal static IntPtr CFSTR(string cStr)
		{
			return __CFStringMakeConstantString(cStr);
		}

		[DllImport(coreFoundationLibrary)]
		public static  extern int CFStringGetLength(IntPtr handle);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern IntPtr CFStringGetCharactersPtr(IntPtr handle);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern IntPtr CFStringGetCharacters(IntPtr handle, CFRange range, IntPtr buffer);
		#endregion
		#region CFReadStream Func
		[DllImport(coreFoundationLibrary)]
		public static  extern IntPtr CFReadStreamCreateWithFile(IntPtr alloc, IntPtr fileURL);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern bool CFReadStreamOpen(IntPtr stream);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern void CFReadStreamClose(IntPtr stream);
		#endregion
		#region CFType Func

		[DllImport(coreFoundationLibrary)]
		internal static extern CFTypeID CFGetTypeID(CFTypeRef cf);
		
		[DllImport(coreFoundationLibrary)]        
		internal static  extern CFStringRef CFCopyDescription(CFTypeRef typeRef);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern IntPtr CFCopyTypeIDDescription(int typeID);

		#endregion        
		#region CFURL
		[DllImport(coreFoundationLibrary)]
		public static  extern IntPtr CFURLCreateWithFileSystemPath(IntPtr allocator, IntPtr filePath, int pathStyle, bool isDirectory);
		#endregion
		#region CFWriteStream
		[DllImport(coreFoundationLibrary)]
		public static  extern IntPtr CFWriteStreamCreateWithFile(IntPtr allocator, IntPtr fileURL);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern bool CFWriteStreamOpen(IntPtr stream);
		
		[DllImport(coreFoundationLibrary)]
		public static  extern void CFWriteStreamClose(IntPtr stream);
		#endregion

		[DllImport(coreFoundationLibrary)]
		internal static extern void CFRelease (CFTypeRef cf);


		#region HID DLL
		[DllImport(hid)]
		public static extern IOHIDManagerRef IOHIDManagerCreate(
			CFAllocatorRef allocator, IOOptionBits options);
		
		// This routine will be called when a new (matching) device is connected.
		[DllImport(hid)]
		public static extern void IOHIDManagerRegisterDeviceMatchingCallback(
			IOHIDManagerRef inIOHIDManagerRef,
			IOHIDDeviceCallback inIOHIDDeviceCallback,
			IntPtr inContext);
		
		[DllImport(hid)]
		public static extern void IOHIDManagerRegisterDeviceMatchingCallback(
			IOHIDManagerRef inIOHIDManagerRef,
			IntPtr inIOHIDDeviceCallback,
			IntPtr inContext);
		
		// This routine will be called when a (matching) device is disconnected.
		[DllImport(hid)]
		public static extern void IOHIDManagerRegisterDeviceRemovalCallback(
			IOHIDManagerRef inIOHIDManagerRef,
			IOHIDDeviceCallback inIOHIDDeviceCallback,
			IntPtr inContext);
		
		[DllImport(hid)]
		public static extern void IOHIDManagerRegisterDeviceRemovalCallback(
			IOHIDManagerRef inIOHIDManagerRef,
			IntPtr inIOHIDDeviceCallback,
			IntPtr inContext);
		
		[DllImport(hid)]
		public static extern void IOHIDManagerScheduleWithRunLoop(
			IOHIDManagerRef inIOHIDManagerRef,
			CFRunLoopRef inCFRunLoop,
			CFStringRef inCFRunLoopMode);
		
		[DllImport(hid)]
		public static extern void IOHIDManagerSetDeviceMatching(
			IOHIDManagerRef manager,
			CFDictionaryRef matching);
		
		
		[DllImport(hid)]
		public static extern void IOHIDManagerSetDeviceMatchingMultiple(
			IOHIDManagerRef manager,
			CFArrayRef multiple);
		
		[DllImport(hid)]
		public static extern CFArrayRef IOHIDDeviceCopyMatchingElements(
			IOHIDDeviceRef device,
			CFDictionaryRef matching,
			IOOptionBits options);
		
		[DllImport(hid)]
		public static extern uint IOHIDElementGetUsage(
			IOHIDElementRef element);
		
		
		[DllImport(hid)]
		public static extern IOReturn IOHIDManagerOpen(
			IOHIDManagerRef manager,
			IOOptionBits options);
		
		[DllImport(hid)]
		public static extern IOReturn IOHIDDeviceOpen(
			IOHIDDeviceRef manager,
			IOOptionBits opts);

		[DllImport(hid)]
		public static extern IOReturn IOHIDDeviceClose( IOHIDDeviceRef  IOHIDDeviceRef,  // IOHIDDeviceRef for the HID device
		                          IOOptionBits    inOptions ); // Option bits to be sent down to the HID device
		
		[DllImport(hid)]
		public static extern CFTypeRef IOHIDDeviceGetProperty(
			IOHIDDeviceRef device,
			CFStringRef key);
		
		
		[DllImport(hid)]
		public static extern IOHIDElementType IOHIDElementGetType(
			IOHIDElementRef element);
		
		[DllImport(hid)]
		public static extern bool IOHIDDeviceConformsTo(
			IOHIDDeviceRef inIOHIDDeviceRef,  // IOHIDDeviceRef for the HID device
			HIDPage inUsagePage,      // the usage page to test conformance with
			HIDUsageGD inUsage);         // the usage to test conformance with
		
		[DllImport(hid)]
		public static extern void IOHIDDeviceRegisterInputValueCallback(
			IOHIDDeviceRef device,
			IOHIDValueCallback callback,
			IntPtr context);
		
		[DllImport(hid)]
		public static extern void IOHIDDeviceRegisterInputValueCallback(
			IOHIDDeviceRef device,
			IntPtr callback,
			IntPtr context);
		
		[DllImport(hid)]
		public static extern void IOHIDDeviceScheduleWithRunLoop(
			IOHIDDeviceRef device,
			CFRunLoopRef inCFRunLoop,
			CFStringRef inCFRunLoopMode);
		
		
		[DllImport(hid)]
		internal static extern bool IOHIDElementHasNullState(
			IOHIDElementRef element);

		[DllImport(hid)]
		internal static extern void IOHIDDeviceUnscheduleFromRunLoop(
			IOHIDDeviceRef device,
			CFRunLoopRef runLoop,
			CFStringRef runLoopMode);// AVAILABLE_MAC_OS_X_VERSION_10_5_AND_LATER;

		[DllImport(hid)]
		internal static extern IOReturn IOHIDDeviceSetReport(
			IOHIDDeviceRef device,
			IOHIDReportType reportType,
			int reportID,
			IntPtr report,
			CFIndex reportLength);// AVAILABLE_MAC_OS_X_VERSION_10_5_AND_LATER;

		[DllImport(hid)]
		internal static extern IOReturn IOHIDDeviceSetReportWithCallback(
			IOHIDDeviceRef device,
			IOHIDReportType reportType,
			int reportID,
			IntPtr report,
			int reportLength,
			CFTimeInterval timeout,
			IOHIDReportCallback callback,
			IntPtr context) ;//AVAILABLE_MAC_OS_X_VERSION_10_5_AND_LATER;
		
	
		
		[DllImport(hid)]
		public static extern IOHIDElementCookie IOHIDElementGetCookie(
			IOHIDElementRef element);
		
		[DllImport(hid)]
		public static extern IOHIDElementRef IOHIDValueGetElement(IOHIDValueRef @value);
		
		[DllImport(hid)]
		public static extern CFIndex IOHIDValueGetIntegerValue(IOHIDValueRef @value);
		
		[DllImport(hid)]
		public static extern double IOHIDValueGetScaledValue(
			IOHIDValueRef @value,
			IOHIDValueScaleType type);
		
		
		[DllImport(hid)]
		public static extern HIDPage IOHIDElementGetUsagePage(IOHIDElementRef elem);

		
		[DllImport(hid)]
		public static extern CFTypeID IOHIDElementGetTypeID();


		[DllImport(hid)]
		public static extern CFIndex IOHIDElementGetPhysicalMax(IOHIDElementRef element);


		[DllImport(hid)]
		public static extern CFIndex IOHIDElementGetLogicalMin(IOHIDElementRef element);
		
		[DllImport(hid)]
		public static extern CFIndex IOHIDElementGetLogicalMax(IOHIDElementRef element);
		
		
		[DllImport(hid)]
		public static extern CFIndex IOHIDValueGetLength(IOHIDValueRef value);

		[DllImport(hid)]
		internal static extern IntPtr IOServiceMatching(
			IntPtr name );

		[DllImport(hid)]
		internal static extern long IOServiceGetMatchingService(
			IntPtr masterPort,
			CFDictionaryRef matching );

		[DllImport(hid)]
		internal static extern IOReturn IOCreatePlugInInterfaceForService(long service,
		                                  IntPtr pluginType, IntPtr interfaceType,
		                                  out IntPtr theInterface,out IntPtr theScore);



//		[DllImport(hid)]
//		internal static extern long AllocateHIDObjectFromIOHIDDeviceRef(IOHIDDeviceRef inIOHIDDeviceRef);

		internal static long AllocateHIDObjectFromIOHIDDeviceRef(IOHIDDeviceRef inIOHIDDeviceRef) {
						long result =0 ;
						if (inIOHIDDeviceRef!=IntPtr.Zero) {
								// Set up the matching criteria for the devices we're interested in.
								// We are interested in instances of class IOHIDDevice.
								// matchingDict is consumed below( in IOServiceGetMatchingService )
								// so we have no leak here.
								//CFMutableDictionaryRef matchingDict = IOServiceMatching(kIOHIDDeviceKey);
				byte[] utf8Bytes = Encoding.UTF8.GetBytes(Native.IOHIDDeviceKey);
				//char[] charArr=Native.IOHIDDeviceKey.ToCharArray(
				IntPtr bufferIntPtr = Marshal.AllocHGlobal(utf8Bytes.Length);
				Marshal.Copy(utf8Bytes, 0, bufferIntPtr, utf8Bytes.Length);



				IntPtr matchingDictRef = Native.IOServiceMatching (bufferIntPtr);
								matchingDictRef = Native.IOServiceMatching (Native.CFSTR(Native.IOHIDDeviceKey));
				Marshal.FreeHGlobal(bufferIntPtr);
								
				                

				if (matchingDictRef != IntPtr.Zero) {

					Native.CFDictionary dict=new Native.CFDictionary(matchingDictRef);

										// Add a key for locationID to our matching dictionary.  This works for matching to
										// IOHIDDevices, so we will only look for a device attached to that particular port
										// on the machine.

										IntPtr tCFTypeRef = Native.IOHIDDeviceGetProperty (inIOHIDDeviceRef, Native.CFSTR (Native.IOHIDLocationIDKey));

										if (tCFTypeRef != IntPtr.Zero) {

												dict[Native.IOHIDLocationIDKey]=tCFTypeRef;



												//CFDictionaryAddValue (matchingDictRef, CFSTR (Native.IOHIDLocationIDKey), tCFTypeRef);
												// CFRelease( tCFTypeRef ); // don't release objects that we "Get".
						
												// IOServiceGetMatchingService assumes that we already know that there is only one device
												// that matches.  This way we don't have to do the whole iteration dance to look at each
												// device that matches.  This is a new API in 10.2
												//result = Native.IOServiceGetMatchingService (kIOMasterPortDefault, matchingDictRef);
												result = Native.IOServiceGetMatchingService (IntPtr.Zero, matchingDictRef);
										}
					
										// Note: We're not leaking the matchingDict.
										// One reference is consumed by IOServiceGetMatchingServices
								}
						}
			return result;
				}

		[DllImport(hid)]
		internal static extern IOReturn IOHIDDeviceGetReport(
			IOHIDDeviceRef device,
			IOHIDReportType reportType,
			CFIndex reportID,
			IntPtr report,
			IntPtr pReportLength);

		[DllImport(hid)]
		public static extern void IOHIDDeviceRegisterInputReportCallback(
			IOHIDDeviceRef device, // IOHIDDeviceRef for the HID device
			IntPtr report,  // pointer to the report data ( uint8_t's )
			int reportLength,// number of bytes in the report ( CFIndex )
			IOHIDReportCallback callback, // the callback routine
			IntPtr context); //AVAILABLE_MAC_OS_X_VERSION_10_5_AND_LATER;

		#endregion
		
		
			public delegate void IOHIDReportCallback(
				IntPtr          inContext,          // context from IOHIDDeviceRegisterInputReportCallback
				IOReturn        inResult,           // completion result for the input report operation
				IOHIDDeviceRef   inSender,           // IOHIDDeviceRef of the device this report is from
				IOHIDReportType inType,             // the report type
				uint        inReportID,         // the report ID
				IntPtr       inReport,           // pointer to the report data
			int inReportLength ); // the actual size of the input report


		public delegate void IOHIDDeviceCallback(IntPtr ctx, IOReturn res, IntPtr sender, IOHIDDeviceRef device);
		public delegate void IOHIDValueCallback(IntPtr ctx, IOReturn res, IntPtr sender, IOHIDValueRef val);


		internal enum IOHIDReportType{
			kIOHIDReportTypeInput = 0,
			kIOHIDReportTypeOutput,
			kIOHIDReportTypeFeature,
			kIOHIDReportTypeCount
		};

		internal enum IOReturn{
			kIOReturnSuccess =0,//        KERN_SUCCESS            // OK

		}
		
			internal enum CFNumberType
			{
				kCFNumberSInt8Type = 1,
				kCFNumberSInt16Type = 2,
				kCFNumberSInt32Type = 3,
				kCFNumberSInt64Type = 4,
				kCFNumberFloat32Type = 5,
				kCFNumberFloat64Type = 6,
				kCFNumberCharType = 7,
				kCFNumberShortType = 8,
				kCFNumberIntType = 9,
				kCFNumberLongType = 10,
				kCFNumberLongLongType = 11,
				kCFNumberFloatType = 12,
				kCFNumberDoubleType = 13,
				kCFNumberCFIndexType = 14,
				kCFNumberNSIntegerType = 15,
				kCFNumberCGFloatType = 16,
				kCFNumberMaxType = 16
			};
			
			internal enum CFRunLoopExitReason
			{
				Finished = 1,
				Stopped = 2,
				TimedOut = 3,
				HandledSource = 4
			}

				/*!
		  @typedef IOHIDOptionsType
		  @abstract Options for opening a device via IOHIDLib.
		  @constant kIOHIDOptionsTypeNone Default option.
		  @constant kIOHIDOptionsTypeSeizeDevice Used to open exclusive
		    communication with the device.  This will prevent the system
		    and other clients from receiving events from the device.
		*/
		internal enum IOHIDOptionsType:uint{
			kIOHIDOptionsTypeNone	 = 0x00,
			kIOHIDOptionsTypeSeizeDevice = 0x01
		}

			
			public static readonly IntPtr RunLoopModeDefault = CFSTR("kCFRunLoopDefaultMode");
		
			internal enum IOHIDValueScaleType
			{
				Physical, // [device min, device max]
				Calibrated // [-1, +1]
			}
			
			internal enum HIDPage
			{
				GenericDesktop = 0x01,
				
		}
			
			// Generic desktop usage
			internal enum HIDUsageGD
			{
				
				Joystick = 0x04, /* Application Collection */
				GamePad = 0x05, /* Application Collection */
				MultiAxisController = 0x08, /* Application Collection */
				Hatswitch = 0x39, /* Dynamic Value */
				
		}
			
			internal enum IOHIDElementType
			{
				kIOHIDElementTypeInput_Misc = 1,
				kIOHIDElementTypeInput_Button = 2,
				kIOHIDElementTypeInput_Axis = 3,
				kIOHIDElementTypeInput_ScanCodes = 4,
				kIOHIDElementTypeOutput = 129,
				kIOHIDElementTypeFeature = 257,
				kIOHIDElementTypeCollection = 513
			}
		

		#region CFType Class
		public class CFType
		{
			internal CFTypeID _CFArray = Native.CFArrayGetTypeID ();// 18;
			internal  CFTypeID _CFBoolean = Native.CFBooleanGetTypeID ();  // 21;
			internal  CFTypeID _CFData = Native.CFDataGetTypeID ();// 19; 
			internal  CFTypeID _CFNumber = Native.CFNumberGetTypeID ();// 22;
			internal  CFTypeID _CFDictionary = Native.CFDictionaryGetTypeID ();//17;
			internal  CFTypeID _CFString = Native.CFStringGetTypeID ();    //7;

			
			internal IntPtr typeRef;
			internal object Value;
			
			public CFType() { }
			
			public CFType(IntPtr handle){this.typeRef = handle;}
			
			/// <summary>
			/// Returns the unique identifier of an opaque type to which a CoreFoundation object belongs
			/// </summary>
			/// <returns></returns>
			public CFTypeID GetTypeID()
			{
				//return CFGetTypeID(typeRef);
				return CFGetTypeID(typeRef);
			}
			/// <summary>
			/// Returns a textual description of a CoreFoundation object
			/// </summary>
			/// <returns></returns>
			public string GetDescription()
			{
				return new CFString(CFCopyDescription(typeRef)).ToString();
			} 


			internal CFType Factory(IntPtr value){
				CFTypeID type = CFGetTypeID (value);

				if (type == _CFString)                               
					return new CFString (value);         
				else if(type==_CFDictionary)  
					return new CFDictionary(value);
				else if(type==_CFArray)
					return new CFArray(value);
				        else if(type== _CFData)
					return new CFData(value);
				        else if(type==_CFBoolean)
					return new CFBoolean(value);
				        else if(type==_CFNumber)
					return new CFNumber(value);                


				return new CFType(value);
			}
			
	

			public override string ToString()
			{
				return String.Empty;
			}

			public static implicit operator IntPtr(CFType value)
			{
				return value.typeRef;
			}
			public static implicit operator CFType(IntPtr value)
			{
				return new CFType(value);
			}
		}

		#endregion

//		#region CFIndex
//		public class CFIndex 
//		{
//			internal IntPtr theIndex;
//			
//			public CFIndex() { }
//			public CFIndex(IntPtr Index) { this.theIndex = Index; }
//			/// <summary>
//			/// Returns the CFIndex object as a Int32
//			/// </summary>
//			/// <returns></returns>
//			public int ToInteger()
//			{
//				theIndex.
//				return System.Runtime.InteropServices.Marshal.ReadInt32(theIndex);
//			}
//			
//			public static implicit operator CFIndex(IntPtr Index)
//			{
//				return new CFIndex(Index);
//			}
//		}
//		#endregion


		#region CFRange
		public struct CFRange 
		{ 
			public int Location, Length; 
			public CFRange(int l, int len) 
			{ 
				Location = l; 
				Length = len; 
			} 
		}
		#endregion


		#region CFPropertyList
		 public class CFPropertyList : CFType 
		 {
			public CFPropertyList() { }
			public CFPropertyList(IntPtr PropertyList)
				: base(PropertyList)
			{
			}
			public CFPropertyList(string plistlocation) 
			{            
				IntPtr inputfilename;
				inputfilename = new CFString(plistlocation);
				
				IntPtr ifile_IntPtr = CFURLCreateWithFileSystemPath(IntPtr.Zero, inputfilename, 2, false);
				IntPtr ifile_CFReadStreamRef = CFReadStreamCreateWithFile(IntPtr.Zero, ifile_IntPtr);
				if ((CFReadStreamOpen(ifile_CFReadStreamRef)) == false)
				{
					typeRef = IntPtr.Zero;
				}
				IntPtr PlistRef = CFPropertyListCreateFromStream(IntPtr.Zero, ifile_CFReadStreamRef, 0, 2, 0, IntPtr.Zero);
				CFReadStreamClose(ifile_CFReadStreamRef);
				typeRef = PlistRef;
			}
			
			public static implicit operator CFPropertyList(IntPtr value)
			{
				return new CFPropertyList(value);
			}
			
			public static implicit operator IntPtr(CFPropertyList value)
			{
				return value.typeRef;
			}
			
			public static implicit operator string(CFPropertyList value)
			{
				return value.ToString();
			}
			
			public static implicit operator CFPropertyList(string value)
			{
				return new CFPropertyList(value);
			}               
		 }
		 #endregion

		#region CFArray
		public class CFArray : CFType
		{
			public CFArray() { }
			public CFArray(IntPtr Number)
				: base(Number)
			{
			}
			public CFArray(IntPtr[] values)
			{
				try
				{
					typeRef = CFArrayCreate(IntPtr.Zero, values, values.Length, IntPtr.Zero);
				}
				catch (Exception Ex)
				{
					UnityEngine.Debug.LogException(Ex);
					typeRef = IntPtr.Zero;
				}
			}
			/// <summary>
			/// Returns the number of values currently in an array
			/// </summary>
			/// <returns></returns>
			public int Length
			{
				get
				{
					return CFArrayGetCount(typeRef);
				}
			}


			public CFType this[int index]{
				get{
					if (index >= this.Length)
						return new CFType(IntPtr.Zero);
					
					return base.Factory(CFArrayGetValueAtIndex(typeRef, index));
				}
			}


			public override string ToString ()
			{

				return Encoding.UTF8.GetString(new CFData(CFPropertyListCreateXMLData(IntPtr.Zero, typeRef)).ToByteArray());

			}
			

		}
		#endregion

		#region CFBoolean
		public class CFBoolean : CFType
		{
			public CFBoolean() { }
			public CFBoolean(IntPtr Number)
				: base(Number)
			{
			}  

			public bool ToBoolean(){
				if (base.Value == null) {
										base.Value = CFBooleanGetValue (this.typeRef);
								}


					return (bool)base.Value;
			}

			public override string ToString ()
			{
					return this.ToBoolean().ToString();
			}
		}
		#endregion

		#region CFData
		public class CFData :  CFType 
		{      
			protected byte[] _value;

			public CFData(){}
			public CFData(IntPtr Data)
				: base(Data)
			{
			}
			unsafe public CFData(byte[] Data)
			{            
				byte[] buffer = Data;            
				int len = buffer.Length;            
				fixed (byte* bytePtr = buffer)
					
					base.typeRef = CFDataCreate(IntPtr.Zero, (IntPtr)bytePtr,len);            
			}
			/// <summary>
			/// Returns the number of bytes contained by the CFData object
			/// </summary>
			/// <returns></returns>
			public int Length()
			{
				return CFDataGetLength(typeRef);
			}
			/// <summary>
			/// Checks if the object is a valid CFData object
			/// </summary>
			/// <returns></returns>
			public bool isData()
			{
				return CFGetTypeID(typeRef) == _CFData;
			}
			/// <summary>
			/// Returns the CFData object as a byte array
			/// </summary>
			/// <returns></returns>
			unsafe public byte[] ToByteArray()
			{       
				if(_value==null){
				int len = Length();
				byte[] buffer = new byte[len];
				fixed (byte* bufPtr = buffer)
					CFDataGetBytes(typeRef, new CFRange(0, len), (IntPtr)bufPtr);
					return _value=buffer; 
				}else 
					return _value;

			}

	

			public override string ToString ()
			{
				return Convert.ToBase64String(this.ToByteArray());
			}
			
			public static implicit operator CFData(IntPtr value)
			{
				return new CFData(value);
			}
			
			public static implicit operator IntPtr(CFData value)
			{
				return value.typeRef;
			}       
		}
		#endregion

		#region CFString
		public sealed class CFString : CFType
		{
			public CFString()
			{            
			}             
			/// <summary>
			/// Creates an immutable string from a constant compile-time string
			/// </summary>
			/// <param name="str"></param>
			public CFString(string str) 
			{
				base.typeRef = CFSTR(str);
			}
			public CFString(IntPtr myHandle) : base(myHandle)
			{    
			}
			/// <summary>
			/// Checks if the current object is a valid CFString object
			/// </summary>
			/// <returns></returns>
			public bool isString()
			{
				return CFGetTypeID(typeRef) == _CFString;
			}

			public override string ToString ()
			{
				if(base.Value==null){

				if (typeRef == IntPtr.Zero)
					return null;
				
				string str;
				int length = CFStringGetLength(typeRef);        
				IntPtr u = CFStringGetCharactersPtr(typeRef);
				IntPtr buffer = IntPtr.Zero;
				if (u == IntPtr.Zero)
				{
					CFRange range = new CFRange(0, length);
					buffer = Marshal.AllocCoTaskMem(length * 2);
					CFStringGetCharacters(typeRef, range, buffer);
					u = buffer;
				}
				unsafe
				{
					str = new string((char*)u, 0, length);
				}
				if (buffer != IntPtr.Zero)
					Marshal.FreeCoTaskMem(buffer);
					base.Value=str;
					return base.Value as String;
				}else 
					return base.Value as String;
			}

			public static implicit operator CFString(IntPtr value)
			{
				return new CFString(value);
			}
			
			public static implicit operator IntPtr(CFString value)
			{
				return value.typeRef;
			}
			
			public static implicit operator string(CFString value)
			{
				return value.ToString();
			}
			
			public static implicit operator CFString(string value)
			{            
				return new CFString(value);
			}            
		}         
		#endregion

		#region CFDictionary
		 public class CFDictionary : CFType
		 {        
			public CFDictionary() { }
			
			public CFDictionary(IntPtr dictionary) : base(dictionary)
			{
			}
			
			public CFDictionary(IntPtr[] keys,IntPtr[] values)
			{

				CFDictionaryKeyCallBacks kcall = new CFDictionaryKeyCallBacks();
				
				CFDictionaryValueCallBacks vcall = new CFDictionaryValueCallBacks();
				base.typeRef = CFDictionaryCreate(IntPtr.Zero,keys,values,keys.Length,ref kcall,ref vcall);            
			}
			



			public CFType this[string key]
			{
				get{
										try {

												
												//return new CFType(CFDictionaryGetValue(base.typeRef, new CFString(value)));
												return base.Factory(CFDictionaryGetValue (base.typeRef, CFSTR (key)));
										} catch (Exception ex) {
												UnityEngine.Debug.LogException (ex);
												return new CFType (IntPtr.Zero);
										}
				   }

			set{

					CFDictionarySetValue(base.typeRef,CFSTR(key),value.typeRef);
				}
				
			}

			/// <summary>
			/// Returns the number of key-value pairs in a dictionary
			/// </summary>
			/// <returns></returns>
			public int Length
			{
				get
				{
					return CFDictionaryGetCount(base.typeRef);
				}
			}    
			
			
			public static implicit operator CFDictionary(IntPtr value)
			{
				return new CFDictionary(value);
			}
			
			public static implicit operator IntPtr(CFDictionary value)
			{
				return value.typeRef;
			}
			
			public static implicit operator string(CFDictionary value)
			{
				return value.ToString();
			}
			
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
			public struct CFDictionaryKeyCallBacks
			{
				int version;
				CFDictionaryRetainCallBack retain;
				CFDictionaryReleaseCallBack release;
				CFDictionaryCopyDescriptionCallBack copyDescription;
				CFDictionaryEqualCallBack equal;
				CFDictionaryHashCallBack hash;
				
			};
			
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
			public struct CFDictionaryValueCallBacks
			{
				int version;
				CFDictionaryRetainCallBack retain;
				CFDictionaryReleaseCallBack release;
				CFDictionaryCopyDescriptionCallBack copyDescription;
				CFDictionaryEqualCallBack equal;
			};
			
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr CFDictionaryRetainCallBack(IntPtr allocator, IntPtr value);
			
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate void CFDictionaryReleaseCallBack(IntPtr allocator, IntPtr value);
			
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr CFDictionaryCopyDescriptionCallBack(IntPtr value);
			
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr CFDictionaryEqualCallBack(IntPtr value1, IntPtr value2);
			
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate IntPtr CFDictionaryHashCallBack(IntPtr value);
		 }
		 #endregion

		#region CFNumber
		public class CFNumber : CFType 
		{
			public CFNumber() { }
			public CFNumber(IntPtr Number)
				: base(Number)
			{
			}
			unsafe public CFNumber(int Number) 
			{

				//new IntPtr(Number);
				int* pNumber=&Number;
				base.typeRef = CFNumberCreate(IntPtr.Zero, CFNumberType.kCFNumberIntType, pNumber);
			} 


			public object ToInteger()
			{
				if (base.Value == null) {



					IntPtr buffer = Marshal.AllocCoTaskMem(CFNumberGetByteSize(typeRef));
					bool scs = CFNumberGetValue(typeRef, CFNumberGetType(typeRef), buffer);
					if (scs != true)
					{
						UnityEngine.Debug.LogError("CFNumber IntPtr to Integer failed.");
					}

					CFNumberType type = CFNumberGetType(typeRef);
					
					switch (type)
					{
					case Native.CFNumberType.kCFNumberSInt8Type:
						return (base.Value=Marshal.ReadInt16(buffer));
					case Native.CFNumberType.kCFNumberSInt16Type:
						return (base.Value=Marshal.ReadInt16(buffer));
					case Native.CFNumberType.kCFNumberSInt32Type:
						return (base.Value=Marshal.ReadInt32(buffer));
					case Native.CFNumberType.kCFNumberSInt64Type:
						return (base.Value=Marshal.ReadInt64(buffer));
					default:
						UnityEngine.Debug.LogError("CFNumber value not recognize type of "+((Native.CFNumberType)type).ToString());
						break;
					}


				}

				return base.Value;

			}

			public override string ToString ()
			{
				return string.Format ("[CFNumber]");
			}
			   
			
		}
	
		#endregion










		
	}
	
	
	
	
	
	
	#endregion








}
#endif