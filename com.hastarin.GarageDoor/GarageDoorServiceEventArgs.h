//-----------------------------------------------------------------------------
// <auto-generated> 
//   This code was generated by a tool. 
// 
//   Changes to this file may cause incorrect behavior and will be lost if  
//   the code is regenerated.
//
//   Tool: AllJoynCodeGenerator.exe
//
//   This tool is located in the Windows 10 SDK and the Windows 10 AllJoyn 
//   Visual Studio Extension in the Visual Studio Gallery.  
//
//   The generated code should be packaged in a Windows 10 C++/CX Runtime  
//   Component which can be consumed in any UWP-supported language using 
//   APIs that are available in Windows.Devices.AllJoyn.
//
//   Using AllJoynCodeGenerator - Invoke the following command with a valid 
//   Introspection XML file and a writable output directory:
//     AllJoynCodeGenerator -i <INPUT XML FILE> -o <OUTPUT DIRECTORY>
// </auto-generated>
//-----------------------------------------------------------------------------
#pragma once

namespace com { namespace hastarin { namespace GarageDoor {

// Methods
public ref class GarageDoorOpenCalledEventArgs sealed
{
public:
    GarageDoorOpenCalledEventArgs(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info, _In_ bool interfaceMemberPartialOpen);

    property Windows::Devices::AllJoyn::AllJoynMessageInfo^ MessageInfo
    {
        Windows::Devices::AllJoyn::AllJoynMessageInfo^ get() { return m_messageInfo; }
    }

    property GarageDoorOpenResult^ Result
    {
        GarageDoorOpenResult^ get() { return m_result; }
        void set(_In_ GarageDoorOpenResult^ value) { m_result = value; }
    }

    property bool PartialOpen
    {
        bool get() { return m_interfaceMemberPartialOpen; }
    }

    Windows::Foundation::Deferral^ GetDeferral();

    static Windows::Foundation::IAsyncOperation<GarageDoorOpenResult^>^ GetResultAsync(GarageDoorOpenCalledEventArgs^ args)
    {
        args->InvokeAllFinished();
        auto t = concurrency::create_task(args->m_tce);
        return concurrency::create_async([t]() -> concurrency::task<GarageDoorOpenResult^>
        {
            return t;
        });
    }
    
private:
    void Complete();
    void InvokeAllFinished();
    void InvokeCompleteHandler();

    bool m_raised;
    int m_completionsRequired;
    concurrency::task_completion_event<GarageDoorOpenResult^> m_tce;
    std::mutex m_lock;
    Windows::Devices::AllJoyn::AllJoynMessageInfo^ m_messageInfo;
    GarageDoorOpenResult^ m_result;
    bool m_interfaceMemberPartialOpen;
};

public ref class GarageDoorCloseCalledEventArgs sealed
{
public:
    GarageDoorCloseCalledEventArgs(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);

    property Windows::Devices::AllJoyn::AllJoynMessageInfo^ MessageInfo
    {
        Windows::Devices::AllJoyn::AllJoynMessageInfo^ get() { return m_messageInfo; }
    }

    property GarageDoorCloseResult^ Result
    {
        GarageDoorCloseResult^ get() { return m_result; }
        void set(_In_ GarageDoorCloseResult^ value) { m_result = value; }
    }

    Windows::Foundation::Deferral^ GetDeferral();

    static Windows::Foundation::IAsyncOperation<GarageDoorCloseResult^>^ GetResultAsync(GarageDoorCloseCalledEventArgs^ args)
    {
        args->InvokeAllFinished();
        auto t = concurrency::create_task(args->m_tce);
        return concurrency::create_async([t]() -> concurrency::task<GarageDoorCloseResult^>
        {
            return t;
        });
    }
    
private:
    void Complete();
    void InvokeAllFinished();
    void InvokeCompleteHandler();

    bool m_raised;
    int m_completionsRequired;
    concurrency::task_completion_event<GarageDoorCloseResult^> m_tce;
    std::mutex m_lock;
    Windows::Devices::AllJoyn::AllJoynMessageInfo^ m_messageInfo;
    GarageDoorCloseResult^ m_result;
};

public ref class GarageDoorPushButtonCalledEventArgs sealed
{
public:
    GarageDoorPushButtonCalledEventArgs(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);

    property Windows::Devices::AllJoyn::AllJoynMessageInfo^ MessageInfo
    {
        Windows::Devices::AllJoyn::AllJoynMessageInfo^ get() { return m_messageInfo; }
    }

    property GarageDoorPushButtonResult^ Result
    {
        GarageDoorPushButtonResult^ get() { return m_result; }
        void set(_In_ GarageDoorPushButtonResult^ value) { m_result = value; }
    }

    Windows::Foundation::Deferral^ GetDeferral();

    static Windows::Foundation::IAsyncOperation<GarageDoorPushButtonResult^>^ GetResultAsync(GarageDoorPushButtonCalledEventArgs^ args)
    {
        args->InvokeAllFinished();
        auto t = concurrency::create_task(args->m_tce);
        return concurrency::create_async([t]() -> concurrency::task<GarageDoorPushButtonResult^>
        {
            return t;
        });
    }
    
private:
    void Complete();
    void InvokeAllFinished();
    void InvokeCompleteHandler();

    bool m_raised;
    int m_completionsRequired;
    concurrency::task_completion_event<GarageDoorPushButtonResult^> m_tce;
    std::mutex m_lock;
    Windows::Devices::AllJoyn::AllJoynMessageInfo^ m_messageInfo;
    GarageDoorPushButtonResult^ m_result;
};

// Readable Properties
public ref class GarageDoorGetIsOpenRequestedEventArgs sealed
{
public:
    GarageDoorGetIsOpenRequestedEventArgs(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);

    property Windows::Devices::AllJoyn::AllJoynMessageInfo^ MessageInfo
    {
        Windows::Devices::AllJoyn::AllJoynMessageInfo^ get() { return m_messageInfo; }
    }

    property GarageDoorGetIsOpenResult^ Result
    {
        GarageDoorGetIsOpenResult^ get() { return m_result; }
        void set(_In_ GarageDoorGetIsOpenResult^ value) { m_result = value; }
    }

    Windows::Foundation::Deferral^ GetDeferral();

    static Windows::Foundation::IAsyncOperation<GarageDoorGetIsOpenResult^>^ GetResultAsync(GarageDoorGetIsOpenRequestedEventArgs^ args)
    {
        args->InvokeAllFinished();
        auto t = concurrency::create_task(args->m_tce);
        return concurrency::create_async([t]() -> concurrency::task<GarageDoorGetIsOpenResult^>
        {
            return t;
        });
    }

private:
    void Complete();
    void InvokeAllFinished();
    void InvokeCompleteHandler();

    bool m_raised;
    int m_completionsRequired;
    concurrency::task_completion_event<GarageDoorGetIsOpenResult^> m_tce;
    std::mutex m_lock;
    Windows::Devices::AllJoyn::AllJoynMessageInfo^ m_messageInfo;
    GarageDoorGetIsOpenResult^ m_result;
};

public ref class GarageDoorGetIsPartiallyOpenRequestedEventArgs sealed
{
public:
    GarageDoorGetIsPartiallyOpenRequestedEventArgs(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);

    property Windows::Devices::AllJoyn::AllJoynMessageInfo^ MessageInfo
    {
        Windows::Devices::AllJoyn::AllJoynMessageInfo^ get() { return m_messageInfo; }
    }

    property GarageDoorGetIsPartiallyOpenResult^ Result
    {
        GarageDoorGetIsPartiallyOpenResult^ get() { return m_result; }
        void set(_In_ GarageDoorGetIsPartiallyOpenResult^ value) { m_result = value; }
    }

    Windows::Foundation::Deferral^ GetDeferral();

    static Windows::Foundation::IAsyncOperation<GarageDoorGetIsPartiallyOpenResult^>^ GetResultAsync(GarageDoorGetIsPartiallyOpenRequestedEventArgs^ args)
    {
        args->InvokeAllFinished();
        auto t = concurrency::create_task(args->m_tce);
        return concurrency::create_async([t]() -> concurrency::task<GarageDoorGetIsPartiallyOpenResult^>
        {
            return t;
        });
    }

private:
    void Complete();
    void InvokeAllFinished();
    void InvokeCompleteHandler();

    bool m_raised;
    int m_completionsRequired;
    concurrency::task_completion_event<GarageDoorGetIsPartiallyOpenResult^> m_tce;
    std::mutex m_lock;
    Windows::Devices::AllJoyn::AllJoynMessageInfo^ m_messageInfo;
    GarageDoorGetIsPartiallyOpenResult^ m_result;
};

// Writable Properties
} } } 